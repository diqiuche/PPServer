using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Reflection;
using PPServer.Config;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/9/5
 * Description:服务基础类
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.DoService
{
    public abstract class DoServiceBase
    {
        /// <summary>
        /// 日志触发代理
        /// </summary>
        /// <param name="Content">日志内容</param>
        /// <param name="IsError">是否为错误日志</param>
        public delegate void OnWriteLogDlg(string Content, bool IsError = false);
        /// <summary>
        /// 当需要写入日志时触发
        /// </summary>
        public static event OnWriteLogDlg OnWriteLog;

        public Dictionary<string, Folder> FolderList = Config.ServerConfig.Instance.System.MapFolders.FolderList;
        public bool DebugMode = Config.ServerConfig.Instance.System.DebugMode;

        protected bool ToAuthenticate(string UserName, string Password, string ServiceType)
        {
            try
            {
                if (UserName.Trim()==string.Empty)
                    throw new Exception("用户名为空，身份验证不通过！");
                return true;
            }
            catch (Exception ex)
            {
                WriteErrorToLog(ex, "[{0}]执行\"{1}\"，身份验证失败。", UserName, ServiceType);
                //throw new FaultException(UserName + "身份验证不通过！");
            }
            return false;
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="Content">日志内容</param>
        /// <param name="IsError">是否为错误日志</param>
        protected void WriteToLog(string Content, bool IsError = false,params object[] Paras)
        {
            if (OnWriteLog != null)
                OnWriteLog(string.Format(Content,Paras), IsError);
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="Content">日志内容</param>
        /// <param name="IsError">是否为错误日志</param>
        protected void WriteErrorToLog(Exception Ex,string Content, params object[] Paras)
        {
            if (OnWriteLog != null)
                OnWriteLog(string.Format(Content+"错误："+Ex.Message+(DebugMode?("\r\n详细错误："+Ex.StackTrace):string.Empty), Paras), true);
        }

        /// <summary>
        /// 动态创建类实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="FilePath">文件路径</param>
        /// <param name="ClassName">类全名</param>
        /// <returns>产生异常时返回null</returns>
        public T CreateClassInstance<T>(string FilePath, string ClassName)
        {
            try
            {
                Assembly _ass = Assembly.LoadFrom(FilePath);
                Type _funcType = _ass.GetType(ClassName);
                if ((_funcType is T) || typeof(T).IsAssignableFrom(_funcType))
                    return (T)Activator.CreateInstance(_funcType);
                else
                    return default(T);
            }
            catch(Exception ex)
            {
                WriteToLog("从\"" + FilePath + "\"新建类\"" + ClassName + "\"实例时产生异常：" + ex.Message, true);
            }
            return default(T);
        }
        /// <summary>
        /// 拆分方法名
        /// </summary>
        /// <param name="FunctionName">输入的方法名</param>
        /// <param name="PublicName">公开方法名</param>
        /// <param name="PrivateName">私有方法名</param>
        public void GetFunctionName(string FunctionName, out string PublicName, out string PrivateName)
        {
            string[] _funcNames = FunctionName.Replace('\\', '/').Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            PublicName = _funcNames[0];
            PrivateName = null;
            if (_funcNames.Length > 1)
                PrivateName = FunctionName.Remove(0, PublicName.Length + 1);
        }

        /// <summary>
        /// 得到合法的文件路径
        /// </summary>
        /// <param name="FolderPath">目录路径</param>
        /// <param name="FilePath">文件路径</param>
        /// <returns>产生异常时返回空字符串</returns>
        public string GetLegalPathString(string FolderPath, string FilePath)
        { 
            try
            {
                FolderPath = FolderPath == null ? string.Empty : FolderPath.Replace('/', '\\').Trim('\\') + "\\";
                FilePath = FilePath == null ? string.Empty : FilePath.Replace('/', '\\').Trim('.', '\\', ' ');
                return FolderPath + FilePath;
            }
            catch{}
            return string.Empty;
        }
    }
}
