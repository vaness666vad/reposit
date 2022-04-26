using Loger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlServer2._0.ServerConfig
{
    [Serializable]
    public class Config
    {
        public static NetworkInterface[] InternetControllerList = NetworkInterface.GetAllNetworkInterfaces();
        public NetworkInterface InternetController
        {
            get
            {
                try
                {
                    internetController = InternetControllerList.First(x => x.Description == internetControllerName);
                }
                catch
                {
                    internetController = InternetControllerList.FirstOrDefault();
                }
                return internetController;
            }
            set
            {
                internetController = value;
                internetControllerName = internetController?.Description;
            }
        }
        [NonSerialized]
        private NetworkInterface internetController;
        private string internetControllerName { get; set; }
        public string IpSite { get; set; }
        public int Port { get; set; }
        public string IMAPhost { get; set; }
        public int IMAPport { get; set; }
        public string SMTPhost { get; set; }
        public int SMTPport { get; set; }
        public string Login { get; set; }
        public string Pass { get; set; }
        public bool Save { get; set; }
        [NonSerialized]
        private static string FileName = "Config.dat";
        [NonSerialized]
        private static string DirPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\ServerConf\";
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
