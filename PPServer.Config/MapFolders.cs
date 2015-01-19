using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PPServer.Object;
using System.Xml.Serialization;

namespace PPServer.Config
{
    [Serializable]
    public class MapFolders : ObjectBase
    {
        /// <summary>
        /// 系统文件夹列表
        /// </summary>
        [XmlElement]
        public List<Folder> Folder;

        /// <summary>
        /// 文件夹列表
        /// </summary>
        private Dictionary<string, Folder> _folderList;

        /// <summary>
        /// 获取系统文件夹列表
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, Folder> FolderList
        { 
            get
            {
                if (_folderList==null)
                {
                    _folderList = new Dictionary<string, Folder>();
                    if (Folder != null && Folder.Count > 0)
                    {
                        foreach (Folder folder in Folder)
                        {
                            try
                            {
                                folder.Path = "\\"+folder.Path.Trim().Replace('/', '\\').Trim('\\')+"\\";
                                //本地文件夹路径
                                folder.RealPath = (folder.Path.Contains(":\\") ? folder.Path.Trim('\\') : (Common.CommonData.AppPath + folder.Path.Trim('\\'))) + "\\";
                                _folderList.Add(folder.Name.Trim(), folder);
                            }
                            catch (Exception ex)
                            {
                                Common.Log.WriteToLog(folder.Name + ":" + ex.Message, true);
                            }
                        }
                    }
                }
                return _folderList;
            }
        }
    }
}
