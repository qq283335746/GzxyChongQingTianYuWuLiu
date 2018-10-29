using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Profile;
using TygaSoft.CustomProvider;

namespace TygaSoft.Web
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        protected void Profile_OnMigrateAnonymous(object sender, ProfileMigrateEventArgs args)
        {
            //CustomProfileCommon profile = new CustomProfileCommon();
            //CustomProfileCommon anonymousProfile = profile.GetProfile(args.AnonymousID, false);

            ProfileManager.DeleteProfile(args.AnonymousID);
            AnonymousIdentificationModule.ClearAnonymousIdentifier();

            //profile.Save();

            // Delete the user row that was created for the anonymous user.
            Membership.DeleteUser(args.AnonymousID, true);
        }
    }
}