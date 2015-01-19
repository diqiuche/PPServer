using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using PPServer.Service;
using PPServer.Object;

namespace PPServer.Client
{
    /// <summary>
    /// 服务中心，管理所有远程连接
    /// </summary>
    public class ServiceCenter : IDisposable
    {
        private IInnerService _innerService = null;
        private IFileService _fileService = null;

        private DuplexListener _duplexClient = null;
        private IDuplexService _duplexService = null;

        private string _userName;               //用户名
        private string _userId;                 //用户唯一标识(注册双工服务后得到)
        private string _password;               //密码（加密后）
        private string _duplexUserName;         //双工用户名
        private string _duplexPassword;         //双工密码
        private bool _isAutoReconnect;          //是否自动重连接双工服务
        private Thread _threadAutoReconnect;    //自动重连线程

        private int _httpPort;                      //http服务端口号
        private int _tcpPort;                       //tcp服务端口号
        private string _serverAddr;                 //服务器地址
        private bool _isOnline = false;             //双工是否在线
        private int _reconnectDuplexTimeGap = 2;    //自动重新连接的时间间隔（秒）
        private int _LOOP_TIME_OSCONNECTED = 100;   //自动重连时检测系统连接的间隔时间(毫秒)

        private string _TEMP_PATH = AppDomain.CurrentDomain.BaseDirectory + "Temp\\";
        private ServiceProtocol _enableDuplexService;  //启用双工服务
        private ServiceProtocol _enableInnerService;   //启用内部服务
        private ServiceProtocol _enablePublicService;  //启用外部服务
        private ServiceProtocol _enableFileService;    //启用文件服务

        private delegate void RegistHandler();


        /// <summary>
        /// 实例化服务中心
        /// </summary>
        /// <param name="ServerAddr">服务器地址</param>
        /// <param name="HttpPort">http服务端口号</param>
        /// <param name="TcpPort">tcp服务端口号</param>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码（加密后）</param>
        /// <param name="EnableDuplexService">是否启用双工服务</param>
        /// <param name="EnableInnerService">是否启用内部服务</param>
        /// <param name="EnableFileService">是否启用文件服务</param>
        /// <param name="EnablePublicService">是否启用外部服务</param>
        public ServiceCenter(string ServerAddr, int HttpPort, int TcpPort, string UserName, string Password, ServiceProtocol EnableDuplexService = ServiceProtocol.Tcp, ServiceProtocol EnableInnerService = ServiceProtocol.Http, ServiceProtocol EnableFileService = ServiceProtocol.Http, ServiceProtocol EnablePublicService = ServiceProtocol.Disabled)
        {
            this._serverAddr = ServerAddr;
            this._httpPort = HttpPort;
            this._tcpPort = TcpPort;
            this._userName = UserName;
            this._password = Password;
            this._enableDuplexService = EnableDuplexService;
            this._enableFileService = EnableFileService;
            this._enableInnerService = EnableInnerService;
            this._enablePublicService = EnablePublicService;

            _duplexClient = new DuplexListener();
            _duplexClient.OnReceiveMessage += new DuplexListener.MessageEventHandler(_duplexClient_OnReceiveMessage);
        }

        #region 属性
        /// <summary>
        /// 文件服务
        /// </summary>
        private IFileService FileService
        {
            get
            {
                if (_enableFileService != ServiceProtocol.Disabled && _fileService == null)
                {
                    _fileService = InitFileService(_serverAddr, _enableFileService == ServiceProtocol.Http ? _httpPort : _tcpPort);
                }
                return _fileService;
            }
        }

        private IInnerService InnerService
        {
            get
            {
                if (_enableInnerService != ServiceProtocol.Disabled && _innerService == null)
                {
                    _innerService = InitInnerService(_serverAddr, _enableInnerService == ServiceProtocol.Http ? _httpPort : _tcpPort);
                }
                return _innerService;
            }
        }

        private IDuplexService DuplexService
        {
            get
            {
                if (_enableDuplexService != ServiceProtocol.Disabled && _duplexService == null)
                {
                    _duplexService = InitDuplexService(_serverAddr, _enableDuplexService == ServiceProtocol.Http ? _httpPort : _tcpPort);
                }
                return _duplexService;
            }
        }

        /// <summary>
        /// 服务使用协议
        /// </summary>
        public enum ServiceProtocol
        {
            /// <summary>
            /// 使用http协议通信
            /// </summary>
            Http,
            /// <summary>
            /// 使用tcp协议通信
            /// </summary>
            Tcp,
            /// <summary>
            /// 服务不启动
            /// </summary>
            Disabled
        }

        /// <summary>
        /// 获取双工服务是否在线
        /// </summary>
        public bool IsOnline
        {
            get
            {
                return _isOnline;
            }
            private set
            {
                if (_isOnline != value)
                {
                    _isOnline = value;
                    if (OnDuplexOnlineChanged != null)
                        OnDuplexOnlineChanged(_isOnline);
                    WriteToLog("双工{0}",false,_isOnline?"上线":"下线");
                    //自动重连
                    if (!_isOnline)
                        AutoReconnect(IsAutoReconnectDuplex);
                    else
                        StopAutoReconnectDuplex();
                }
            }
        }
        /// <summary>
        /// 获取在线用户列表
        /// </summary>
        public string[] OnlineUsers
        {
            get;
            private set;
        }
        /// <summary>
        /// 是否自动重连双工服务
        /// </summary>
        public bool IsAutoReconnectDuplex
        {
            get
            {
                return _isAutoReconnect;
            }
            private set
            {
                _isAutoReconnect = value;                
            }
        }

        /// <summary>
        /// 获取或设置临时文件目录
        /// </summary>
        public string TEMP_PATH
        {
            get
            {
                return _TEMP_PATH;
            }
            set
            {
                _TEMP_PATH = value.TrimEnd('\\') + "\\";
            }
        }
        /// <summary>
        /// 重新连接的时间间隔（秒）
        /// </summary>
        public int ReconnectDuplexTimeGap
        {
            get
            { return _reconnectDuplexTimeGap; }
            set
            { _reconnectDuplexTimeGap = value; }
        }
        #endregion

        #region 事件
        /// <summary>
        /// 日志触发代理
        /// </summary>
        /// <param name="Content">日志内容</param>
        /// <param name="IsError">是否为错误日志</param>
        public delegate void OnWriteLogHandler(string Content, bool IsError = false);
        /// <summary>
        /// 当需要写入日志时触发
        /// </summary>
        public event OnWriteLogHandler OnWriteLog;
        /// <summary>
        /// 收到文本消息事件句柄
        /// </summary>
        /// <param name="FromUser">发送方用户</param>
        /// <param name="TextMessage">文本消息内容</param>
        /// <param name="MessageTime">消息发送时间</param>
        /// <param name="IsBroadcast">是否为广播消息</param>
        public delegate void OnReceiveTextHandler(string FromUser,string TextMessage,DateTime MessageTime,bool IsBroadcast);
        /// <summary>
        /// 收到文本消息时触发
        /// </summary>
        public event OnReceiveTextHandler OnReceiveTextMessage;
        /// <summary>
        /// 收到命令消息事件句柄
        /// </summary>
        /// <param name="FromUser">发送方用户</param>
        /// <param name="CommandText">命令文本</param>
        /// <param name="CommandData">命令数据</param>
        /// <param name="MessageTime">消息发送时间</param>
        /// <param name="IsBroadcast">是否为广播消息</param>
        public delegate void OnReceiveCommandHandler(string FromUser, string CommandText,Dictionary<string,object> CommandData, DateTime MessageTime, bool IsBroadcast);
        /// <summary>
        /// 收到命令消息时触发
        /// </summary>
        public event OnReceiveCommandHandler OnReceiveCommandMessage;

        /// <summary>
        /// 在线用户更新代理
        /// </summary>
        /// <param name="Users">在线用户列表</param>
        public delegate void OnlineUserEventHandler(string[] Users);
        /// <summary>
        /// 更新在线用户状态时触发
        /// </summary>
        public event OnlineUserEventHandler OnUpdateOnlineUsers;
        /// <summary>
        /// 收到文件代理
        /// </summary>
        /// <param name="FromUser">发送用户</param>
        /// <param name="FilePath">文件路径</param>
        /// <param name="SendTime">发送文件时间</param>
        /// <param name="UserData">发送过来的用户数据</param>
        public delegate void OnReceivedFileHandler(string FromUser, string FilePath, DateTime SendTime, Dictionary<string, object> UserData);
        /// <summary>
        /// 收到推送的文件时触发
        /// </summary>
        public event OnReceivedFileHandler OnReceivedFile;
        /// <summary>
        /// 进度反馈代理
        /// </summary>
        /// <param name="FileName">正在处理的文件名</param>
        /// <param name="Progress">处理进度</param>
        /// <param name="TransportStatus">处理状态</param>
        public delegate void OnProgressHandler(string FileName, double Progress, FileMessage.TransportStatus TransportStatus);
        /// <summary>
        /// 反馈传输进度时触发
        /// </summary>
        public event OnProgressHandler OnProgress;
        /// <summary>
        /// 双工状态改变代理
        /// </summary>
        /// <param name="IsOnline">状态是否在线</param>
        public delegate void OnDuplexOnlineChangedHandler(bool IsOnline);
        /// <summary>
        /// 双工在线状态改变时触发
        /// </summary>
        public event OnDuplexOnlineChangedHandler OnDuplexOnlineChanged;

        /// <summary>
        /// 更新文件完毕时触发
        /// </summary>
        /// <param name="FileFolder"></param>
        public delegate void OnUpdateFilesFinished(string FileFolder);
        #endregion

        #region 公开方法
                
        /// <summary>
        /// 设置通信服务已知类型集合
        /// </summary>
        /// <param name="Types">类型集合</param>
        public void SetKnownTypes(List<Type> Types)
        {
            Service.ServiceFuncLib.SetKnownTypes(Types);
        }
        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        public void SetUserInfo(string UserName, string Password)
        {
            this._userName = UserName;
            this._password = Password;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DuplexUserName"></param>
        /// <param name="DuplexPassword"></param>
        public void SetDuplexUserInfo(string DuplexUserName, string DuplexPassword)
        {
            this._duplexUserName = DuplexUserName;
            this._duplexPassword = DuplexPassword;
        }
        /// <summary>
        /// 设置通信服务已知类型所在文件集合
        /// </summary>
        /// <param name="FilePathCollection">文件路径集合</param>
        public void SetKnownTypes(params string[] FilePathCollection)
        {
            Service.ServiceFuncLib.SetKnownTypes(FilePathCollection);
        }
        /// <summary>
        /// 添加通信服务已知类型集合
        /// </summary>
        /// <param name="Types">已知类型集合</param>
        public void AddKnownTypes(List<Type> Types)
        {
            Service.ServiceFuncLib.AddKnownTypes(Types);
        }
        /// <summary>
        /// 添加单个通信服务已知类型
        /// </summary>
        /// <param name="Type">已知类型</param>
        public void AddKnownType(Type Type)
        {
            Service.ServiceFuncLib.AddKnownType(Type);
        }
        #endregion

        #region 初始化服务
        IDuplexService InitDuplexService(string ServerIP, int Port)
        {
            string _uri = string.Empty;
            Binding _binding = null;
            try
            {
                if (Port == _httpPort)
                {
                    //http方式初始化
                    _uri = string.Format("http://{0}:{1}/Duplex", ServerIP, Port);
                    _binding = ServiceFuncLib.GetServiceBinding<WSDualHttpBinding>();
                }
                else
                {
                    //tcp方式初始化
                    _uri = string.Format("net.tcp://{0}:{1}/Duplex", ServerIP, Port);
                    _binding = ServiceFuncLib.GetServiceBinding<NetTcpBinding>();
                }
                InstanceContext _duplex = new InstanceContext(_duplexClient);
                var _channelFactory = new DuplexChannelFactory<IDuplexService>(_duplex, _binding, new EndpointAddress(_uri));
                IDuplexService _duplexService=_channelFactory.CreateChannel();
                var _channel=_duplex.OutgoingChannels.FirstOrDefault();
                if (_channel!=null)
                    _channel.Faulted += new EventHandler(Channel_Faulted);
                return _duplexService;
            }
            catch (Exception ex)
            {
                WriteToLog(string.Format("注册双工通道[{0}]失败，用户:{1}，错误：{2}", _uri, _userName, ex.Message), true);
            }
            return null;
        }

        void Channel_Faulted(object sender, EventArgs e)
        {
            ReleaseService(_duplexService, true);
        }

        /// <summary>
        /// 初始化内部服务
        /// </summary>
        /// <param name="ServerIP">服务器地址</param>
        /// <param name="Port">端口号</param>
        /// <returns>返回成功初始化句柄</returns>
        IInnerService InitInnerService(string ServerIP, int Port)
        {
            string _uri = string.Empty;
            Binding _binding = null;
            try
            {

                if (Port == _httpPort)
                {
                    //http方式初始化
                    _uri = string.Format("http://{0}:{1}/Inner", ServerIP, Port);
                    _binding = ServiceFuncLib.GetServiceBinding<BasicHttpBinding>();
                }
                else
                {
                    //tcp方式初始化
                    _uri = string.Format("net.tcp://{0}:{1}/Inner", ServerIP, Port);
                    _binding = ServiceFuncLib.GetServiceBinding<NetTcpBinding>();
                }
                var _channelFactory = new ChannelFactory<IInnerService>(_binding, new EndpointAddress(_uri));
                //_channelFactory.Endpoint.Behaviors.Add(new WebHttpBehavior() { DefaultBodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.Wrapped, FaultExceptionEnabled = true });
                IInnerService _service = _channelFactory.CreateChannel();
                WriteToLog(string.Format("初始化内部服务成功"));
                return _service;
            }
            catch (Exception ex)
            {
                WriteToLog(string.Format("初始化内部服务[{0}]失败，错误：{1}", _uri, ex.Message), true);
            }
            return null;
        }

        /// <summary>
        /// 初始化文件服务
        /// </summary>
        /// <param name="ServerIP">服务器地址</param>
        /// <param name="Port">端口号</param>
        /// <returns>返回成功初始化句柄</returns>
        IFileService InitFileService(string ServerIP, int Port)
        {
            string _uri = string.Empty;
            Binding _binding = null;
            try
            {
                if (Port == _httpPort)
                {
                    //http方式初始化
                    _uri = string.Format("http://{0}:{1}/File", ServerIP, Port);
                    _binding = ServiceFuncLib.GetServiceBinding<BasicHttpBinding>(true);
                }
                else
                {
                    //tcp方式初始化
                    _uri = string.Format("net.tcp://{0}:{1}/File", ServerIP, Port);
                    _binding = ServiceFuncLib.GetServiceBinding<NetTcpBinding>(true);
                }
                var _channelFactory = new ChannelFactory<IFileService>(_binding, new EndpointAddress(_uri));
                IFileService _service = _channelFactory.CreateChannel();
                WriteToLog(string.Format("初始化文件服务成功"));
                return _service;
            }
            catch (Exception ex)
            {
                WriteToLog(string.Format("初始化文件服务[{0}]失败，错误：{1}", _uri, ex.Message), true);
            }
            return null;
        }
        #endregion

        #region 公开服务
        /// <summary>
        /// 连接至双工服务
        /// </summary>
        /// <param name="ToAutoReconnect">是否自动重连接</param>
        public void ConnectDuplex(bool ToAutoReconnect=true)
        {
            try
            {
                if (!IsOnline)
                {
                    ReleaseService(_duplexService);
                    _duplexService = null;
                    _isAutoReconnect = ToAutoReconnect;
                    RegistHandler _regHandler = Regist;
                    _regHandler.BeginInvoke(AfterRegist, null);
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 手工断开双工服务
        /// </summary>
        public void DisconnectDuplex()
        {
            if (IsOnline)
            {
                //处理自动重连状态,手工断线则不自动重连
                bool _tempAutoReconnect = IsAutoReconnectDuplex;
                IsAutoReconnectDuplex = false;

                ReleaseService(_duplexService);
                IsAutoReconnectDuplex = _tempAutoReconnect;
            }
        }

        /// <summary>
        /// 停止自动重连双工服务
        /// </summary>
        public void StopAutoReconnectDuplex()
        {
            if (_threadAutoReconnect != null && _threadAutoReconnect.ThreadState != ThreadState.Stopped)
            {
                try
                {
                    _threadAutoReconnect.Abort();
                    _threadAutoReconnect = null;
                }
                catch (ThreadAbortException)
                { }
                catch { }
            }
        }

        /// <summary>
        /// 获取在线用户列表，获取后由事件触发
        /// </summary>
        public void GetOnlineUsers()
        {
            try
            {
                DuplexService.GetOnlineUsers(_userName, _userId);
            }
            catch { }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="ServerFilePath">服务端文件路径</param>
        /// <param name="LocalFilePath">本地文件路径</param>
        /// <param name="AfterFunctionList">下载后执行的服务端任务集合</param>
        /// <param name="BeforeFunctionList">下载之前执行的服务端任务集合</param>
        /// <param name="ToCompress">是否压缩传输</param>
        /// <param name="ToEncrypt">是否加密传输</param>
        /// <returns></returns>
        public ReturnResult DownloadFile(string ServerFilePath, string LocalFilePath, List<DoServerFunction> AfterFunctionList = null, List<DoServerFunction> BeforeFunctionList = null, bool ToCompress = true, bool ToEncrypt = false)
        {
            return (new ProcessFile(this)).DownloadFile(_userName, _password, FileService, ServerFilePath, LocalFilePath,AfterFunctionList,BeforeFunctionList, ToCompress, ToEncrypt);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="AfterFunctionList">上传完毕后执行的服务端任务集合</param>
        /// <param name="BeforeFunctionList">上传之前执行的服务端任务集合</param>
        /// <param name="LocalFilePath">本地文件路径</param>
        /// <param name="ToCompress">是否压缩传输</param>
        /// <param name="ToEncrypt">是否加密传输</param>
        /// <returns></returns>
        public ReturnResult UploadFile(string LocalFilePath, List<DoServerFunction> AfterFunctionList = null,List<DoServerFunction> BeforeFunctionList=null, bool ToCompress = true, bool ToEncrypt = false)
        {
            return (new ProcessFile(this)).UploadFile(_userName, _password, FileService, LocalFilePath,AfterFunctionList,BeforeFunctionList, ToCompress, ToEncrypt);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="LocalFilePath">本地文件路径</param>
        /// <param name="ToUserName">接收方用户名</param>
        /// <param name="UserData">用户附加数据</param>
        /// <param name="AfterFunctionList">用户接收后执行的服务端任务集合</param>
        /// <param name="BeforeFunctionList">发送至服务器之前执行的服务端任务集合</param>
        /// <param name="OfflineSave">是否发送离线文件</param>
        /// <param name="ToCompress">是否压缩传输</param>
        /// <param name="ToEncrypt">是否加密传输</param>
        /// <returns></returns>
        public ReturnResult SendFile(string LocalFilePath, string ToUserName, Dictionary<string, object> UserData=null, List<DoServerFunction> AfterFunctionList = null, List<DoServerFunction> BeforeFunctionList = null, bool OfflineSave = true, bool ToCompress = true, bool ToEncrypt = false)
        {
            return (new ProcessFile(this)).SendFile(_userName, _password, ToUserName, UserData, AfterFunctionList, BeforeFunctionList, OfflineSave, FileService, LocalFilePath, ToCompress, ToEncrypt);
        }
        /// <summary>
        /// 获得更新文件集合
        /// </summary>
        /// <param name="ServerFolder">服务端更新目录名，根据此目录下所有</param>
        /// <param name="LocalCompareFolder">本地待比较目录名，根据本地文件目录内所有文件比较待更新文件，如果为null则不比较直接下载所有文件</param>
        /// <returns></returns>
        public ReturnResult GetUpdateFiles(string ServerFolder = "", string LocalCompareFolder = "")
        {
            //当服务端文件夹为空时，先在update目录中查找对应的程序名文件夹，如果存在则更新该目录内容，如果不存在则使用update根目录
            ReturnResult _rr=new ReturnResult();
            return _rr;
        }
        /// <summary>
        /// 执行内部服务方法
        /// </summary>
        /// <param name="FunctionName">服务端方法名</param>
        /// <param name="Paras">参数集合</param>
        /// <returns></returns>
        public ReturnResult DoInnerService(string FunctionName, object[] Paras=null)
        {
            return ProcessService.DoInnerService(_userName, _password, InnerService, FunctionName, Paras);
        }
        /// <summary>
        /// 执行内部服务方法
        /// </summary>
        /// <param name="FunctionName">服务端方法名</param>
        /// <param name="PrivateFunctionName">私有方法名，可为空</param>
        /// <param name="Paras">参数集合</param>
        /// <returns></returns>
        public ReturnResult DoInnerService(string FunctionName, string PrivateFunctionName, params object[] Paras)
        {
            return ProcessService.DoInnerService(_userName, _password, InnerService, FunctionName + (string.IsNullOrEmpty(PrivateFunctionName)?string.Empty:("/" + PrivateFunctionName)), Paras);
        }
        /// <summary>
        /// 执行内部服务方法
        /// </summary>
        /// <param name="Function">服务端方法</param>
        /// <returns></returns>
        public ReturnResult DoInnerService(DoServerFunction Function)
        {
            return ProcessService.DoInnerService(_userName, _password, InnerService, Function.ToString(), Function.Paras);
        }
        /// <summary>
        /// 发送消息至指定用户或用户类型
        /// </summary>
        /// <param name="ToUser">接收用户或用户类型，若为用户类型，须以用户类型分隔符结尾</param>
        /// <param name="Message">消息内容</param>
        /// <param name="OfflineSave">用户不在线时，是否发送离线消息。ToUser为用户类型时，此参数不起作用，不发送离线消息</param>
        /// <returns></returns>
        public ReturnResult SendMessage(string ToUser,string Message,bool OfflineSave=false)
        {
            return ProcessDuplex.SendMessage(_userName, _userId, DuplexService, ToUser, Message, OfflineSave);
        }

        /// <summary>
        /// 发送消息至指定多用户或用户类型集合
        /// </summary>
        /// <param name="ToUsers">接收用户或用户类型集合，若为用户类型，须以用户类型分隔符结尾</param>
        /// <param name="Message">消息内容</param>
        /// <param name="OfflineSave">用户不在线时，是否发送离线消息。ToUser为用户类型时，此参数不起作用，不发送离线消息</param>
        /// <returns></returns>
        public ReturnResult SendMessage(List<string> ToUsers, string Message, bool OfflineSave = false)
        {
            return ProcessDuplex.SendMessage(_userName, _userId, DuplexService, ToUsers, Message, OfflineSave);
        }

        /// <summary>
        /// 广播消息至所有在线用户
        /// </summary>
        /// <param name="Message">消息内容</param>
        /// <returns></returns>
        public ReturnResult BroadcastMessage(string Message)
        {
            return ProcessDuplex.BroadcastMessage(_userName, _userId, DuplexService, Message);
        }

        /// <summary>
        /// 发送消息至指定用户或用户类型
        /// </summary>
        /// <param name="ToUser">接收用户或用户类型，若为用户类型，须以用户类型分隔符结尾</param>
        /// <param name="Command">命令文本</param>
        /// <param name="CommandData">命令数据</param>
        /// <param name="OfflineSave">用户不在线时，是否发送离线消息。ToUser为用户类型时，此参数不起作用，不发送离线消息</param>
        /// <returns></returns>
        public ReturnResult SendCommand(string ToUser, string Command, Dictionary<string, object> CommandData, bool OfflineSave = true)
        {
            return ProcessDuplex.SendCommand(_userName, _userId, DuplexService, ToUser, Command, CommandData, OfflineSave);
        }

        /// <summary>
        /// 发送命令至指定多用户或用户类型集合
        /// </summary>
        /// <param name="ToUsers">接收用户或用户类型集合，若为用户类型，须以用户类型分隔符结尾</param>
        /// <param name="Command">命令文本</param>
        /// <param name="CommandData">命令数据</param>
        /// <param name="OfflineSave">用户不在线时，是否发送离线消息。ToUser为用户类型时，此参数不起作用，不发送离线消息</param>
        /// <returns></returns>
        public ReturnResult SendCommand(List<string> ToUsers, string Command, Dictionary<string, object> CommandData, bool OfflineSave = true)
        {
            return ProcessDuplex.SendCommand(_userName, _userId, DuplexService, ToUsers, Command, CommandData, OfflineSave);
        }
        /// <summary>
        /// 广播命令至所有在线用户
        /// </summary>
        /// <param name="Command">命令文本</param>
        /// <param name="CommandData">命令数据</param>
        /// <returns></returns>
        public ReturnResult BroadcastCommand(string Command, Dictionary<string, object> CommandData)
        {
            return ProcessDuplex.BroadcastCommand(_userName, _userId, DuplexService, Command, CommandData);
        }
        #endregion

        #region 私有方法
        
        private void Regist()
        {
            try
            {
                if (string.IsNullOrEmpty(_duplexUserName))
                    _userId = DuplexService.Regist(_userName, _password);
                else
                    _userId = DuplexService.Regist(_duplexUserName, _duplexPassword);
            }
            catch (Exception ex)
            {
                _userId = "-1";
                WriteToLog("注册双工通道出错：{0}", true, ex.Message);
            }            
        }

        private void AfterRegist(IAsyncResult Result)
        {
            if (string.IsNullOrEmpty(_userId))
            {
                IsOnline = false;
                WriteToLog("用户名或密码不正确", true);
            }
            else if (_userId == "-1")
            {
                IsOnline = false;
                WriteToLog("无法连接至双工服务，请检查网络地址配置", true);
                //无法连接时自动重连
                AutoReconnect(IsAutoReconnectDuplex);
            }
            else
            {
                IsOnline = true;
                WriteToLog(string.Format("注册双工通道成功，用户名:{0}，用户标识：{1}", _userName, _userId));
            }
        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="Content">日志内容</param>
        /// <param name="IsError">是否为错误日志</param>
        /// <param name="Paras">参数集合</param>
        internal void WriteToLog(string Content, bool IsError = false, params object[] Paras)
        {
            if (OnWriteLog != null)
                OnWriteLog(string.Format(Content, Paras), IsError);
        }

        internal void ReportTransportProgress(string FilePath, double Progress, FileMessage.TransportStatus Status)
        {
            if (OnProgress != null)
                OnProgress(FilePath, Progress, Status);
        }

        void _duplexClient_OnReceiveMessage(DuplexMessage Message)
        {
            if (OnReceiveTextMessage != null && Message.MessageType == DuplexMessage.MessageTypes.Text)
                OnReceiveTextMessage(Message.FromUser,Message.MessageText,Message.MessageTime,Message.IsBroadcast);
            else if (Message.MessageType == DuplexMessage.MessageTypes.Command)
            {
                //处理内部应用命令和自定义命令
                if (Message.CommandType == DuplexMessage.CommandTypes.CUSTOMER && OnReceiveCommandMessage != null)
                {
                    //收到自定义命令
                    WriteToLog("接收到从用户[{0}]发送的命令：{1}", false, Message.FromUser, Message.MessageText);
                    OnReceiveCommandMessage(Message.FromUser, Message.MessageText,Message.MessageData, Message.MessageTime, Message.IsBroadcast);
                }
                else if (Message.CommandType == DuplexMessage.CommandTypes.GET_ONLINE_USERS && OnUpdateOnlineUsers != null)
                {
                    //获得在线用户列表
                    WriteToLog("接收到获得在线用户列表命令", false);
                    this.OnlineUsers = Message.MessageText.Split('\t');
                    OnUpdateOnlineUsers(OnlineUsers);
                }
                else if (Message.CommandType == DuplexMessage.CommandTypes.LOGOUT)
                { 
                    //强制登出,不自动重连
                    WriteToLog("接收到强制登出命令", false);
                    IsAutoReconnectDuplex = false;
                    IsOnline = false;
                }
                else if (Message.CommandType == DuplexMessage.CommandTypes.GET_FILE)
                {
                    //收到文件
                    WriteToLog("接收到从用户[{0}]发送的文件命令：{1}", false,Message.FromUser,Message.MessageText);
                    ReturnResult _rr = ProcessDuplex.ProcessReceivedFile(_userName, _password, TEMP_PATH, Message.MessageText, this, FileService,Message );
                    Dictionary<string,object> _userData=new Dictionary<string,object>();
                    //处理用户数据
                    if(Message.MessageData!=null)
                        foreach(string key in Message.MessageData.Keys)
                        {
                            if (key.StartsWith("USER@") && key.Length > (key.IndexOf('@')+1))
                                _userData.Add(key.Substring(key.IndexOf('@')+1), Message.MessageData[key]);
                        }
                    if (_rr.ExeCode == 1 && OnReceivedFile != null)
                        OnReceivedFile(Message.FromUser, _rr.ExeResult["FILE_PATH"].ToString(), Message.MessageTime, _userData.Count==0?null:_userData);
                    else
                        WriteToLog("接收文件{0}出错：{1}", true,Message.MessageText, _rr.ExeInfo);
                }
            }
        }

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Connection, int ReservedValue);
        /// <summary>
        /// 得到操作系统连接状态
        /// </summary>
        /// <returns></returns>
        internal bool GetOSConnected()
        {
            int _connection = 0;
            return InternetGetConnectedState(out _connection, 0);
        }
        
        private void AutoReconnect(bool ToAutoConnect)
        {
            if (ToAutoConnect && !IsOnline)
            {
                WriteToLog("系统开始自动重连双工服务");
                StopAutoReconnectDuplex();
                _threadAutoReconnect = new Thread(new ThreadStart(StartReconnect));
                _threadAutoReconnect.Start();
            }
        }        

        private void StartReconnect()
        {
            try
            {                
                while (true)
                {                    
                    //先检测系统连接状态
                    if (GetOSConnected())
                    {
                        Thread.Sleep(ReconnectDuplexTimeGap * 1000);
                        ConnectDuplex();
                        break;
                    }
                    else
                        Thread.Sleep(_LOOP_TIME_OSCONNECTED);
                }
            }
            catch (ThreadAbortException) { }
            catch { }
        }
        #endregion

        #region 系统退出
        private void ReleaseService(ServiceBase Service,bool IsFault=false)
        {
            try
            {
                if (Service != null)
                {
                    if (Service is IDuplexService && IsOnline)
                    {
                        //信道未出错时注销
                        if (!IsFault)
                        {
                            try
                            {
                                ((IDuplexService)Service).Unregist(this._userName, _userId);
                            }
                            catch { }
                        }
                        IsAutoReconnectDuplex = false;
                        IsOnline = false;
                    }
                    //信道未出错时销毁
                    if (!IsFault)
                        (Service as IDisposable).Dispose();
                }
            }
            catch { }
            finally
            { Service = null; }
        }
        /// <summary>
        /// 系统退出时销毁本对象
        /// </summary>
        public void Dispose()
        {
            StopAutoReconnectDuplex();
            ReleaseService(_innerService);
            ReleaseService(_duplexService);
            ReleaseService(_fileService);
        }
        #endregion
    }
}
