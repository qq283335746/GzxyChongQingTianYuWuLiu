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

namespace TygaSoft.Web
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            //if (Request.Browser["IsMobile"] == "True")
            //    MasterPageFile = "~/Shares/Mobile.Master";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        //private void OnSave()
        //{
        //    string userName = txtUserName.Value.Trim();
        //    string password = txtPsw.Value.Trim();
        //    string email = txtEmail.Value.Trim();
        //    string sVc = txtVc.Value.Trim();

        //    if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
        //    {
        //        MessageBox.Messager(this.Page, lbtnPostBack, "用户名、密码、邮箱为必填项", "操作错误", "error");
        //        return;
        //    }

        //    Regex r = new Regex(@"(([0-9]+)|([a-zA-Z]+)){6,30}");
        //    if (!r.IsMatch(password))
        //    {
        //        MessageBox.Messager(this.Page, lbtnPostBack, "密码正确格式由数字或字母组成的字符串，且最小6位，最大30位", "操作错误", "error");
        //        return;
        //    }
        //    r = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        //    if (!r.IsMatch(email))
        //    {
        //        Messager(this.Page, lbtnPostBack, "请输入正确的电子邮箱格式", "操作错误", "error");
        //        return;
        //    }

        //    if (string.IsNullOrEmpty(sVc))
        //    {
        //        MessageBox.Messager(this.Page, lbtnPostBack, "验证码输入不能为空！", "操作错误", "error");
        //        return;
        //    }

        //    if (sVc.ToLower() != Request.Cookies["RegisterVc"].Value.ToLower())
        //    {
        //        MessageBox.Messager(this.Page, lbtnPostBack, "验证码输入不正确，请检查！", "操作错误", "error");
        //        return;
        //    }

        //    string errorMsg = string.Empty;
        //    try
        //    {
        //        MembershipUser user = Membership.CreateUser(userName, password, email);

        //        if (user == null)
        //        {
        //            MessageBox.Messager(this.Page, lbtnPostBack, "注册失败，请重试", "系统提示");
        //            return;
        //        }

        //        Task.Factory.StartNew(() => Roles.AddUserToRole(user.UserName, "Users"));

        //        //系统自动分配该用户的棋子数
        //        UserPoint uModel = new UserPoint();
        //        uModel.UserID = user.ProviderUserKey;
        //        uModel.PointNum = Common.POINTNUM;
        //        uModel.LastUpdatedDate = DateTime.Now;

        //        UserPoint upBll = new UserPoint();

        //        Task.Factory.StartNew(() => upBll.AddUserPoint(uModel));

        //        bool isPersistent = true;
        //        double d = 180;
        //        string userData = user.ProviderUserKey.ToString() + "," + "Users";

        //        //创建票证
        //        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddMinutes(d),
        //            isPersistent, userData, FormsAuthentication.FormsCookiePath);
        //        //加密票证
        //        string encTicket = FormsAuthentication.Encrypt(ticket);
        //        //创建cookie
        //        Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

        //        //FormsAuthentication.RedirectFromLoginPage(userName, isPersistent);//使用此行会清空ticket中的userData ？！！！
        //        Response.Redirect(FormsAuthentication.GetRedirectUrl(userName, isPersistent));
        //    }
        //    catch (MembershipCreateUserException ex)
        //    {
        //        errorMsg = EnumMembershipCreateStatus.GetStatusMessage(ex.StatusCode);
        //    }
        //    catch (HttpException ex)
        //    {
        //        errorMsg = ex.Message;
        //    }
        //    if (!string.IsNullOrEmpty(errorMsg))
        //    {
        //        MessageBox.Messager(this.Page, lbtnPostBack, errorMsg,"系统提示");
        //        return;
        //    }
        //}
    }
}