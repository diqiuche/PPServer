using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using PPServer.Service;
using PPServer.Object;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/9/5
 * Description:双工服务，发送消息、更新在线用户
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.DoService
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public class DuplexDoService : DoServiceBase, PPServer.Service.IDuplexService
    {
        /// <summary>
        /// 注册消息监听
        /// </summary>
        /// <param name="UserName">当前用户名</param>
        /// <param name="Password">密码</param>
        /// <returns>返回用户标识，如果注册不成功则</returns>
        public string Regist(string UserName, string Password)
        {
            try
            {
                return DuplexCenter.Instance.UserLogin(CreateUserInstance(UserName,Password));
            }
            catch (Exception ex)
            {
                WriteErrorToLog(ex,"用户[{0}]注册双工通道出错。", UserName);
            }
            return string.Empty;
        }

        /// <summary>
        /// 注销消息监听
        /// </summary>
        /// <param name="UserName">当前用户名</param>
        /// <param name="UserId">用户标识</param>
        public void Unregist(string UserName, string UserId)
        {
            try
            {
                DuplexCenter.Instance.UserLogout(UserName, UserId);
            }
            catch (Exception ex)
            {
                WriteErrorToLog(ex,"用户[{0}]注销双工通道出错。", UserName);
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="UserName">源用户名</param>
        /// <param name="UserId">源用户标识</param>
        /// <param name="Message">需发送的消息</param>
        public void SendMessage(string UserName, string UserId, DuplexMessage Message)
        {
            try
            {
                DuplexCenter.Instance.SendMessage(UserName,UserId, Message);
            }
            catch (Exception ex)
            {
                WriteErrorToLog(ex,"用户[{0}]发送消息出错。", UserName);
                //throw new FaultException("发送消息出错：" + ex.Message);
            }
        }

        /// <summary>
        /// 获得在线用户列表
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="UserId">用户标识</param>
        public void GetOnlineUsers(string UserName, string UserId)
        {
            try
            {
                DuplexCenter.Instance.SendOnlineUserList(UserName, UserId);
            }
            catch (Exception ex)
            {
                WriteErrorToLog(ex,"用户[{0}]获取在线用户列表出错。", UserName);
            }
        }

        private User CreateUserInstance(string UserName, string Password)
        {
            //Authenticate User's Identity
            this.ToAuthenticate(UserName, Password, this.GetType().ToString() + "/Regist");

            RemoteEndpointMessageProperty _remoteEndPointProp = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            IDuplexListener _callback = OperationContext.Current.GetCallbackChannel<IDuplexListener>();
            return new User(_remoteEndPointProp.Address, _remoteEndPointProp.Port, UserName, _callback, OperationContext.Current.Channel);
        }

    }
}
