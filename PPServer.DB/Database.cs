using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Linq;

namespace PPServer.DB
{
    internal abstract class Database
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DBName
        {
            protected set;
            get;
        }
        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool Inited
        {
            protected set;
            get;
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString
        {
            protected set;
            get;
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="Content">日志内容</param>
        /// <param name="IsError">是否为错误日志</param>
        protected void WriteToLog(string Content, bool IsError = false)
        {
            ServerDB.DB_OnWriteLog(Content, IsError);
        }

        /// <summary>
        /// 处理空数据的参数
        /// </summary>
        /// <param name="Params">参数集合</param>
        protected void DealNullParam(ref DbParameter[] Params)
        {
            if (Params != null)
            {
                Params.ToList().ForEach(t => t.Value = (t.Value == null ? DBNull.Value : t.Value));
            }
        }
    }
}
