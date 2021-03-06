﻿using Newtonsoft.Json.Linq;
using MetricDM.AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetricDM.Models
{
    public class dscUser
    {
        private const string AUTHORIZATION_END_POINT = "getuserroles";
        private const string AUTHENTICATION_END_POINT = "loginrzdmuser";
        public string dbUserId { get; set; }
        public bool isValidUser { get; set; }
        public bool isAuthenticated { get; set; }
        public string userStatusMsg { get; set; }
        public string SSO { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string emailAddress { get; set; }
        public string fullName { get { return (FirstName + " " + LastName).Trim(); } }    //Read Only Property
        public string other2 { get; set; }
        public List<userRole> roles = new List<userRole>();
        public List<building> buildings = new List<building>();
        #region Constructors
        //--------------- Constructors -------------------------------
        //..... Empty Constructor, does not do any validation nor retrieves user info........
        public dscUser()
        {
            dbUserId = null;
            FirstName = "";
            LastName = "";
            SSO = "";
            isValidUser = false;
            isAuthenticated = false;
            userStatusMsg = "User has not Been Defined";
        }
        //..... User SSO constructor. Creates user and retrieves its associated info. It Does not perform User authentication
        public dscUser(string userSSO)
        {
            SSO = userSSO;
            isAuthenticated = false;
            FirstName = userSSO;  //Default the name to the SSO Id (For invalid users that do not have a real DB Name)
            loadUserDetails(SSO);
        }
        //..... Full User Constructor, accepts SSO and password to perform Authentication and Retrieve the Associated user Info (Roles, buildings) ............
        public dscUser(string userSSO, string userPwd)
        {
            SSO = userSSO;
            loadUserDetails(userSSO, userPwd);
        }
        //---------- Constructors Section Ends-------------------------------
        #endregion
        #region classMethods 
        public void loadUserDetails(string SSO, string uPassword = "")
        {
            // This function will retrieve the user information and load all roles,building, user info, etc if found.
            // If a password is specified, then it will also perform authentication
            SSO = SSO.ToUpper();
            string jsonData = String.IsNullOrEmpty(uPassword) ? getJsonUserData(SSO) : getJsonUserData(SSO, uPassword);
            try      // -------- Try Parsing the User Data properties --------
            {
                JObject parsed_result = JObject.Parse(jsonData);

                //Verify that the Data Retrieval was successful before attempting to parse any data
                if (parsed_result["result"].ToString().Equals("Success") && !parsed_result["user_id"].ToString().Equals("0"))
                {
                    // -------------- Get Personal Details --------------------------
                    string uName = (string)parsed_result["username"];
                    dbUserId = (string)parsed_result["user_id"];
                    emailAddress = (string)parsed_result["email"];

                    //Routine to parse the First & Last Name
                    if (String.IsNullOrEmpty(uName))
                    { //Default the Name to the SSO Id if there is no name in the database
                        FirstName = SSO;
                    }
                    else
                    {
                        string[] names = uName.Trim().Split(' ');
                        if (names.Length > 1)
                        {
                            LastName = names[names.Length - 1];
                            FirstName = uName.Replace(LastName, "").Trim();
                        }
                        else { FirstName = uName; }
                    }
                    //if (userSSO.Contains("_"))
                    //{
                    //    string[] names = userSSO.Split('_');
                    //    FirstName = Util.Capitalize(names[0].Trim());
                    //    LastName = Util.Capitalize(names[1].Trim());
                    //}
                    //else { FirstName = userSSO; }
                    // -------------END OF: Get Personal Details --------------------------


                    // ------- Retrieve all the User ROLES --------
                    JArray jRoles = (JArray)parsed_result["roles"];
                    if (jRoles.HasValues)
                    {
                        int roleIndex = 0;
                        foreach (var jRole in jRoles)
                        {
                            userRole uRole = new userRole();
                            uRole.roleDesc = (string)jRole["role_desc"];
                            uRole.id = (string)jRole["role_id"];
                            uRole.roleName = (string)jRole["role_name"];
                            uRole.appProduct = (string)jRole["prod_name"];

                            //Add all the metrics for this role
                            foreach (var rMetric in jRole["metrics"])
                            {
                                roleMetric roleMetricInfo = new roleMetric();
                                roleMetricInfo.mpId = (string)rMetric["metric_period_id"];
                                roleMetricInfo.mpName = (string)rMetric["metric_period_name"];
                                uRole.roleMetrics.Add(roleMetricInfo);  //Add a metric entry to this Role's Metrics
                            }
                            roles.Add(uRole); //Add a "Role" Entry to this user Roles
                            roleIndex++;      //Get Index of the Next Role in the Json Data
                        }
                    }

                    //*********** DEVELOPMENT ONLY CODE ***************
                    //Add any additional Roles (soft coded values) that might be needed  
                    foreach (string roleNew in MetricDM.AppCode.Util.getUserRolesList(SSO))
                    {
                        addRole(roleNew, "Red Zone");
                    }
                    //********* END OF DEVELOPMENT ONLY CODE **********

                    //Once Roles List is compiled, sort it by Product/Role in ascending order
                    roles = roles.OrderBy(p => p.appProduct).ThenBy(r => r.roleName).ToList();

                    // ------ Retrieve and parse all the User BUILDINGS ----------
                    JArray jbldgList = (JArray)parsed_result["buildings"];
                    if (jbldgList != null && jbldgList.HasValues)
                    {
                        foreach (var building in jbldgList)
                        {
                            building userBuilding = new building();
                            userBuilding.id = (string)building["dsc_mtrc_lc_bldg_id"];      //Not Implemented yet. Name might change !!!!!!!!!!!!!!!
                            userBuilding.buildingName = (string)building["dsc_mtrc_lc_bldg_name"];      //Not Implemented yet. Name might change !!!!!!!!!!!!!!!
                            buildings.Add(userBuilding);
                        }
                    }


                    //-------- Section to be Used during Development --------------------------------\
                    //Retrieve the User information for Developers
                    //switch (SSO)
                    //{
                    //    case "DELGADO_FELICIANO":
                    //        building dummyBuilding = new building() { id = "999", buildingName = "ALL", buildingCode = "ALL" };
                    //        buildings.Add(dummyBuilding);
                    //    break;
                    //    default: break;
                    //}
                    //-------- END of Section to be Used during Development -------------------------/


                    isValidUser = true;
                    isAuthenticated = String.IsNullOrEmpty(uPassword) ? false : true;
                    userStatusMsg = "User Information Loaded Successfully";
                }
                else
                {
                    string apiCallResult = (string)parsed_result["message"];
                    isValidUser = false;
                    isAuthenticated = false;
                    dbUserId = "0";
                    userStatusMsg = String.IsNullOrEmpty(apiCallResult) ? "User Roles Information Not Found in the Database" : apiCallResult;
                }
            }
            catch (Exception ex)
            {
                isValidUser = false;
                isAuthenticated = false;
                dbUserId = "0";
                userStatusMsg = ex.Message;
            }
        }
        public string getJsonUserData(string userSSO = "", string userPwd = "")
        {
            // This function wil retrieve the API Json data. If no password is specified, it will retrieve User Role API data.
            // If a password is specified it will return the Authentication Jason API data
            string endPoint = String.Empty;
            string payload = String.Empty;
            if (String.IsNullOrEmpty(userSSO)) { userSSO = SSO; }
            if (String.IsNullOrEmpty(userPwd))
            {    //Perform User Data Role Retrieval Only
                endPoint = AUTHORIZATION_END_POINT;
                payload = "{\"sso_id\":\"" + userSSO + "\"}";
            }
            else
            { //Perform user Authentication and Data retrieval
                endPoint = AUTHENTICATION_END_POINT;
                payload = "{\"sso_id\":\"" + userSSO + "\", \"password\":\"" + userPwd + "\"}";
            }
            //DataRetrieval api = new DataRetrieval();

            return DataRetrieval.executeAPI(endPoint, payload);
        }
        public string getUserRoles()
        {
            //return String.Join(";", roles.OrderBy(x => x.roleName).Select(x => x.roleName).ToList());
            return "|" + String.Join("|", roles.Select(x => x.roleName).ToList()) + "|";
        } // Returns roles as a "|" delimited string
        public string[] getUserRolesList()
        {
            return roles.Select(x => x.roleName).ToArray();
        } // Reuturns a list Of the User Roles Name
        public void addRole(string roleName, string roleProduct)
        {
            if (!roles.Any(p => p.roleName.ToUpper() == roleName.ToUpper()))
            {//Role Name does not exist on the current list
                userRole roleNew = new userRole();
                roleNew.id = "9999";
                roleNew.appProduct = roleProduct;
                roleNew.roleName = roleName;
                roleNew.roleDesc = "Softcoded Role - Development Use Only";
                roles.Add(roleNew);
            }
        }
        public string getUserBuildings()
        {
            return "|" + String.Join("|", buildings.Select(x => x.id).ToList()) + "|";
        }
        public string getReviewerMetricIds()
        {
            //Retrieve the User Reviewer Roll (If any)
            userRole reviewerRole = roles.FirstOrDefault(x => x.roleName == "RZ_AP_REVIEWER");
            if (reviewerRole == null) { return "||"; }   //No Metrics assigned as a Reviewer
            return "|" + reviewerRole.metricIds + "|";
        }
        public bool hasRole(string roleName)
        {
            return roles.FirstOrDefault(x => x.roleName == roleName) == null ? false : true;
        }
        public bool hasReviewerMetric(string metricPeriodId)
        {
            //Retrieve the User Reviewer Roll (If any)
            userRole reviewerRole = roles.FirstOrDefault(x => x.roleName == "RZ_AP_REVIEWER");
            if (reviewerRole == null) { return false; }   //No Metrics assigned as a Reviewer
            //roleMetric selectedMetric = reviewerRole.roleMetrics.FirstOrDefault(x => x.mpId == metricPeriodId);

            return reviewerRole.roleMetrics.FirstOrDefault(x => x.mpId == metricPeriodId) == null ? false : true;
        }
        public bool hasRoleMetric(string uRole, string metricPeriodId)
        {
            //Retrieve the User Reviewer Roll (If any) Role is Case Sensitive
            userRole reviewerRole = roles.FirstOrDefault(x => x.roleName == uRole);
            if (reviewerRole == null) { return false; }   //User does not even have the Specified Role
            //roleMetric selectedMetric = reviewerRole.roleMetrics.FirstOrDefault(x => x.mpId == metricPeriodId);
            return reviewerRole.roleMetrics.FirstOrDefault(x => x.mpId == metricPeriodId) == null ? false : true;
        }
        public bool hasBuilding(string building_Id)
        {
            return (buildings.FirstOrDefault(x => x.id == building_Id) == null) ? false : true;
        }
        #endregion
    }

    public class userRole
    {
        public string id { get; set; }
        public string appProduct { get; set; }
        public string roleName { get; set; }
        public string roleDesc { get; set; }
        public List<roleMetric> roleMetrics = new List<roleMetric>();
        public string metricIds { get { return String.Join("|", roleMetrics.OrderBy(x => x.mpName).Select(x => x.mpId).ToList()); } }
        public string metricNames { get { return String.Join("|", roleMetrics.OrderBy(x => x.mpName).Select(x => x.mpName).ToList()); } }
    }

    public class roleMetric
    {
        public string mpId { get; set; }             //Metric Period
        public string mpName { get; set; }
        public string mpDisplayName { get; set; }
    }

    public class building
    {
        public string id { get; set; }
        public string buildingName { get; set; }
        public string buildingCode { get; set; }
    }

}