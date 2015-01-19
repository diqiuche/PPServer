using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using PPServer.Service;
using PPServer.Object;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/9/5
 * Description:定义在线用户类
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.DoService
{
    /// <summary>
    /// 在线用户
    /// </summary>
    public class User
    {
        private string USERTYPE_SPLIT = Config.ServerConfig.Instance.System.UserTypeSplitChar;    //用户类型分隔符
        public event Action<User> OnUserChannelFault;
        /// <summary>
        /// 登陆的用户名
        /// </summary>
        public string UserName
        {
            get;
            private set;
        }

        /// <summary>
        /// 登陆的用户类型
        /// </summary>
        public string UserType
        {
            get;
            private set;
        }

        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string UserId
        {
            get;
            private set;
        }

        /// <summary>
        /// 客户端IP地址
        /// </summary>
        public string IP
        {
            get;
            private set;
        }
        /// <summary>
        /// 客户端端口号
        /// </summary>
        public int Port
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取激活时间
        /// </summary>
        public DateTime ActiveTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 客户端监听
        /// </summary>
        private IDuplexListener _clientListener;

        /// <summary>
        /// 构造在线用户
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <param name="Port">端口号</param>
        /// <param name="UserName">用户名</param>
        /// <param name="ClientListener">客户端监听</param>
        /// <param name="Channel">客户端通道</param>
        public User(string IP, int Port, string UserName, IDuplexListener ClientListener,IContextChannel Channel)
        {
            this.IP = IP;
            this.Port = Port;
            this.UserName = UserName;
            //分隔用户类型
            if (!string.IsNullOrEmpty(USERTYPE_SPLIT) && UserName.Contains(USERTYPE_SPLIT))
                this.UserType = UserName.Split(USERTYPE_SPLIT[0])[0];
            _clientListener = ClientListener;
            this.UserId = Guid.NewGuid().ToString();
            Channel.Faulted += new EventHandler(Channel_Faulted);
            Active();
        }

        void Channel_Faulted(object sender, EventArgs e)
        {
            if (OnUserChannelFault != null)
                OnUserChannelFault(this);
        }

        /// <summary>
        /// 通知消息
        /// </summary>
        /// <param name="Message">消息内容</param>
        public void Notify(DuplexMessage Message)
        {
            _clientListener.OnReceive(Message);            
        }

        /// <summary>
        /// 激活用户
        /// </summary>
        public void Active()
        {
            ActiveTime = DateTime.Now;
        }
    }
}
