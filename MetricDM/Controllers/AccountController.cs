using MetricDM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using MetricDM.AppCode;

namespace MetricDM.Controllers
{
    public class AccountController : Controller
    {
        //GET: /Account/getLoginToken
        [AllowAnonymous]
        [HttpGet]
        public string getIV()
        {
            //This controller will generate a random IV (initialization Vector) that the client browser can use to encrypt credentials and login using AES encryption
            string encryptToken = System.Web.Security.Membership.GeneratePassword(16, 3);
            Session["loginToken"] = encryptToken;
            return encryptToken;
        }

        // GET: /Account/Login
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.errorMessage = "";
            //Reset all authentication cookies and session variables (To prevent orphan authentication cookie and users getting locked out in some rare instance where new version of the app is deployed and an usser is signon)
            FormsAuthentication.SignOut();
            Session.Remove("emp_id");    //Session["emp_id"] = null;
            Session.Remove("first_name");
            Session.Remove("last_name");
            Session.Remove("email");
            Session.Remove("roles");    //Session["role"] = null;
            return View();
        }

        //--------------------------------------------------------------------------------------------------------------\\
        // This is a new Login Page Using Modal View (POST)
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult login(LoginViewModel loginModel, string uPWD, string ReturnUrl)
        {
            //--------------------- Decryption and Input Validation Section ----------------------------------
            //Retrieve the one-time-use decryption Key from Memory and remove it so it can't be used again
            string decryptToken = "";
            Session["errorMessage"] = "";
            Session["buildingFilter"] = "N";        //Initial Building Filter Status for user is set to "N"
            //Session["loginToken"] = null;
            try
            {  //try to decrypt the password
                decryptToken = Session["loginToken"].ToString();
                Session.Remove("loginToken");  //Remove the session Id with the encoding key for security purposes

                loginModel.Password = AppCode.AESEncrytDecry.DecryptStringAES(uPWD, decryptToken);
                if (loginModel.Password.Equals("keyError"))
                {
                    Session["errorMessage"] = "Failed to decrypt credentials. Try again or contact Support if the problem persist";
                    return RedirectToAction("login", "Account", new { returnUrl = ReturnUrl });
                    //ModelState.AddModelError("", "Failed to decrypt credentials. Try again or contact Support if the problem persist");
                }
            }
            catch (Exception ex)
            {
                Session["errorMessage"] = "LOGIN FAILED: " + ex.Message;
                return RedirectToAction("login", "Account", new { returnUrl = ReturnUrl });
                //ModelState.AddModelError("", "ERROR: " + ex.Message);             
            }
            //-------- END of the Decryption and Input Validation Section ----------------------------------

            if (!ModelState.IsValid)
            {
                Session["errorMessage"] = "LOGIN FAILED: " + ModelState.ToString();
                return RedirectToAction("login", "Account", new { returnUrl = ReturnUrl });
                //return View(loginModel); 
            }

            // ----- Reset/Remove the Authorization Cookie and other related session variables if any -----
            FormsAuthentication.SignOut();
            Session.Remove("emp_id");            //Session["emp_id"] = null;
            Session.Remove("role");              //Session["role"] = null;
            Session.Remove("first_name");
            Session.Remove("last_name");
            Session.Remove("email");
            Session.Remove("userRole");
            Session.Remove("userBuildings");

            try
            {
                //Model State is Valid. Check Password                
                if (logonUser(loginModel))
                {  // Is User authentication is successful, redirect
                   // the user to the link it came from (Or the Home page is no return URL was specified)

                    if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/")
                        && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
                    { return Redirect(ReturnUrl); }
                    else { return RedirectToAction("Index", "Home"); }
                }
                else
                { //Failed to authenticate user. Back to Login page with Validation errors
                    return RedirectToAction("login", "Account", new { returnUrl = ReturnUrl });
                    //ViewBag.ReturnUrl = ReturnUrl;
                    //ModelState.AddModelError("", "Failed to Logon User: " + ViewBag.errorMessage);
                    //return View(loginModel);
                }
            }
            catch (Exception ex)
            {
                Session["errorMessage"] = ex.Message + "\nContact the Service Desk for assistance.";
                return RedirectToAction("login", "Account", new { returnUrl = ReturnUrl });
                //ViewBag.errorMessage = ex.Message + "\nContact the Service Desk for assistance.";
                //return View(loginModel);
            }
        }

        //--------------------------------------------------------------------------------------------------------------\\
        // GET: /Account/LogOff
        [HttpGet]
        [AllowAnonymous]
        public ActionResult LogOff(string backUrl)
        {
            if (String.IsNullOrEmpty(backUrl)) { backUrl = "\\Home\\Index"; }
            FormsAuthentication.SignOut();
            Session.Remove("emp_id");    //Session["emp_id"] = null;
            Session.Remove("role");      //Session["role"] = null;
            Session.Remove("first_name");
            Session.Remove("last_name");
            Session.Remove("email");
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //Session["ReturnURL"] = "";
            return RedirectToAction("Login", new { returnUrl = backUrl });
        }

        //--------------------------------------------------------------------------------------------------------------\\
        // POST: /Account/LogOff           (Called via ajax from the Client side)
        [HttpPost]
        public string resetUserInfo(string uFName, string uLName, string uLoginName, string email, string uRole, string uBuildings, string uId, bool turnOff)
        {
            //This function will be trigger via Ajax from the client to reset all the User Info Session Variables if lost 
            try
            {
                if (!turnOff)
                {   //Only reset the Session Variables if the TurnOff flag is set to "False"
                    //First check to make sure we have valid values to use, otherwise retrieve them from DB through API call
                    if (String.IsNullOrEmpty(uId))
                    {
                        //All client Local Storage values are null or were erased somehow. Retrieve data from DB
                        dscUser currentUser = new dscUser(User.Identity.Name);
                        if (currentUser.isValidUser)
                        {
                            uFName = currentUser.FirstName;
                            uLName = currentUser.LastName;
                            uLoginName = currentUser.SSO;
                            email = currentUser.emailAddress;
                            uRole = currentUser.getUserRoles();
                            uBuildings = currentUser.getUserBuildings();
                            uId = currentUser.dbUserId;
                        }
                    }
                    Session["first_name"] = uFName;
                    Session["last_name"] = uLName;
                    Session["userSSO"] = uLoginName;
                    //Session["userSSO"] = User.Identity.Name;
                    Session["email"] = email;
                    Session["userRole"] = uRole;
                    Session["userBuildings"] = uBuildings;
                    Session["emp_id"] = uId;
                }

                Session["firstLoad"] = "False";    //Always reset this flag
                return "True";
            }
            catch
            {
                return "False";
            }
        }
        //--------------------------------------------------------------------------------------------------------------\\

        //--------------------------------------------------------------------------------------------------------------\\
        // GET: /Account/UserInfo
        [HttpGet]
        public ActionResult userInquire(string userSSO = "")
        {
            dscUser appUser;
            userSSO = userSSO.Trim();
            if (String.IsNullOrEmpty(userSSO))
            {
                //No User was defined. Create Empty User and redirect to the View
                appUser = new dscUser();
                return View(appUser);
            }

            //---------------- TEST SECTION FOR LDAP Signon Validation using encryption ---------------
            if (userSSO.Contains("^"))
            {
                //This is AD Testing Data. Create a AD validation test used and return user to view
                appUser = new dscUser();
                string[] temp = userSSO.Split('^');
                appUser.FirstName = temp[0];
                appUser.LastName = temp[1];
                appUser.userStatusMsg = "LDAP Authentication Test Message";
                appUser.dbUserId = "999";
                return View(appUser);
            }
            //----------- END of LDAP TEST Validation  ---------------

            appUser = new dscUser(userSSO);
            //string uRoles = appUser.getUserRoles();
            //List<string> roleList = appUser.getUserRolesList();
            //ViewBag.rzUserData = String.IsNullOrEmpty(userSSO)? "" : xUser.getUserJsonData();
            return View(appUser);
        }

        [HttpGet]
        public PartialViewResult _UserInfo()
        { //This controller will display the current Logged On User Credential Information
            dscUser appUser = new dscUser(User.Identity.Name);
            return PartialView(appUser);
        }

        #region CustomHelpers
        //============= PRIVATE LOGIN HELPER METHODS ==================
        private bool logonUser(LoginViewModel loginModel)
        {
            dscUser logggedUser = new dscUser();
            bool isDeveloper = false;

            //-------- Section to be Used during Development --------------------------------\
            //Retrieve the User information for Developers
            switch (loginModel.Username.ToUpper() + loginModel.Password)
            {
                case "DELGADO_FELICIANO~~":
                case "ABDUGUEV_RASUL~~":
                case "CHEN_ALEX~~":
                    isDeveloper = true;
                    logggedUser = new dscUser(loginModel.Username.Trim());  //Retrieve all User Info
                    logggedUser.isAuthenticated = true;
                    break;
                default: break;
            }
            //-------- END of Section to be Used during Development -------------------------/

            if (!isDeveloper) { logggedUser = new dscUser(loginModel.Username.Trim(), loginModel.Password.Trim()); }

            if (logggedUser.isAuthenticated)
            {
                Session.Add("first_name", logggedUser.FirstName);
                Session.Add("last_name", logggedUser.LastName);
                Session.Add("username", logggedUser.fullName);
                Session["userSSO"] = logggedUser.SSO;
                Session["email"] = logggedUser.emailAddress;
                Session["emp_id"] = logggedUser.dbUserId;
                Session["firstLoad"] = "True";      //To trigger localStorage logic when first logged in
                Session["userRole"] = logggedUser.getUserRoles();
                Session["userBuildings"] = logggedUser.getUserBuildings();

                //Register the User with the Server as an authenticated user
                //"registerUser()"; Roles parameter irrelevant (for now) if those roles are already defined on the Session["userRole"]
                registerUser(loginModel.Username, logggedUser.getUserRolesList());

                return true;
            }
            else
            {
                Session["errorMessage"] = "LOGIN FAILED: " + logggedUser.userStatusMsg;
                ViewBag.errorMessage = logggedUser.userStatusMsg;
                return false;
            }

        }

        //=========================================================================================================
        private void registerUser(string userName, string[] roles)
        {
            // Set the Authentication Encrypted Cookie
            //FormsAuthentication.SetAuthCookie(loginModel.Username, true);      //Simple Application User Registration without roles

            string userRoles = String.Empty;         // String.Join(";", roles);
            //userRoles = (Session["userRole"] == null) ? "|" + String.Join("|", roles) + "|" : Session["userRole"].ToString();
            userRoles = "|" + String.Join("|", roles) + "|";

            var authTicket = new FormsAuthenticationTicket(
                 1,                             // version
                 userName,                      // user name
                 DateTime.Now,                  // created
                 DateTime.Now.AddMinutes(60),   // expires
                 true,                          // persistent?
                 userRoles                      // User Data portion [can be used to store roles as a string delimited field] 
              );

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            this.ControllerContext.HttpContext.Response.Cookies.Add(authCookie);

            //HttpContext.Current.Response.Cookies.Add(authCookie);
        }
        #endregion
    }
}