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

namespace TygaSoft.Web.Shares.AboutSite
{
    public partial class ShowContent : System.Web.UI.Page
    {
        Guid gId;
        string htmlAppend;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["nId"]))
            {
                if (!Guid.TryParse(Request.QueryString["nId"], out gId))
                {
                    MessageBox.Messager(Page, MessageContent.Request_InvalidError, MessageContent.AlertTitle_Error, "error");
                    return;
                }
            }
            if (!Page.IsPostBack)
            {
                Bind();
            }
        }

        private void Bind()
        {
            if (gId != null && !gId.Equals(Guid.Empty))
            {
                var cdModel = ContentDetailDataProxy.GetModel(gId);
                if (cdModel == null) ltrContent.Text = "<div class=\"tc\"><h3>数据不存在或已被删除</h3></div>";

                Page.Title = cdModel.Title;

                htmlAppend += "<div id=\"aa\" class=\"row bdb\"><div id=\"aaa\"><h3>"+cdModel.Title+"</h3> </div>";
                htmlAppend += "<span class=\"fr\">"+cdModel.LastUpdatedDate.ToString("yyyy-MM-dd")+"</span></div>";
                htmlAppend += "<div id=\"ab\" class=\"row\">"+cdModel.ContentText+"</div>";
            }

            ltrContent.Text = htmlAppend;
        }
    }
}