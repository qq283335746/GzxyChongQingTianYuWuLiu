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

namespace TygaSoft.Web.Admin.AboutSite
{
    public partial class ListReportContent : System.Web.UI.Page
    {
        string sqlWhere;
        ParamsHelper parms;
        int pageIndex = Common.PageIndex;
        int pageSize = Common.Admin_MinPageSize;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                NameValueCollection nvc = Request.QueryString;
                string query = string.Empty;
                int index = 0;
                foreach (string item in nvc.AllKeys)
                {
                    if (item == "pageIndex") pageIndex = Int32.Parse(nvc[item]);
                    else if (item == "pageSize") pageSize = Int32.Parse(nvc[item]);
                    else if (item != "pageIndex" || item != "pageSize")
                    {
                        index++;
                        if (index > 1) query += "&";
                        query += string.Format("{0}={1}", item, nvc[item]);
                    }
                }

                hQuery.Value = query;
                hPageIndex.Value = pageIndex.ToString();
                hPageSize.Value = pageSize.ToString();

                //数据绑定
                Bind();
            }
            else
            {
                switch (hOp.Value.Trim())
                {
                    case "OnSearch":
                        OnSearch();
                        break;
                    case "OnDel":
                        OnDelete();
                        break;
                    default:
                        break;
                }
            }
        }

        private void Bind()
        {
            //查询条件
            GetSearchItem();

            int totalCount = 0;
            ReportContent bll = new ReportContent();

            rpData.DataSource = bll.GetList(pageIndex, pageSize, out totalCount, sqlWhere, parms == null ? null : parms.ToArray());
            rpData.DataBind();
            hTotal.Value = totalCount.ToString();
        }

        /// <summary>
        /// 获取列表查询条件项,并构建查询参数集
        /// </summary>
        private void GetSearchItem()
        {
            parms = new ParamsHelper();

            string sName = txtName.Value.Trim();
            if (!string.IsNullOrEmpty(sName))
            {
                sqlWhere += "and UserName like @UserName ";
                SqlParameter parm = new SqlParameter("@UserName", SqlDbType.NVarChar, 256);
                parm.Value = "%" + sName + "%";
                parms = new ParamsHelper();
                parms.Add(parm);
            }
        }

        /// <summary>
        /// 查询事件
        /// </summary>
        private void OnSearch()
        {
            Bind();
        }

        /// <summary>
        /// 批量删除数据
        /// </summary>
        private void OnDelete()
        {
            string itemsAppend = hV.Value.Trim();
            if (string.IsNullOrEmpty(itemsAppend))
            {
                MessageBox.Messager(this.Page, this.Page.Controls[0], MessageContent.Submit_InvalidRow, "错误提醒", "error");
                return;
            }

            ReportContent bll = new ReportContent();
            string[] itemsAppendArr = itemsAppend.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<object> list = itemsAppendArr.ToList<object>();
            if (bll.DeleteBatch(list))
            {
                MessageBox.MessagerShow(this.Page, this.Page.Controls[0], MessageContent.Submit_Success);
                Bind();
            }
            else
            {
                MessageBox.Messager(this.Page, this.Page.Controls[0], MessageContent.Submit_Error, "系统提示");
            }
        }
    }
}