using System;
using System.Collections.Generic;
using System.Text;
using PPServer.Object;
using System.Xml.Serialization;

namespace PPServer.Config
{
    [Serializable]
    public class FuncLib : ObjectBase
    {
        /// <summary>
        /// 功能库文件名
        /// </summary>
        [XmlAttribute]
        public string FilePath;

        /// <summary>
        /// 文件真实路径
        /// </summary>
        public string FileRealPath;
    }
}
