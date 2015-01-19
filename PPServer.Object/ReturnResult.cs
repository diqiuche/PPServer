using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/7/2
 * Description:定义返回的执行结果
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.Object
{
    /// <summary>
    /// 定义返回数据
    /// </summary>
    [DataContract]
    public class ReturnResult : ObjectBase
    {
        /// <summary>
        /// 获取执行后返回代码，1为成功，0为系统性错误,负数为业务处理失败
        /// </summary>
        [DataMember]
        public int ExeCode = 1;

        /// <summary>
        /// 获取执行后返回信息
        /// </summary>
        [DataMember]
        public string ExeInfo = string.Empty;

        /// <summary>
        /// 获取执行时长（毫秒）
        /// </summary>
        [DataMember]
        public long ExeTimeSpan = 0;

        /// <summary>
        /// 获取执行结束服务端时间
        /// </summary>
        [DataMember]
        public DateTime ServerTime = DateTime.Now;

        /// <summary>
        /// 获取传输数据大小
        /// </summary>
        [DataMember]
        public long TransportSize = 0;

        /// <summary>
        /// 获取原始数据大小
        /// </summary>
        [DataMember]
        public long OriginalSize = 0;

        /// <summary>
        /// 获取数据校验码
        /// </summary>
        [DataMember]
        public string VerifyCode = string.Empty;

        /// <summary>
        /// 执行结果，key为返回标志，value为返回内容
        /// </summary>
        [DataMember]
        public Dictionary<string, object> ExeResult = new Dictionary<string, object>();
    }
}
