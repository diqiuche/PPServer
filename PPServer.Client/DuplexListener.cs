using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PPServer.Service;
using PPServer.Object;
using System.ServiceModel;

namespace PPServer.Client
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.PerCall)]
    internal class DuplexListener : IDuplexListener
    {
        /// <summary>
        /// 消息事件句柄
        /// </summary>
        /// <param name="Message"></param>
        public delegate void MessageEventHandler(DuplexMessage Message);

        /// <summary>
        /// 收到消息时触发
        /// </summary>
        public event MessageEventHandler OnReceiveMessage;
        /// <summary>
        /// 收到消息
        /// </summary>
        /// <param name="Message">消息</param>
        public void OnReceive(DuplexMessage Message)
        {
            if (OnReceiveMessage != null)
                OnReceiveMessage(Message);
        }
    }
}
