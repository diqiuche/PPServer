using System;
using System.Collections.Generic;
using System.Text;
using PPServer.Object;
using System.Xml.Serialization;

namespace PPServer.Config
{
    [Serializable]
    public class Database : ObjectBase
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        [XmlAttribute]
        public bool Enable = false;
        /// <summary>
        /// 数据库服务器地址
        /// </summary>
        [XmlAttribute]
        public string DBServer;
        /// <summary>
        /// 连接端口
        /// </summary>
        [XmlAttribute]
        public int Port;
        /// <summary>
        /// 初始数据库
        /// </summary>
        [XmlAttribute]
        public string InitDB;
        /// <summary>
        /// 登陆用户
        /// </summary>
        [XmlAttribute]
        public string UserName;
        /// <summary>
        /// 登陆密码
        /// </summary>
        [XmlAttribute]
        public string Password;
        /// <summary>
        /// 其他连接参数
        /// </summary>
        [XmlAttribute]
        public string OtherParams;
        /// <summary>
        /// 是否使用Windows身份验证（仅对SQL Server配置有效）
        /// </summary>
        [XmlAttribute]
        public bool UseWindowsAuthentication;
    }
}
