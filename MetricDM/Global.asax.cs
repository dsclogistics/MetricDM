using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace MetricDM
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ViewEngines.Engines.Add(new RazorViewEngine());
            //GlobalFilters.Filters.Add(new System.Web.Mvc.AuthorizeAttribute());
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || authCookie.Value == "")
                return;

            FormsAuthenticationTicket authTicket;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                return;
            }

            //  ===================== SET UP THE USER CURRENT CONTEXT USER with ROLES ====================================
            //This logic will force the Application Authorization Request to retrieve the "User Data" from the authTicket 
            // split it into an array of 'Roles' that will be assigned as the current Logged on "User Identity Roles"
            if (Context.User != null)
            {
                //// retrieve roles from "UserData"  portion of the authTicket
                string[] roles = authTicket.UserData.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                if (roles.Length == 0 || roles[0] == "") { roles = new string[] { "NONE" }; }
                //    Context.User = new GenericPrincipal(Context.User.Identity, roles);
                HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                      new System.Security.Principal.GenericIdentity(Context.User.Identity.Name, "Forms"), roles);
            }  // ================= FINISHED SETTING UP THE CONTEXT USER with ROLES ===================================================
        }
    }
}
