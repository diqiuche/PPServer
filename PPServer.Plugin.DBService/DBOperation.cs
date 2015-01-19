using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using PPServer.Interface;

namespace PPServer.Plugin.DBService
{
    /// <summary>
    /// 操作数据库
    /// </summary>
    public class DBOperation : IInnerFunction
    {
        /// <summary>
        /// 执行服务端方法
        /// </summary>
        /// <param name="UserName">执行的用户</param>
        /// <param name="Paras">由客户端传输的参数集合</param>
        /// <returns></returns>
        public Dictionary<string, object> DoFunction(string UserName, List<object> Paras)
        {
            Dictionary<string, object> _result = new Dictionary<string, object>();
            string _sql = Paras[0].ToString();
            DataSet _ds=PPServer.DB.ServerDB.Instance.CurrentDB.ExecuteDataSet(_sql);
            _result.Add("DBResult", _ds.GetXml()); ;
            return _result;
        }
        /// <summary>
        /// 执行服务端方法
        /// </summary>
        /// <param name="UserName">执行的用户</param>
        /// <param name="PrivateFunctionName">私有方法名</param>
        /// <param name="Paras">由客户端传输的参数集合</param>
        /// <returns></returns>
        public Dictionary<string, object> DoFunction(string UserName, string PrivateFunctionName, List<object> Paras)
        {
            throw new NotImplementedException();
        }
    }
}
