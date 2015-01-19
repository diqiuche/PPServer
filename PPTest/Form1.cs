using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using PPServer.Object;
using PPServer.Client;
using SimCenter.Plugin.Model;
using System.Runtime.InteropServices;

namespace PPTest
{
    public partial class Form1 : Form
    {
        [DllImport("wininet.dll")]
        public extern static bool InternetGetConnectedState(out int Connection, int ReservedValue);

        PPServer.Client.ServiceCenter _sc = new PPServer.Client.ServiceCenter("localhost", 4000, 4001, ConfigurationManager.AppSettings["UserName"], "");
        public Form1()
        {
            InitializeComponent();
            //_sc.SetKnownTypes("SimCenter.Plugin.Model.dll", "XHHIS.Plugin.Model.dll");
            //_sc.AddKnowTypes(typeof(DuplexCommand));
            _sc.OnProgress += new PPServer.Client.ServiceCenter.OnProgressHandler(_sc_OnProgress);
            _sc.OnDuplexOnlineChanged += new PPServer.Client.ServiceCenter.OnDuplexOnlineChangedHandler(_sc_OnDuplexOnlineChanged);
            _sc.OnUpdateOnlineUsers += new PPServer.Client.ServiceCenter.OnlineUserEventHandler(_sc_OnUpdateOnlineUsers);
            _sc.OnReceivedFile += new ServiceCenter.OnReceivedFileHandler(_sc_OnReceivedFile);
            _sc.OnReceiveTextMessage += new ServiceCenter.OnReceiveTextHandler(_sc_OnReceiveTextMessage);
            _sc.OnReceiveCommandMessage += new ServiceCenter.OnReceiveCommandHandler(_sc_OnReceiveCommandMessage);
            CheckForIllegalCrossThreadCalls = false;
            this.Text = ConfigurationManager.AppSettings["UserName"];
        }

        void _sc_OnReceivedFile(string FromUser, string FilePath, DateTime SendTime, Dictionary<string, object> UserData)
        {
            MessageBox.Show(string.Format("从[{0}]收到文件\"{1}\"，发送时间：{2}", FromUser, FilePath, SendTime));
        }

        void _sc_OnReceiveCommandMessage(string FromUser, string CommandText, Dictionary<string, object> CommandData, DateTime MessageTime, bool IsBroadcast)
        {
            txtRcvCmd.AppendText(string.Format("{0} 从[{1}]{2}命令\"{3}\"：{4}", MessageTime, FromUser, IsBroadcast ? "广播" : "发送", CommandText, CommandData == null ? "" : string.Join(",", CommandData.Select(t => t.Key + "=" + t.Value.ToString()))) + "\r\n");
        }

        void _sc_OnReceiveTextMessage(string FromUser, string TextMessage, DateTime MessageTime, bool IsBroadcast)
        {
            txtRcvMsg.AppendText(string.Format("{0} 从[{1}]{2}消息：{3}", MessageTime, FromUser, IsBroadcast ? "广播" : "发送", TextMessage) + "\r\n");
        }

        void _sc_OnDuplexOnlineChanged(bool IsOnline)
        {            
            lblOnline.Text = IsOnline ? "在线" : "离线";
        }

        void _sc_OnProgress(string FilePath, double Progress, PPServer.Object.FileMessage.TransportStatus Status)
        {
            
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            string _serverFolder=@"Download\";
            string _serverFile = "Grid++Report5.6.exe";
            SaveFileDialog _sf = new SaveFileDialog();
            _sf.FileName = _serverFile;
            if (_sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DoServerFunction _func = new DoServerFunction("AfterDownloadTest", new object[] { "XXA", 23 });
                _sc.DownloadFile(_serverFolder + _serverFile, _sf.FileName, new List<DoServerFunction>() { _func });
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog _of = new OpenFileDialog();
            if (_of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DoServerFunction _func = new DoServerFunction("AfterUploadTest",new object[]{"XXA",23});
                
                _sc.UploadFile(_of.FileName, new List<DoServerFunction>() { _func });
                //_sc.UploadFile(_of.FileName);
            }
        }

        private void btnInner_Click(object sender, EventArgs e)
        {
            List<T_BASE_MENUEntity> _menu = new List<T_BASE_MENUEntity>();
            T_BASE_MENUEntity _test1 = new T_BASE_MENUEntity();
            T_BASE_MENUEntity _test2 = new T_BASE_MENUEntity();
            _test1.CLASS_NAME = "test111";
            _test2.CLASS_NAME = "test222";
            _menu.Add(_test1);
            _menu.Add(_test2);
            ReturnResult _rr = _sc.DoInnerService("GetEntity", "GetMenu", _menu);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            _sc.SetUserInfo(txtUserName.Text, txtPassword.Text);
            ReturnResult _rr = _sc.DoInnerService("SimCenter.Plugin.Base.Login/OnLogin", txtPassword.Text);
            if (_rr.ExeCode==1)
            {
                bool _canLogon = (bool)_rr.ExeResult["CanLogon"];
                MessageBox.Show(_canLogon ? "登录成功" : "登录失败");
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            OpenFileDialog _of = new OpenFileDialog();
            if (_of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DoServerFunction _func = new DoServerFunction("AfterSendTest",new object[]{"xa",123,456});
                Dictionary<string,object> _userData=new Dictionary<string,object>();
                _userData.Add("DATAAA", 123);
                _sc.SendFile(_of.FileName, "TEST_A", _userData, new List<DoServerFunction>() { _func });
            }
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            try
            {
                _sc.ConnectDuplex();
            }
            catch { }
        }

        void _sc_OnUpdateOnlineUsers(string[] Users)
        {
            lblOnlineUsers.Text = string.Join("\r\n", Users);
        }

        private void btnGetOnlineUsers_Click(object sender, EventArgs e)
        {
            _sc.GetOnlineUsers();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _sc.Dispose();
        }

        private void btnBCCommand_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add(txtBCCmdDataKey.Text, txtBCCmdData.Text);
            _sc.BroadcastCommand(txtBCCmd.Text, string.IsNullOrEmpty(txtBCCmdDataKey.Text) ? null : data);
        }

        private void btnBCMessage_Click(object sender, EventArgs e)
        {
            _sc.BroadcastMessage(txtBCMsg.Text);
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            _sc.SendMessage(txtSendUser.Text, txtSendMsg.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int _connection=0;
            bool connected=InternetGetConnectedState(out _connection, 0);
        }

        private void btnSendCommand_Click(object sender, EventArgs e)
        {

        }
    }
}
