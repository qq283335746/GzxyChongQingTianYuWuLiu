using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace TygaSoft.Web.Shares
{
    public partial class Shares : System.Web.UI.MasterPage
    {
        //string rawUrl;

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = Page.Header.Title + " - 重庆天宇物流";

            if (!Page.IsPostBack)
            {
                //rawUrl = Intelligencia.UrlRewriter.RewriterHttpModule.RawUrl;

                
            }
        }
    }
}