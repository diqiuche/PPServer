using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/7/10
 * Description:定义公共数据集
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.Common
{
    /// <summary>
    /// 公共数据集
    /// </summary>
    public class CommonData
    {
        /// <summary>
        /// application startup path
        /// </summary>
        public static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// config file name
        /// </summary>
        public const string ConfigFile = "config.xml";

    }
}
