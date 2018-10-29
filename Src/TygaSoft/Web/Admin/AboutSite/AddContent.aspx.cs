using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TygaSoft.Model;
using TygaSoft.BLL;
using TygaSoft.DBUtility;
using TygaSoft.WebHelper;

namespace TygaSoft.Web.Admin
{
    public partial class AddContent : System.Web.UI.Page
    {
        Guid gId = Guid.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            string nId = Request.QueryString["nId"];
            if (!string.IsNullOrWhiteSpace(nId))
            {
                Guid.TryParse(nId, out gId);
            }

            if (!Page.IsPostBack)
            {
                Bind();
            }
            else
            {
                OnSave();
            }
        }

        private void Bind()
        {
            if (!gId.Equals(Guid.Empty))
            {
                ContentDetail bll = new ContentDetail();
                ContentDetailInfo model = bll.GetModel(gId);
                if (model != null)
                {
                    txtTitle.Value = model.Title;
                    txtParent.Value = model.ContentTypeId.ToString();
                    hEditor1.Value = model.ContentText;
                }
            }
        }

        private void OnSave()
        {
            string sTitle = txtTitle.Value.Trim();
            if (string.IsNullOrEmpty(sTitle))
            {
                MessageBox.Messager(this.Page, this.Page.Controls[0], MessageContent.Submit_Params_InvalidError, "错误提示", "error");
                return;
            }
            string sParent = txtParent.Value.Trim();
            Guid contentTypeId = Guid.Empty;
            if (!string.IsNullOrEmpty(sParent))
            {
                Guid.TryParse(sParent, out contentTypeId);
            }
            string sContent = HttpUtility.UrlDecode(hEditor1.Value).Trim();
            hEditor1.Value = sContent;

            ContentDetail bll = new ContentDetail();
            ContentDetailInfo model = new ContentDetailInfo();
            model.Title = sTitle;
            model.ContentTypeId = contentTypeId;
            model.ContentText = sContent;
            model.Sort = 0;
            model.LastUpdatedDate = DateTime.Now;
            model.UserId = Common.GetUserId(Context);

            int effectCount = -1;
            if (!gId.Equals(Guid.Empty))
            {
                model.Id = gId;
                effectCount = bll.Update(model);
            }
            else
            {
                effectCount = bll.Insert(model);
            }

            if (effectCount == 110)
            {
                MessageBox.Messager(this.Page, this.Page.Controls[0], MessageContent.Submit_Exist, "系统提示", "error");
                return;
            }
            if (effectCount > 0)
            {
                MessageBox.MessagerShow(this.Page, this.Page.Controls[0], MessageContent.Submit_Success);
            }
            else
            {
                MessageBox.Messager(this.Page, this.Page.Controls[0], MessageContent.Submit_Error);
            }
        }
    }
}