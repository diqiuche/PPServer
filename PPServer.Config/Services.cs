using System;
using System.Collections.Generic;
using System.Text;
using PPServer.Object;
using System.Xml.Serialization;

namespace PPServer.Config
{
    [Serializable]
    public class Services : ObjectBase
    {
        /// <summary>
        /// 服务器IP地址
        /// </summary>
        [XmlAttribute]
        public string ServerIP;
        /// <summary>
        /// Http服务端口号
        /// </summary>
        [XmlAttribute]
        public int HttpPort;
        /// <summary>
        /// Tcp服务端口号
        /// </summary>
        [XmlAttribute]
        public int TcpPort;
        /// <summary>
        /// 双工服务
        /// </summary>
        [XmlElement]
        public Service Duplex;
        /// <summary>
        /// 内部服务
        /// </summary>
        [XmlElement]
        public Service Inner;
        /// <summary>
        /// 对外服务
        /// </summary>
        [XmlElement]
        public Service Public;
        /// <summary>
        /// 对外服务
        /// </summary>
        [XmlElement]
        public Service File;

        /// <summary>
        /// 传输的对象
        /// </summary>
        [XmlElement]
        public TransportObjects TransportObjects;
    }
}
