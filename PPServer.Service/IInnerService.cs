using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using PPServer.Object;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/7/2
 * Description:公开内部服务
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.Service
{    
    /// <summary>
    /// 内部服务接口
    /// </summary>
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(ServiceFuncLib))]
    public interface IInnerService : ServiceBase
    {
        /// <summary>
        /// 执行内部服务统一接口
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="FunctionName">公开的服务名(可以通过"公开服务名/私有服务名"来调用私有服务)</param>
        /// <param name="Param">参数集合</param>
        /// <returns></returns>
        [OperationContract]
        ReturnResult DoService(string UserName, string Password, string FunctionName, List<object> Param);
    }
}
