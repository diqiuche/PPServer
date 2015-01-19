using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPServer.Interface
{
    /// <summary>
    /// 认证处理函数接口
    /// </summary>
    public interface IAuthentication
    {
        /// <summary>
        /// 开始认证
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="UserType">用户类型</param>
        /// <param name="Password">密码</param>
        /// <returns>返回认证结果，正数为通过认证，0或负数为认证不通过</returns>
        int ToVerify(string UserName,string UserType, string Password);
    }
}
