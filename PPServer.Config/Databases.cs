using System;
using System.Collections.Generic;
using System.Text;
using PPServer.Object;
using System.Xml.Serialization;

namespace PPServer.Config
{
    [Serializable]
    public class Databases : ObjectBase
    {
        /// <summary>
        /// Oracle配置
        /// </summary>
        [XmlElement]
        public Database Oracle;
        /// <summary>
        /// SQL Server配置
        /// </summary>
        [XmlElement]
        public Database MsSql;
        /// <summary>
        /// SQLite配置（未实现）
        /// </summary>
        [XmlElement]
        public Database SQLite;
        /// <summary>
        /// Access配置（未实现）
        /// </summary>
        [XmlElement]
        public Database Access;
        /// <summary>
        /// MySQL配置
        /// </summary>
        [XmlElement]
        public Database MySql;
        /// <summary>
        /// 当前数据库
        /// </summary>
        [XmlAttribute]
        public string CurrentDB;
    }
}
