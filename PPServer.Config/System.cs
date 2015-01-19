using System;
using System.Collections.Generic;
using System.Text;
using PPServer.Object;
using System.Xml.Serialization;

namespace PPServer.Config
{
    [Serializable]
    public class System
    {
        /// <summary>
        /// 离线消息超时时间(d)
        /// </summary>
        [XmlElement]
        public int OfflineMessageTimeout;

        /// <summary>
        /// 映射文件夹
        /// </summary>
        [XmlElement]
        public MapFolders MapFolders;

        /// <summary>
        /// 用户类型与用户名之间分隔符
        /// </summary>
        [XmlElement]
        public string UserTypeSplitChar;

        /// <summary>
        /// 是否为调试模式
        /// </summary>
        [XmlElement]
        public bool DebugMode=false;

        /// <summary>
        /// 是否为写日志文件
        /// </summary>
        [XmlElement]
        public bool WriteLog = true;
    }
}
