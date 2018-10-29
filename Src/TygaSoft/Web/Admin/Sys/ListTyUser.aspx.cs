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

namespace TygaSoft.Web.Admin.Sys
{
    public partial class ListTyUser : System.Web.UI.Page
    {
        string sqlWhere;
        ParamsHelper parms;
        int pageIndex = Common.PageIndex;
        int pageSize = Common.Admin_MinPageSize;
        int totalCount = 0;
        string queryStr;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
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

            TyUser tyuBll = new TyUser();

            rpData.DataSource = tyuBll.GetList(pageIndex, pageSize, out totalCount, sqlWhere, parms == null ? null : parms.ToArray());
            rpData.DataBind();

            ltrMyData.Text = "<div id=\"myDataForPage\" style=\"display:none;\">[{\"PageIndex\":\"" + pageIndex + "\",\"PageSize\":\"" + pageSize + "\",\"TotalRecord\":\"" + totalCount + "\",\"QueryStr\":\"" + queryStr + "\"}]</div>";
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
            string sUserName = Request.QueryString["userName"];
            if (!string.IsNullOrWhiteSpace(sUserName))
            {
                parms = new ParamsHelper();

                sqlWhere += "and UserName = @UserName ";
                parms.Add(new SqlParameter("@UserName", sUserName));
            }
        }
    }
}