using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TygaSoft.Model;
using TygaSoft.BLL;
using TygaSoft.WebHelper;

namespace TygaSoft.Web.WebUserControls
{
    public partial class UcSmsTemplate : System.Web.UI.UserControl
    {
        string htmlAppend;
        int pageIndex = Common.PageIndex;
        int pageSize = Common.Users_MinPageSize;
        int totalRecords = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindSmsTemplate();

                ltrMyData.Text = htmlAppend;
            }
        }

        private void BindSmsTemplate()
        {
            htmlAppend += "<div id=\"myDataForSmsTemplate\" style=\"display:none;\">";
            SmsTemplate bll = new SmsTemplate();
            var list = bll.GetList(pageIndex, pageSize, out totalRecords,"");
            if (list != null && list.Count > 0)
            {
                string itemAppend = "";
                foreach (var model in list)
                {
                    string templateType = "";
                    if(model.TemplateType == "auto") templateType = "自动";
                    else if(model.TemplateType == "custom") templateType = "自定义";
                    itemAppend += "{\"Id\":\"" + model.Id + "\",\"Title\":\"" + model.Title + "\",\"TemplateType\":\"" + templateType + "\",\"IsDefault\":\"" + model.IsDefault + "\",}";
                }

                itemAppend = itemAppend.Trim(',');
                itemAppend = "{\"total\":" + totalRecords + ",\"rows\":[" + itemAppend + "]}";

                htmlAppend += itemAppend;
            }

            htmlAppend += "</div>";
        }
    }
}