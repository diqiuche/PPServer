using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using PPServer.Object;
using PPServer.Interface;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/6/5
 * Description:Read config file for system using
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.Config
{    
    /// <summary>
    /// 服务器配置
    /// </summary>
    [Serializable]
    public class ServerConfig : ObjectBase
    {
        /// <summary>
        /// 数据库配置
        /// </summary>
        [XmlElement]
        public Databases Databases;

        /// <summary>
        /// 服务配置
        /// </summary>
        [XmlElement]
        public Services Services;

        /// <summary>
        /// 系统配置
        /// </summary>
        [XmlElement]
        public System System;

        /// <summary>
        /// 私有单例
        /// </summary>
        private static ServerConfig _instance;
        /// <summary>
        /// 配置单例
        /// </summary>
        public static ServerConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = ObjectBase.LoadObjFromXML<ServerConfig>(Common.CommonData.AppPath + Common.CommonData.ConfigFile);
                    //设置common中变量
                    Common.Log.FolderPath = _instance.System.MapFolders.FolderList["Log"].RealPath;
                    Common.Log.WriteLogFile = _instance.System.WriteLog;

                    //Init all services function list
                    _instance.Services.Duplex.InitFunctionList<IDuplexFunction>(_instance.System.MapFolders.FolderList["Plugin"].RealPath);
                    _instance.Services.Inner.InitFunctionList<IInnerFunction>(_instance.System.MapFolders.FolderList["Plugin"].RealPath);
                    _instance.Services.Public.InitFunctionList<IPublicFunction>(_instance.System.MapFolders.FolderList["Plugin"].RealPath);
                    _instance.Services.File.InitFunctionList<IFileFunction>(_instance.System.MapFolders.FolderList["Plugin"].RealPath);
                    //Init all Transportobjects list
                    _instance.Services.TransportObjects.InitObjects(_instance.System.MapFolders.FolderList["Plugin"].RealPath);
                    //Load all Plugin file
                    _instance.LoadAssemblyFolder(_instance.System.MapFolders.FolderList["Plugin"].RealPath, "dll");
                }
                return _instance;
            }
        }

        /// <summary>
        /// 根据目录加载执行文件
        /// </summary>
        /// <param name="FolderPath">目录名</param>
        /// <param name="FileExtensions">文件扩展名，可以逗号,分割</param>
        private void LoadAssemblyFolder(string FolderPath, string FileExtensions)
        {
            if (Directory.Exists(FolderPath))
            {
                DirectoryInfo _di = new DirectoryInfo(FolderPath);
                string[] _extensions=FileExtensions.Split(',').Select(t=>{
                return t.StartsWith("*")?t.Trim('*'):(t.StartsWith(".")?t:("."+t));
                }).ToArray();
                //先加载本文件夹文件
                FileInfo[] _subFileList = _di.GetFiles();
                foreach (FileInfo fi in _subFileList)
                {
                    try
                    {
                        if(_extensions.Contains(fi.Extension))
                            Assembly.LoadFrom(fi.FullName);
                    }
                    catch { }
                }
                //再通过递归加载内部文件夹文件
                DirectoryInfo[] _subDictList = _di.GetDirectories();
                foreach (DirectoryInfo di in _subDictList)
                {
                    LoadAssemblyFolder(di.FullName, FileExtensions);
                }
            }
        }
    }
}
