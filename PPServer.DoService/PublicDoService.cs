using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Data;
using PPServer.Object;
using PPServer.Interface;
using System.Threading;
using System.ServiceModel;
using System.Reflection;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/7/11
 * Description:执行具体服务操作
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.DoService
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public class PublicDoService : DoServiceBase,PPServer.Service.IPublicService
    {
        /// <summary>
        /// 直接执行服务端方法
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="FunctionName">方法名</param>
        /// <param name="Paras">参数集合</param>
        /// <returns></returns>
        public List<string> DoService(string UserName, string Password, string FunctionName, string[] Paras)
        {
            Stopwatch _sw = new Stopwatch();
            _sw.Start();
            string[] _rr = new string[5];
            _rr[0] = "1";       //返回的结果
            _rr[1] = "";        //返回的消息
            try
            {

                _rr[4] = ToDoService(UserName, Password, FunctionName, Paras);
            }
            catch (Exception ex)
            {
                _rr[0] = "0";
                _rr[1] = "服务端异常："+ex.Message;
                WriteErrorToLog(ex,"用户[{0}]，执行内部服务\"{1}\"出错。", UserName, FunctionName);
            }
            _sw.Stop();
            _rr[2] = _sw.ElapsedMilliseconds.ToString();
            _rr[3] = DateTime.Now.ToString();
            return _rr.ToList(); ;
        }

        public void DoServiceNoReturn(string UserName, string Password, string FunctionName, string Paras)
        {
            try
            {

                ToDoService(UserName, Password, FunctionName, Paras.Split('&'));
            }
            catch (Exception ex)
            {
                WriteErrorToLog(ex, "用户[{0}]，执行内部服务\"{1}\"出错。", UserName, FunctionName);
            }
        }

        /// <summary>
        /// 私有执行服务
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="FunctionName">函数名</param>
        /// <param name="Paras">参数集合</param>
        /// <returns>返回执行字符串</returns>
        private string ToDoService(string UserName, string Password, string FunctionName, string[] Paras)
        {
            //Authenticate User's Identity
            this.ToAuthenticate(UserName, Password, this.GetType().ToString());

            //处理调用方法名
            string _publicFuncName;
            string _privateFuncName;
            GetFunctionName(FunctionName, out _publicFuncName, out _privateFuncName);
            //加载程序集
            if (!Config.ServerConfig.Instance.Services.Public.FunctionList.ContainsKey(_publicFuncName))
                throw new Exception("没有此方法可被调用！");
            Config.Function _func = Config.ServerConfig.Instance.Services.Public.FunctionList[_publicFuncName];
            //参数集合字符串
            string _paraStr = (Paras == null || Paras.Length == 0) ? "无" : (string.Join(",", Paras));

            IPublicFunction _instance = CreateClassInstance<IPublicFunction>(_func.FilePath, _func.ClassName);
            WriteToLog(string.Format("用户[{0}]，开始执行外部服务\"{1}\"，参数：{2}", UserName, FunctionName, _paraStr), false);
            return _instance.DoFunction(UserName, Paras);
        }
    }
}
