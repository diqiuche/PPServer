using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using PPServer.Config;
using PPServer.Object;
using PPServer.Service;
using PPServer.DoService;
using PPServer.DB;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/6/5
 * Description:Startup 4 services
 * inner:use tcp protocol to transfer data for client system
 * public:publish web service for third-part system calling
 * duplex:use duplex tcp protocol to send message between client and server,or between one to other one.
 * file: upload,download,transfer files
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost _innerHost = null;
            ServiceHost _publicHost = null;
            ServiceHost _duplexHost = null;
            ServiceHost _fileHost = null;
            List<ServiceHost> _hostList = new List<ServiceHost>() { _innerHost, _publicHost, _duplexHost, _fileHost };
            try
            {
                //网络地址
                string _ip = ServerConfig.Instance.Services.ServerIP;
                int _httpPort = ServerConfig.Instance.Services.HttpPort;
                int _tcpPort = ServerConfig.Instance.Services.TcpPort;
                //传输对象
                List<string> _filesPath = new List<string>();
                //从配置文件的TransportObjects中加载KnownTypes
                foreach (Config.ObjectFile obj in Config.ServerConfig.Instance.Services.TransportObjects.ObjectFile)
                {
                    _filesPath.Add(obj.FilePath);
                }
                ServiceFuncLib.SetKnownTypes(_filesPath.ToArray());

                //添加日志事件
                ServerDB.OnWriteLog += new ServerDB.OnWriteLogDlg(Instance_OnWriteLog);
                DoServiceBase.OnWriteLog += new DoServiceBase.OnWriteLogDlg(Instance_OnWriteLog);

                if (InitDBConnection() && 
                    InitInnerService(ref _innerHost, _ip, _httpPort, _tcpPort) && 
                    InitFileService(ref _fileHost, _ip, _httpPort, _tcpPort) &&
                    InitDuplexService(ref _duplexHost,_ip,_httpPort,_tcpPort) &&
                    InitPublicService(ref _publicHost,_ip,_httpPort,_tcpPort))
                {
                    Common.Log.WriteToLog("系统初始化完毕。", false);
                }
                else
                {
                    Common.Log.WriteToLog("####### 初始化无法完成，请检查配置文件或数据库，并重启本软件 #######", true);
                    CleanSystemService(_hostList);
                }
                
            }
            catch (Exception e)
            {
                Common.Log.WriteToLog("全局错误：" + e.Message, true);
            }
            LoopToQuit();
            CleanSystemService(_hostList);
            Common.Log.WriteToLog("系统正常退出。", false);
        }

        //循环读取用户命令
        private static void LoopToQuit()
        {
            while (true)
            {
                string cmd = Console.ReadLine();
                if (cmd.ToLower() == "quit" || cmd.ToLower() == "q")
                    return;
            }
        }

        /// <summary>
        /// Init Database connection
        /// </summary>
        /// <returns></returns>
        public static bool InitDBConnection()
        {
            try
            {
                bool _allDBInited = true;
                if (ServerDB.Instance.CurrentDB == null)
                {
                    //未初始化
                    if (ServerConfig.Instance.Databases.MsSql != null && ServerConfig.Instance.Databases.MsSql.Enable)
                    {
                        //MSSQL初始化
                        _allDBInited &= ServerDB.Instance.InitDBConnectionsFactory("MsSql", ServerConfig.Instance.Databases.MsSql.DBServer, ServerConfig.Instance.Databases.MsSql.Port,
                            ServerConfig.Instance.Databases.MsSql.UseWindowsAuthentication, ServerConfig.Instance.Databases.MsSql.UserName,
                            ServerConfig.Instance.Databases.MsSql.Password, ServerConfig.Instance.Databases.MsSql.InitDB, ServerConfig.Instance.Databases.MsSql.OtherParams);
                    }
                    if (ServerConfig.Instance.Databases.Oracle.Enable)
                    {
                        //ORACLE初始化
                    }
                    if (ServerConfig.Instance.Databases.MySql.Enable)
                    {
                        //MySql初始化
                        _allDBInited &= ServerDB.Instance.InitDBConnectionsFactory("MySql", 
                            ServerConfig.Instance.Databases.MySql.DBServer,
                            ServerConfig.Instance.Databases.MySql.Port,
                            ServerConfig.Instance.Databases.MySql.UserName,
                            ServerConfig.Instance.Databases.MySql.Password,
                            ServerConfig.Instance.Databases.MySql.InitDB);
                    }
                    ServerDB.Instance.SetCurrentDB(ServerConfig.Instance.Databases.CurrentDB.Trim());
                }
                return _allDBInited;
            }
            catch (Exception ex)
            {
                Common.Log.WriteToLog("初始化数据库出错:" + ex.Message, true);
            }
            return false;
        }

        static void Instance_OnWriteLog(string Content, bool IsError = false)
        {
            Common.Log.WriteToLog(Content, IsError);
        }
                

        #region Services
        /// <summary>
        /// Init inner services
        /// </summary>
        /// <param name="Host">Host object</param>
        /// <param name="ServerIP">Host Ip address</param>
        /// <param name="HttpPort">Http protocol</param>
        /// <param name="TcpPort"></param>
        /// <returns></returns>
        public static bool InitInnerService(ref ServiceHost Host, string ServerIP, int HttpPort, int TcpPort)
        {
            if (!ServerConfig.Instance.Services.Inner.Enable)
                return true;
            string _uri = string.Empty;
            Binding _binding = null;
            try
            {
                if (ServerConfig.Instance.Services.Inner.Protocol.Trim().ToLower() == "tcp")
                {
                    //Tcp协议
                    _uri = string.Format("net.tcp://{0}:{1}/Inner", ServerIP, TcpPort);
                    _binding = ServiceFuncLib.GetServiceBinding<NetTcpBinding>();
                }
                else
                {
                    //http协议
                    _uri = string.Format("http://{0}:{1}/Inner", ServerIP, HttpPort);
                    _binding = ServiceFuncLib.GetServiceBinding<BasicHttpBinding>();
                }
                Host = new ServiceHost(typeof(InnerDoService));
                Host.AddServiceEndpoint(typeof(IInnerService), _binding, _uri);
                //Host.AddServiceEndpoint(typeof(IInnerService), _binding, _uri).Behaviors.Add(new ServiceMetadataBehavior() { HttpGetEnabled=true, FaultExceptionEnabled = true, DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Json });
                Host.Open();
                Common.Log.WriteToLog(string.Format("初始化内部服务[{0}]", _uri), false);
                return true;
            }
            catch (Exception ex)
            {
                Common.Log.WriteToLog(string.Format("初始化内部服务[{0}] 出错:{1}", _uri, ex.Message), true);
            }
            return false;
        }

        /// <summary>
        /// Init file services
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="ServerIP"></param>
        /// <param name="HttpPort"></param>
        /// <param name="TcpPort"></param>
        /// <returns></returns>
        public static bool InitFileService(ref ServiceHost Host, string ServerIP, int HttpPort, int TcpPort)
        {
            if (!ServerConfig.Instance.Services.File.Enable)
                return true;
            string _uri = string.Empty;
            Binding _binding = null;
            try
            {
                if (ServerConfig.Instance.Services.File.Protocol.Trim().ToLower() == "tcp")
                {
                    //Tcp协议
                    _uri = string.Format("net.tcp://{0}:{1}/File", ServerIP, TcpPort);
                    _binding = ServiceFuncLib.GetServiceBinding<NetTcpBinding>(true);
                }
                else
                {
                    //http协议
                    _uri = string.Format("http://{0}:{1}/File", ServerIP, HttpPort);
                    _binding = ServiceFuncLib.GetServiceBinding<BasicHttpBinding>(true);
                }
                Host = new ServiceHost(typeof(FileDoService));
                Host.AddServiceEndpoint(typeof(IFileService), _binding, _uri);
                Host.Open();
                Common.Log.WriteToLog(string.Format("初始化文件服务[{0}]", _uri), false);
                return true;
            }
            catch (Exception ex)
            {
                Common.Log.WriteToLog(string.Format("初始化文件服务[{0}] 出错:{1}", _uri, ex.Message), true);
            }
            return false;
        }

        /// <summary>
        /// Init duplex service
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="ServerIP"></param>
        /// <param name="HttpPort"></param>
        /// <param name="TcpPort"></param>
        /// <returns></returns>
        public static bool InitDuplexService(ref ServiceHost Host, string ServerIP, int HttpPort, int TcpPort)
        {
            if (!ServerConfig.Instance.Services.Duplex.Enable)
                return true;
            string _uri = string.Empty;
            Binding _binding = null;
            try
            {
                if (ServerConfig.Instance.Services.Duplex.Protocol.Trim().ToLower() == "tcp")
                {
                    //Tcp协议
                    _uri = string.Format("net.tcp://{0}:{1}/Duplex", ServerIP, TcpPort);
                    _binding = ServiceFuncLib.GetServiceBinding<NetTcpBinding>();
                }
                else
                {
                    //http协议
                    _uri = string.Format("http://{0}:{1}/Duplex", ServerIP, HttpPort);
                    _binding = ServiceFuncLib.GetServiceBinding<WSDualHttpBinding>();
                }
                Host = new ServiceHost(typeof(DuplexDoService));
                Host.AddServiceEndpoint(typeof(IDuplexService), _binding, _uri);
                Host.Open();
                
                Common.Log.WriteToLog(string.Format("初始化双工服务[{0}]", _uri), false);
                return true;
            }
            catch (Exception ex)
            {
                Common.Log.WriteToLog(string.Format("初始化双工服务[{0}] 出错:{1}", _uri, ex.Message), true);
            }
            return false;
        }

        /// <summary>
        /// Init public service
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="ServerIP"></param>
        /// <param name="HttpPort"></param>
        /// <param name="TcpPort"></param>
        /// <returns></returns>
        public static bool InitPublicService(ref ServiceHost Host, string ServerIP, int HttpPort, int TcpPort)
        {
            if (!ServerConfig.Instance.Services.Public.Enable)
                return true;
            Uri _uri = null;
            Binding _binding = null;
            try
            {
                if (ServerConfig.Instance.Services.Public.Protocol.Trim().ToLower() == "tcp")
                {
                    //Tcp协议
                    //_uri = string.Format("net.tcp://{0}:{1}/Public", ServerIP, TcpPort);
                    //_binding = ServiceFuncLib.GetServiceBinding<NetTcpBinding>();
                }
                else
                {
                    //http协议
                    _uri = new Uri(string.Format("http://{0}:{1}/Public", ServerIP, HttpPort));
                    //_binding = ServiceFuncLib.GetServiceBinding<BasicHttpBinding>();
                    _binding = new BasicHttpBinding();
                    _binding.Name = "PPServerWS";
                }
                Host = new ServiceHost(typeof(PublicDoService),_uri);

                ServiceMetadataBehavior behaviour = Host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (behaviour == null)
                    behaviour = new ServiceMetadataBehavior();
                //设置允许进行HttpGet操作。  
                behaviour.HttpGetEnabled = true;
                //设置MetadataExporter导出Metadata时遵循WS-Policy 1.5规范。  
                //behaviour.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                
                ////将创建好的behaviour加入到宿主实例的行为描述组当中。  
                Host.Description.Behaviors.Add(behaviour);
                ////加入MetadataExchange endpoint.  
                //Host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), "mex");  

                Host.AddServiceEndpoint(typeof(IPublicService), _binding, _uri);
                //Host.AddServiceEndpoint(typeof(IInnerService), _binding, _uri).Behaviors.Add(new ServiceMetadataBehavior() { HttpGetEnabled=true, FaultExceptionEnabled = true, DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Json });
                Host.Open();
                Common.Log.WriteToLog(string.Format("初始化外部服务[{0}]", _uri), false);
                return true;
            }
            catch (Exception ex)
            {
                Common.Log.WriteToLog(string.Format("初始化外部服务[{0}] 出错:{1}", _uri, ex.Message), true);
            }
            return false;
        }

        /// <summary>
        /// Quit system and release resource
        /// </summary>
        /// <param name="HostList">Service host list</param>
        public static void CleanSystemService(List<ServiceHost> HostList)
        {
            foreach (ServiceHost host in HostList)
            {
                try
                {
                    if (host != null)
                    {
                        host.Abort();
                        host.Close();
                    }
                }
                catch { }
            }
        }
        #endregion
    }
}
