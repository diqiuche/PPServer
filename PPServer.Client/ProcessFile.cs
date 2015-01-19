using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PPServer.Service;
using PPServer.Object;

namespace PPServer.Client
{
    internal class ProcessFile
    {
        ServiceCenter _center;
        public ProcessFile(ServiceCenter Center)
        {
            _center = Center;
        }
        /// <summary>
        /// 处理下载文件
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="FileService">文件服务对象</param>
        /// <param name="ServerFilePath">服务端文件路径</param>
        /// <param name="LocalFilePath">本地文件路径</param>
        /// <param name="AfterFunctionList">下载后执行的服务端任务集合</param>
        /// <param name="BeforeFunctionList">下载之前执行的服务端任务集合</param>
        /// <param name="ToCompress">是否压缩传输</param>
        /// <param name="ToEncrypt">是否加密传输</param>
        /// <returns></returns>
        public ReturnResult DownloadFile(string UserName, string Password, IFileService FileService, string ServerFilePath, string LocalFilePath, List<DoServerFunction> AfterFunctionList = null, List<DoServerFunction> BeforeFunctionList = null, bool ToCompress = true, bool ToEncrypt = false)
        {
            ReturnResult _rr = new ReturnResult();
            try
            {
                FileMessageRequest _fileRequest = new FileMessageRequest(UserName, Password, ServerFilePath, ToCompress, ToEncrypt);
                //添加事后方法
                if (AfterFunctionList != null)
                    AfterFunctionList.ForEach(t => _fileRequest.AddAfterFunction(t.ToString(), t.Paras));
                //添加事前方法
                if (BeforeFunctionList != null)
                    BeforeFunctionList.ForEach(t => _fileRequest.AddBeforeFunction(t.ToString(), t.Paras));
                FileMessage _msg = FileService.Download(_fileRequest);
                _msg.OnWriteProgress += new FileMessage.ReportProgressHandler(_msg_OnWriteProgress);
                _rr = _msg.Result;
                if (_rr.ExeCode >= 1)
                {
                    string _localFilePath = LocalFilePath + (_msg.IsCompressed ? FileMessage.TEMPFILE_SUFFIX : string.Empty);
                    Stream _localFileData = _msg.CopyStream(_msg.FileData, _localFilePath,_msg.TransLength);
                    if (_msg.IsCompressed)
                    {
                        //解压
                        _center.ReportTransportProgress(LocalFilePath, 0, FileMessage.TransportStatus.DeCompressing);
                        _localFileData.Position = 0;
                        TransportStream.DeCompressFile(_localFileData, _localFilePath, LocalFilePath, _msg.VerifyCode);
                        _center.ReportTransportProgress(LocalFilePath, 100, FileMessage.TransportStatus.DeCompressing);
                    }
                    else
                    {
                        _localFileData.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                _rr.ExeCode = -1;
                _rr.ExeInfo = "本地异常：" + ex.Message + "\r\n" + "详细：" + ex.StackTrace;
            }
            return _rr;
        }

        internal void GetFile(string UserName, string Password, IFileService FileService, string ServerFilePath, string LocalFilePath, Dictionary<string, List<object>> AfterFunctions, bool Compressed, bool Encrypted)
        {
            FileMessageRequest _fileRequest = new FileMessageRequest(UserName, Password, ServerFilePath, false, false,true);
            //添加事后方法
            _fileRequest.AfterFunctions = AfterFunctions;

            FileMessage _msg = FileService.Download(_fileRequest);
            _msg.OnWriteProgress += new FileMessage.ReportProgressHandler(_msg_OnWriteProgress);
            if (_msg.Result.ExeCode >= 1)
            {
                string _localFilePath = LocalFilePath + (Compressed ? FileMessage.TEMPFILE_SUFFIX : string.Empty);
                Stream _localFileData = _msg.CopyStream(_msg.FileData, _localFilePath, _msg.TransLength);
                if (Encrypted)
                { 
                    //解密
                }
                if (Compressed)
                {
                    //解压
                    _center.ReportTransportProgress(LocalFilePath, 0, FileMessage.TransportStatus.DeCompressing);
                    _localFileData.Position = 0;
                    TransportStream.DeCompressFile(_localFileData, _localFilePath, LocalFilePath, _msg.VerifyCode,true,false);
                    _center.ReportTransportProgress(LocalFilePath, 100, FileMessage.TransportStatus.DeCompressing);
                }
                else
                {
                    _localFileData.Close();
                }
            }
        }

        void _msg_OnWriteProgress(FileMessage FileInfo, string FilePath, double Progress, FileMessage.TransportStatus Status)
        {

            _center.ReportTransportProgress(FilePath, Progress, FileMessage.TransportStatus.Downloading);
        }

        /// <summary>
        /// 处理上传文件
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="FileService">文件服务对象</param>
        /// <param name="LocalFilePath">本地文件路径</param>
        /// <param name="AfterFunctionList">上传完毕后置执行函数集合</param>
        /// <param name="BeforeFunctionList">上传之前前置执行函数集合</param>
        /// <param name="ToCompress">是否压缩传输</param>
        /// <param name="ToEncrypt">是否加密传输</param>
        public ReturnResult UploadFile(string UserName, string Password, IFileService FileService, string LocalFilePath,List<DoServerFunction> AfterFunctionList,List<DoServerFunction> BeforeFunctionList, bool ToCompress = true, bool ToEncrypt = false)
        {
            ReturnResult _rr = new ReturnResult();
            try
            {
                FileMessage _msg = new FileMessage(LocalFilePath, ToCompress, ToEncrypt);

                _msg.OnReadProgress +=new FileMessage.ReportProgressHandler(_msg_OnReadProgress);
                _msg.SystemInfo.Add("UserName", UserName);
                _msg.SystemInfo.Add("Password", Password);
                //添加事后方法
                if (AfterFunctionList != null)
                    AfterFunctionList.ForEach(t =>_msg.AddAfterFunction(t.ToString(), t.Paras));
                //添加事前方法
                if (BeforeFunctionList != null)
                    BeforeFunctionList.ForEach(t => _msg.AddBeforeFunction(t.ToString(), t.Paras));
                _rr.OriginalSize = _msg.Length;
                _rr.TransportSize = _msg.TransLength;
                _rr.VerifyCode = _msg.VerifyCode;
                FileService.Upload(_msg);
            }
            catch(Exception ex)
            {
                _rr.ExeCode = -1;
                _rr.ExeInfo = "本地异常：" + ex.Message + "\r\n" + "详细：" + ex.StackTrace;
            }
            return _rr;
        }

        void _msg_OnReadProgress(FileMessage FileInfo, string FilePath, double Progress, FileMessage.TransportStatus Status)
        {
            _center.ReportTransportProgress(FilePath, Progress,Status);
        }

        /// <summary>
        /// 处理发送文件
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="ToUserName">接收方用户名</param>
        /// <param name="UserInfo">用户附加信息</param>
        /// <param name="AfterFunctionList">用户接收后执行的服务端任务集合</param>
        /// <param name="BeforeFunctionList">发送至服务器之前执行的服务端任务集合</param>
        /// <param name="OfflineSave">是否发送离线文件</param>
        /// <param name="FileService">文件服务对象</param>
        /// <param name="LocalFilePath">本地文件路径</param>
        /// <param name="ToCompress">是否压缩传输</param>
        /// <param name="ToEncrypt">是否加密传输</param>
        public ReturnResult SendFile(string UserName, string Password, string ToUserName, Dictionary<string, object> UserInfo, List<DoServerFunction> AfterFunctionList, List<DoServerFunction> BeforeFunctionList, bool OfflineSave, IFileService FileService, string LocalFilePath, bool ToCompress = true, bool ToEncrypt = false)
        {
            ReturnResult _rr = new ReturnResult();
            try
            {
                FileMessage _msg = new FileMessage(LocalFilePath, ToCompress, ToEncrypt);

                _msg.OnReadProgress += new FileMessage.ReportProgressHandler(_msg_OnReadProgress);
                _msg.SystemInfo.Add("UserName", UserName);
                _msg.SystemInfo.Add("Password", Password);
                _msg.SystemInfo.Add("ToUser", ToUserName);
                _msg.SystemInfo.Add("OfflineSave", OfflineSave);
                _msg.UserInfo = UserInfo;
                //添加事后方法
                if (AfterFunctionList != null)
                    AfterFunctionList.ForEach(t => _msg.AddAfterFunction(t.ToString(), t.Paras));
                //添加事前方法
                if (BeforeFunctionList != null)
                    BeforeFunctionList.ForEach(t => _msg.AddBeforeFunction(t.ToString(), t.Paras));
                _rr.OriginalSize = _msg.Length;
                _rr.TransportSize = _msg.TransLength;
                _rr.VerifyCode = _msg.VerifyCode;
                FileService.Send(_msg);
            }
            catch (Exception ex)
            {
                _rr.ExeCode = -1;
                _rr.ExeInfo = "本地异常：" + ex.Message + "\r\n" + "详细：" + ex.StackTrace;
            }
            return _rr;
        }

        public ReturnResult GetUpdateFiles(string ServerFolder = "", string LocalCompareFolder = "")
        {
            return null;
        }
    }
}
