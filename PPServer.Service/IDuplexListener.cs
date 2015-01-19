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
 * Create Date:2013/9/5
 * Description:公开双工服务监听(client使用)
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.Service
{
    [ServiceKnownType("GetKnownTypes", typeof(ServiceFuncLib))]
    public interface IDuplexListener
    {
        /// <summary>
        /// 收到消息
        /// </summary>
        /// <param name="Message"></param>
        [OperationContract(IsOneWay = true)]
        void OnReceive(DuplexMessage Message);
    }
}
