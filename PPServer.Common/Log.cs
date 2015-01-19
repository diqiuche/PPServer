using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PPServer.Common
{
    public class Log
    {
        private static object LOG_LOCKER = new object();
        /// <summary>
        /// 日志文件目录
        /// </summary>
        public static string FolderPath
        {
            get;
            set;
        }
        /// <summary>
        /// 是否写入日志文件
        /// </summary>
        public static bool WriteLogFile
        {
            get;
            set;
        }
        public static void WriteToLog(string message, bool isError)
        {
            try
            {
                string infoHead = DateTime.Now.ToString();                 //输出头,以时间为开始，格式为：时间+医院注册码+信息
                if (isError)
                {
                    infoHead += " ##错误  ";
                }
                else
                {
                    infoHead += " 信  息  ";
                }
                Console.WriteLine(infoHead + message);
                //写入日志文件
                if (WriteLogFile)
                {
                    string folder = DateTime.Now.ToString("yyyyMM");
                    string file = DateTime.Now.ToString("dd") + ".log";
                    string path = (string.IsNullOrEmpty(FolderPath) ? "Log\\" : FolderPath) + folder;
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    lock (LOG_LOCKER)
                    {
                        FileStream fs = new FileStream(path + "\\" + file, FileMode.Append);       //文件流变量
                        StreamWriter streamWriter = new StreamWriter(fs);
                        streamWriter.WriteLine(infoHead + message);
                        streamWriter.Flush();
                        streamWriter.Close();
                        fs.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("####################### 日志写入错误 #######################");
                Console.WriteLine(e.Message);
                Console.WriteLine("############################################################");
            }
        }
    }
}
