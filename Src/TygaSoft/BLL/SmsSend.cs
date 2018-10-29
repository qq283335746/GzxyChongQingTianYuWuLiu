using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;
using TygaSoft.IDAL;
using TygaSoft.Model;
using TygaSoft.DALFactory;
using TygaSoft.IBLLStrategy;
using TygaSoft.MessagingFactory;

namespace TygaSoft.BLL
{
    public class SmsSend
    {
        private static readonly ISmsSend dal = DataAccess.CreateSmsSend();
        private static readonly ISmsSendStrategy smsInsertStrategy = LoadInsertStrategy();
        private static readonly IMessaging.ISmsSend smsQueue = QueueAccess.CreateSmsSend();

        #region 成员方法

        /// <summary>
        /// 使用异步或同步
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void InsertByStrategy(SmsSendInfo model)
        {
            smsInsertStrategy.Insert(model);
        }

        public SmsSendInfo ReceiveFromQueue(int timeout)
        {
            return smsQueue.Receive(timeout);
        }

        /// <summary>
        /// 添加数据到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SmsSendInfo model)
        {
            return dal.Insert(model);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(SmsSendInfo model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除对应数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public int Delete(object Id)
        {
            return dal.Delete(Id);
        }

        /// <summary>
        /// 批量删除数据（启用事务）
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool DeleteBatch(IList<object> list)
        {
            return dal.DeleteBatch(list);
        }

        /// <summary>
        /// 获取对应的数据
        /// </summary>
        /// <param name="numberId"></param>
        /// <returns></returns>
        public SmsSendInfo GetModel(object Id)
        {
            return dal.GetModel(Id);
        }

        /// <summary>
        /// 获取数据分页列表，并返回所有记录数
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public List<SmsSendInfo> GetList(int pageIndex, int pageSize, out int totalRecords, string sqlWhere, params SqlParameter[] cmdParms)
        {
            return dal.GetList(pageIndex, pageSize, out totalRecords, sqlWhere, cmdParms);
        }

        /// <summary>
        /// 获取满足当前条件的数据列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public List<SmsSendInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms)
        {
            return dal.GetList(pageIndex, pageSize, sqlWhere, cmdParms);
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        public List<SmsSendInfo> GetList(string sqlWhere, params SqlParameter[] cmdParms)
        {
            return dal.GetList(sqlWhere, cmdParms);
        }

        /// <summary>
        /// 获取当前订单参数集
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        public DataSet GetSmsParams(string orderCode)
        {
            return dal.GetSmsParams(orderCode);
        }

        /// <summary>
        /// 获取当前订单号下要发送的手机号和短信内容参数对应值
        /// </summary>
        /// <param name="orderCode"></param>
        /// <param name="tranNode"></param>
        /// <param name="mobile"></param>
        /// <param name="dt"></param>
        public void GetSmsSendMobile(string orderCode, string tranNode, out string mobile, out DataTable dt)
        {
            mobile = string.Empty;
            dt = null;
            DataSet ds = GetSmsParams(orderCode);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                dt = ds.Tables[0];
            }
            if (dt == null) return;

            DataRow dr = dt.Rows[0];

            switch (tranNode)
            {
                case "SendGoods":
                    if (dr["ReceiverMobilePhone"] != DBNull.Value)
                    {
                        mobile = dr["ReceiverMobilePhone"].ToString().Trim();
                    }
                    break;
                case "MainSend":
                    if (dr["SenderMobilePhone"] != DBNull.Value)
                    {
                        mobile += dr["SenderMobilePhone"].ToString().Trim() + ",";
                    }
                    if (dr["ReceiverMobilePhone"] != DBNull.Value)
                    {
                        mobile += dr["ReceiverMobilePhone"].ToString().Trim();
                    }
                    mobile = mobile.Trim(',');
                    break;
                case "MainReach":
                    if (dr["SenderMobilePhone"] != DBNull.Value)
                    {
                        mobile += dr["SenderMobilePhone"].ToString().Trim() + ",";
                    }
                    if (dr["ReceiverMobilePhone"] != DBNull.Value)
                    {
                        mobile += dr["ReceiverMobilePhone"].ToString().Trim();
                    }
                    mobile = mobile.Trim(',');
                    break;
                case "GoodsShipment":
                    if (dr["SenderMobilePhone"] != DBNull.Value)
                    {
                        mobile += dr["SenderMobilePhone"].ToString().Trim() + ",";
                    }
                    if (dr["ReceiverMobilePhone"] != DBNull.Value)
                    {
                        mobile += dr["ReceiverMobilePhone"].ToString().Trim();
                    }
                    mobile = mobile.Trim(',');
                    break;
                case "CustomerReceive":
                    if (dr["SenderMobilePhone"] != DBNull.Value)
                    {
                        mobile = dr["SenderMobilePhone"].ToString().Trim();
                    }
                    break;
                default:
                    if (dr["SenderMobilePhone"] != DBNull.Value)
                    {
                        mobile += dr["SenderMobilePhone"].ToString().Trim() + ",";
                    }
                    if (dr["ReceiverMobilePhone"] != DBNull.Value)
                    {
                        mobile += dr["ReceiverMobilePhone"].ToString().Trim();
                    }
                    mobile = mobile.Trim(',');
                    break;
            }
        }

        /// <summary>
        /// 获取短信内容
        /// </summary>
        /// <param name="orderCode"></param>
        /// <param name="content"></param>
        /// <param name="paramsCode"></param>
        /// <returns></returns>
        public string GetSmsContent(string orderCode,string content, string paramsCode)
        {
            DataSet ds = GetSmsParams(orderCode);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return "";

            DataTable dt = ds.Tables[0];
            DataColumnCollection dcc = dt.Columns;
            DataRow dr = dt.Rows[0];

            string[] tParms = paramsCode.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] tParmsValues = new string[tParms.Length];

            for (int i = 0; i < tParms.Length; i++)
            {
                if (dcc.Contains(tParms[i]))
                {
                    if (dr[tParms[i]] != DBNull.Value)
                    {
                        tParmsValues[i] = dr[tParms[i]].ToString();
                    }
                    else
                    {
                        tParmsValues[i] = "";
                    }
                }
                else
                {
                    tParmsValues[i] = "";
                }
            }

            return string.Format(content, tParmsValues);
        }

        private static ISmsSendStrategy LoadInsertStrategy()
        {
            string path = ConfigurationManager.AppSettings["StrategyAssembly"];
            string className = ConfigurationManager.AppSettings["SmsStrategyClass"];

            return (ISmsSendStrategy)Assembly.Load(path).CreateInstance(className);
        }

        #endregion
    }
}
