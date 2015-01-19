using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/7/2
 * Description:对外公开服务，基于WebService
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.Service
{
    [ServiceContract]
    public interface IPublicService : ServiceBase
    {
        /// <summary>
        /// 对外服务
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="FunctionName">功能名</param>
        /// <param name="Paras">参数集合</param>
        /// <returns>返回数组大小为5，如下定义：
        /// [0]:返回代码，1为成功，0为系统性错误,负数为业务处理失败
        /// [1]:返回消息，执行未成功的情况下的错误信息
        /// [2]:执行时长
        /// [3]:执行完毕服务端时间
        /// [4]:执行返回数据内容字符串</returns>
        [OperationContract]
        List<string> DoService(string UserName, string Password, string FunctionName, string[] Paras);

        /// <summary>
        /// 不返回的对外服务
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="FunctionName">功能名</param>
        /// <param name="Paras">参数集合</param>
        [OperationContract]
        void DoServiceNoReturn(string UserName, string Password, string FunctionName, string Paras);
    }
}
