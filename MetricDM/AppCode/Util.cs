using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MetricDM.AppCode
{
    public class Util
    {
        // ===== Retrieve the API URL USed for the application based on the current Environment's Server Name ==========
        public static string getAPIurl()
        {
            string serverName = Environment.MachineName.ToUpper();
            string applicationAPIurl = String.Empty;

            switch (serverName)
            {
                case "DSCAPPSQA1":
                    //QA Server
                    applicationAPIurl = ReadSetting("apiBaseURLQA");
                    break;
                case "DSCAPPSPROD1":
                    //PROD Server  192.168.1.181,  192.168.1.183 and 192.168.1.184
                    applicationAPIurl = ReadSetting("apiBaseURLPROD");
                    break;
                case "L-9L28F12":
                    applicationAPIurl = ReadSetting("apiBaseURLLocal");
                    break;
                default:
                    //Default to the Development Server   192.168.43.43
                    applicationAPIurl = ReadSetting("apiBaseURLDEV");
                    break;
            }
            return applicationAPIurl;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        // This function reads the current Server Webconfig File and retrieves the value of the Appsetting Variable passed as a parameter "key"
        public static string ReadSetting(string key)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string result = appSettings[key] ?? "";
            return result;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public static List<string> getUserRolesList(string username)
        {
            List<string> roleList = new List<string>();

            switch (username.ToUpper())
            {
                // Set ADMIN Group Level
                // Ed, John, Kevin G, Tracey White, Chris Boughey, Darrell, Jennifer Krueger, me, Giri, and all developers.
                case "CHEN_ALEX":
                case "ABDUGUEV_RASUL":
                case "POGANY_KEVIN":
                case "GOPAL_GIRI":
                case "ZUISS_EDWARD":
                case "OCALLAGHAN_JOHN":            //John.OCallaghan@dsc-logistics.com
                case "GLYNN_KEVIN":                //kevin.glynn@dsc-logistics.com
                case "WHITE_TRACEY":
                case "BOUGHEY_CHRISTOPHER":        //Chris.Boughey@dsc-logistics.com
                case "REED_DARRELL":               //darrell.reed@dsc-logistics.com
                case "FROSETH_ERICK":
                case "KRUEGER_JENNIFER":           //jennifer.krueger@dsc-logistics.com
                    roleList.Add("ADMIN");
                    roleList.Add("REVIEWER");
                    break;
                case "DELGADO_FELICIANO":
                    roleList.Add("ADMIN");
                    break;
                default:
                    roleList.Add("RZ_USER");
                    break;
            }

            return roleList;
        }

        public static string getUserRoles(string username)
        {
            //Get User Role from DB or from harcoded List
            string appUserRoles = String.Empty;
            //appUserRoles = "1;2;3;4;5;6;7;8";          //Temp Hardcoding
            switch (username.ToUpper())
            {
                // Set ADMIN Group Level
                // Ed, John, Kevin G, Tracey White, Chris Boughey, Darrell, Jennifer Krueger, me, Giri, and all developers.

                case "POGANY_KEVIN":
                case "GOPAL_GIRI":
                case "ZUISS_EDWARD":
                case "OCALLAGHAN_JOHN":            //John.OCallaghan@dsc-logistics.com
                case "GLYNN_KEVIN":                //kevin.glynn@dsc-logistics.com
                case "WHITE_TRACEY":
                case "BOUGHEY_CHRISTOPHER":        //Chris.Boughey@dsc-logistics.com
                case "REED_DARRELL":               //darrell.reed@dsc-logistics.com
                case "FROSETH_ERICK":
                case "KRUEGER_JENNIFER":           //jennifer.krueger@dsc-logistics.com
                    appUserRoles = "ADMIN;REVIEWER";
                    break;
                case "DELGADO_FELICIANO":
                    appUserRoles = "ADMIN;";
                    break;
                case "CHEN_ALEX":
                    appUserRoles = "ADMIN;";
                    break;
                case "ABDUGUEV_RASUL":
                    appUserRoles = "ADMIN;REVIEWER;SUPERAPPROVER";
                    break;
                case "...":
                    appUserRoles = "SUPER";
                    break;
                default:
                    appUserRoles = "USER";
                    break;
            }
            return appUserRoles;
        }
    }
}