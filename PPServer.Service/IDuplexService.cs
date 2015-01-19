using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Net.Security;
using PPServer.Object;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.4
 * Create Date:2013/9/4
 * Description:公开双工服务
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.Service
{
    [ServiceKnownType("GetKnownTypes", typeof(ServiceFuncLib))]
    [ServiceContract(CallbackContract = typeof(IDuplexListener))]
    public interface IDuplexService : ServiceBase
    {
        /// <summary>
        /// 注册消息监听
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <returns>返回用户标识，如果注册不成功则</returns>
        [OperationContract(IsOneWay = false)]
        string Regist(string UserName,string Password);

        /// <summary>
        /// 注销消息监听
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="UserId">用户标识</param>
        [OperationContract(IsOneWay = true)]
        void Unregist(string UserName,string UserId);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="UserId">用户标识</param>
        /// <param name="Message">发送的消息</param>
        [OperationContract(IsOneWay = true)]
        void SendMessage(string UserName, string UserId, DuplexMessage Message);

        /// <summary>
        /// 获得在线用户列表
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="UserId">用户标识</param>
        [OperationContract(IsOneWay = true)]
        void GetOnlineUsers(string UserName, string UserId);
    }
}
