using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.IO;
using PPServer.Object;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/9/4
 * Description:公开文件传输服务：下载、上传、传送
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.Service
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(ServiceFuncLib))]
    public interface IFileService : ServiceBase
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="FileInfo">文件数据,须在SystemInfo中添加UserName和Password</param>
        [OperationContract(IsOneWay=true)]
        void Upload(FileMessage FileInfo);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="ServerMapPath">服务端文件路径</param>
        /// <param name="ToCompress">是否压缩传输</param>
        /// <param name="ToEncrypt">是否加密传输</param>
        /// <returns></returns>
        [OperationContract(IsOneWay = false)]
        FileMessage Download(FileMessageRequest Request);

        /// <summary>
        /// 传送文件至指定用户，用户不在线时则缓存至服务器
        /// </summary>
        /// <param name="FileInfo">文件信息,otherinfo中:UserName-用户名,Password-密码,ToUser-接收用户,bool OfflineSave-是否发送离线文件</param>
        [OperationContract(IsOneWay = true)]
        void Send(FileMessage FileInfo);

        /// <summary>
        /// 获取更新文件列表
        /// </summary>
        /// <param name="UpdatePath">服务端更新文件夹路径</param>
        /// <returns></returns>
        [OperationContract(IsOneWay = false)]
        List<FileInfoRequest> GetUpdateFiles(string UpdatePath);
    }
}
