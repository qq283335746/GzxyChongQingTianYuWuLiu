using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using TygaSoft.Model;
using TygaSoft.BLL;
using TygaSoft.DBUtility;
using TygaSoft.WebHelper;

namespace TygaSoft.Web.Shares.Orders
{
    public partial class SearchOrder : System.Web.UI.Page
    {
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
            string sOrderCode = "";
            if (!string.IsNullOrWhiteSpace(Request.QueryString["order"]))
            {
                sOrderCode = Server.HtmlEncode(Request.QueryString["order"]).Trim();
            }
            txtOrderCode.Value = sOrderCode;
            if (string.IsNullOrEmpty(sOrderCode)) return;

            if (parms == null) parms = new ParamsHelper();

            sqlWhere += "and OrderCode = @OrderCode";
            SqlParameter parm = new SqlParameter("@OrderCode", SqlDbType.VarChar, 36);
            parm.Value = sOrderCode;

            parms.Add(parm);

            List<PDAOrderScanDetailInfo> list = null;

            PDAOrderScanDetail osdBll = new PDAOrderScanDetail();
            list = osdBll.GetList(pageIndex, pageSize, out totalRecords, sqlWhere, parms == null ? null : parms.ToArray());

            rpData.DataSource = list;
            rpData.DataBind();

            ltrMyData.Text = "<div id=\"myDataForPage\" style=\"display:none;\">[{\"PageIndex\":\"" + pageIndex + "\",\"PageSize\":\"" + pageSize + "\",\"TotalRecord\":\"" + totalRecords + "\",\"QueryStr\":\"" + queryStr + "\"}]</div>";
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
    }
}