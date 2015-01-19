using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PPServer.Object;
using System.Xml.Serialization;
using System.Reflection;

namespace PPServer.Config
{
    [Serializable]
    public class Service : ObjectBase
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        [XmlAttribute]
        public bool Enable = false;

        /// <summary>
        /// 服务端口
        /// </summary>
        [XmlAttribute]
        public string Protocol;

        /// <summary>
        /// 是否需要身份验证
        /// </summary>
        [XmlAttribute]
        public bool UseAuthentication;

        /// <summary>
        /// 身份验证模块文件路径
        /// </summary>
        [XmlAttribute]
        public string AuthFilePath;

        /// <summary>
        /// 身份验证模块全类名
        /// </summary>
        [XmlAttribute]
        public string AuthFileClassName;

        /// <summary>
        /// 设置功能模块列表
        /// </summary>
        [XmlElement]
        public List<Function> Function;

        /// <summary>
        /// 功能模块分类
        /// </summary>
        [XmlElement]
        public List<FuncCategory> FuncCategory;

        /// <summary>
        /// 功能库文件列表
        /// </summary>
        [XmlElement]
        public List<FuncLib> FuncLib;

        private Dictionary<string, Function> _funcList;
        /// <summary>
        /// 获得函数列表
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, Function> FunctionList
        {
            get 
            {
                return _funcList;
            }
        }

        public void InitFunctionList<T>(string PluginFolder)
        {
            if (_funcList == null)
            {
                _funcList = new Dictionary<string, Function>();
                //Get uncategoried function list
                AddFunctionToList(this.Function, PluginFolder);
                //Get categoried function list
                if (this.FuncCategory != null && this.FuncCategory.Count > 0)
                {
                    foreach (FuncCategory cate in this.FuncCategory)
                    {
                        AddFunctionToList(cate.Function, PluginFolder);
                    }
                }
                //Get function list in lib file
                if (this.FuncLib != null && this.FuncLib.Count > 0)
                {
                    foreach (FuncLib lib in this.FuncLib)
                    {
                        AddFunctionsFromLib<T>(lib, PluginFolder);
                    }
                }
            }
        }

        /// <summary>
        /// add Functions to list from lib file
        /// </summary>
        /// <param name="Lib"></param>
        private void AddFunctionsFromLib<T>(FuncLib Lib, string PluginFolder)
        {
            if (!string.IsNullOrEmpty(Lib.FilePath))
            {
                try
                {
                    string _filePath = Lib.FilePath.Contains(":\\") ? Lib.FilePath : (PluginFolder + Lib.FilePath.Trim('\\'));
                    if (File.Exists(_filePath))
                    {
                        Assembly _ass = Assembly.LoadFrom(_filePath);
                        Type[] _types = _ass.GetTypes();
                        List<Function> _funcList=new List<Function>();
                        foreach(Type type in _types)
                        {
                            if (typeof(T).IsAssignableFrom(type))
                                _funcList.Add(new Function() { Name = type.FullName, ClassName = type.FullName, FilePath = _filePath });
                        }
                        AddFunctionToList(_funcList, PluginFolder);
                        //string _fileFolder = (new FileInfo(_filePath)).DirectoryName;
                        //string _currentDomainPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
                        //if (_currentDomainPath == null)
                        //    _currentDomainPath = string.Empty;

                        //if (!(_currentDomainPath + ";").Contains(_fileFolder + ";"))
                        //{
                        //    AppDomain.CurrentDomain.SetupInformation.ShadowCopyDirectories = _fileFolder + ";";
                        //}
                    }
                }
                catch (Exception ex)
                {
                    Common.Log.WriteToLog("Load funclib file ["+Lib.FilePath + "] exception:" + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// add functions to local variable
        /// </summary>
        /// <param name="FuncList"></param>
        private void AddFunctionToList(List<Function> FuncList, string PluginFolder)
        {
            if (FuncList != null && FuncList.Count > 0)
            {
                foreach (Function func in FuncList)
                {
                    try
                    {
                        func.FilePath = func.FilePath.Contains(":\\") ? func.FilePath : (PluginFolder + func.FilePath.Trim('\\'));
                        _funcList.Add(func.Name.Trim(), func);
                    }
                    catch (Exception ex)
                    {
                        Common.Log.WriteToLog("Load function ["+func.Name + "] exception:" + ex.Message, true);
                    }
                }
            }
        }
    }
}
