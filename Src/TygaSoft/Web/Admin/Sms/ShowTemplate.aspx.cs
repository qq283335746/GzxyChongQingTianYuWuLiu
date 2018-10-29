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
    public partial class ShowTemplate : System.Web.UI.Page
    {
        string htmlAppend;
        Guid templateId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["nId"]))
            {
                Guid.TryParse(Request.QueryString["nId"], out templateId);
            }
            if (!Page.IsPostBack)
            {
                //绑定运输环节
                BindTranNode();

                //绑定当前模板
                BindTemplate();

                ltrMyData.Text = htmlAppend;
            }
        }

        /// <summary>
        /// 绑定运输环节
        /// </summary>
        private void BindTranNode()
        {
            SysEnum seBll = new SysEnum();
            var list = seBll.GetList("and t2.EnumCode = 'SendReceiveType' ");
            string itemAppend = "";
            foreach (var item in list)
            {
                itemAppend += "{\"id\":\"" + item.EnumCode + "\",\"text\":\"" + item.EnumValue + "\"},";
            }
            itemAppend = itemAppend.Trim(',');
            htmlAppend += "<div id=\"myDataForTranNode\" style=\"display:none;\">[";

            htmlAppend += itemAppend;

            htmlAppend += "]</div>";
        }

        /// <summary>
        /// 绑定当前模板
        /// </summary>
        private void BindTemplate()
        {
            if (!templateId.Equals(Guid.Empty))
            {
                SmsTemplate smstBll = new SmsTemplate();
                var model = smstBll.GetModel(templateId);
                if (model != null)
                {
                    hTemplateId.Value = model.Id.ToString();
                    txtaContent.Value = model.SmsContent;
                    string sDivParam = "";
                    if (model.TemplateType == "auto")
                    {
                        if (!string.IsNullOrEmpty(model.ParamsName))
                        {
                            string[] parmsArr = model.ParamsName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < parmsArr.Length; i++)
                            {
                                sDivParam += "{" + i + "}：" + parmsArr[i] + "，";
                            }
                            sDivParam = sDivParam.Trim('，');
                        }
                    }
                    else if (model.TemplateType == "custom")
                    {
                        string[] parmsArr = model.ParamsName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] parmsValueArr = model.ParamsValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < parmsArr.Length; i++)
                        {
                            sDivParam += "{" + i + "}：" + parmsArr[i] + "(" + parmsValueArr[i] + ")，";
                        }
                        sDivParam = sDivParam.Trim('，');
                    }

                    divParam.InnerHtml = sDivParam;
                }
            }
        }
    }
}