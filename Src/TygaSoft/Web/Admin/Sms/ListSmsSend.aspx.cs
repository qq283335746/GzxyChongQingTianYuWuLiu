using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Threading.Tasks;
using TygaSoft.Model;
using TygaSoft.BLL;
using TygaSoft.DBUtility;
using TygaSoft.WebHelper;

namespace TygaSoft.Web.Admin.Sms
{
    public partial class ListSmsSend : System.Web.UI.Page
    {
        string htmlAppend;
        object userId;
        string sqlWhere;
        ParamsHelper parms;
        int pageIndex = Common.PageIndex;
        int pageSize = Common.Users_MinPageSize;
        int totalRecords = 0;
        string queryStr;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                userId = Common.GetUserId();

                NameValueCollection nvc = Request.QueryString;
                int index = 0;
                foreach (string item in nvc.AllKeys)
                {
                    GetParms(item, nvc);

                    if (item != "pageIndex" && item != "pageSize")
                    {
                        index++;
                        if (index > 1) queryStr += "&";
                        queryStr += string.Format("{0}={1}", item, Server.HtmlEncode(nvc[item]));
                    }
                }

                //数据绑定
                Bind();

                ltrMyData.Text = htmlAppend;
            }
        }

        private void Bind()
        {
            GetSearch();

            List<SmsSendInfo> list = null;

            SmsSend bll = new SmsSend();
            list = bll.GetList(pageIndex, pageSize, out totalRecords, sqlWhere, parms == null ? null : parms.ToArray());

            rpData.DataSource = list;
            rpData.DataBind();

            htmlAppend += "<div id=\"myDataForPage\" style=\"display:none;\">[{\"PageIndex\":\"" + pageIndex + "\",\"PageSize\":\"" + pageSize + "\",\"TotalRecord\":\"" + totalRecords + "\",\"QueryStr\":\"" + queryStr + "\"}]</div>";
        }

        /// <summary>
        /// 获取请求参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="nvc"></param>
        private void GetParms(string key, NameValueCollection nvc)
        {
            switch (key)
            {
                case "pageIndex":
                    Int32.TryParse(nvc[key], out pageIndex);
                    break;
                case "pageSize":
                    Int32.TryParse(nvc[key], out pageSize);
                    break;
                default:
                    break;
            }
        }

        private void GetSearch()
        {
            if (parms == null) parms = new ParamsHelper();

            string sStartDate = Request.QueryString["startDate"];
            string sEndDate = Request.QueryString["endDate"];

            txtStartDate.Value = sStartDate;
            txtEndDate.Value = sEndDate;

            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;

            if (!string.IsNullOrEmpty(sStartDate))
            {
                if (!DateTime.TryParse(sStartDate, out startDate))
                {
                    MessageBox.Messager(Page, MessageContent.GetString(MessageContent.Submit_Params_InvalidRegex, "开始日期"), MessageContent.AlertTitle_Error, "error");
                    return;
                }
            }
            if (!string.IsNullOrEmpty(sEndDate))
            {
                if (!DateTime.TryParse(sEndDate, out endDate))
                {
                    MessageBox.Messager(Page, MessageContent.GetString(MessageContent.Submit_Params_InvalidRegex, "截止日期"), MessageContent.AlertTitle_Error, "error");
                    return;
                }
            }

            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
            {
                if (startDate > endDate)
                {
                    MessageBox.Messager(Page, MessageContent.GetString(MessageContent.Request_DateTime_CompareToError, "开始日期", "截止日期"), MessageContent.AlertTitle_Error, "error");
                    return;
                }
            }

            if (startDate != DateTime.MinValue || endDate != DateTime.MinValue)
            {
                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    sqlWhere += "and SendDate between @StartDate and @EndDate ";
                    parms.Add(new SqlParameter("@StartDate", startDate));
                    parms.Add(new SqlParameter("@EndDate", endDate));
                }
                else
                {
                    if (startDate != DateTime.MinValue)
                    {
                        sqlWhere += "and SendDate >= @StartDate ";
                        parms.Add(new SqlParameter("@StartDate", startDate));
                    }
                    if (endDate != DateTime.MinValue)
                    {
                        sqlWhere += "and SendDate <= @EndDate ";
                        parms.Add(new SqlParameter("@EndDate", endDate));
                    }
                }
            }
        }
    }
}