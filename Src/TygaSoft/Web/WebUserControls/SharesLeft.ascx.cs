using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace TygaSoft.Web.WebUserControls
{
    public partial class SharesLeft : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    ltrUserInfo.Text = "<div id=\"myDataForUserInfo\" style=\"display:none;\">[{\"UserIsLogin\":\"1\"}]</div>";
                }
                else
                {
                    ltrUserInfo.Text = "<div id=\"myDataForUserInfo\" style=\"display:none;\">[{\"UserIsLogin\":\"0\"}]</div>";
                }
            }
        }
    }
}