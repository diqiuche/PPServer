using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PPServer.Object;
using System.Xml.Serialization;

namespace PPServer.Config
{
    [Serializable]
    public class ObjectFile : ObjectBase
    {
        /// <summary>
        /// 配置的文件路径
        /// </summary>
        [XmlAttribute]
        public string FilePath;
    }
}
