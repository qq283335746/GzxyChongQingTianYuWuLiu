using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using TygaSoft.Model;
using TygaSoft.BLL;
using TygaSoft.DBUtility;
using TygaSoft.WebHelper;

namespace TygaSoft.Web.Shares
{
    public partial class RegisterCheck : System.Web.UI.Page
    {
        string userName;
        string registerTime;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["u"]))
            {
                userName = Request.QueryString["u"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request.QueryString["d"]))
            {
                registerTime = Request.QueryString["d"].Trim();
            }

            if (!Page.IsPostBack)
            {
                OnCheck();
            }
        }

        private void OnCheck()
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(registerTime))
            {
                MessageBox.Messager(this.Page, Page.Controls[0], "非法操作！", "系统提示", "error");
                return;
            }

            string errorMsg = string.Empty;
            try
            {
                MembershipUser user = Membership.GetUser(userName);
                if (user == null)
                {
                    MessageBox.Messager(this.Page, Page.Controls[0], "用户" + userName + "不存在，请检查！", "系统提示");
                    return;
                }

                if (user.CreationDate.ToString("yyMMddHHmmss") != registerTime)
                {
                    MessageBox.Messager(this.Page, Page.Controls[0], "非法操作！", "系统提示", "error");
                    return;
                }

                user.IsApproved = true;
                Membership.UpdateUser(user);

                //注册成功，则
                string userData = user.ProviderUserKey.ToString();
                bool isPersistent = false;
                //bool isRemember = true;
                //bool isAuto = false;
                double d = 150;
                //if (cbRememberMe.Checked) isAuto = true;
                //自动登录 设置时间为7天
                //if (isAuto) d = 10080;

                //创建票证
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddMinutes(d),
                    isPersistent, userData, FormsAuthentication.FormsCookiePath);
                //加密票证
                string encTicket = FormsAuthentication.Encrypt(ticket);
                //创建cookie
                Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                //FormsAuthentication.RedirectFromLoginPage(userName, isPersistent);//使用此行会清空ticket中的userData ？！！！
                Response.Redirect(FormsAuthentication.GetRedirectUrl(userName, isPersistent));
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }

            if (!string.IsNullOrEmpty(errorMsg))
            {
                MessageBox.Messager(this.Page, Page.Controls[0], "异常：" + errorMsg + "", "系统提示", "error");
                return;
            }
        }
    }
}