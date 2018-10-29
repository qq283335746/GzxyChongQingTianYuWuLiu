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
    public partial class ImportSmsSend : System.Web.UI.Page
    {
        string htmlAppend;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //模板参数
                BindTemplateParams();

                ltrMyData.Text = htmlAppend;
            }
        }

        /// <summary>
        /// 获取模板参数并绑定到前端控件
        /// </summary>
        private void BindTemplateParams()
        {
            htmlAppend += "<div id=\"myDataForSmsParam\" style=\"display:none;\">[";

            SysEnum seBll = new SysEnum();
            string itemAppend = "";
            var list = seBll.GetList("and t2.EnumCode = 'SmsParam'", null);
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    itemAppend += "{\"EnumCode\":\"" + item.EnumCode + "\",\"EnumValue\":\"" + item.EnumValue + "\",\"ParamsValue\":\"\",\"status\":\"否\"},";
                }

                itemAppend = itemAppend.Trim(',');
                htmlAppend += itemAppend;
            }

            htmlAppend += "]</div>";
        }
    }
}