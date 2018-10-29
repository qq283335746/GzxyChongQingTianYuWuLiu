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

namespace TygaSoft.Web.Users.Orders
{
    public partial class ListOrder : System.Web.UI.Page
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
            }
        }

        private void Bind()
        {
            GetSearch();

            List<OrderInfo> list = null;

            Order oBll = new Order();
            list = oBll.GetList(pageIndex, pageSize, out totalRecords, sqlWhere, parms == null ? null : parms.ToArray());

            rpData.DataSource = list;
            rpData.DataBind();

            GetListInOrder(list);

            htmlAppend += "<div id=\"myDataForPage\" style=\"display:none;\">[{\"PageIndex\":\"" + pageIndex + "\",\"PageSize\":\"" + pageSize + "\",\"TotalRecord\":\"" + totalRecords + "\",\"QueryStr\":\"" + queryStr + "\"}]</div>";
            ltrMyData.Text = htmlAppend;
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
            if(parms == null) parms = new ParamsHelper();

            if (!User.IsInRole("System"))
            {
                sqlWhere += "and u.Code = @UserName ";
                parms.Add(new SqlParameter("@UserName", User.Identity.Name));
            }

            string sStartDate = Request.QueryString["startDate"];
            string sEndDate = Request.QueryString["endDate"];
            string sOrderCode = Request.QueryString["order"];

            txtOrderCode.Value = sOrderCode;
            txtStartDate.Value = sStartDate;
            txtEndDate.Value = sEndDate;

            if (!string.IsNullOrWhiteSpace(sOrderCode))
            {
                sqlWhere += "and o.Code like @OrderCode ";
                parms.Add(new SqlParameter("@OrderCode", "%" + sOrderCode.Trim() + "%"));
            }

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
                    sqlWhere += "and BusinessDate between @StartDate and @EndDate ";
                    parms.Add(new SqlParameter("@StartDate", startDate));
                    parms.Add(new SqlParameter("@EndDate", endDate));
                }
                else
                {
                    if (startDate != DateTime.MinValue)
                    {
                        sqlWhere += "and BusinessDate >= @StartDate ";
                        parms.Add(new SqlParameter("@StartDate", startDate));
                    }
                    if (endDate != DateTime.MinValue)
                    {
                        sqlWhere += "and BusinessDate <= @EndDate ";
                        parms.Add(new SqlParameter("@EndDate", endDate));
                    }
                }
            }
        }

        private void GetListInOrder(List<OrderInfo> list)
        {
            if (list != null && list.Count > 0)
            {
                string orderCodeAppend = "";

                Dictionary<string, string> dic = new Dictionary<string, string>();

                foreach (var item in list)
                {
                    if (dic.ContainsKey(item.OrderCode)) continue;
                    dic.Add(item.OrderCode, item.OrderCode);

                    orderCodeAppend += "'" + item.OrderCode + "',";
                }

                orderCodeAppend = orderCodeAppend.Trim(',');

                PDAOrderScanDetail osdBll = new PDAOrderScanDetail();
                var osdList = osdBll.GetList("and OrderCode in(" + orderCodeAppend + ") ", null);
                if (osdList != null && osdList.Count > 0)
                {
                    foreach (KeyValuePair<string, string> kvp in dic)
                    {
                        var currOsdList = osdList.FindAll(m => m.OrderCode == kvp.Key);
                        if (currOsdList == null || currOsdList.Count == 0) continue;
                        htmlAppend += "<div id=\"myDataFor" + kvp.Key + "\" style=\"display:none;\">{\"total\":"+currOsdList.Count+",\"rows\": [";

                        string currAppend = "";
                        foreach (var currItem in currOsdList)
                        {
                            //string descr = currItem.SysEnumValue + "：" + currItem.Remark;
                            currAppend += "{\"Remark\":\"" + currItem.Remark + "\",\"ScanTime\":\"" + currItem.ScanTime.ToString("yyyy-MM-dd HH:mm") + "\"},";
                        }
                        currAppend = currAppend.Trim();

                        htmlAppend += currAppend + "]}</div>";
                    }
                }
            }
        }
    }
}