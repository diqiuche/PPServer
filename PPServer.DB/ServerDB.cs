using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPServer.DB
{
    /// <summary>
    /// 数据库操作类集合，请使用单例模式
    /// </summary>
    public class ServerDB
    {
        /// <summary>
        /// 日志触发代理
        /// </summary>
        /// <param name="Content">日志内容</param>
        /// <param name="IsError">是否为错误日志</param>
        public delegate void OnWriteLogDlg(string Content, bool IsError = false);
        /// <summary>
        /// 当需要写入日志时触发
        /// </summary>
        public static event OnWriteLogDlg OnWriteLog;
        //数据库实例
        private static ServerDB _instance;
        /// <summary>
        /// 获得数据库实例
        /// </summary>
        public static ServerDB Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ServerDB();
                return _instance;
            }
        }
        /// <summary>
        /// 初始化数据库连接
        /// </summary>
        /// <param name="DBName">数据库名称</param>
        /// <param name="InitParams">参数集合,请按以下格式传参，没有数据的参数请传null，
        /// 
        /// A.MSSQL参数：0:数据库地址,1:端口号,2:是否使用本地身份验证,3:用户名,4:密码,5:当前数据库,6:其他参数
        /// 
        /// B.SQLite参数：0.数据库文件地址，1.密码
        /// 
        /// C.ORACLE参数：
        /// </param>
        public bool InitDBConnectionsFactory(string DBName,params object[] InitParams)
        {
            bool _initResult = false;
            switch (DBName.ToLower())
            {
                case "mssql":
                    MsSQL _msDB = new MsSQL();
                    _initResult=_msDB.Init(InitParams);
                    Instance.DBList.Add(_msDB.DBName, _msDB);
                    break;
                case "sqlite":
                    SQLite _liteDB = new SQLite();
                    _initResult=_liteDB.Init(InitParams);
                    Instance.DBList.Add(_liteDB.DBName, _liteDB);
                    break;
                case "mysql":
                    MySQL _myDB = new MySQL();
                    _initResult = _myDB.Init(InitParams);
                    Instance.DBList.Add(_myDB.DBName, _myDB);
                    break;
                default:
                    break;
            }
            SetCurrentDB(DBName);
            return _initResult;
        }

        internal static void DB_OnWriteLog(string Content, bool IsError = false)
        {
            if (OnWriteLog != null)
                OnWriteLog(Content, IsError);
        }
        /// <summary>
        /// 设置当前默认的数据库连接
        /// </summary>
        /// <param name="CurrentDBName">当前数据库名称</param>
        public void SetCurrentDB(string CurrentDBName)
        {
            if (!string.IsNullOrEmpty(CurrentDBName))
            {
                foreach (string dbName in Instance.DBList.Keys)
                {
                    Instance._currentDB = Instance.DBList[dbName];
                    if (CurrentDBName.ToLower() == dbName.ToLower())
                        break;
                }
            }
        }
        private Dictionary<string,IDatabase> _DBList;
        private IDatabase _currentDB;
        /// <summary>
        /// 当前数据库连接
        /// </summary>
        public IDatabase CurrentDB
        {
            get
            {
                return _currentDB;
            }
        }
        /// <summary>
        /// 数据库连接列表
        /// </summary>
        public Dictionary<string, IDatabase> DBList
        {
            get
            {
                if (_DBList == null)
                    _DBList = new Dictionary<string, IDatabase>();
                return _DBList;
            }
        }
    }
}
