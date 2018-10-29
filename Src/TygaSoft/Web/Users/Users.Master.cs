using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TygaSoft.Web.Users
{
    public partial class Users : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = Page.Header.Title + " - 重庆天宇物流";
        }
    }
}