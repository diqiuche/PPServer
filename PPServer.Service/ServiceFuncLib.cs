using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace PPServer.Service
{
    /// <summary>
    /// 设置服务已知对象类
    /// </summary>
    public class ServiceFuncLib
    {
        #region 服务已知对象
        //服务已知对象集合
        private static List<Type> _knownTypes = null;

        private static List<string> _knowTypeDlls = new List<string>();
        /// <summary>
        /// 从配置文件中获取所有服务已知对象
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
        {
            try
            {
                if (_knownTypes == null)
                {
                    _knownTypes = new List<Type>();
                    SetKnownTypes(_knowTypeDlls.ToArray());
                }
            }
            catch
            { }
            return _knownTypes;
        }
        /// <summary>
        /// 设置服务已知对象
        /// </summary>
        /// <param name="Types">已知对象列表</param>
        public static void SetKnownTypes(List<Type> Types)
        {
            if (Types != null)
            {
                _knownTypes = new List<Type>();
                Types.ForEach(t => { AddType(t, ref _knownTypes); });
            }
        }
        /// <summary>
        /// 增加服务已知对象集合
        /// </summary>
        /// <param name="Types">已知对象集合</param>
        public static void AddKnownTypes(List<Type> Types)
        {
            if(Types!=null)
                Types.ForEach(t => { AddType(t, ref _knownTypes); });
        }
        /// <summary>
        /// 增加服务已知对象
        /// </summary>
        /// <param name="Type">已知对象</param>
        public static void AddKnownType(Type Type)
        {
            if (Type != null)
                AddType(Type, ref _knownTypes);
        }
        /// <summary>
        /// 设置服务已知对象
        /// </summary>
        /// <param name="FilePathCollection">文件路径集合</param>
        public static void SetKnownTypes(params string[] FilePathCollection)
        {
            if (FilePathCollection != null && FilePathCollection.Length > 0)
            {
                try
                {
                    _knownTypes = new List<Type>();
                    //从入参中文件路径加载KnownTypes
                    foreach (string filePath in FilePathCollection)
                    {
                        Type[] _dllTypes = Assembly.LoadFrom(filePath).GetTypes();
                        foreach (Type type in _dllTypes)
                        {
                            if (type.IsSerializable)
                                AddType(type, ref _knownTypes);
                        }
                    }
                }
                catch
                {}
            }
        }

        private static void AddType(Type KnownType,ref List<Type> TypesList)
        {
            if (TypesList == null)
                TypesList = new List<Type>();

            TypesList.Add(KnownType);
            TypesList.Add(typeof(List<>).MakeGenericType(new System.Type[] { KnownType }));
        }
        #endregion

        #region 绑定协议
        /// <summary>
        /// 获得服务绑定
        /// </summary>
        /// <typeparam name="T">从binding继承的服务绑定</typeparam>
        /// <param name="IsFileService">是否为文件服务</param>
        /// <returns></returns>
        public static T GetServiceBinding<T>(bool IsFileService = false) where T : Binding, new()
        {
            if (typeof(T) == typeof(BasicHttpBinding))
            {
                BasicHttpBinding _binding = new BasicHttpBinding();
                _binding.Security.Mode = BasicHttpSecurityMode.None;
                _binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                _binding.ReceiveTimeout = TimeSpan.MaxValue;//设置连接自动断开的空闲时长；
                _binding.SendTimeout = new TimeSpan(0, 5, 0);
                _binding.TransferMode = TransferMode.Buffered;
                _binding.AllowCookies = false;
                _binding.BypassProxyOnLocal = false;
                _binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                _binding.UseDefaultWebProxy = true;
                _binding.MaxBufferPoolSize = 4096;
                _binding.MaxBufferSize = 2147483647;
                _binding.MaxReceivedMessageSize = 2147483647;
                _binding.ReaderQuotas.MaxDepth = 32;
                _binding.ReaderQuotas.MaxArrayLength = 2147483647;
                _binding.ReaderQuotas.MaxStringContentLength = 2147483647;
                _binding.ReaderQuotas.MaxBytesPerRead = 4096;
                _binding.ReaderQuotas.MaxNameTableCharCount = 16384;
                _binding.OpenTimeout = new TimeSpan(0, 5, 0);
                _binding.CloseTimeout = new TimeSpan(0, 5, 0);

                //如果是文件传输则使用流传输
                if (IsFileService)
                {
                    _binding.TransferMode = TransferMode.Streamed;
                    _binding.MessageEncoding = WSMessageEncoding.Mtom;
                }
                return _binding as T;
            }
            if (typeof(T) == typeof(WSHttpBinding))
            {
                WSHttpBinding _binding = new WSHttpBinding();
                _binding.Security.Mode = SecurityMode.None;
                _binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                _binding.ReceiveTimeout = TimeSpan.MaxValue;//设置连接自动断开的空闲时长；
                _binding.SendTimeout = new TimeSpan(0, 5, 0);
                _binding.OpenTimeout = new TimeSpan(0, 5, 0);
                _binding.CloseTimeout = new TimeSpan(0, 5, 0);
                _binding.MessageEncoding = WSMessageEncoding.Text;
                return _binding as T;
            }
            else if (typeof(T) == typeof(WSDualHttpBinding))
            {
                WSDualHttpBinding _binding = new WSDualHttpBinding();
                _binding.Security.Mode = WSDualHttpSecurityMode.None;
                _binding.Security.Message.ClientCredentialType = MessageCredentialType.None;
                _binding.ReceiveTimeout = TimeSpan.MaxValue;//设置连接自动断开的空闲时长；
                _binding.SendTimeout = new TimeSpan(0, 5, 0);
                _binding.BypassProxyOnLocal = false;
                _binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                _binding.UseDefaultWebProxy = true;
                _binding.MaxBufferPoolSize = 4096;
                _binding.MaxReceivedMessageSize = 2147483647;
                _binding.ReaderQuotas.MaxDepth = 32;
                _binding.ReaderQuotas.MaxArrayLength = 2147483647;
                _binding.ReaderQuotas.MaxStringContentLength = 2147483647;
                _binding.ReaderQuotas.MaxBytesPerRead = 4096;
                _binding.ReaderQuotas.MaxNameTableCharCount = 16384;
                _binding.OpenTimeout = new TimeSpan(0, 5, 0);
                _binding.CloseTimeout = new TimeSpan(0, 5, 0);

                return _binding as T;
            }
            else if (typeof(T) == typeof(NetTcpBinding))
            {
                NetTcpBinding _binding = new NetTcpBinding();
                _binding.Security.Mode = SecurityMode.None;
                _binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
                _binding.ReceiveTimeout = TimeSpan.MaxValue;//设置连接自动断开的空闲时长；
                _binding.SendTimeout = new TimeSpan(0, 5, 0);
                _binding.TransferMode = TransferMode.Buffered;
                _binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                _binding.MaxBufferPoolSize = 4096;
                _binding.MaxBufferSize = 2147483647;
                _binding.MaxReceivedMessageSize = 2147483647;
                _binding.ReaderQuotas.MaxDepth = 32;
                _binding.ReaderQuotas.MaxArrayLength = 2147483647;
                _binding.ReaderQuotas.MaxStringContentLength = 2147483647;
                _binding.ReaderQuotas.MaxBytesPerRead = 4096;
                _binding.ReaderQuotas.MaxNameTableCharCount = 16384;
                _binding.OpenTimeout = new TimeSpan(0, 5, 0);
                _binding.CloseTimeout = new TimeSpan(0, 5, 0);

                //如果是文件传输则使用流传输
                if (IsFileService)
                {
                    _binding.TransferMode = TransferMode.Streamed;
                }
                return _binding as T;
            }
            return null;
        }
        #endregion
    }
}
