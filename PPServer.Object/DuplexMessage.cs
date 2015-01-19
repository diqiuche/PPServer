using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Linq;
using PPServer.Object;
using System.Runtime.Serialization;
using System.ServiceModel;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.4
 * Create Date:2013/7/2
 * Description:定义双工服务的通讯消息
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.Object
{    
    /// <summary>
    /// 双工服务通讯消息
    /// </summary>
    [DataContract]
    public class DuplexMessage : ObjectBase
    {
        internal DuplexMessage()
        { }

        /// <summary>
        /// 获取消息唯一标示
        /// </summary>
        [DataMember]
        public string MessageId
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取消息创建时间
        /// </summary>
        [DataMember]
        public DateTime MessageTime
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取发送方用户名
        /// </summary>
        [DataMember]
        public string FromUser
        {
            private set;
            get;
        }
        /// <summary>
        /// 获取接收方用户名集合，当为广播消息时，不处理此属性
        /// </summary>
        [DataMember]
        public List<string> ToUsers
        {
            private set;
            get;
        }
        /// <summary>
        /// 获取是否为广播消息
        /// </summary>
        [DataMember]
        public bool IsBroadcast
        {
            private set;
            get;
        }
        /// <summary>
        /// 双工消息类型
        /// </summary>
        public enum MessageTypes
        {
            /// <summary>
            /// 文本
            /// </summary>
            Text,
            /// <summary>
            /// 命令
            /// </summary>
            Command
        }
        /// <summary>
        /// 命令类型
        /// </summary>
        public enum CommandTypes
        {
            /// <summary>
            /// 获得文件
            /// </summary>
            GET_FILE,
            /// <summary>
            /// 获得当前在线用户
            /// </summary>
            GET_ONLINE_USERS,
            /// <summary>
            /// 强制登出
            /// </summary>
            LOGOUT,
            /// <summary>
            /// 自定义
            /// </summary>
            CUSTOMER
        }
        /// <summary>
        /// 获取是否保存离线消息
        /// </summary>
        [DataMember]
        public bool OfflineSave
        {
            private set;
            get;
        }
        /// <summary>
        /// 获取消息类型
        /// </summary>
        [DataMember]
        public MessageTypes MessageType
        {
            private set;
            get;
        }

        /// <summary>
        /// 获取消息类型
        /// </summary>
        [DataMember]
        public CommandTypes CommandType
        {
            private set;
            get;
        }
        /// <summary>
        /// 获取或设置消息数据，需可被序列化
        /// </summary>
        [DataMember]
        public Dictionary<string, object>  MessageData
        {
            set;
            get;
        }

        /// <summary>
        /// 获取上传或下载完毕服务端执行方法，key为方法名，value为参数集合，object须为可序列化对象
        /// </summary>
        [DataMember]
        public Dictionary<string, List<object>> AfterFunctions
        {
            set;
            get;
        }

        /// <summary>
        /// 获取或设置消息文本内容
        /// </summary>
        [DataMember]
        public string MessageText
        {
            set;
            get;
        }

        /// <summary>
        /// 构造发送到指定用户的双工服务消息
        /// </summary>
        /// <param name="FromUser">发送方用户名</param>
        /// <param name="ToUsers">接收方用户名集合</param>
        /// <param name="MessageType">双工消息类型</param>
        /// <param name="CommandType">命令类型</param>
        /// <param name="MessageText">消息文本</param>
        /// <param name="MessageData">发送的数据内容</param>
        /// <param name="OfflineSave">是否保存离线消息</param>
        private DuplexMessage(string FromUser, List<string> ToUsers, MessageTypes MessageType, CommandTypes CommandType, string MessageText, Dictionary<string, object> MessageData, bool OfflineSave)
        {
            MessageId = Guid.NewGuid().ToString();
            MessageTime = DateTime.Now;
            this.CommandType = CommandType;
            this.MessageText = MessageText;
            this.MessageData = MessageData;
            this.FromUser = FromUser;
            this.ToUsers = ToUsers;
            this.MessageType = MessageType;
            this.OfflineSave = OfflineSave;
            if (ToUsers == null)
                IsBroadcast = true;
            else
                IsBroadcast = false;
        }
        /// <summary>
        /// 构造广播命令消息
        /// </summary>
        /// <param name="FromUser">发送方用户名</param>
        /// <param name="CommandType">命令类型</param>
        /// <param name="CommandText">命令文本</param>
        /// <param name="CommandData">命令数据</param>
        public DuplexMessage(string FromUser, CommandTypes CommandType,string CommandText, Dictionary<string, object> CommandData)
            : this(FromUser, null, MessageTypes.Command, CommandType, CommandText, CommandData, false)
        {
        }
        /// <summary>
        /// 构造发送至指定用户的命令消息
        /// </summary>
        /// <param name="FromUser">发送方用户名</param>
        /// <param name="ToUsers">接收方用户名集合</param>
        /// <param name="CommandType">命令类型</param>
        /// <param name="CommandText">命令文本</param>
        /// <param name="CommandData">命令数据</param>
        /// <param name="OfflineSave">是否保存离线消息</param>
        public DuplexMessage(string FromUser, List<string> ToUsers, CommandTypes CommandType, string CommandText, Dictionary<string, object> CommandData, bool OfflineSave = false)
            : this(FromUser, ToUsers, MessageTypes.Command, CommandType, CommandText, CommandData, OfflineSave)
        { }

        /// <summary>
        /// 构造广播文本消息
        /// </summary>
        /// <param name="FromUser">发送方用户名</param>
        /// <param name="Text">广播文本</param>
        public DuplexMessage(string FromUser, string Text)
            : this(FromUser, null, MessageTypes.Text, CommandTypes.CUSTOMER, Text, null, false)
        { }
        /// <summary>
        /// 构造发送至指定用户的文本消息
        /// </summary>
        /// <param name="FromUser">发送方用户名</param>
        /// <param name="ToUsers">接收方用户名集合</param>
        /// <param name="Text">发送文本</param>
        /// <param name="OfflineSave">是否保存离线消息</param>
        public DuplexMessage(string FromUser, List<string> ToUsers, string Text, bool OfflineSave = false)
            : this(FromUser, ToUsers, MessageTypes.Text, CommandTypes.CUSTOMER, Text, null, OfflineSave)
        { }

        /// <summary>
        /// 克隆所有数据
        /// </summary>
        /// <returns></returns>
        public DuplexMessage Clone(string ToUser)
        {
            DuplexMessage _newMsg = new DuplexMessage(FromUser, new List<string>() { ToUser }, this.MessageType, this.CommandType,this.MessageText,this.MessageData, this.OfflineSave);
            _newMsg.AfterFunctions = this.AfterFunctions;
            return _newMsg;
        }

        /// <summary>
        /// 返回当前对象的字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (MessageType == MessageTypes.Text)
                return MessageText;
            else
            {
                string _result = (string.IsNullOrEmpty(MessageText) ? string.Empty : ("\"" + MessageText + "\""));
                if (MessageData != null && MessageData.Count > 0)
                {
                    _result += (_result == string.Empty ? _result : ":") + string.Join(",",MessageData.Select(t => t.Key + "=" + t.Value));
                }
                return _result;
            }
        }

        /// <summary>
        /// 添加事后执行方法
        /// </summary>
        /// <param name="FunctionName">方法名</param>
        /// <param name="Param">参数集合</param>
        public void AddAfterFunction(string FunctionName, params object[] Param)
        {
            if (AfterFunctions == null)
                AfterFunctions = new Dictionary<string, List<object>>();
            AfterFunctions.Add(FunctionName, Param == null ? null : Param.ToList());
        }
    }
}
