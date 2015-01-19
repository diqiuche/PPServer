using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PPServer.Object;
using System.Xml.Serialization;

namespace PPServer.Config
{
    [Serializable]
    public class Folder : ObjectBase
    {
        /// <summary>
        /// 文件夹映射名
        /// </summary>
        [XmlAttribute]
        public string Name;
        /// <summary>
        /// 文件夹路径
        /// </summary>
        [XmlAttribute]
        public string Path;
        /// <summary>
        /// 是否可被下载
        /// </summary>
        [XmlAttribute]
        public bool CanDownload = false;

        /// <summary>
        /// 实际的目录路径
        /// </summary>
        [XmlIgnore]
        public string RealPath;
    }
}
