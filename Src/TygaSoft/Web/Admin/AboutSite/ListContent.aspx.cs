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

namespace TygaSoft.Web.Admin
{
    public partial class ListContent : System.Web.UI.Page
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
            else
            {
                switch (hOp.Value.Trim())
                {
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

           ContentDetail bll = new ContentDetail();

            rpData.DataSource = bll.GetList(pageIndex, pageSize, out totalCount, sqlWhere, parms == null ? null : parms.ToArray());
            rpData.DataBind();

            ltrMyData.Text = "<div id=\"myDataForPage\" style=\"display:none;\">[{\"PageIndex\":\"" + pageIndex + "\",\"PageSize\":\"" + pageSize + "\",\"TotalRecord\":\"" + totalCount + "\",\"QueryStr\":\"" + queryStr + "\"}]</div>";
        }

        /// <summary>
        /// 获取列表查询条件项,并构建查询参数集
        /// </summary>
        private void GetSearchItem()
        {
            ContentDetail bll = new ContentDetail();
            if (parms == null) parms = new ParamsHelper();

            string sTitle = txtTitle.Value.Trim();
            if (!string.IsNullOrEmpty(sTitle))
            {
                sqlWhere += "and Title like @Title ";
                SqlParameter parm = new SqlParameter("@Title", SqlDbType.NVarChar, 256);
                parm.Value = "%" + sTitle + "%";
                parms.Add(parm);
            }
            string sContentTypeId = txtParent.Value.Trim();

            if (!string.IsNullOrEmpty(sContentTypeId))
            {
                ContentType ctBll = new ContentType();
                ContentTypeInfo ctModel = ctBll.GetModel(sContentTypeId);
                if (ctModel != null && (ctModel.TypeName.IndexOf("所有") > -1)) return;

                sqlWhere += "and ContentTypeId = @ContentTypeId ";
                SqlParameter parm = new SqlParameter("@ContentTypeId", SqlDbType.UniqueIdentifier);
                parm.Value = Guid.Parse(sContentTypeId);
                parms.Add(parm);
            }
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

            ContentDetail bll = new ContentDetail();
            string[] itemsAppendArr = itemsAppend.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> list = itemsAppendArr.ToList<string>();
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