using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;

namespace Loger
{
    public static class LogWriter
    {
        static LogWriter()
        {
            AppDomain.CurrentDomain.ProcessExit += StaticClass_Dtor;
            waitSendlog = new Mutex();
        }
        static void StaticClass_Dtor(object sender, EventArgs e)
        {
            waitSendlog.Close();
            waitSendlog.Dispose();
        }
        private static string writePath = Directory.GetCurrentDirectory() + @"\Log.txt";
        private static Mutex waitSendlog;
        /// <summary>
        /// Выполняет запись в лог
        /// </summary>
        /// <param name="message">сообщение</param>
        public static void SendLog(string message, ConsoleColor color = ConsoleColor.Green, bool showMessageBox = false)
        {
            waitSendlog.WaitOne();
            message = '[' + DateTime.Now.ToString() + "] " + message;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, true, Encoding.UTF8))
                {
                    sw.WriteLine(message);
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка логирования");
            }
            waitSendlog.ReleaseMutex();
            if (showMessageBox)
                MessageBox.Show(message);
        }
    }
}
