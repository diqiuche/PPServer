using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PPServer.Service;
using PPServer.Object;

namespace PPServer.Client
{
    internal class ProcessService
    {
        public static ReturnResult DoInnerService(string UserName, string Password, IInnerService InnerService, string FunctionName, params object[] Param)
        {
            ReturnResult _rr = new ReturnResult();
            try
            {
                List<object> _paras = new List<object>(Param);
                //foreach (object obj in Param)
                //    if (!obj.GetType().IsSerializable)
                //        throw new Exception("参数不可被序列化，无法传输！");
                _rr=InnerService.DoService(UserName, Password, FunctionName, _paras);
            }
            catch (Exception ex)
            {
                _rr.ExeCode = -1;
                _rr.ExeInfo = "本地异常：" + ex.Message + "\r\n" + "详细：" + ex.StackTrace;
            }
            return _rr;
        }
    }
}
