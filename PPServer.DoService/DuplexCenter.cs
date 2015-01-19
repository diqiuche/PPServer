using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;
using System.Threading;
using PPServer.Object;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/9/5
 * Description:双工服务中心，管理在线用户
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.DoService
{
    public class DuplexCenter : DoServiceBase
    {
        private static readonly object SYNC_LOCK = new object();//线程同步锁
        private static int OFFLINE_MESSAGE_TIMEOUT = Config.ServerConfig.Instance.System.OfflineMessageTimeout;  //检测在线用户超时时间(d)
        private static string USERTYPE_SPLIT = Config.ServerConfig.Instance.System.UserTypeSplitChar;           //用户类型分隔符
        private const string SYSTEM_USER_NAME = "[SYSTEM]";     //系统用户名
        private static DuplexCenter _instance;
        List<User> OnlineUsers = new List<User>();

        protected Thread OnlineUsersThread = null;               //检测用户状态线程

        private DuplexCenter()
        {
            try
            {
                //OnlineUsersThread = new Thread(new ThreadStart(CheckUsersOnline));
                //OnlineUsersThread.Start();
            }
            catch (Exception ex)
            {
                WriteToLog("开启用户在线检测线程出错：{0}", true, ex.Message);
            }
        }
        /// <summary>
        /// 获取双工服务的唯一实例；
        /// </summary>
        public static DuplexCenter Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SYNC_LOCK)
                    {
                        if (_instance == null)
                        {
                            _instance = new DuplexCenter();                            
                        }
                    }
                }
                return _instance;
            }
        }

        public string UserLogin(User User)
        {
            //注册用户并添加至在线用户列表后，查看缓存池中是否有待收消息，如果有则发送消息至该用户
            User _user = GetOnlineUser(User.UserName);
            if (_user != null)
            {
                try
                {
                    _user.Notify(new DuplexMessage(SYSTEM_USER_NAME, new List<string>() { _user.UserName }, DuplexMessage.CommandTypes.LOGOUT, string.Empty, null));
                    WriteToLog(string.Format("强行登出用户[{0}]，IP[{1}:{2}]", _user.UserName, _user.IP, _user.Port));
                }
                catch
                { }
                finally
                {
                    RemoveUser(_user);
                }
            }
            lock (SYNC_LOCK)
            {
                OnlineUsers.Add(User);
            }
            User.OnUserChannelFault += new Action<DoService.User>(User_OnUserChannelFault);
            WriteToLog(string.Format("接入用户[{0}]，IP[{1}:{2}]",User.UserName,User.IP,User.Port));
            //获取离线消息
            GetOfflineMessage(User.UserName);
            //广播至所有用户
            BroadcastOnlineUserList();
            return User.UserId;
        }

        void User_OnUserChannelFault(User Sender)
        {
            lock (SYNC_LOCK)
            {
                if (OnlineUsers.Contains(Sender))
                    OnlineUsers.Remove(Sender);
                WriteToLog(string.Format("用户[{0}]连接异常，系统自动清理", Sender.UserName));
            }
            BroadcastOnlineUserList();
        }

        public void UserLogout(string UserName,string UserId)
        {
            User _user = GetOnlineUser(UserName, UserId,false);
            if (_user != null)
            {
                RemoveUser(_user);
                WriteToLog(string.Format("登出用户[{0}]，IP[{1}:{2}]", UserName, _user.IP, _user.Port));
                //广播至所有用户
                BroadcastOnlineUserList();
            }
            else
                throw new Exception("找不到对应的在线用户");
        }

        private void RemoveUser(User User)
        {
            lock (SYNC_LOCK)
            {
                try
                {
                    OnlineUsers.Remove(User);
                }
                catch { }
            }
        }
        /// <summary>
        /// 直接发送消息至指定用户，仅内部使用
        /// </summary>
        /// <param name="User">源用户</param>
        /// <param name="Message">发送的消息</param>
        public void SendMessage(DuplexMessage Message)
        { 
            //先查看是否为广播消息
            //再查看接收用户是否在线，如果不在线并且需发送离线消息则缓存
            if (Message.IsBroadcast)
            {
                BroadcastMessage(Message);
            }
            else if(Message.ToUsers!=null && Message.ToUsers.Count>0)
            {
                //先循环，直接发送至用户的消息则发送，发送至用户类型的消息则记录，稍后发送
                List<string> _userTypes = new List<string>();
                foreach (string userName in Message.ToUsers)
                {
                    if (userName.EndsWith(USERTYPE_SPLIT))  //用户类型
                        _userTypes.Add(userName.TrimEnd(USERTYPE_SPLIT[0]));
                    else                                    //指定用户
                    {
                        User _toUser = GetOnlineUser(userName);
                        DuplexMessage _userMsg = Message.Clone(userName);
                        if (_toUser == null && _userMsg.OfflineSave)
                        {
                            //离线
                            SaveMessageCache(_userMsg);
                        }
                        else if (_toUser != null)
                        {
                            //在线
                            SendMessage(_toUser, _userMsg);
                        }
                        else
                        {
                            WriteToLog("用户[{0}]不在线，由[{1}]发送的消息无法抵达", false, userName, Message.FromUser);
                        }
                    }
                }
                if(_userTypes.Count>0)
                    BroadcastMessage(Message, _userTypes);
            }
        }

        /// <summary>
        /// 源用户通过身份验证后发送消息至指定用户
        /// </summary>
        /// <param name="UserName">源用户名</param>
        /// <param name="UserId">源用户标识</param>
        /// <param name="Message">发送的消息</param>
        public void SendMessage(string UserName,string UserId, DuplexMessage Message)
        {
            User _user=GetOnlineUser(UserName, UserId);
            if (_user != null)
                SendMessage(Message);
            else
                throw new Exception("发送用户" + UserName + "不在线或用户标识不正确，请重新登录");
        }

        /// <summary>
        /// 广播消息至所有在线用户
        /// </summary>
        /// <param name="FromUser"></param>
        /// <param name="Message"></param>
        private void BroadcastMessage(DuplexMessage Message,List<string> UserType=null)
        {
            WriteToLog("用户[{0}]广播{1}至{2}：{3}", false, Message.FromUser.Trim('[', ']'), Message.MessageType == DuplexMessage.MessageTypes.Command ? ("命令" + Message.CommandType.ToString()) : "消息", (UserType == null || UserType.Count == 0) ? "所有在线用户" : ("[用户类型" + string.Join(",", UserType) + "]"), Message.ToString());
            lock (SYNC_LOCK)
            {                
                foreach (User user in OnlineUsers)
                {
                    try
                    {
                        if (UserType == null || UserType.Count == 0 || UserType.Contains(user.UserType))
                        if (user.UserName.ToUpper() != Message.FromUser.ToUpper())
                            user.Notify(Message);                        
                    }
                    catch
                    {
                    }
                }
            }
        }
        /// <summary>
        /// 发送在线用户列表至指定用户
        /// </summary>
        /// <param name="ToUser">指定用户</param>
        public void SendOnlineUserList(string UserName, string UserId)
        {
            string _userList=string.Join("\t",GetOnlineUserList());
            DuplexMessage _msg = new DuplexMessage(UserName, new List<string>() { UserName }, DuplexMessage.CommandTypes.GET_ONLINE_USERS, _userList, null);
            SendMessage(UserName, UserId, _msg);
        }
        /// <summary>
        /// 广播在线用户列表至所有在线用户
        /// </summary>
        public void BroadcastOnlineUserList()
        {
            string _userList = string.Join("\t", GetOnlineUserList());
            DuplexMessage _msg = new DuplexMessage(SYSTEM_USER_NAME, DuplexMessage.CommandTypes.GET_ONLINE_USERS, _userList, null);
            BroadcastMessage(_msg);
        }

        /// <summary>
        /// 通过用户名与用户标识获得在线用户
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="UserId">用户标识</param>
        /// <returns></returns>
        public User GetOnlineUser(string UserName, string UserId, bool ToActive = true)
        {
            User _user = null;
            //lock (SYNC_LOCK)
            {
                _user = OnlineUsers.Where(t => t.UserName.ToUpper() == UserName.ToUpper() && t.UserId == UserId).FirstOrDefault();
            }
            if (_user != null && ToActive)
                _user.Active();
            return _user;
        }

        /// <summary>
        /// 通过用户名获得在线用户
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <returns></returns>
        public User GetOnlineUser(string UserName)
        {
            User _user = null;
            //lock (SYNC_LOCK)
            {
                _user = OnlineUsers.Where(t => t.UserName.ToUpper() == UserName.ToUpper()).FirstOrDefault();
            }
            return _user;
        }

        /// <summary>
        /// 获得在线用户列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetOnlineUserList()
        {
            List<string> _userList = new List<string>();
            //lock (SYNC_LOCK)
            {
                _userList = OnlineUsers.Select(t => t.UserName).ToList();
            }
            return _userList;
        }
        /// <summary>
        /// 保存消息至文件缓存
        /// </summary>
        /// <param name="Message">消息列表</param>
        private void SaveMessageCache(DuplexMessage Message)
        {
            string _fileName = string.Format("{0}#{1}#{2}.msg", Message.ToUsers[0], DateTime.Now.ToString("HHmmss"), Message.MessageId);
            try
            {                
                string _fileFolder = FolderList["Cache"].RealPath + DateTime.Now.ToString("yyyyMMdd")+"\\";
                if (!Directory.Exists(_fileFolder))
                    Directory.CreateDirectory(_fileFolder);
                Message.SaveDataContractToXML(_fileFolder + _fileName);
                WriteToLog("用户[{0}]不在线，由[{1}]发送的消息缓存至\"{2}\"", false, Message.ToUsers[0],Message.FromUser, _fileName);
            }
            catch (Exception ex)
            {
                WriteErrorToLog(ex, "由[{0}]发送至[{1}]的消息缓存至本地文件\"{2}\"时出错。", Message.FromUser, Message.ToUsers[0], _fileName);
                //throw new FaultException("缓存发送至用户["+Message.ToUsers[0]+"]的消息\"" + Message.MessageId + "\"出错：" + ex.Message);
            }
        }

        /// <summary>
        /// 向目标用户发送消息
        /// </summary>
        /// <param name="ToUser">目标用户</param>
        /// <param name="Message">消息内容</param>
        private void SendMessage(User ToUser, DuplexMessage Message)
        {
            try
            {
                ToUser.Notify(Message);
                WriteToLog("用户[{0}]发送{1}至[{2}]：{3}", false, Message.FromUser.Trim('[', ']'), Message.MessageType == DuplexMessage.MessageTypes.Command ? ("命令" + Message.CommandType.ToString()) : "消息", Message.ToUsers[0], Message.ToString());
            }
            catch
            {
                if (Message.OfflineSave)
                    SaveMessageCache(Message);
            }
        }
        
        private void GetOfflineMessage(string UserName)
        {
            try
            { 
                //查找所有缓存文件夹中的所有文件，如果有以用户名开头的文件，则视为该用户离线文件
                string _cacheFolder = FolderList["Cache"].RealPath;
                (new DirectoryInfo(_cacheFolder)).GetDirectories().ToList().ForEach(t =>
                {
                    int _dirName = 0;
                    //查找当前时间与离线超时时间之间内的文件夹
                    if (int.TryParse(t.Name, out _dirName) && int.Parse(DateTime.Now.AddDays(-OFFLINE_MESSAGE_TIMEOUT).ToString("yyyyMMdd")) <= _dirName)
                    {
                        //查找所有msg并且用户名匹配的文件
                        t.GetFiles().ToList().ForEach(f =>
                        {
                            if (f.Extension == ".msg" && f.Name.Split('#')[0].ToUpper() == UserName.ToUpper())
                            {
                                DuplexMessage _offlineMsg = DuplexMessage.LoadDataContractFromXML<DuplexMessage>(f.FullName);
                                File.Delete(f.FullName);
                                SendMessage(_offlineMsg);
                            }
                        });
                    }
                });
            }
            catch(Exception ex)
            {
                WriteToLog("获取离线消息出错：{0}", true, ex.Message);
            }
        }
    }
}
