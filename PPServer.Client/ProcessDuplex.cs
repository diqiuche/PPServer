using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PPServer.Object;
using PPServer.Service;

namespace PPServer.Client
{
    internal class ProcessDuplex
    {
        public static ReturnResult SendMessage(string UserName, string UserId, IDuplexService DuplexService,string ToUser,string Message,bool OfflineSave)
        {
            return SendMessage(UserName, UserId, DuplexService, new List<string>() { ToUser }, Message, OfflineSave);
        }

        public static ReturnResult SendMessage(string UserName, string UserId, IDuplexService DuplexService,List<string> ToUsers,string Message,bool OfflineSave)
        {
            DuplexMessage _msg = new DuplexMessage(UserName, ToUsers, Message, OfflineSave);
            return SendMessage(UserName, UserId, DuplexService, _msg);
        }

        public static ReturnResult BroadcastMessage(string UserName, string UserId, IDuplexService DuplexService, string Message)
        {
            DuplexMessage _msg = new DuplexMessage(UserName, Message);
            return SendMessage(UserName, UserId, DuplexService, _msg);
        }

        public static ReturnResult SendCommand(string UserName, string UserId, IDuplexService DuplexService, string ToUser, string Command, Dictionary<string, object> CommandData, bool OfflineSave)
        {
            return SendCommand(UserName, UserId, DuplexService, new List<string>() { ToUser }, Command, CommandData, OfflineSave);
        }

        public static ReturnResult SendCommand(string UserName, string UserId, IDuplexService DuplexService, List<string> ToUsers, string Command, Dictionary<string, object> CommandData, bool OfflineSave)
        {
            DuplexMessage _msg = new DuplexMessage(UserName, ToUsers, DuplexMessage.CommandTypes.CUSTOMER,Command,CommandData, OfflineSave);
            return SendMessage(UserName, UserId, DuplexService, _msg);
        }

        public static ReturnResult BroadcastCommand(string UserName, string UserId, IDuplexService DuplexService, string Command, Dictionary<string, object> CommandData)
        {
            DuplexMessage _msg = new DuplexMessage(UserName, DuplexMessage.CommandTypes.CUSTOMER, Command, CommandData);
            return SendMessage(UserName,UserId,DuplexService,_msg);
        }

        private static ReturnResult SendMessage(string UserName, string UserId, IDuplexService DuplexService, DuplexMessage Message)
        {
            ReturnResult _rr = new ReturnResult();
            try
            {
                DuplexService.SendMessage(UserName, UserId, Message);
            }
            catch (Exception ex)
            {
                _rr.ExeCode = -1;
                _rr.ExeInfo = "发送消息本地异常：" + ex.Message + "\r\n" + "详细：" + ex.StackTrace;
            }
            return _rr;
        }
        /// <summary>
        /// 处理收到文件消息操作
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="TempFolder">临时文件目录</param>
        /// <param name="ServerPath">文件在服务器上的路径</param>
        /// <param name="Center">服务中心</param>
        /// <param name="FileService">文件服务</param>
        /// <param name="Message">收到的双工消息</param>
        /// <returns></returns>
        internal static ReturnResult ProcessReceivedFile(string UserName, string Password,string TempFolder, string ServerPath,ServiceCenter Center, IFileService FileService,DuplexMessage Message)
        {
            ReturnResult _rr = new ReturnResult();
            try
            {
                //收到文件，则自动下载文件
                //处理文件名
                string _fileName = ServerPath.Substring(ServerPath.LastIndexOf('\\')+1).Split('#').Last();
                string _localFile = string.Format("{0}", _fileName);
                bool _compressed = (bool)Message.MessageData["FILE@COMPRESSED"];
                bool _encrypted = (bool)Message.MessageData["FILE@ENCRYPTED"];
                if (!Directory.Exists(TempFolder))
                    Directory.CreateDirectory(TempFolder);
                (new ProcessFile(Center)).GetFile(UserName, Password, FileService, ServerPath, TempFolder + _localFile, Message.AfterFunctions, _compressed, _encrypted);

                //FILE_PATH存储文件路径
                _rr.ExeResult.Add("FILE_PATH",TempFolder+ _localFile);
            }
            catch (Exception ex)
            {
                _rr.ExeCode = -1;
                _rr.ExeInfo = "处理接收的文件本地异常：" + ex.Message + "\r\n" + "详细：" + ex.StackTrace;
            }
            return _rr;
        }
    }
}
