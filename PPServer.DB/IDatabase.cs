using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace PPServer.DB
{
    /// <summary>
    /// 数据库处理接口
    /// </summary>
    public interface IDatabase
    {
        #region 基础执行方法
        /// <summary>
        /// 初始化数据库连接
        /// </summary>
        /// <param name="ConnectionParams">连接参数</param>
        /// <returns></returns>
        bool Init(params object[] ConnectionParams);
        /// <summary>
        /// 执行SQL语句并返回影响行数
        /// </summary>
        /// <param name="SQL">SQL语句</param>
        /// <param name="Params">参数集合</param>
        /// <returns></returns>
        int ExecuteSQL(string SQL, DbParameter[] Params=null);
        /// <summary>
        /// 执行SQL语句并返回Reader
        /// </summary>
        /// <param name="SQL">SQL语句</param>
        /// <param name="Params">参数集合</param>
        /// <param name="IsProcedure">是否为存储过程</param>
        /// <returns></returns>
        DbDataReader ExecuteReader(string SQL, DbParameter[] Params = null, bool IsProcedure = false);
        /// <summary>
        /// 执行SQL语句并返回第一行第一个字段的数据值
        /// </summary>
        /// <param name="SQL">SQL语句</param>
        /// <param name="Params">参数集合</param>
        /// <returns></returns>
        object ExecuteScalar(string SQL, DbParameter[] Params = null);
        /// <summary>
        /// 执行SQL语句并返回数据集
        /// </summary>
        /// <param name="SQL">SQL语句</param>
        /// <param name="Params">参数集合</param>
        /// <param name="IsProcedure">是否为存储过程</param>
        /// <returns></returns>
        DataSet ExecuteDataSet(string SQL, DbParameter[] Params = null, bool IsProcedure = false);

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="Name">参数名称</param>
        /// <param name="Value">参数值</param>
        /// <returns></returns>
        DbParameter CreateParameter(string Name, object Value);
        #endregion

        #region 扩展执行方法
        /// <summary>
        /// 执行存储过程并返回Reader
        /// </summary>
        /// <param name="ProcName">存储过程名</param>
        /// <param name="Params">参数集合</param>
        /// <returns></returns>
        DbDataReader RunProcedureReader(string ProcName, DbParameter[] Params = null);
        /// <summary>
        /// 执行存储过程并返回数据集
        /// </summary>
        /// <param name="ProcName">存储过程名</param>
        /// <param name="Params">参数集合</param>
        /// <returns></returns>
        DataSet RunProcedureDataSet(string ProcName, DbParameter[] Params = null);
        /// <summary>
        /// 插入数据至表并返回影响的行数
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="Params">参数集合</param>
        /// <returns></returns>
        int Insert(string TableName, DbParameter[] Params);
        /// <summary>
        /// 更新数据至表并返回影响的行数
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="Params">参数集合</param>
        /// <param name="WhereParams">查询条件参数集合</param>
        /// <returns></returns>
        int Update(string TableName, DbParameter[] Params, DbParameter[] WhereParams);
        /// <summary>
        /// 从表中删除数据至表并返回影响的行数
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="Params">参数集合</param>
        /// <returns></returns>
        int Delete(string TableName, DbParameter[] Params);
        /// <summary>
        /// 以传参方式查询数据并返回Reader
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="Fields">字段集合，以逗号分隔，当为null时等同于*</param>
        /// <param name="WhereParams">查询条件参数集合</param>
        /// <param name="Order">排序规则</param>
        /// <returns></returns>
        DbDataReader SelectReader(string TableName, string Fields = null, DbParameter[] WhereParams = null, string Order = null);
        /// <summary>
        /// 以字符串方式查询数据并返回Reader
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="Fields">字段集合，以逗号分隔，当为null时等同于*</param>
        /// <param name="Where">查询条件字符串</param>
        /// <param name="Order">排序规则</param>
        /// <returns></returns>
        DbDataReader SelectReader(string TableName, string Fields = null, string Where = null, string Order = null);
        /// <summary>
        /// 以传参方式查询数据并返回数据集
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="Fields">字段集合，以逗号分隔，当为null时等同于*</param>
        /// <param name="WhereParams">查询条件参数集合</param>
        /// <param name="Order">排序规则</param>
        /// <returns></returns>
        DataSet SelectDataSet(string TableName, string Fields = null, DbParameter[] WhereParams = null, string Order = null);
        /// <summary>
        /// 以字符串方式查询数据并返回数据集
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="Fields">字段集合，以逗号分隔，当为null时等同于*</param>
        /// <param name="Where">查询条件字符串</param>
        /// <param name="Order">排序规则</param>
        /// <returns></returns>
        DataSet SelectDataSet(string TableName, string Fields = null, string Where = null, string Order = null);
        #endregion
    }
}
