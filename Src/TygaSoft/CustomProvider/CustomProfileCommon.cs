using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Profile;

namespace TygaSoft.CustomProvider
{
    public class CustomProfileCommon : ProfileBase
    {
        public void Save()
        {
            HttpContext.Current.Profile.Save();
        }

        public CustomProfileCommon GetProfile(string userName, bool isAuthenticated)
        {
            return (CustomProfileCommon)ProfileBase.Create(userName, isAuthenticated);
        }

        public string GetUserName()
        {
            if (HttpContext.Current.Profile.IsAnonymous) return HttpContext.Current.Request.AnonymousID;
            else return HttpContext.Current.Profile.UserName;
        }
    }
}
