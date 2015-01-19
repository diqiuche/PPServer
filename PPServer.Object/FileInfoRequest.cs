using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;
using System.IO;

/*************************************************
 * Creater:liuyan
 * Version:0.2.1.2
 * Create Date:2013/10/12
 * Description:定义传输的文件基本信息，可用来更新文件
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.Object
{
    /// <summary>
    /// 文件上传消息
    /// </summary>
    [MessageContract]
    public class FileInfoRequest
    {
        #region 文件基本属性
        /// <summary>
        /// 获取消息唯一标示
        /// </summary>
        [MessageHeader]
        public string MessageId
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取消息创建时间
        /// </summary>
        [MessageHeader]
        public DateTime MessageTime
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取文件名
        /// </summary>
        [MessageHeader]
        public string FileName
        {
            private set;
            get;
        }
        /// <summary>
        /// 获取文件路径文件夹
        /// </summary>
        public string FileFolder
        {
            set;
            get;
        }

        /// <summary>
        /// 获取文件版本号
        /// </summary>
        public string FileVersion
        {
            set;
            get;
        }
        /// <summary>
        /// 获取文件长度(Byte)
        /// </summary>
        [MessageHeader]
        public long Length
        {
            private set;
            get;
        }

        /// <summary>
        /// 获取文件修改时间
        /// </summary>
        [MessageHeader]
        public DateTime ModifyTime
        {
            private set;
            get;
        }
        /// <summary>
        /// 获取文件创建时间
        /// </summary>
        [MessageHeader]
        public DateTime CreateTime
        {
            private set;
            get;
        }
        /// <summary>
        /// 获取文件校验码
        /// </summary>
        [MessageHeader]
        public string VerifyCode
        {
            private set;
            get;
        }
        #endregion
                
        #region 构造函数
        /// <summary>
        /// 构造文件消息
        /// </summary>
        public FileInfoRequest()
        {
            MessageId = Guid.NewGuid().ToString();
            MessageTime = DateTime.Now;
        }
        /// <summary>
        /// 构造文件传输消息
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        public FileInfoRequest(string FilePath)
        {
            MessageId = Guid.NewGuid().ToString();
            MessageTime = DateTime.Now;

            //文件绝对路径
            string _fileFullPath = string.Empty;
            if (FilePath.Contains(@":\"))
            {
                //绝对路径
                _fileFullPath = FilePath;
            }
            else
            { 
                //相对路径
                _fileFullPath = AppDomain.CurrentDomain.BaseDirectory + FilePath.Trim('\\');
                FilePath = "\\" + FilePath.Trim('\\');                
            }
            if (File.Exists(_fileFullPath))
            {
                FileInfo _fi = new FileInfo(_fileFullPath);
                this.FileName = _fi.Name;
                this.FileFolder = FilePath.Substring(0, FilePath.LastIndexOf('\\') + 1);
                this.Length = _fi.Length;
                this.CreateTime = _fi.CreationTime;
                this.ModifyTime = _fi.LastWriteTime;
                this.FileVersion = GetFileVersion(_fileFullPath);
                //效验码
                this.VerifyCode = TransportStream.GetFileVerifyCode(_fileFullPath);
            }
            else
                throw new Exception("文件" + _fileFullPath + "不存在！");
        }

        /// <summary>
        /// 得到文件版本号
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        public static string GetFileVersion(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(FilePath);
                return fileInfo.FileVersion;
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion
    }    
}
