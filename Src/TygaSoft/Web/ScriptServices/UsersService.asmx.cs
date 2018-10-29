using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using TygaSoft.Model;
using TygaSoft.BLL;
using TygaSoft.DBUtility;
using TygaSoft.WebHelper;

namespace TygaSoft.Web.ScriptServices
{
    /// <summary>
    /// UsersService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tygaweb.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    [System.Web.Script.Services.ScriptService]
    public class UsersService : System.Web.Services.WebService
    {
        HttpContext context = HttpContext.Current;

        #region 菜单导航

        [WebMethod]
        public string GetTreeJsonForMenu()
        {
            string[] roles = Roles.GetRolesForUser(HttpContext.Current.User.Identity.Name);
            var roleList = roles.ToList();
            if (roleList.Contains("Administrators"))
            {
                roleList.Remove("Administrators");
            }
            roleList.Add("Users");
            SitemapHelper.Roles = roleList;
            return SitemapHelper.GetTreeJsonForMenu();
        }

        #endregion

        #region 用户角色

        [WebMethod]
        public string SaveIsApproved(string userName)
        {
            MembershipUser user = Membership.GetUser(userName);
            if (user == null)
            {
                return MessageContent.GetString(MessageContent.Submit_Params_InvalidExist, "用户");
            }
            if (user.IsApproved)
            {
                user.IsApproved = false;
            }
            else
            {
                user.IsApproved = true;
            }

            Membership.UpdateUser(user);

            return user.IsApproved ? "1" : "0";
        }

        #endregion
    }
}
