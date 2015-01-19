using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPServer.Client
{
    /// <summary>
    /// 执行服务端方法
    /// </summary>
    public class DoServerFunction
    {
        /// <summary>
        /// 设置或获取服务端配置的方法名
        /// </summary>
        public string FunctionName
        {
            get;
            set;
        }
        /// <summary>
        /// 设置或获取私有方法名
        /// </summary>
        public string PrivateFunctionName
        {
            get;
            set;
        }
        /// <summary>
        /// 设置或获取传输的参数集合
        /// </summary>
        public object[] Paras
        {
            get;
            set;
        }
        /// <summary>
        /// 构造服务端执行方法类
        /// </summary>
        /// <param name="FunctionName">服务端配置的方法名</param>
        /// <param name="Paras">传输的参数集合</param>
        public DoServerFunction(string FunctionName, object[] Paras=null)
        {
            this.FunctionName = FunctionName;
            this.PrivateFunctionName = null;
            this.Paras = Paras;
        }
        /// <summary>
        /// 构造服务端执行方法类
        /// </summary>
        /// <param name="FunctionName">服务端配置的方法名</param>
        /// <param name="PrivateFunctionName">私有方法名</param>
        /// <param name="Paras">传输的参数集合</param>
        public DoServerFunction(string FunctionName,string PrivateFunctionName, params object[] Paras)
        {
            this.FunctionName = FunctionName;
            this.PrivateFunctionName = PrivateFunctionName;
            this.Paras = Paras;
        }
        /// <summary>
        /// 返回当前对象的字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FunctionName + (string.IsNullOrEmpty(PrivateFunctionName) ? string.Empty : ("/" + PrivateFunctionName));
        }
    }
}
