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
    public partial class AddSmsTemplate : System.Web.UI.Page
    {
        Guid gId;
        string htmlAppend;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["nId"]))
            {
                Guid.TryParse(Request.QueryString["nId"], out gId);
            }
            if (!Page.IsPostBack)
            {
                //模板参数
                BindTemplateParams();

                //获取当前编辑数据
                GetSmsTemplateById();

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

        /// <summary>
        /// 获取当前编辑行的数据
        /// </summary>
        private void GetSmsTemplateById()
        {
            if (!gId.Equals(Guid.Empty))
            {
                SmsTemplate smstBll = new SmsTemplate();
                var model = smstBll.GetModel(gId);
                if (model != null)
                {
                    hId.Value = model.Id.ToString();
                    txtTitle.Value = model.Title;
                    if (model.IsDefault) txtTitle.Disabled = true;
                    cbIsDefault.Checked = model.IsDefault;
                    cbbTemplateType.Value = model.TemplateType;
                    txtaContent.Value = model.SmsContent;
                    hParamsCode.Value = model.ParamsCode;
                    hParamsName.Value = model.ParamsName;
                    hParamsValue.Value = model.ParamsValue;
                    string sDivParam = "";
                    if(model.TemplateType == "auto")
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