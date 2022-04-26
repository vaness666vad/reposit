using Loger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Terminal
{
    [Serializable]
    public class Config
    {
        public string InternetControllerName { get; set; }
        public bool LocalConnect { get; set; }
        public bool ForwardConnect { get; set; }
        public bool MailConnect { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Pass { get; set; }
        public string Mail { get; set; }
        public int MailPort { get; set; }
        public byte Ip1 { get; set; }
        public byte Ip2 { get; set; }
        public byte Ip3 { get; set; }
        public byte Ip4 { get; set; }
        public int Port { get; set; }
        public bool IsBind { get; set; }
        public int BindPort { get; set; }
        public bool Save { get; set; }
        [NonSerialized]
        private static string FileName = "Config.dat";
        [NonSerialized]
        private static string DirPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\TerminalConf\";
        public static void SaveParams(Config conf)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                if (!Directory.Exists(DirPath))
                    Directory.CreateDirectory(DirPath);
                using (FileStream fs = new FileStream(DirPath + FileName, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, conf);
                }
            }
            catch (Exception e)
            {
                LogWriter.SendLog("Ошибка сохранения параметров сервера: " + e.Message);
            }
        }
        public static Config LoadParams()
        {
            Config newConfig = new Config();
            BinaryFormatter formatter = new BinaryFormatter();
            if (File.Exists(DirPath + FileName))
                using (FileStream fs = new FileStream(DirPath + FileName, FileMode.Open))
                {
                    try
                    {
                        newConfig = (Config)formatter.Deserialize(fs);
                    }
                    catch (Exception e)
                    {
                        LogWriter.SendLog("Ошибка загрузки параметров сервера: " + e.Message);
                    }
                }
            return newConfig;
        }
        public static void RemoveParam()
        {
            if (File.Exists(DirPath + FileName))
                try
                {
                    File.Delete(DirPath + FileName);
                }
                catch
                {

                }
        }
    }
}
