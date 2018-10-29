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

namespace TygaSoft.Web.Admin.Sys
{
    public partial class AddSitePoint : System.Web.UI.Page
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
                //Bind();
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
                SitePoint bll = new SitePoint();
                SitePointInfo model = bll.GetModel(gId);
                if (model != null)
                {
                    txtName.Value = model.PointName;
                    txtPointNum.Value = model.PointNum.ToString();
                    txtRemark.Value = model.Remark;
                }
            }
        }

        private void OnSave()
        {
            string sName = txtName.Value.Trim();
            if (string.IsNullOrWhiteSpace(sName))
            {
                MessageBox.Messager(this.Page, Page.Controls[0], "有“*”标识的为必填项，请检查");
                return;
            }
            string sPointNum = txtPointNum.Value.Trim();
            decimal pointNum = 0;
            if (!decimal.TryParse(sPointNum, out pointNum))
            {
                MessageBox.Messager(this.Page, Page.Controls[0], "积分数输入值格式不正确，请检查");
                return;
            }
            string sRemark = txtRemark.Value.Trim();

            SitePointInfo model = new SitePointInfo();
            model.PointName = sName;
            model.PointNum = pointNum;
            model.Remark = sRemark;

            int effect = -1;
            SitePoint bll = new SitePoint();
            if (!gId.Equals(Guid.Empty))
            {
                model.Id = gId;
                effect = bll.Update(model);
            }
            else
            {
                effect = bll.Insert(model);
            }

            if (effect == 110)
            {
                MessageBox.Messager(this.Page, Page.Controls[0], "已存在相同记录", "系统提示", "error");
                return;
            }
            if (effect > 0)
            {
                MessageBox.MessagerShow(this.Page, Page.Controls[0], "操作成功");
            }
            else
            {
                MessageBox.Messager(this.Page, Page.Controls[0], "操作失败，请检查", "系统提示", "error");
            }
        }
    }
}