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
    public partial class ListSmsTemplate : System.Web.UI.Page
    {
        string htmlAppend;
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
            //查询条件
            GetSearch();

            List<SmsTemplateInfo> list = null;

            SmsTemplate bll = new SmsTemplate();
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

        /// <summary>
        /// 构造查询条件
        /// </summary>
        private void GetSearch()
        {
            if (parms == null) parms = new ParamsHelper();

            string sTitle = Request.QueryString["title"];
            txtTitle.Value = sTitle;

            if (!string.IsNullOrWhiteSpace(sTitle))
            {
                sqlWhere += "and Title like @Title ";
                parms.Add(new SqlParameter("@Title", "%" + sTitle.Trim() + "%"));
            } 
        }
    }
}