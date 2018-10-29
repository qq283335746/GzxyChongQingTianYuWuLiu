using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TygaSoft.Web.WebUserControls
{
    public partial class SharesTop : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //Bind();

                ////滚动公告
                //BindNotice();
            }
        }
    }
}