using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Diagnostics;
using PPServer.Interface;
using PPServer.Object;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/7/2
 * Description:文件传输服务：下载、上传、传送
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.DoService
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,IncludeExceptionDetailInFaults=true,InstanceContextMode=InstanceContextMode.PerCall)]
    public class FileDoService : DoServiceBase, PPServer.Service.IFileService
    {        
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="FileInfo">文件信息</param>>
        public void Upload(FileMessage FileInfo)
        {
            string _userName = FileInfo.SystemInfo["UserName"].ToString();
            string _password = FileInfo.SystemInfo["Password"].ToString();
            Stopwatch _sw = new Stopwatch();
            _sw.Start();
            try
            {
                //Authenticate User's Identity
                this.ToAuthenticate(_userName, _password, this.GetType().ToString() + "/Upload");
                //上传至配置文件中upload文件夹
                //实际本地上传文件路径
                string _uploadFileFolder = FolderList["Upload"].RealPath + _userName + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                string _uploadFilePath=_uploadFileFolder+ FileInfo.FileName + (FileInfo.IsCompressed ? FileMessage.TEMPFILE_SUFFIX : string.Empty);
                //执行事前方法
                DoFunction(_userName, _uploadFilePath, FileInfo.BeforeFunctions, DoFunctionTypeEnum.Upload,true);
                //读取文件并保存本地
                string _newFileFullPath = SaveFileInfoTo(FileInfo, _uploadFilePath);
                //执行事后方法
                DoFunction(_userName, _newFileFullPath, FileInfo.AfterFunctions, DoFunctionTypeEnum.Upload, false);
            }
            catch (Exception ex)
            {
                WriteErrorToLog(ex, "用户[{0}]，上传文件\"{1}\"出错。", _userName, FileInfo.FileName);
                return;
                //throw new FaultException(string.Format("上传文件\"{0}\"出错：{1}", FileInfo.FileName, ex.Message));
            }
            finally
            {
                _sw.Stop();
            }
            WriteToLog(string.Format("用户[{0}]，上传文件\"{1}\"，耗时：{2}s", _userName, FileInfo.FileName, _sw.ElapsedMilliseconds / (double)1000));
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="Request">请求的参数</param>
        /// <returns></returns>
        public FileMessage Download(FileMessageRequest Request)
        {
            ReturnResult _rr = new ReturnResult();
            FileMessage _fm = null;
            Stopwatch _sw = new Stopwatch();
            _sw.Start();
            try
            {
                this.ToAuthenticate(Request.UserName, Request.Password, this.GetType().ToString() + "/Download");
                //服务端文件路径替换
                string _serverMapPath = GetLegalPathString(null, Request.ServerMapPath);
                //实际文件路径
                string _realFilePath = string.Empty;
                if (_serverMapPath.Contains(@":\") || !_serverMapPath.Contains(@"\"))
                {
                    throw new Exception(string.Format("请求下载文件\"{0}\"中包含非法路径，处理后路径：{1}", Request.ServerMapPath, _serverMapPath));
                }

                //处理Map路径(未达到最佳匹配)
                //排序ServerConfig.Instance.System.MapFolders.Folder.Sort((t)=>t.Name);
                foreach (var folder in FolderList.Values)
                {
                    if (folder.CanDownload)
                    {
                        //映射路径加上反斜杠
                        string _mapFolderName = folder.Name.Trim('\\') + "\\";
                        if (_serverMapPath.StartsWith(_mapFolderName))
                        {
                            _realFilePath = folder.RealPath + _serverMapPath.Substring(_mapFolderName.Length);
                            break;
                        }
                    }
                }
                if (_realFilePath == string.Empty || !File.Exists(_realFilePath))
                    throw new Exception(string.Format("非法请求下载文件\"{0}\"，实际文件路径：{1}", Request.ServerMapPath,_realFilePath));
                //WebOperationContext.Current.OutgoingResponse.ContentType = "application/force-download";

                //执行事前方法
                DoFunction(Request.UserName, _realFilePath, Request.BeforeFunctions, DoFunctionTypeEnum.Download, true);

                _fm = new FileMessage(_realFilePath, Request.ToCompress, Request.ToEncrypt);
                _rr.TransportSize = _fm.TransLength;
                _rr.VerifyCode = _fm.VerifyCode;
                _rr.OriginalSize = _fm.Length;
                //下载完后是否删除
                _fm.AddSystemInfo("USERNAME", Request.UserName);
                _fm.AddSystemInfo("FILE@ToDeleteAfterDownloaded", Request.ToDeleteAfterDownloaded);
                //添加下载完成事后执行方法
                _fm.AfterFunctions = Request.AfterFunctions;
                _fm.OnReadCompleted += new FileMessage.ReportCompleteHandler(_fm_OnReadCompleted);

                _sw.Stop();
                WriteToLog(string.Format("用户[{0}]，下载文件\"{1}\"，耗时：{2}s", Request.UserName, Request.ServerMapPath, _sw.ElapsedMilliseconds / (double)1000));
            }
            catch (Exception ex)
            {
                _rr.ExeCode = 0;
                _rr.ExeInfo = ex.Message;
                WriteErrorToLog(ex, "用户[{0}]，下载文件\"{1}\"出错。", Request.UserName, Request.ServerMapPath);
                _fm = new FileMessage();
            }
            _rr.ExeTimeSpan = _sw.ElapsedMilliseconds;
            _rr.ServerTime = DateTime.Now;
            _fm.Result = _rr;
            return _fm;
        }

        void _fm_OnReadCompleted(FileMessage FileInfo, string FileName)
        {            
            try
            {
                //下载完成后删除文件
                if(FileInfo.SystemInfo.ContainsKey("FILE@ToDeleteAfterDownloaded") && (bool)FileInfo.SystemInfo["FILE@ToDeleteAfterDownloaded"])
                    File.Delete(FileName);
                //执行事后方法
                DoFunction(FileInfo.SystemInfo["USERNAME"].ToString(), FileName, FileInfo.AfterFunctions, DoFunctionTypeEnum.Download, false);
            }
            catch { }
        }

        /// <summary>
        /// 传送文件至指定用户，用户不在线时则缓存至服务器
        /// </summary>
        /// <param name="FileInfo">文件信息,otherinfo中:UserName-用户名,Password-密码,ToUser-接收用户,bool OfflineSave-是否发送离线文件</param>
        public void Send(FileMessage FileInfo)
        {
            string _userName = FileInfo.SystemInfo["UserName"].ToString();
            string _password = FileInfo.SystemInfo["Password"].ToString();
            string _toUser = FileInfo.SystemInfo["ToUser"].ToString();
            bool _offlineSave = (bool)FileInfo.SystemInfo["OfflineSave"];
            DuplexMessage _msg = null;
            Stopwatch _sw = new Stopwatch();
            _sw.Start();
            try
            {
                this.ToAuthenticate(_userName, _password, this.GetType().ToString() + "/Send");

                //先保存文件至缓存，再向在线用户发送消息，用户收到消息后下载文件
                //上传至配置文件中cache文件夹
                //实际本地上传文件路径
                string _cacheFileRealFolder = FolderList["Cache"].RealPath + DateTime.Now.ToString("yyyyMMdd") + "\\";
                //网络映射上传文件路径
                string _cacheFileMapFolder = "Cache\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                string _cacheFileName = _userName + "#" + _toUser + "#" + DateTime.Now.ToString("HHmmss") + "#" + FileInfo.FileName;
                //执行事前方法
                DoFunction(_userName, _cacheFileRealFolder + _cacheFileName, FileInfo.BeforeFunctions, DoFunctionTypeEnum.Send, true);
                //读取文件并保存本地
                SaveFileInfoTo(FileInfo, _cacheFileRealFolder + _cacheFileName, false);

                //向用户发送消息
                Dictionary<string, object> _file = new Dictionary<string, object>();
                _file.Add("FILE@COMPRESSED", FileInfo.IsCompressed);
                _file.Add("FILE@ENCRYPTED", FileInfo.IsEncrypted);
                if(FileInfo.UserInfo!=null)
                    foreach (string key in FileInfo.UserInfo.Keys)
                    {
                        _file.Add("USER@" + key, FileInfo.UserInfo[key]);
                    }
                _msg = new DuplexMessage(_userName, new List<string>() { _toUser }, DuplexMessage.CommandTypes.GET_FILE, _cacheFileMapFolder + _cacheFileName, _file, _offlineSave);
                //添加事后执行函数
                _msg.AfterFunctions = FileInfo.AfterFunctions;
            }
            catch (Exception ex)
            {
                WriteErrorToLog(ex, "用户[{0}]，发送文件\"{1}\"至用户[{2}]出错。", _userName, FileInfo.FileName, _toUser);
                return;
                //throw new FaultException(string.Format("发送文件\"{0}\"至用户[{1}]出错：{2}", FileInfo.FileName, _toUser, ex.Message));
            }
            _sw.Stop();
            WriteToLog(string.Format("用户[{0}]，发送文件\"{1}\"至用户[{2}]，耗时：{3}s", _userName, FileInfo.FileName, _toUser, _sw.ElapsedMilliseconds / (double)1000));
            DuplexCenter.Instance.SendMessage(_msg);
        }

        /// <summary>
        /// 获取更新文件列表
        /// </summary>
        /// <param name="UpdatePath">服务端更新文件夹路径</param>
        /// <returns></returns>
        public List<FileInfoRequest> GetUpdateFiles(string UpdatePath)
        {
            string _updateRealPath = GetLegalPathString(FolderList["Update"].RealPath, UpdatePath);
            List<FileInfoRequest> _updateFiles = GetUpdateFilesInfo(_updateRealPath);
            _updateFiles.ForEach(t => t.FileFolder = t.FileFolder.Replace(FolderList["Update"].RealPath, ""));
            return _updateFiles;
        }

        private List<FileInfoRequest> GetUpdateFilesInfo(string UpdateRealPath)
        {
            List<FileInfoRequest> _fir = new List<FileInfoRequest>();
            try
            {
                if (Directory.Exists(UpdateRealPath))
                {
                    DirectoryInfo _di = new DirectoryInfo(UpdateRealPath);
                    //加入所有文件
                    _di.GetFiles().ToList().ForEach(t => _fir.Add(new FileInfoRequest(t.FullName)));

                    //处理所有文件夹
                    _di.GetDirectories().ToList().ForEach(t => _fir.AddRange(GetUpdateFilesInfo(t.FullName)));
                }
            }
            catch { }
            return _fir;
        }
        /// <summary>
        /// 保存文件信息至文件
        /// </summary>
        /// <param name="FileInfo">文件信息</param>
        /// <param name="SaveToPath">保存路径</param>
        /// <param name="ToDeCompress">是否解压文件</param>
        private string SaveFileInfoTo(FileMessage FileInfo, string SaveToPath, bool ToDeCompress=true)
        {
            string _newFileFullPath = SaveToPath;
            if (!Directory.Exists(SaveToPath.Substring(0,SaveToPath.LastIndexOf('\\'))))
                Directory.CreateDirectory(SaveToPath.Substring(0, SaveToPath.LastIndexOf('\\')));
            Stream _localFileData = FileInfo.CopyStream(FileInfo.FileData, SaveToPath, FileInfo.TransLength);
            if (ToDeCompress && FileInfo.IsCompressed)
            {
                //解压
                _newFileFullPath = SaveToPath.TrimEnd(FileMessage.TEMPFILE_SUFFIX.ToCharArray());
                _localFileData.Position = 0;
                TransportStream.DeCompressFile(_localFileData, SaveToPath, _newFileFullPath, FileInfo.VerifyCode);
            }
            else
            {
                _localFileData.Close();
            }
            return _newFileFullPath;
        }
        /// <summary>
        /// 执行服务端方法
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="FilePath">文件路径</param>
        /// <param name="FunctionList">方法集合</param>
        private void DoFunction(string UserName, string FilePath, Dictionary<string, List<object>> FunctionList, DoFunctionTypeEnum DoFunctionType,bool IsPrefix)
        {
            if (FunctionList != null)
            {
                foreach (string functionName in FunctionList.Keys)
                {
                    try
                    {
                        List<object> _paras = FunctionList[functionName];
                        //处理调用方法名
                        string _publicFuncName;
                        string _privateFuncName;
                        GetFunctionName(functionName, out _publicFuncName, out _privateFuncName);
                        //加载程序集
                        if (!Config.ServerConfig.Instance.Services.File.FunctionList.ContainsKey(_publicFuncName))
                            throw new Exception(string.Format("没有方法{0}可被调用！", _publicFuncName));
                        Config.Function _func = Config.ServerConfig.Instance.Services.File.FunctionList[_publicFuncName];
                        //参数集合字符串
                        string _paraStr = (_paras == null || _paras.Count == 0) ? "无" : (string.Join(",", _paras.Select(t => _paras.IndexOf(t) + "='" + t.ToString() + "'")));

                        IFileFunction _instance = CreateClassInstance<IFileFunction>(_func.FilePath, _func.ClassName);
                        if (_instance != null)
                        {
                            WriteToLog(string.Format("用户[{0}]，开始执行{1}{2}服务：{3}，参数：{4}", UserName, DoFunctionType.ToString(), IsPrefix ? "前置" : "后置", functionName, _paraStr), false);

                            if (string.IsNullOrEmpty(_privateFuncName))
                                _instance.DoFunction(UserName, FilePath, _paras);
                            else //当须调用私有函数时
                                _instance.DoFunction(UserName, FilePath, _privateFuncName, _paras);
                        }
                    }
                    catch(Exception ex)
                    {
                        WriteToLog(string.Format("用户[{0}]，执行{1}{2}服务：{3}出错：", UserName, DoFunctionType.ToString(), IsPrefix ? "前置" : "后置", functionName, ex.Message), false);
                    }
                }
            }
        }

        private enum DoFunctionTypeEnum
        { 
            Upload,
            Send,
            Download
        }
    }
}
