using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PPServer.Object;
using System.Xml.Serialization;

namespace PPServer.Config
{
    [Serializable]
    public class TransportObjects : ObjectBase
    {
        [XmlElement]
        public List<ObjectFile> ObjectFile;

        /// <summary>
        /// 初始化传输对象的路径
        /// </summary>
        /// <param name="PluginFolder"></param>
        public void InitObjects(string PluginFolder)
        {
            if (ObjectFile != null)
                foreach (ObjectFile obj in ObjectFile)
                {
                    obj.FilePath = obj.FilePath.Contains(":\\") ? obj.FilePath : (PluginFolder + obj.FilePath.Trim('\\'));
                    if (!File.Exists(obj.FilePath))
                        throw new Exception("配置文件错误:未能正确加载TransportObjects的文件" + obj.FilePath + "，请检查文件是否存在！");
                }
            else
                ObjectFile = new List<ObjectFile>();
        }
    }
}
