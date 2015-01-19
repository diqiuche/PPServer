namespace PPTest
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnInner = new System.Windows.Forms.Button();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.btnSendCommand = new System.Windows.Forms.Button();
            this.btnBCMessage = new System.Windows.Forms.Button();
            this.btnBCCommand = new System.Windows.Forms.Button();
            this.lblOnline = new System.Windows.Forms.Label();
            this.btnInit = new System.Windows.Forms.Button();
            this.lblOnlineUsers = new System.Windows.Forms.Label();
            this.btnGetOnlineUsers = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRcvMsg = new System.Windows.Forms.TextBox();
            this.txtRcvCmd = new System.Windows.Forms.TextBox();
            this.txtBCMsg = new System.Windows.Forms.TextBox();
            this.txtBCCmd = new System.Windows.Forms.TextBox();
            this.txtBCCmdDataKey = new System.Windows.Forms.TextBox();
            this.txtBCCmdData = new System.Windows.Forms.TextBox();
            this.txtSendMsg = new System.Windows.Forms.TextBox();
            this.txtSendUser = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(148, 100);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(75, 23);
            this.btnUpload.TabIndex = 0;
            this.btnUpload.Text = "上传文件";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(148, 138);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(75, 23);
            this.btnDown.TabIndex = 0;
            this.btnDown.Text = "下载文件";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(148, 182);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 0;
            this.btnSend.Text = "发送文件";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnInner
            // 
            this.btnInner.Location = new System.Drawing.Point(250, 100);
            this.btnInner.Name = "btnInner";
            this.btnInner.Size = new System.Drawing.Size(75, 23);
            this.btnInner.TabIndex = 1;
            this.btnInner.Text = "内部测试";
            this.btnInner.UseVisualStyleBackColor = true;
            this.btnInner.Click += new System.EventHandler(this.btnInner_Click);
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(144, 326);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(100, 21);
            this.txtUserName.TabIndex = 2;
            this.txtUserName.Text = "liuyan";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(259, 326);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(100, 21);
            this.txtPassword.TabIndex = 2;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(365, 324);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 1;
            this.btnLogin.Text = "登录测试";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Location = new System.Drawing.Point(344, 138);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(75, 23);
            this.btnSendMessage.TabIndex = 1;
            this.btnSendMessage.Text = "发送消息";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // btnSendCommand
            // 
            this.btnSendCommand.Location = new System.Drawing.Point(344, 176);
            this.btnSendCommand.Name = "btnSendCommand";
            this.btnSendCommand.Size = new System.Drawing.Size(75, 23);
            this.btnSendCommand.TabIndex = 1;
            this.btnSendCommand.Text = "发送命令";
            this.btnSendCommand.UseVisualStyleBackColor = true;
            this.btnSendCommand.Click += new System.EventHandler(this.btnSendCommand_Click);
            // 
            // btnBCMessage
            // 
            this.btnBCMessage.Location = new System.Drawing.Point(344, 220);
            this.btnBCMessage.Name = "btnBCMessage";
            this.btnBCMessage.Size = new System.Drawing.Size(75, 23);
            this.btnBCMessage.TabIndex = 1;
            this.btnBCMessage.Text = "广播消息";
            this.btnBCMessage.UseVisualStyleBackColor = true;
            this.btnBCMessage.Click += new System.EventHandler(this.btnBCMessage_Click);
            // 
            // btnBCCommand
            // 
            this.btnBCCommand.Location = new System.Drawing.Point(344, 272);
            this.btnBCCommand.Name = "btnBCCommand";
            this.btnBCCommand.Size = new System.Drawing.Size(75, 23);
            this.btnBCCommand.TabIndex = 1;
            this.btnBCCommand.Text = "广播命令";
            this.btnBCCommand.UseVisualStyleBackColor = true;
            this.btnBCCommand.Click += new System.EventHandler(this.btnBCCommand_Click);
            // 
            // lblOnline
            // 
            this.lblOnline.AutoSize = true;
            this.lblOnline.Location = new System.Drawing.Point(12, 9);
            this.lblOnline.Name = "lblOnline";
            this.lblOnline.Size = new System.Drawing.Size(29, 12);
            this.lblOnline.TabIndex = 3;
            this.lblOnline.Text = "离线";
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(250, 40);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(75, 23);
            this.btnInit.TabIndex = 1;
            this.btnInit.Text = "初始化";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // lblOnlineUsers
            // 
            this.lblOnlineUsers.AutoSize = true;
            this.lblOnlineUsers.Location = new System.Drawing.Point(12, 40);
            this.lblOnlineUsers.Name = "lblOnlineUsers";
            this.lblOnlineUsers.Size = new System.Drawing.Size(77, 12);
            this.lblOnlineUsers.TabIndex = 4;
            this.lblOnlineUsers.Text = "在线用户列表";
            // 
            // btnGetOnlineUsers
            // 
            this.btnGetOnlineUsers.Location = new System.Drawing.Point(344, 100);
            this.btnGetOnlineUsers.Name = "btnGetOnlineUsers";
            this.btnGetOnlineUsers.Size = new System.Drawing.Size(75, 23);
            this.btnGetOnlineUsers.TabIndex = 5;
            this.btnGetOnlineUsers.Text = "获取在线用户";
            this.btnGetOnlineUsers.UseVisualStyleBackColor = true;
            this.btnGetOnlineUsers.Click += new System.EventHandler(this.btnGetOnlineUsers_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(622, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "收到的消息";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(622, 181);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "收到的命令";
            // 
            // txtRcvMsg
            // 
            this.txtRcvMsg.Location = new System.Drawing.Point(624, 44);
            this.txtRcvMsg.Multiline = true;
            this.txtRcvMsg.Name = "txtRcvMsg";
            this.txtRcvMsg.Size = new System.Drawing.Size(316, 117);
            this.txtRcvMsg.TabIndex = 8;
            // 
            // txtRcvCmd
            // 
            this.txtRcvCmd.Location = new System.Drawing.Point(624, 196);
            this.txtRcvCmd.Multiline = true;
            this.txtRcvCmd.Name = "txtRcvCmd";
            this.txtRcvCmd.Size = new System.Drawing.Size(316, 117);
            this.txtRcvCmd.TabIndex = 9;
            // 
            // txtBCMsg
            // 
            this.txtBCMsg.Location = new System.Drawing.Point(434, 222);
            this.txtBCMsg.Name = "txtBCMsg";
            this.txtBCMsg.Size = new System.Drawing.Size(139, 21);
            this.txtBCMsg.TabIndex = 10;
            // 
            // txtBCCmd
            // 
            this.txtBCCmd.Location = new System.Drawing.Point(434, 263);
            this.txtBCCmd.Name = "txtBCCmd";
            this.txtBCCmd.Size = new System.Drawing.Size(139, 21);
            this.txtBCCmd.TabIndex = 10;
            // 
            // txtBCCmdDataKey
            // 
            this.txtBCCmdDataKey.Location = new System.Drawing.Point(434, 285);
            this.txtBCCmdDataKey.Name = "txtBCCmdDataKey";
            this.txtBCCmdDataKey.Size = new System.Drawing.Size(54, 21);
            this.txtBCCmdDataKey.TabIndex = 10;
            // 
            // txtBCCmdData
            // 
            this.txtBCCmdData.Location = new System.Drawing.Point(488, 285);
            this.txtBCCmdData.Name = "txtBCCmdData";
            this.txtBCCmdData.Size = new System.Drawing.Size(85, 21);
            this.txtBCCmdData.TabIndex = 11;
            // 
            // txtSendMsg
            // 
            this.txtSendMsg.Location = new System.Drawing.Point(488, 140);
            this.txtSendMsg.Name = "txtSendMsg";
            this.txtSendMsg.Size = new System.Drawing.Size(105, 21);
            this.txtSendMsg.TabIndex = 12;
            // 
            // txtSendUser
            // 
            this.txtSendUser.Location = new System.Drawing.Point(434, 140);
            this.txtSendUser.Name = "txtSendUser";
            this.txtSendUser.Size = new System.Drawing.Size(54, 21);
            this.txtSendUser.TabIndex = 13;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(488, 18);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(958, 359);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtSendUser);
            this.Controls.Add(this.txtSendMsg);
            this.Controls.Add(this.txtBCCmdData);
            this.Controls.Add(this.txtBCCmdDataKey);
            this.Controls.Add(this.txtBCCmd);
            this.Controls.Add(this.txtBCMsg);
            this.Controls.Add(this.txtRcvCmd);
            this.Controls.Add(this.txtRcvMsg);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnGetOnlineUsers);
            this.Controls.Add(this.lblOnlineUsers);
            this.Controls.Add(this.lblOnline);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.btnInit);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.btnBCCommand);
            this.Controls.Add(this.btnBCMessage);
            this.Controls.Add(this.btnSendCommand);
            this.Controls.Add(this.btnSendMessage);
            this.Controls.Add(this.btnInner);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUpload);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnInner;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.Button btnSendCommand;
        private System.Windows.Forms.Button btnBCMessage;
        private System.Windows.Forms.Button btnBCCommand;
        private System.Windows.Forms.Label lblOnline;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.Label lblOnlineUsers;
        private System.Windows.Forms.Button btnGetOnlineUsers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtRcvMsg;
        private System.Windows.Forms.TextBox txtRcvCmd;
        private System.Windows.Forms.TextBox txtBCMsg;
        private System.Windows.Forms.TextBox txtBCCmd;
        private System.Windows.Forms.TextBox txtBCCmdDataKey;
        private System.Windows.Forms.TextBox txtBCCmdData;
        private System.Windows.Forms.TextBox txtSendMsg;
        private System.Windows.Forms.TextBox txtSendUser;
        private System.Windows.Forms.Button button1;
    }
}

