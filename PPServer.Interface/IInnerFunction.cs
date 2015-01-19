using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPServer.Interface
{
    /// <summary>
    /// 内部服务处理函数接口
    /// </summary>
    public interface IInnerFunction
    {
        /// <summary>
        /// 执行服务端方法
        /// </summary>
        /// <param name="UserName">执行的用户</param>
        /// <param name="Paras">由客户端传输的参数集合</param>
        /// <returns></returns>
        Dictionary<string, object> DoFunction(string UserName, List<object> Paras);
        /// <summary>
        /// 执行服务端方法
        /// </summary>
        /// <param name="UserName">执行的用户</param>
        /// <param name="PrivateFunctionName">私有方法名</param>
        /// <param name="Paras">由客户端传输的参数集合</param>
        /// <returns></returns>
        Dictionary<string, object> DoFunction(string UserName, string PrivateFunctionName, List<object> Paras);
    }
}
