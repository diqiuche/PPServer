using System;
using System.Collections.Generic;
using System.Text;
using PPServer.Object;
using System.Xml.Serialization;

namespace PPServer.Config
{
    [Serializable]
    public class FuncCategory : ObjectBase
    {
        /// <summary>
        /// 功能模块分类名
        /// </summary>
        [XmlAttribute]
        public string Name;

        /// <summary>
        /// 功能模块列表
        /// </summary>
        [XmlElement]
        public List<Function> Function;
    }
}
