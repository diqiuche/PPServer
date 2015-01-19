using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPServer.Interface
{
    /// <summary>
    /// 外部服务处理函数接口
    /// </summary>
    public interface IPublicFunction
    {
        /// <summary>
        /// 处理函数
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Paras">参数集合</param>
        /// <returns></returns>
        string DoFunction(string UserName, string[] Paras);
    }
}
