using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/7/2
 * Description:定义请求的文件消息，可用来下载文件
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.Object
{
    /// <summary>
    /// 文件服务请求消息
    /// </summary>
    [MessageContract]
    public class FileMessageRequest
    {
        /// <summary>
        /// 获取消息唯一标示
        /// </summary>
        [MessageHeader]
        public string MessageId
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取消息创建时间
        /// </summary>
        [MessageHeader]
        public DateTime MessageTime
        {
            get;
            private set;
        }
        /// <summary>
        /// 设置或获取服务器文件路径
        /// </summary>
        [MessageHeader]
        public string ServerMapPath
        {
            get;
            set;
        }
        /// <summary>
        /// 设置或获取用户名
        /// </summary>
        [MessageHeader]
        public string UserName
        {
            get;
            set;
        }
        /// <summary>
        /// 设置或获取密码
        /// </summary>
        [MessageHeader]
        public string Password
        {
            get;
            set;
        }
        /// <summary>
        /// 设置或获取是否压缩传输
        /// </summary>
        [MessageHeader]
        public bool ToCompress
        {
            get;
            set;
        }
        /// <summary>
        /// 设置或获取是否加密传输
        /// </summary>
        [MessageHeader]
        public bool ToEncrypt
        {
            get;
            set;
        }

        /// <summary>
        /// 设置或获取下载完毕后删除原文件
        /// </summary>
        [MessageHeader]
        public bool ToDeleteAfterDownloaded
        {
            get;
            set;
        }
        /// <summary>
        /// 获取上传或下载完毕服务端执行方法，key为方法名，value为参数集合，object须为可序列化对象
        /// </summary>
        [MessageHeader]
        public Dictionary<string, List<object>> AfterFunctions
        {
            set;
            get;
        }
        /// <summary>
        /// 获取上传或下载之前服务端执行方法，key为方法名，value为参数集合，object须为可序列化对象
        /// </summary>
        [MessageHeader]
        public Dictionary<string, List<object>> BeforeFunctions
        {
            set;
            get;
        }
        /// <summary>
        /// 构造文件请求消息
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="FilePath">请求的文件路径</param>
        /// <param name="ToCompress">是否压缩传输</param>
        /// <param name="ToEncrypt">是否加密传输</param>
        /// <param name="ToDeleteAfterDownloaded">设置或获取下载完毕后删除原文件</param>
        public FileMessageRequest(string UserName, string Password, string FilePath, bool ToCompress = true, bool ToEncrypt = false, bool ToDeleteAfterDownloaded=false)
        {
            MessageId = Guid.NewGuid().ToString();
            MessageTime = DateTime.Now;
            this.UserName = UserName;
            this.Password = Password;
            this.ServerMapPath = FilePath;
            this.ToCompress = ToCompress;
            this.ToEncrypt = ToEncrypt;
            this.ToDeleteAfterDownloaded = ToDeleteAfterDownloaded;
        }

        internal FileMessageRequest()
        { 
            
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
        /// <summary>
        /// 添加事前执行方法
        /// </summary>
        /// <param name="FunctionName">方法名</param>
        /// <param name="Param">参数集合</param>
        public void AddBeforeFunction(string FunctionName, params object[] Param)
        {
            if (BeforeFunctions == null)
                BeforeFunctions = new Dictionary<string, List<object>>();
            BeforeFunctions.Add(FunctionName, Param == null ? null : Param.ToList());
        }
    }
}
