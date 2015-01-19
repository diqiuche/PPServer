using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;
using System.IO;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/7/2
 * Description:定义传输的文件消息，可用来上传文件
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
    public class FileMessage
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
        /// 获取文件路径
        /// </summary>
        private string FilePath
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
        /// 获取传输文件长度(Byte)
        /// </summary>
        [MessageHeader]
        public long TransLength
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

        #region 文件附加属性

        /// <summary>
        /// 是否被压缩
        /// </summary>
        [MessageHeader]
        public bool IsCompressed
        {
            private set;
            get;
        }
        /// <summary>
        /// 是否被加密
        /// </summary>
        [MessageHeader]
        public bool IsEncrypted
        {
            private set;
            get;
        }

        /// <summary>
        /// 获取上传或下载完毕服务端执行方法，key为方法名，value为参数集合，object须为可序列化对象
        /// </summary>
        [MessageHeader]
        public Dictionary<string, List<object>> AfterFunctions
        {
            set;
            get;
        }
        /// <summary>
        /// 获取上传或下载之前服务端执行方法，key为方法名，value为参数集合，object须为可序列化对象
        /// </summary>
        [MessageHeader]
        public Dictionary<string, List<object>> BeforeFunctions
        {
            set;
            get;
        }
        /// <summary>
        /// 系统信息
        /// </summary>
        [MessageHeader]
        public Dictionary<string, object> SystemInfo
        {
            private set;
            get;
        }
        /// <summary>
        /// 用户数据信息
        /// </summary>
        [MessageHeader]
        public Dictionary<string, object> UserInfo
        {
            set;
            get;
        }
        /// <summary>
        /// 执行返回结果
        /// </summary>
        [MessageHeader]
        public ReturnResult Result;
        #endregion

        #region 文件流
        /// <summary>
        /// 获取文件内容流
        /// </summary>
        [MessageBodyMember]
        public Stream FileData
        {
            private set;
            get;
        }
        #endregion
        /// <summary>
        /// 临时文件后缀名
        /// </summary>
        public const string TEMPFILE_SUFFIX = ".bz2";   //临时文件后缀名
        /// <summary>
        /// 读取数据时本地缓存大小
        /// </summary>
        public const int BUFFER_SIZE = 4096;            //读取数据时本地缓存大小
        /// <summary>
        /// 反馈处理进度句柄
        /// </summary>
        /// <param name="FileInfo">文件消息</param>
        /// <param name="FileName">文件名</param>
        /// <param name="Progress">处理进度</param>
        /// <param name="CurrentStatus">当前处理状态</param>
        public delegate void ReportProgressHandler(FileMessage FileInfo, string FileName, double Progress, TransportStatus CurrentStatus);
        /// <summary>
        /// 反馈处理完成句柄
        /// </summary>
        /// <param name="FileInfo">文件消息</param>
        /// <param name="FileName">文件名</param>
        public delegate void ReportCompleteHandler(FileMessage FileInfo, string FileName);
        /// <summary>
        /// 反馈读取进度
        /// </summary>
        public event ReportProgressHandler OnReadProgress;

        /// <summary>
        /// 反馈写入本地文件进度
        /// </summary>
        public event ReportProgressHandler OnWriteProgress;

        /// <summary>
        /// 当读取完成后触发
        /// </summary>
        public event ReportCompleteHandler OnReadCompleted;
        /// <summary>
        /// 传输状态
        /// </summary>
        public enum TransportStatus
        { 
            /// <summary>
            /// 压缩中
            /// </summary>
            Compressing,
            /// <summary>
            /// 解压中
            /// </summary>
            DeCompressing,
            /// <summary>
            /// 上传中
            /// </summary>
            Uploading,
            /// <summary>
            /// 下载中
            /// </summary>
            Downloading
        }
        #region 构造函数
        /// <summary>
        /// 构造文件消息
        /// </summary>
        public FileMessage()
        {
            MessageId = Guid.NewGuid().ToString();
            MessageTime = DateTime.Now;
        }
        /// <summary>
        /// 构造文件传输消息
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <param name="ToCompress">是否压缩传输</param>
        /// <param name="ToEncrypt">是否加密传输</param>
        public FileMessage(string FilePath,bool ToCompress=true,bool ToEncrypt=false)
        {
            MessageId = Guid.NewGuid().ToString();
            MessageTime = DateTime.Now;

            //文件绝对路径
            string _fileFullPath = string.Empty;
            SystemInfo = new Dictionary<string, object>();
            if (FilePath.Contains(@":\"))
            {
                //绝对路径
                _fileFullPath = FilePath;
            }
            else
            { 
                //相对路径
                _fileFullPath = AppDomain.CurrentDomain.BaseDirectory + (FilePath.StartsWith(@"\") ? FilePath .Substring(1): FilePath);
            }
            if (File.Exists(_fileFullPath))
            {
                FileInfo _fi = new FileInfo(_fileFullPath);
                this.FileName = _fi.Name;
                this.FilePath = FilePath;
                this.Length = _fi.Length;
                this.TransLength = _fi.Length;
                this.CreateTime = _fi.CreationTime;
                this.ModifyTime = _fi.LastWriteTime;
                this.IsCompressed = false;
                this.IsEncrypted = false;

                try
                {
                    this.FileData = new TransportStream(_fi.FullName);

                    //效验码
                    this.VerifyCode = TransportStream.GetFileVerifyCode(this.FileData);
                    //压缩
                    if (ToCompress)
                    {
                        ReportProgress(0, TransportStatus.Compressing);
                        this.FileData = TransportStream.CompressFile(this.FileData, _fi.FullName + TEMPFILE_SUFFIX);
                        ReportProgress(100, TransportStatus.Compressing);
                        this.IsCompressed = true;
                        this.TransLength = this.FileData.Length;
                    }
                    //加密
                    if (ToEncrypt)
                    {
                        this.IsEncrypted = true;
                    }
                    ((TransportStream)this.FileData).OnReadCompleted += new EventHandler<TransportEventArgs>(FileMessage_ReadCompleted);
                    ((TransportStream)this.FileData).OnReadProgress += new Action<double>(FileMessage_OnReadProgress);
                }
                catch (Exception ex)
                {                    
                    throw new Exception("初始化文件消息" + _fileFullPath + "发生异常："+ex.Message);
                }
            }
            else
                throw new Exception("初始化文件消息出错：文件" + _fileFullPath + "不存在！");
        }

        /// <summary>
        /// 向外爆破上传处理事件
        /// </summary>
        /// <param name="Progress">读取进度</param>
        void FileMessage_OnReadProgress(double Progress)
        {
            ReportProgress(Progress, TransportStatus.Uploading);
        }
        /// <summary>
        /// 反馈处理进度
        /// </summary>
        /// <param name="Progress">进度</param>
        /// <param name="Status">状态</param>
        void ReportProgress(double Progress,TransportStatus Status)
        {
            if ((Status== TransportStatus.Uploading || Status== TransportStatus.Compressing) && OnReadProgress != null)
                OnReadProgress(this, FilePath, Progress, Status);
            else if ((Status == TransportStatus.Downloading || Status == TransportStatus.DeCompressing) && OnWriteProgress != null)
                OnWriteProgress(this, FilePath, Progress, Status);
        }

        /// <summary>
        /// 当读取完文件后，删除压缩文件
        /// </summary>
        /// <param name="sender">文件流</param>
        /// <param name="e">文件路径</param>
        void FileMessage_ReadCompleted(object sender, TransportEventArgs e)
        {
            try
            {
                e.Sender.Close();
                if (this.IsCompressed && e.FileName.EndsWith(TEMPFILE_SUFFIX))
                {
                    File.Delete(e.FileName);
                }
                if (OnReadCompleted != null)
                    OnReadCompleted(this, e.FileName);
            }
            catch { }
        }        
        #endregion

        #region 一般方法
        /// <summary>
        /// 添加事后执行方法
        /// </summary>
        /// <param name="FunctionName">方法名</param>
        /// <param name="Param">参数集合</param>
        public void AddAfterFunction(string FunctionName, params object[] Param)
        {
            if (AfterFunctions == null)
                AfterFunctions = new Dictionary<string, List<object>>();
            AfterFunctions.Add(FunctionName, Param == null ? null : Param.ToList());
        }
        /// <summary>
        /// 添加事前执行方法
        /// </summary>
        /// <param name="FunctionName">方法名</param>
        /// <param name="Param">参数集合</param>
        public void AddBeforeFunction(string FunctionName, params object[] Param)
        {
            if (BeforeFunctions == null)
                BeforeFunctions = new Dictionary<string, List<object>>();
            BeforeFunctions.Add(FunctionName, Param == null ? null : Param.ToList());
        }
        /// <summary>
        /// 添加系统信息
        /// </summary>
        /// <param name="InfoName">信息名称</param>
        /// <param name="Info">信息内容</param>
        public void AddSystemInfo(string InfoName, object Info)
        {
            if (SystemInfo == null)
                SystemInfo = new Dictionary<string, object>();
            SystemInfo.Add(InfoName, Info);
        }

        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="InfoName">信息名称</param>
        /// <param name="Info">信息内容</param>
        public void AddUserInfo(string InfoName, object Info)
        {
            if (UserInfo == null)
                UserInfo = new Dictionary<string, object>();
            UserInfo.Add(InfoName, Info);
        }


        /// <summary>
        /// 拷贝数据流至数据流
        /// </summary>
        /// <param name="FromStream">源数据流</param>
        /// <param name="ToStream">目的数据流</param>
        public void CopyStream(Stream FromStream, Stream ToStream)
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            int count = 0, offset = 0;
            try
            {
                while ((count = FromStream.Read(buffer, offset, BUFFER_SIZE - offset)) > 0)
                {
                    offset += count;
                    if (offset == BUFFER_SIZE)
                    {
                        ToStream.Write(buffer, 0, offset);
                        offset = 0;
                    }
                }
                if (offset > 0)
                    ToStream.Write(buffer, 0, offset);
            }
            catch
            {
            }
            finally
            {
                //position = ToStream.Position;
                FromStream.Dispose();
                ToStream.Dispose();
            }
        }

        /// <summary>
        /// 拷贝数据流至文件
        /// </summary>
        /// <param name="FromStream">源数据流</param>
        /// <param name="ToFileName">目的文件</param>
        /// <param name="FileLenth">源流长度</param>
        /// <returns>返回目的文件流</returns>
        public Stream CopyStream(Stream FromStream, string ToFileName,long FileLenth)
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            int count = 0, offset = 0;
            try
            {
                Stream _toStream = new FileStream(ToFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                if (OnWriteProgress != null)
                    OnWriteProgress(this,ToFileName.TrimEnd(TEMPFILE_SUFFIX.ToCharArray()), 0, TransportStatus.Downloading);
                while ((count = FromStream.Read(buffer, offset, BUFFER_SIZE - offset)) > 0)
                {
                    offset += count;
                    if (offset == BUFFER_SIZE)
                    {
                        _toStream.Write(buffer, 0, offset);
                        offset = 0;
                        
                    if (OnWriteProgress != null)
                        OnWriteProgress(this, ToFileName.TrimEnd(TEMPFILE_SUFFIX.ToCharArray()), Math.Round(_toStream.Length * 100 / (double)FileLenth, 2), TransportStatus.Downloading);
                    }
                }
                if (offset > 0)
                    _toStream.Write(buffer, 0, offset);
                if (OnWriteProgress != null)
                    OnWriteProgress(this, ToFileName.TrimEnd(TEMPFILE_SUFFIX.ToCharArray()), 100, TransportStatus.Downloading);
                return _toStream;
            }
            catch
            {
            }
            finally
            {
                FromStream.Dispose();
            }
            return null;
        }
        #endregion
    }    
}
