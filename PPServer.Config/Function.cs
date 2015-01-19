using System;
using System.Collections.Generic;
using System.Text;
using PPServer.Object;
using System.Xml.Serialization;

namespace PPServer.Config
{
    [Serializable]
    public class Function : ObjectBase
    {
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public string FilePath;

        [XmlAttribute]
        public string ClassName;
    }
}
