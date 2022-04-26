using Loger;
using ProtectedValues;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProtocolCommunication
{
    /// <summary>
    /// Прототип клиента
    /// </summary>
    public abstract class ClientObjPrototype
    {
        public ProtectedVal<int> Id;
        public delegate void AceptReportAction(object arg);
        public Socket Client { get; private set; }
        private AutoResetEvent writeMutex;
        private Thread thread;
        private Thread pinger;
        private ProtectedVal<List<Tuple<string, AceptReportAction, object>>> waitDelevered;
        public ClientObjPrototype(Socket c)
        {
            writeMutex = new AutoResetEvent(true);
            waitDelevered.Value = new List<Tuple<string, AceptReportAction, object>>();
            isConnect.Value = true;

            Client = c;
            Client.ReceiveTimeout = -1;

            thread = new Thread(new ThreadStart(read)) { IsBackground = true };
            thread.Start();

            pinger = new Thread(new ThreadStart(startPing)) { IsBackground = true };
            pinger.Start();
        }
        ~ClientObjPrototype()
        {
            try { writeMutex.Close(); } catch { }
            try { writeMutex.Dispose(); } catch { }
        }
        /// <summary>
        /// Добавляет в список ожидающих доставку новый guid и событие
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="act"></param>
        private void addWaitDelvered(string guid, AceptReportAction act, object arg)
        {
            waitDelevered.ExecuteMethod((x) => x.Add(new Tuple<string, AceptReportAction, object>(guid, act, arg)));
        }
        /// <summary>
        /// Возвращает true если в списке ожидающих доставку нет заданного guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool IsDelevered(string guid)
        {
            bool res = false;
            waitDelevered.ExecuteMethod((x) => res = !x.Any(y => y.Item1 == guid));
            return res;
        }
        /// <summary>
        /// Убирает guid из списка ожидающих доставку и запускает новый поток если задано событие
        /// </summary>
        /// <param name="guid"></param>
        private void setDelvered(string guid)
        {
            Tuple<string, AceptReportAction, object> delevered = null;
            waitDelevered.ExecuteMethod((x) => 
            {
                delevered = x.FirstOrDefault(y => y.Item1 == guid);
                x.Remove(delevered);
            });
            if (delevered?.Item2 != null)
            {
                Thread t = new Thread(new ParameterizedThreadStart(delevered.Item2)) { IsBackground = true };
                t.Start(delevered.Item3);
            }
        }
        /// <summary>
        /// Завершает работу клиента
        /// </summary>
        public void CloseClient()
        {
            if(isConnect.SetAndReturnOld(false))
            {
                Protocol.ProtectedCloseSocket(Client);
                EventAfterCloseClient?.Invoke();
            }
        }
        public Action EventAfterCloseClient { get; set; }
        /// <summary>
        /// Возвращается true если клиент на связи
        /// </summary>
        public bool IsConnect { get => isConnect.Value; }
        private ProtectedVal<bool> isConnect;
        /// <summary>
        /// Выполняет чтение данных из сокета и из обработку
        /// </summary>
        private void read()
        {
            try
            {
                byte[] Borderbufer = new byte[Protocol.Border.Length];
                while (IsConnect)
                {                 
                    Protocol.ReadStream(Client, Borderbufer, Borderbufer.Length - 1, 1);
                    if (Borderbufer.SequenceEqual(Protocol.BorderBytes))
                    {
                        byte[] buferCover = new byte[Protocol.CoverSize];
                        Array.Copy(Borderbufer, buferCover, Borderbufer.Length);
                        Protocol.ReadStream(Client, buferCover, Borderbufer.Length, buferCover.Length - Borderbufer.Length);

                        Cover cov = Protocol.BufferToObject<Cover>(buferCover);

                        if (cov.DataSize <= DataCover128kb.MaxDataSize)
                        {
                            byte[] data = new byte[cov.DataSize + buferCover.Length];
                            Array.Copy(buferCover, data, buferCover.Length);
                            Protocol.ReadStream(Client, data, buferCover.Length, data.Length - buferCover.Length);

                            DataCover128kb dc8b = Protocol.BufferToObject<DataCover128kb>(data);
                            if (cov.BufferType == DataType.report)
                            {
                                string guid = Encoding.ASCII.GetString(dc8b.Data);
                                setDelvered(guid);
                            }
                            else
                            {
                                if (cov.DeliveryReport)
                                {
                                    DataCover128kb dc8rep = new DataCover128kb(cov.GuidCoverBytes, DataType.report);
                                    Write(dc8rep.Pack());
                                }
                                Thread t = new Thread(new ParameterizedThreadStart(aceptMessage)) { IsBackground = true };
                                t.Start(dc8b);
                            }
                        }
                        else
                            LogWriter.SendLog("Получен пакет не верного размера");
                        Array.Copy(new byte[Borderbufer.Length], Borderbufer, Borderbufer.Length);
                    }
                    else
                        Array.Copy(Borderbufer, 1, Borderbufer, 0, Borderbufer.Length - 1);
                }
            }
            catch (Exception e)
            {
                LogWriter.SendLog(e.Message, ConsoleColor.Red);   
            }
            finally
            {
                CloseClient();
            }
        }
        /// <summary>
        /// Выполняется в новом потоке если получено новое сообщение (для переопределения)
        /// </summary>
        /// <param name="data"></param>
        public virtual void AceptMessage(DataCover128kb data)
        {

        }
        private void aceptMessage(object data)
        {
            AceptMessage((DataCover128kb)data);
        }
        /// <summary>
        /// Выполняет отправку конверта клиенту
        /// </summary>
        /// <param name="data">конверт</param>
        /// <param name="act">этот метод будет выполнен новым потоком при получении отчета о доставке</param>
        public bool Write(DataCover128kb data, AceptReportAction act = null, object arg = null)
        {
            if (data.DeliveryReport)
                addWaitDelvered(data.GuidCover, act, arg);
            return Write(data.Pack());
        }
        /// <summary>
        /// Выполняет отправку байтов клиенту
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Write(byte[] data)
        {
            bool res = false;
            if (IsConnect)
            {
                writeMutex.WaitOne();
                try
                {
                    Client.Send(data, 0, data.Length, SocketFlags.None);
                    res = true;
                }
                catch (Exception e)
                {
                    LogWriter.SendLog(e.Message, ConsoleColor.Red);
                }
                writeMutex.Set();
            }
            return res;
        }
        /// <summary>
        /// Пингует клиента
        /// </summary>
        private void startPing()
        {
            while (IsConnect)
            {                
                DataCover128kb dc128 = new DataCover128kb(new byte[0], DataType.ping, true);
                Stopwatch stw = new Stopwatch();
                stw.Start();
                Write(dc128, SavePing, stw);
                Thread.Sleep(5000);
            } 
        }
        private void SavePing(object arg)
        {
            Stopwatch stw = (Stopwatch)arg;
            stw.Stop();
            ping.Value = stw.ElapsedMilliseconds;
        }
        /// <summary>
        /// Возвращает пинг клиента
        /// </summary>
        public long Ping { get => ping.Value; }
        private ProtectedVal<long> ping;
    }
}
