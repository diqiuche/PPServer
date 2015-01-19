using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel;
using ICSharpCode.SharpZipLib.BZip2;
using System.Security.Cryptography;
using System.IO.Compression;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/7/9
 * Description:定义只读的文件流
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.Object
{
    /// <summary>
    /// 自定义的读取流
    /// </summary>
    public class TransportStream : Stream
    {
        Stream _innerStream;            //内部实际流
        string _fileName;               //原文件名
        /// <summary>
        /// 读取完毕事件，参数为当前流的文件名
        /// </summary>
        public event EventHandler<TransportEventArgs> OnReadCompleted;

        /// <summary>
        /// 反馈读取进度，返回值为当前进度值
        /// </summary>
        public event Action<double> OnReadProgress;

        /// <summary>
        /// 从文件生成只读流
        /// </summary>
        /// <param name="FileName">文件路径名称</param>
        public TransportStream(string FileName)
        {
            this._innerStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            this._fileName = FileName;
        }

        /// <summary>
        /// 直接使用现有文件流
        /// </summary>
        /// <param name="FileStream">现有文件流</param>
        /// <param name="FileName">文件路径名称</param>
        public TransportStream(FileStream FileStream, string FileName)
        {
            this._innerStream = FileStream;
            this._fileName = FileName;
        }

        /// <summary>
        /// 从自定义流读取指定长度到array，但是不超过初始化时设定的长度。
        /// </summary>
        /// <returns>读取的字节数</returns>
        public override int Read(byte[] array, int offset, int count)
        {
            int readcount = 0;
            if (Position >= Length)
            {
                if (OnReadCompleted != null)
                    OnReadCompleted(this, new TransportEventArgs(this, this._fileName));
            }
            else
            {
                if (Position + count > Length)
                    readcount = _innerStream.Read(array, offset, (int)(Length - Position));
                else
                    readcount = _innerStream.Read(array, offset, count);
                if (OnReadProgress != null)
                    OnReadProgress(Math.Round(Position*100 / (double)Length,2));
            }
            
            return readcount;
        }
        /// <summary>
        /// 流是否可读
        /// </summary>
        public override bool CanRead
        {
            get { return _innerStream.CanRead; }
        }
        /// <summary>
        /// 流是否可查找
        /// </summary>
        public override bool CanSeek
        {
            get { return false; }
        }
        /// <summary>
        /// 流是否可写
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }
        /// <summary>
        /// 强制输出
        /// </summary>
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 自定义流剩余长度。
        /// </summary>
        public override long Length
        {
            get { return _innerStream.Length; }
        }

        /// <summary>
        /// 自定义流位置，返回原始流的位置
        /// </summary>
        public override long Position
        {
            get
            {
                return _innerStream.Position;
            }
            set
            {
                _innerStream.Position=value;
            }
        }
        /// <summary>
        /// 流查找
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <param name="origin">参考点</param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 设置流字节长度
        /// </summary>
        /// <param name="value">长度</param>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 写入流数据
        /// </summary>
        /// <param name="buffer">缓存区</param>
        /// <param name="offset">偏移量</param>
        /// <param name="count">数量</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 关闭流
        /// </summary>
        public override void Close()
        {
            _innerStream.Close();
            base.Close();
        }
        /// <summary>
        /// 销毁对象
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            _innerStream.Dispose();
            base.Dispose(disposing);
        }



        /// <summary>
        /// 压缩文件流
        /// </summary>
        /// <param name="OrgFileData">原文件流</param>
        /// <param name="ZipFileName">压缩后临时文件名</param>
        /// <param name="CompressLevel">压缩级别</param>
        /// <returns></returns>
        public static Stream CompressFile(Stream OrgFileData, string ZipFileName, CompressLevel CompressLevel= CompressLevel.High)
        {
            try
            {
                FileStream _tmpStream = new FileStream(ZipFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                GZipStream _zipStream = new GZipStream(_tmpStream, CompressionMode.Compress, true);
                BZip2.Compress(OrgFileData, _tmpStream, false, (int)CompressLevel);
                _tmpStream.Position = 0;
                OrgFileData.Close();
                //GZipStream _zip= new GZipStream(FileData, ToDeCompress ? CompressionMode.Decompress : CompressionMode.Compress,true);
                return new TransportStream(_tmpStream,ZipFileName);
            }
            catch (Exception ex)
            {
                throw new Exception("压缩文件流至" + ZipFileName + "出错：" + ex.Message);
            }
        }
        /// <summary>
        /// 解压缩文件流
        /// </summary>
        /// <param name="ZipFileData">压缩文件流</param>
        /// <param name="ZipFileName">压缩文件名</param>
        /// <param name="NewFileName">解压文件名</param>
        /// <param name="VerifyCode">校验码</param>
        /// <param name="DelZipFile">是否删除原压缩文件</param>
        /// <param name="VerifyOriginalFile">是否校验原始文件，否则校验压缩文件</param>
        /// <returns></returns>
        public static void DeCompressFile(Stream ZipFileData, string ZipFileName, string NewFileName, string VerifyCode = "", bool DelZipFile = true,bool VerifyOriginalFile=true)
        {
            FileStream _newStream = null;
            try
            {
                //校验压缩文件
                if (!VerifyOriginalFile && VerifyCode.Trim() != "" && VerifyCode.Trim().ToUpper() != GetFileVerifyCode(ZipFileData).Trim().ToUpper())
                    throw new Exception("校验码不匹配，请重新传输。");
                _newStream = new FileStream(NewFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                BZip2.Decompress(ZipFileData, _newStream, false);
                ZipFileData.Close();
                _newStream.Position = 0;
                //校验
                if (VerifyOriginalFile && VerifyCode.Trim() != "" && VerifyCode.Trim().ToUpper() != GetFileVerifyCode(_newStream).Trim().ToUpper())
                    throw new Exception("原文件校验码不匹配，请重新传输。");
                if (DelZipFile && File.Exists(ZipFileName))
                    File.Delete(ZipFileName);
            }
            catch (Exception ex)
            {
                throw new Exception("解压文件流至" + NewFileName + "出错：" + ex.Message);
            }
            finally
            {
                _newStream.Close();
            }
        }
        
        /// <summary>
        /// 获取文件校验码
        /// </summary>
        /// <param name="FileData">文件内容流</param>
        /// <returns></returns>
        public static string GetFileVerifyCode(Stream FileData)
        {
            try
            {
                MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();
                _md5.ComputeHash(FileData);
                StringBuilder _sb = new StringBuilder();
                foreach (byte b in _md5.Hash)
                {
                    _sb.Append(string.Format("{0:X1}", b));
                }
                FileData.Position = 0;
                return _sb.ToString();
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// 获取文件校验码
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        public static string GetFileVerifyCode(string FilePath)
        {
            try
            {
                if (!File.Exists(FilePath))
                    return string.Empty;
                StringBuilder _sb = new StringBuilder();
                using (Stream _fileData = new FileStream(FilePath, FileMode.Open))
                {
                    MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();
                    _md5.ComputeHash(_fileData);                    
                    foreach (byte b in _md5.Hash)
                    {
                        _sb.Append(string.Format("{0:X1}", b));
                    }
                }
                return _sb.ToString();
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// 文件压缩级别
        /// </summary>
        public enum CompressLevel
        { 
            /// <summary>
            /// 高压缩比，压缩率高，压缩时间长
            /// </summary>
            High=9,
            /// <summary>
            /// 普通压缩比，压缩率与压缩时间均衡
            /// </summary>
            Normal=5,
            /// <summary>
            /// 低压缩比，压缩率低，压缩时间短
            /// </summary>
            Low=1
        }
    }

    /// <summary>
    /// 定义传输流的传递参数
    /// </summary>
    public class TransportEventArgs:EventArgs
    {
        /// <summary>
        /// 原始流
        /// </summary>
        public Stream Sender
        {
            get;
            private set;
        }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }
        /// <summary>
        /// 构造传输的数据
        /// </summary>
        /// <param name="Sender">原始流</param>
        /// <param name="FileName">文件名</param>
        public TransportEventArgs(Stream Sender, string FileName)
        {
            this.FileName = FileName;
            this.Sender = Sender;
        }
    }
}
