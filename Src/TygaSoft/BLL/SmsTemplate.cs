using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using TygaSoft.IDAL;
using TygaSoft.Model;
using TygaSoft.DALFactory;

namespace TygaSoft.BLL
{
    public class SmsTemplate
    {
        private static readonly ISmsTemplate dal = DataAccess.CreateSmsTemplate();

        #region 成员方法

        /// <summary>
        /// 添加数据到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SmsTemplateInfo model)
        {
            return dal.Insert(model);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(SmsTemplateInfo model)
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
        public SmsTemplateInfo GetModel(object Id)
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
        public List<SmsTemplateInfo> GetList(int pageIndex, int pageSize, out int totalRecords, string sqlWhere, params SqlParameter[] cmdParms)
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
        public List<SmsTemplateInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms)
        {
            return dal.GetList(pageIndex, pageSize, sqlWhere, cmdParms);
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        public List<SmsTemplateInfo> GetList(string sqlWhere, params SqlParameter[] cmdParms)
        {
            return dal.GetList(sqlWhere, cmdParms);
        }

        /// <summary>
        /// 设为默认
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public int SetDefault(object Id)
        {
            return dal.SetDefault(Id);
        }

        /// <summary>
        /// 获取当前系统枚举代码对应的模板
        /// </summary>
        /// <param name="enumCode"></param>
        /// <returns></returns>
        public SmsTemplateInfo GetModelByEnumCode(string enumCode)
        {
            return dal.GetModelByEnumCode(enumCode);
        }

        /// <summary>
        /// 获取自动模板类型的内容
        /// </summary>
        /// <param name="dt">订单号对应的参数集</param>
        /// <returns></returns>
        public string GetTemplateContentByAuto(DataTable dt, string formatContent, string paramsCode)
        {
            if (dt == null || dt.Rows.Count == 0)
                throw new ArgumentException("订单参数集不存在，自动模板类型必须有订单对应的参数集", "dt");

            DataRow dr = dt.Rows[0];
            DataColumnCollection dcc = dt.Columns;

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

            return string.Format(formatContent, tParmsValues);
        }

        /// <summary>
        /// 获取当前模板的内容
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        public string GetTemplateContent(object templateId, string orderCode)
        {
            string content = "";
            var model = GetModel(templateId);
            if (model == null) return "";
            if (model.TemplateType == "auto")
            {
                if (string.IsNullOrEmpty(orderCode)) return "";

                DataTable dt = null;
                SmsSend smsSend = new SmsSend();
                DataSet ds = smsSend.GetSmsParams(orderCode);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    dt = ds.Tables[0];
                }

                return GetTemplateContentByAuto(dt, model.SmsContent, model.ParamsCode);
            }
            else if (model.TemplateType == "custom")
            {
                content = string.Format(model.SmsContent, model.ParamsValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                content = model.SmsContent;
            }

            return content;
        }

        /// <summary>
        /// 获取当前模板内容
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="dt">订单号对应的参数集</param>
        /// <returns></returns>
        public string GetTemplateContent(object templateId, DataTable dt)
        {
            string content = "";
            var model = GetModel(templateId);
            if (model == null) return "";
            if (model.TemplateType == "auto")
            {
                return GetTemplateContentByAuto(dt, model.SmsContent, model.ParamsCode);
            }
            else if (model.TemplateType == "custom")
            {
                content = string.Format(model.SmsContent, model.ParamsValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                content = model.SmsContent;
            }

            return content;
        }

        /// <summary>
        /// 获取当前模板内容
        /// </summary>
        /// <param name="orderCode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public string GetTemplateContent(string orderCode, SmsTemplateInfo model)
        {
            if (model.TemplateType == "auto")
            {
                DataTable dt = null;
                SmsSend ssBll = new SmsSend();
                DataSet ds = ssBll.GetSmsParams(orderCode);
                if (ds != null && ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                }
                return GetTemplateContentByAuto(dt, model.SmsContent, model.ParamsCode);
            }
            else if (model.TemplateType == "custom")
            {
                return string.Format(model.SmsContent, model.ParamsValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                return model.SmsContent;
            }
        }

        /// <summary>
        /// 获取模板内容
        /// </summary>
        /// <param name="dt">订单号对应的参数集</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public string GetTemplateContentByTranNode(DataTable dt, SmsTemplateInfo model)
        {
            if (model == null) return "";

            if (model.TemplateType == "auto")
            {
                if (dt == null || dt.Rows.Count == 0) return "";
                DataRow dr = dt.Rows[0];
                DataColumnCollection dcc = dt.Columns;

                string[] tParms = model.ParamsCode.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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

                return string.Format(model.SmsContent, tParmsValues);
            }
            else if (model.TemplateType == "custom")
            {
                return string.Format(model.SmsContent, model.ParamsValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                return model.SmsContent;
            }
        }

        /// <summary>
        /// 获取订单各个运输节点发送模板内容
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tranNode"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        public List<string> GetTemplateContentByTranNode(DataTable dt, string tranNode)
        {
            List<string> list = new List<string>();
            SmsTemplateInfo model = null;

            switch (tranNode)
            {
                case "SendGoods":

                    model = GetModelByEnumCode("SendGoodsTemplate");

                    #region 取货模板

                    string content = GetTemplateContentByTranNode(dt, model);
                    if (!string.IsNullOrEmpty(content)) list.Add(content);

                    #endregion

                    break;
                case "MainSend":

                    model = GetModelByEnumCode("MainSendForSenderTemplate");

                    #region 干线发运发货方短信模板

                    content = GetTemplateContentByTranNode(dt, model);
                    if (!string.IsNullOrEmpty(content)) list.Add(content);

                    #endregion

                    model = GetModelByEnumCode("MainSendForReceiverTemplate");

                    #region 干线发运收货方短信模板

                    content = GetTemplateContentByTranNode(dt, model);
                    if (!string.IsNullOrEmpty(content)) list.Add(content);

                    #endregion

                    break;
                case "MainReach":

                    model = GetModelByEnumCode("MainReachForSenderTemplate");

                    content = GetTemplateContentByTranNode(dt, model);
                    if (!string.IsNullOrEmpty(content)) list.Add(content);

                    model = GetModelByEnumCode("MainReachForReceiverTemplate");
                    content = GetTemplateContentByTranNode(dt, model);
                    if (!string.IsNullOrEmpty(content)) list.Add(content);

                    break;
                case "GoodsShipment":

                    model = GetModelByEnumCode("GoodsShipmentForSenderTemplate");
                    content = GetTemplateContentByTranNode(dt, model);
                    if (!string.IsNullOrEmpty(content)) list.Add(content);

                    model = GetModelByEnumCode("GoodsShipmentForReceiverTemplate");
                    content = GetTemplateContentByTranNode(dt, model);
                    if (!string.IsNullOrEmpty(content)) list.Add(content);

                    break;
                case "CustomerReceive":

                    model = GetModelByEnumCode("CustomerReceiveTemplate");
                    content = GetTemplateContentByTranNode(dt, model);
                    if (!string.IsNullOrEmpty(content)) list.Add(content);

                    break;
                default:
                    break;
            }

            return list;
        }

        #endregion
    }
}
