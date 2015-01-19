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
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.PerCall)]
    public class InnerDoService : DoServiceBase,PPServer.Service.IInnerService
    {
        /// <summary>
        /// 直接执行服务端方法
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="FunctionName">方法名</param>
        /// <param name="Paras">参数集合</param>
        /// <returns></returns>
        public ReturnResult DoService(string UserName, string Password, string FunctionName, List<object> Paras)
        {
            Stopwatch _sw = new Stopwatch();
            _sw.Start();
            ReturnResult _rr = new ReturnResult();
            try
            {
                //Authenticate User's Identity
                this.ToAuthenticate(UserName, Password, this.GetType().ToString());
                
                //处理调用方法名
                string _publicFuncName;
                string _privateFuncName;
                GetFunctionName(FunctionName, out _publicFuncName, out _privateFuncName);
                //加载程序集
                if(!Config.ServerConfig.Instance.Services.Inner.FunctionList.ContainsKey(_publicFuncName))
                    throw new Exception("没有此方法可被调用！");
                Config.Function _func = Config.ServerConfig.Instance.Services.Inner.FunctionList[_publicFuncName];
                //参数集合字符串
                string _paraStr = (Paras == null || Paras.Count == 0) ? "无" : (string.Join(",", Paras.Select(t => Paras.IndexOf(t) + "='" + t.ToString() + "'")));

                IInnerFunction _instance = CreateClassInstance<IInnerFunction>(_func.FilePath, _func.ClassName);
                WriteToLog(string.Format("用户[{0}]，开始执行内部服务\"{1}\"，参数：{2}", UserName, FunctionName, _paraStr), false);
                
                if (string.IsNullOrEmpty(_privateFuncName))
                    _rr.ExeResult=_instance.DoFunction(UserName, Paras);
                else //当须调用私有函数时
                    _rr.ExeResult = _instance.DoFunction(UserName,_privateFuncName, Paras);
            }
            catch (Exception ex)
            {
                _rr.ExeCode = 0;
                _rr.ExeInfo = "服务端异常："+ex.Message;
                WriteErrorToLog(ex,"用户[{0}]，执行内部服务\"{1}\"出错。", UserName, FunctionName);
            }
            _sw.Stop();
            _rr.ExeTimeSpan = _sw.ElapsedMilliseconds;
            _rr.ServerTime = DateTime.Now;
            return _rr;
        }
    }
}
