using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml.Linq;
using TygaSoft.CustomProvider;
using TygaSoft.WebHelper;

namespace TygaSoft.Web.Admin
{
    public partial class Admin : System.Web.UI.MasterPage
    {
        string htmlAppend;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //Page.Title = Page.Header.Title + " - 东莞市凯软信息科技有限公司";
                //rawUrl = Intelligencia.UrlRewriter.RewriterHttpModule.RawUrl;

                Bind();

                ltrMyData.Text = htmlAppend;
            }
        }

        private void Bind()
        {
            int isSysDataAdmin = 0;
            if (HttpContext.Current.User.IsInRole("SysDataAdmin"))
            {
                isSysDataAdmin = 1;
            }
            htmlAppend += "<div id=\"myDataForUserInfo\" style=\"display:none;\">[{\"SysDataAdmin\":\"" + isSysDataAdmin + "\"}]</div>";
            
        }
    }
}