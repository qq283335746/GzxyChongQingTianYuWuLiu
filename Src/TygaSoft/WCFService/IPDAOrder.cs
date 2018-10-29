using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;
using System.Data.SqlClient;
using TygaSoft.Model;
using TygaSoft.BLL;

namespace TygaSoft.WCFService
{
    [ServiceContract(Namespace = "http://TygaSoft.WCFService")]
    public interface IPDAOrder
    {
        /// <summary>
        /// 测试网络连接
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string HelloWord();

        /// <summary>
        /// 新增PDA扫描数据
        /// </summary>
        /// <param name="opType">操作类型：取货、干线发运、干线到达、送货发运、客户接收</param>
        /// <param name="barCode">单号条码</param>
        /// <param name="scanTime">扫描时间</param>
        /// <param name="userName">用户名</param>
        /// <returns>返回：包含“成功”，则调用成功，否则返回调用失败原因</returns>
        [OperationContract]
        string Insert(string opType, string barCode, DateTime scanTime, string userName);

        /// <summary>
        /// 批量提交PDA扫描数据
        /// </summary>
        /// <param name="dt">应包含opType、barCode、scanTime、userName等列</param>
        /// <returns>返回：包含“成功”，则调用成功，否则返回调用失败原因</returns>
        [OperationContract]
        string InsertByBatch(DataTable dt);

        /// <summary>
        /// 获取所有有效的用户信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string[] GetAllUsers();

        /// <summary>
        /// 验证提供的用户名和密码是有效的。
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [OperationContract]
        string ValidateUser(string username, string password);
    }
}
