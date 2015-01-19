using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPServer.Interface
{
    /// <summary>
    /// 文件处理函数接口
    /// </summary>
    public interface IFileFunction
    {
        /// <summary>
        /// 执行服务端方法
        /// </summary>
        /// <param name="UserName">执行的用户</param>
        /// <param name="FileFullPath">上传的文件实际路径</param>
        /// <param name="Paras">由客户端传输的参数集合</param>
        void DoFunction(string UserName,string FileFullPath, List<object> Paras);
        /// <summary>
        /// 执行服务端方法
        /// </summary>
        /// <param name="UserName">执行的用户</param>
        /// <param name="FileFullPath">上传的文件实际路径</param>
        /// <param name="PrivateFunctionName">私有方法名</param>
        /// <param name="Paras">由客户端传输的参数集合</param>
        void DoFunction(string UserName, string FileFullPath, string PrivateFunctionName, List<object> Paras);
    }
}
