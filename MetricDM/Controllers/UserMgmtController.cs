using PagedList;
using PagedList.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MetricDM.Models;

namespace MetricDM.Controllers
{
    public class UserMgmtController : Controller
    {
        private DSC_MTRC_DEVEntities db = new DSC_MTRC_DEVEntities();

        // GET: UserMgmt
        public ActionResult Index(string search, int? page, int? pageSize)
        {
            ViewBag.CurrentItemsPerPage = pageSize ?? 10;

            if (!String.IsNullOrWhiteSpace(search))
            {
                var appUserList = db.DSC_APP_USER.Include(d => d.DSC_EMPLOYEE);

                //Filters list where search string is contained in the full name, email address, or sso_id
                var tempAppUserList = appUserList.Where(x => x.app_user_full_name.Contains(search)
                                                        || x.app_user_email_addr.Contains(search)
                                                        || x.app_user_sso_id.Contains(search)).ToList();
                if (tempAppUserList.Count != 0)
                {
                    return View(tempAppUserList.OrderBy(x => x.app_user_sso_id).ThenBy(x => x.app_user_full_name).ToList().ToPagedList(page ?? 1, pageSize ?? 10));
                }
                else
                {
                    try
                    {
                        string[] words = search.Split(' ');
                        string word0 = words[0];
                        string word1 = words[1];
                    }
                    catch
                    {

                    }
                }

                return View(appUserList.ToList());
            }
            else
            {
                var dSC_APP_USER = db.DSC_APP_USER.Include(d => d.DSC_EMPLOYEE);
                return View(dSC_APP_USER.ToList().ToPagedList(page ?? 1, pageSize ?? 10));
            }
            
        }

        // GET: UserMaintenance
        public ActionResult UserMaintenance(int? id)
        {
            UserMgmtViewModel userMgmtViewModel = new UserMgmtViewModel();

            if (id == null || id == 0)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.appUserId = id;

                //Add app user details to view model
                DSC_APP_USER dSC_APP_USER = db.DSC_APP_USER.Find(id);
                if (dSC_APP_USER == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    userMgmtViewModel.appUserDetails = dSC_APP_USER;
                }

                //Add employee details to view model
                if(dSC_APP_USER.dsc_emp_id != null)
                {
                    DSC_EMPLOYEE dSC_EMPLOYEE = db.DSC_EMPLOYEE.Find(dSC_APP_USER.dsc_emp_id);
                    if (dSC_EMPLOYEE != null)
                    {
                        userMgmtViewModel.employeeDetails = dSC_EMPLOYEE;
                    }
                }

                //Add assigned user building list to view model
                userMgmtViewModel.userBldgList = getUserBuildingList(id);

                //Add unassigned building list to view model
                userMgmtViewModel.unassignedBldgList = getAllBuildingList().Except(userMgmtViewModel.userBldgList).ToList();

                //Add assigned user roles to view model
                List<UserAppProduct> userProductList = getUserProductRoleList(id);
                userMgmtViewModel.userProductRoleList = userProductList;

                return View(userMgmtViewModel);
            }

        }

        // GET: _UserBldgAssign
        public ActionResult _UserBldgAssign(int? app_user_id)
        {
            BldgAsgnViewModel bldgAsgnViewModel = new BldgAsgnViewModel();

            if (app_user_id == null || app_user_id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                bldgAsgnViewModel.userBldgList = getUserBuildingList(app_user_id);
                bldgAsgnViewModel.unassignedBldgList = getAllBuildingList().Except(bldgAsgnViewModel.userBldgList).ToList();
            }

            return PartialView(bldgAsgnViewModel);
        }
        // POST: UserMgmt/_UserBldgAssign
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public string _UserBldgAssign(string raw_json)
        {
            string msg = updateUserBuildingList(raw_json);

            //return RedirectToAction("UserMgmt/UserMaintenance/" + app_user_id);

            return msg;
        }

        //Get: _UserRoleMtrcAssign
        public ActionResult _UserRoleMtrcAssign(int? app_user_role_id)
        {
            MtrcAsgnViewModel mtrcAsgnViewModel = new MtrcAsgnViewModel();
            mtrcAsgnViewModel.product = db.MTRC_PRODUCT.Find(db.MTRC_USER_APP_ROLES.Find(app_user_role_id).MTRC_APP_ROLE.prod_id);

            if (app_user_role_id == null || app_user_role_id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                mtrcAsgnViewModel.userRoleId = app_user_role_id ?? 0;
                mtrcAsgnViewModel.userRoleMtrcList = getUserRoleMtrcList(app_user_role_id);
                mtrcAsgnViewModel.unassignedMtrcList = getAllMetricList().Except(mtrcAsgnViewModel.userRoleMtrcList).ToList();
            }

            return PartialView(mtrcAsgnViewModel);
        }
        // POST: UserMgmt/_UserRoleMtrcAssign
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public string _UserRoleMtrcAssign(string raw_json)
        {
            string msg = updateUserRoleMetricList(raw_json);

            return msg;
        }

        //Get: _UserRoleAssign
        public ActionResult _UserRoleAssign(int? app_user_id, int? prod_id, int? mar_id)
        {
            RoleAsgnViewModel roleAsgnViewModel = new RoleAsgnViewModel();

            if (app_user_id == null || app_user_id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                //Display products that have roles associated with them
                int prodId = prod_id ?? 0;
                var query = (
                    from a in db.MTRC_PRODUCT
                    join b in db.MTRC_APP_ROLE on a.prod_id equals b.prod_id
                    select a).Distinct();

                ViewBag.prod_sel_list = new SelectList(query, "prod_id", "prod_name", prodId).OrderBy(x => x.Text);
                roleAsgnViewModel.product = db.MTRC_PRODUCT.Find(prod_id);

                if(prodId == 0)
                {
                    ViewBag.role_sel_list_display = false;
                }
                else
                {
                    //If product selected, display roles
                    int marId = mar_id ?? 0;

                    List<MTRC_APP_ROLE> userRoleList = getUserMtrcAppRoleList(app_user_id);
                    List<MTRC_APP_ROLE> roleAddList = db.MTRC_APP_ROLE.Where(x => x.prod_id == prod_id).ToList().Except(userRoleList).ToList();

                    ViewBag.role_sel_list_display = true;
                    ViewBag.role_sel_list = new SelectList(roleAddList, "mar_id", "mar_name", marId).OrderBy(x => x.Text);

                    if(marId == 0)
                    {
                        ViewBag.role_display = false;
                    }
                    else
                    {
                        //If role selected, display building and metric assignment screens if required.
                        ViewBag.role_display = true;
                        roleAsgnViewModel.appRole = db.MTRC_APP_ROLE.Find(marId);

                        roleAsgnViewModel.userBldgList = getUserBuildingList(app_user_id);
                        roleAsgnViewModel.unassignedBldgList = getAllBuildingList().Except(roleAsgnViewModel.userBldgList).ToList();
                        roleAsgnViewModel.mtrcList = getAllMetricList();
                    }
                }
                
            }

            return PartialView(roleAsgnViewModel);
        }
        //POST: _UserRoleAssign
        [HttpPost]
        public string _UserRoleAssign(string raw_json)
        {
            string msg = addUserRole(raw_json);

            return msg;
        }
        //POST: RemoveUserRole
        [HttpPost]
        public string _RemoveUserRole(string raw_json)
        {
            string msg = removeUserRole(raw_json);

            return msg;
        }

        //-------------------------------------------------------------------------------------------------------------------
        //-------------//
        //"API" Catalog//
        //-------------//
        //Return a list of all buildings
        private List<DSC_MTRC_LC_BLDG> getAllBuildingList()
        {
            List<DSC_MTRC_LC_BLDG> bldgList = new List<DSC_MTRC_LC_BLDG>();

            //Query Style 1 - SQL notation
            var query1 =
                from a in db.DSC_MTRC_LC_BLDG
                select a;

            bldgList = query1.ToList();

            return bldgList;
        }

        //Return a list of buildings associated with a particular app user id
        private List<DSC_MTRC_LC_BLDG> getUserBuildingList(int? appUserId)
        {
            List<DSC_MTRC_LC_BLDG> bldgList = new List<DSC_MTRC_LC_BLDG>();
            if(appUserId == null || appUserId == 0)
            {
                
            }
            else
            {
                ////Query Style 1 - SQL notation
                //var query1 =
                //    from a in db.DSC_MTRC_LC_BLDG
                //    join b in db.RZ_BLDG_AUTHORIZATION on a.dsc_mtrc_lc_bldg_id equals b.dsc_mtrc_lc_bldg_id
                //    join c in db.DSC_APP_USER on b.app_user_id equals c.app_user_id
                //    where c.app_user_id == appUserId
                //    select a;

                //Query Style 2 - Dot notation
                var query2 =
                    from child in db.RZ_BLDG_AUTHORIZATION
                    where child.DSC_APP_USER.app_user_id == appUserId
                    select child.DSC_MTRC_LC_BLDG;

                bldgList = query2.ToList();
            }

            return bldgList;
        }

        //Return a list of all metric periods
        private List<MTRC_METRIC_PERIOD> getAllMetricList()
        {
            List<MTRC_METRIC_PERIOD> mtrcList = new List<MTRC_METRIC_PERIOD>();

            //Query Style 1 - SQL notation
            var query1 =
                from a in db.MTRC_METRIC_PERIOD
                select a;

            mtrcList = query1.ToList();

            return mtrcList;
        }

        //Return a list of metric periods associated with a particular app user id
        private List<MTRC_METRIC_PERIOD> getUserRoleMtrcList(int? appUserRoleId)
        {
            List<MTRC_METRIC_PERIOD> mtrcList = new List<MTRC_METRIC_PERIOD>();

            if (appUserRoleId == null || appUserRoleId == 0)
            {

            }
            else
            {
                //Query Style 2 - Dot notation
                var query2 =
                    from child in db.MTRC_MGMT_AUTH_NEW
                    where child.muar_id == appUserRoleId
                            && DateTime.Today >= DbFunctions.TruncateTime(child.mma_eff_start_date)
                            && DateTime.Today <= DbFunctions.TruncateTime(child.mma_eff_end_date)
                    select child.MTRC_METRIC_PERIOD;

                mtrcList = query2.ToList();
            }

            return mtrcList;
        }

        private List<MTRC_METRIC_PERIOD> getUserRoleMtrcListCreatedToday(int? appUserRoleId)
        {
            List<MTRC_METRIC_PERIOD> mtrcList = new List<MTRC_METRIC_PERIOD>();

            if (appUserRoleId == null || appUserRoleId == 0)
            {

            }
            else
            {
                //Query Style 2 - Dot notation
                var query2 =
                    from child in db.MTRC_MGMT_AUTH_NEW
                    where child.muar_id == appUserRoleId
                            && DateTime.Today == DbFunctions.TruncateTime(child.mma_eff_start_date)
                    select child.MTRC_METRIC_PERIOD;

                mtrcList = query2.ToList();
            }

            return mtrcList;
        }

        //Return a list of metric app roles associated with a particular app user id
        private List<MTRC_APP_ROLE> getUserMtrcAppRoleList(int? appUserId)
        {
            List<MTRC_APP_ROLE> roleList = new List<MTRC_APP_ROLE>();

            if (appUserId == null || appUserId == 0)
            {

            }
            else
            {
                var query =
                    from a in db.MTRC_USER_APP_ROLES
                    join b in db.DSC_APP_USER on a.app_user_id equals b.app_user_id
                    join c in db.MTRC_APP_ROLE on a.mar_id equals c.mar_id
                    where b.app_user_id == appUserId
                    select c;

                roleList = query.ToList();
            }

            return roleList;
        }

        //Return a nested list of roles (Product > Role > Metric) associated with a particular app user id
        private List<UserAppProduct> getUserProductRoleList(int? appUserId)
        {
            List<UserAppProduct> productList = new List<UserAppProduct>();
            if (appUserId == null || appUserId == 0)
            {

            }
            else
            {
                //Query for products and roles
                var query =
                    from a in db.MTRC_USER_APP_ROLES
                    join b in db.DSC_APP_USER on a.app_user_id equals b.app_user_id
                    join c in db.MTRC_APP_ROLE on a.mar_id equals c.mar_id
                    join d in db.MTRC_PRODUCT on c.prod_id equals d.prod_id

                    where b.app_user_id == appUserId
                    orderby d.prod_name, c.mar_name
                    select new
                    {
                        prodName = d.prod_name,
                        prodId = d.prod_id,
                        userAppRoleId = a.muar_id,
                        appRoleId = c.mar_id,
                        appRoleName = c.mar_name,
                        appRoleDesc = c.mar_desc,
                        reqBldgAuth = c.mar_req_bldg_auth,
                        reqMtrcAuth = c.mar_req_mtrc_mgmt_auth
                    };

                productList = (
                    from a in query
                    group a by new { a.prodId, a.prodName } into grouped
                    select new UserAppProduct
                    {
                        productId = grouped.Key.prodId.ToString(),
                        productName = grouped.Key.prodName,
                        userRoles = (from b in grouped
                                select new UserAppRole
                                {
                                    userAppRoleId = b.userAppRoleId.ToString(),
                                    appRoleId = b.appRoleId.ToString(),
                                    appRoleName = b.appRoleName,
                                    appRoleDesc = b.appRoleDesc == null ? "" : b.appRoleDesc,
                                    reqBldgAuth = b.reqBldgAuth,
                                    reqMtrcAuth = b.reqMtrcAuth
                                }).ToList()
                    }).ToList().OrderBy(x=>x.productName).ToList();

                //Query for metric assignments
                var query2 =
                    from a in db.MTRC_USER_APP_ROLES
                    join b in db.DSC_APP_USER on a.app_user_id equals b.app_user_id
                    join c in db.MTRC_MGMT_AUTH_NEW on a.muar_id equals c.muar_id 
                    join d in db.MTRC_METRIC_PERIOD on c.mtrc_period_id equals d.mtrc_period_id
                    where b.app_user_id == appUserId
                            && DateTime.Today >= DbFunctions.TruncateTime(c.mma_eff_start_date)
                            && DateTime.Today <= DbFunctions.TruncateTime(c.mma_eff_end_date)
                    select new RoleMetricAuthority
                    {
                        userAppRoleId = a.muar_id.ToString(),
                        mtrcPeriod = d
                    };

                List<RoleMetricAuthority> metricList = query2.ToList();

                //Check roles for metric mgmt flag = 'Y', then assign metric authorities to their 
                //corresponding metric user app roles.
                foreach(RoleMetricAuthority metric in metricList)
                {
                    List<UserAppRole> roleList = productList.SelectMany(x => x.userRoles)
                                                            .Where(x => x.userAppRoleId == metric.userAppRoleId 
                                                                     && x.reqMtrcAuth == "Y").ToList();
                    
                    foreach(UserAppRole role in roleList)
                    {
                        role.roleMetrics.Add(metric.mtrcPeriod);
                        //metric.mtrcPeriod.
                    }    

                }

            }

            return productList;
        }

        //Updates the assigned building list for a particular app_user_id
        //private string updateUserBuildingList(int? appUserId, List<int> newBldgIdList)
        private string updateUserBuildingList(string raw_json)
        {
            string notused = "Success";

            try
            {
                //Parse JSON
                JObject parsed_result = JObject.Parse(raw_json);
                int app_user_id = (int)parsed_result["app_user_id"];

                List<int> asgndBldgList = new List<int>();
                JArray jBldgs = (JArray)parsed_result["asgndBldgList"];
                foreach (var res in jBldgs)
                {
                    int bldgId = (int)res;
                    asgndBldgList.Add(bldgId);
                }

                //
                if (app_user_id == 0)
                {
                    notused = "User Id = 0";
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    List<int> userBldgList = new List<int>();
                    List<int> newUserBldgList = asgndBldgList;

                    userBldgList = getUserBuildingList(app_user_id).Select(x => Convert.ToInt32(x.dsc_mtrc_lc_bldg_id)).ToList();

                    //Begin Add Transaction
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //Add Rows
                            foreach (int bldgId in newUserBldgList.Except(userBldgList))
                            {
                                var addRow = new RZ_BLDG_AUTHORIZATION
                                {
                                    app_user_id = app_user_id,
                                    dsc_mtrc_lc_bldg_id = Convert.ToInt16(bldgId)
                                };

                                if (ModelState.IsValid)
                                {
                                    db.RZ_BLDG_AUTHORIZATION.Add(addRow);
                                }
                            }

                            //Delete Rows
                            foreach (int bldgId in userBldgList.Except(newUserBldgList))
                            {
                                var removeRow = db.RZ_BLDG_AUTHORIZATION.Where(x => x.app_user_id == app_user_id &&
                                                                    x.dsc_mtrc_lc_bldg_id == bldgId).First();

                                if (ModelState.IsValid)
                                {
                                    db.RZ_BLDG_AUTHORIZATION.Remove(removeRow);
                                }
                            }

                            db.SaveChanges();
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            notused = e.Message;
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                notused = e.Message;
            }

            return notused;
        }
        private string updateUserBuildingListWithoutCommit(int app_user_id, List<int> asgndBldgList)
        {
            string notused = "Success";

            try
            {
                if (app_user_id == 0)
                {
                    notused = "User Id = 0";
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    List<int> userBldgList = new List<int>();
                    List<int> newUserBldgList = asgndBldgList;

                    userBldgList = getUserBuildingList(app_user_id).Select(x => Convert.ToInt32(x.dsc_mtrc_lc_bldg_id)).ToList();

                    //Begin Add Transaction
                    try
                    {
                        //Add Rows
                        foreach (int bldgId in newUserBldgList.Except(userBldgList))
                        {
                            var addRow = new RZ_BLDG_AUTHORIZATION
                            {
                                app_user_id = app_user_id,
                                dsc_mtrc_lc_bldg_id = Convert.ToInt16(bldgId)
                            };

                            if (ModelState.IsValid)
                            {
                                db.RZ_BLDG_AUTHORIZATION.Add(addRow);
                            }
                        }

                        //Delete Rows
                        foreach (int bldgId in userBldgList.Except(newUserBldgList))
                        {
                            var removeRow = db.RZ_BLDG_AUTHORIZATION.Where(x => x.app_user_id == app_user_id &&
                                                                x.dsc_mtrc_lc_bldg_id == bldgId).First();

                            if (ModelState.IsValid)
                            {
                                db.RZ_BLDG_AUTHORIZATION.Remove(removeRow);
                            }
                        }

                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        notused = e.Message;
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                notused = e.Message;
                throw;
            }

            return notused;
        }

        //Updates the assigned building list for a particular app_user_id
        //private string updateUserBuildingList(int? appUserId, List<int> newBldgIdList)
        private string updateUserRoleMetricList(string raw_json)
        {
            string notused = "Success";

            try
            {
                //Parse JSON
                JObject parsed_result = JObject.Parse(raw_json);
                int app_user_id = (int)parsed_result["app_user_id"];
                int app_user_role_id = (int)parsed_result["app_user_role_id"];

                List<int> asgndMtrcList = new List<int>();
                JArray jMtrcs = (JArray)parsed_result["asgndMtrcList"];
                foreach (var res in jMtrcs)
                {
                    int mpId = (int)res;
                    asgndMtrcList.Add(mpId);
                }

                //
                if (app_user_id == 0)
                {
                    notused = "User Id cannot be 0.";
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    List<int> userRoleMtrcList = new List<int>();
                    List<int> userRoleMtrcListCreatedToday = new List<int>();
                    List<int> newUserRoleMtrcList = asgndMtrcList;

                    userRoleMtrcList = getUserRoleMtrcList(app_user_role_id).Select(x => Convert.ToInt32(x.mtrc_period_id)).ToList();
                    userRoleMtrcListCreatedToday = getUserRoleMtrcListCreatedToday(app_user_role_id).Select(x => Convert.ToInt32(x.mtrc_period_id)).ToList();

                    //Begin Add Transaction
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //Add Row if no active assignment for mpId (there may be a record from the past that has been effective end dated)
                            foreach (int mpId in newUserRoleMtrcList.Except(userRoleMtrcList))
                            {
                                
                                var addRow = new MTRC_MGMT_AUTH_NEW
                                {
                                    muar_id = app_user_role_id,
                                    mtrc_period_id = Convert.ToInt16(mpId),
                                    mma_eff_start_date = DateTime.Today,
                                    mma_eff_end_date = new DateTime(2060, 12, 31)
                                };

                                if (ModelState.IsValid)
                                {
                                    db.MTRC_MGMT_AUTH_NEW.Add(addRow);
                                }

                            }

                            //Delete records that are currently being unassigned and whose effective start date is today.
                            foreach (int mpId in userRoleMtrcListCreatedToday.Except(newUserRoleMtrcList))
                            {
                                var removeRow = db.MTRC_MGMT_AUTH_NEW.Where(x => x.muar_id == app_user_role_id &&
                                                                    x.mtrc_period_id == mpId).First();

                                if (ModelState.IsValid)
                                {
                                    db.MTRC_MGMT_AUTH_NEW.Remove(removeRow);
                                }
                            }

                            //Update records that are currently being unassigned and whose effective start date is NOT today.
                            //Set effective end date to yesterday's date
                            foreach (int mpId in userRoleMtrcList.Except(newUserRoleMtrcList).Except(userRoleMtrcListCreatedToday))
                            {
                                var updateRow = db.MTRC_MGMT_AUTH_NEW.Where(x => x.muar_id == app_user_role_id &&
                                                                    x.mtrc_period_id == mpId).First();
                                updateRow.mma_eff_end_date = DateTime.Today.AddDays(-1);

                                if (ModelState.IsValid)
                                {
                                    db.Entry(updateRow).State = EntityState.Modified;
                                }
                            }


                            db.SaveChanges();
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            notused = e.Message;
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                notused = e.Message;
            }

            return notused;
        }
        private string updateUserRoleMetricListWithoutCommit(int app_user_id, int app_user_role_id, List<int> asgndMtrcList)
        {
            string notused = "Success";

            try
            {
                //
                if (app_user_id == 0)
                {
                    notused = "User Id cannot be 0.";
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    List<int> userRoleMtrcList = new List<int>();
                    List<int> userRoleMtrcListCreatedToday = new List<int>();
                    List<int> newUserRoleMtrcList = asgndMtrcList;

                    userRoleMtrcList = getUserRoleMtrcList(app_user_role_id).Select(x => Convert.ToInt32(x.mtrc_period_id)).ToList();
                    userRoleMtrcListCreatedToday = getUserRoleMtrcListCreatedToday(app_user_role_id).Select(x => Convert.ToInt32(x.mtrc_period_id)).ToList();

                    //Begin Add Transaction
                    try
                    {
                        //Add Row if no active assignment for mpId (there may be a record from the past that has been effective end dated)
                        foreach (int mpId in newUserRoleMtrcList.Except(userRoleMtrcList))
                        {

                            var addRow = new MTRC_MGMT_AUTH_NEW
                            {
                                muar_id = app_user_role_id,
                                mtrc_period_id = Convert.ToInt16(mpId),
                                mma_eff_start_date = DateTime.Today,
                                mma_eff_end_date = new DateTime(2060, 12, 31)
                            };

                            if (ModelState.IsValid)
                            {
                                db.MTRC_MGMT_AUTH_NEW.Add(addRow);
                            }

                        }

                        //Delete records that are currently being unassigned and whose effective start date is today.
                        foreach (int mpId in userRoleMtrcListCreatedToday.Except(newUserRoleMtrcList))
                        {
                            var removeRow = db.MTRC_MGMT_AUTH_NEW.Where(x => x.muar_id == app_user_role_id &&
                                                                x.mtrc_period_id == mpId).First();

                            if (ModelState.IsValid)
                            {
                                db.MTRC_MGMT_AUTH_NEW.Remove(removeRow);
                            }
                        }

                        //Update records that are currently being unassigned and whose effective start date is NOT today.
                        //Set effective end date to yesterday's date
                        foreach (int mpId in userRoleMtrcList.Except(newUserRoleMtrcList).Except(userRoleMtrcListCreatedToday))
                        {
                            var updateRow = db.MTRC_MGMT_AUTH_NEW.Where(x => x.muar_id == app_user_role_id &&
                                                                x.mtrc_period_id == mpId).First();
                            updateRow.mma_eff_end_date = DateTime.Today.AddDays(-1);

                            if (ModelState.IsValid)
                            {
                                db.Entry(updateRow).State = EntityState.Modified;
                            }
                        }

                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        notused = e.Message;
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                notused = e.Message;
                throw;
            }

            return notused;
        }

        //Adds a user role. Adds building assignments to the user if required. Adds metric assignments to the role if required
        private string addUserRole(string raw_json)
        {
            string msg = "Success";

            try
            {
                //Parse JSON
                JObject parsed_result = JObject.Parse(raw_json);
                int app_user_id = (int)parsed_result["app_user_id"];
                int mar_id = (int)parsed_result["mar_id"];

                List<int> asgndMtrcList = new List<int>();
                JArray jMtrcs = (JArray)parsed_result["asgndMtrcList"];
                foreach (var res in jMtrcs)
                {
                    int mpId = (int)res;
                    asgndMtrcList.Add(mpId);
                }

                List<int> asgndBldgList = new List<int>();
                JArray jBldgs = (JArray)parsed_result["asgndBldgList"];
                foreach (var res in jBldgs)
                {
                    int bldgId = (int)res;
                    asgndBldgList.Add(bldgId);
                }

                //
                if (app_user_id == 0)
                {
                    msg = "User Id cannot be 0.";
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    //Begin Add Transaction
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //Create Metric User App Role record. Effective start date = today. Effective end date = 2060-12-31 00:00:00.000
                            var addRow = new MTRC_USER_APP_ROLES
                            {
                                app_user_id = app_user_id,
                                mar_id = mar_id,
                                muar_eff_start_dt = DateTime.Today,
                                muar_eff_end_dt = new DateTime(2060, 12, 31)
                            };

                            if (ModelState.IsValid)
                            {
                                db.MTRC_USER_APP_ROLES.Add(addRow);
                            }

                            db.SaveChanges();

                            //Check to see if building or metric assignments are necessary.
                            var query =
                                from role in db.MTRC_APP_ROLE
                                where role.mar_id == mar_id
                                select role;

                            MTRC_APP_ROLE thisRole = query.ToList().First();

                            //Make changes to user building assignments if required.
                            if(thisRole.mar_req_bldg_auth == "Y")
                            {
                                updateUserBuildingListWithoutCommit(app_user_id, asgndBldgList);
                            }

                            //Add new user role metric assignments if required. Get metric user app id from new Metric User Ap Role record.
                            if (thisRole.mar_req_mtrc_mgmt_auth == "Y")
                            {
                                int muar_id = addRow.muar_id;
                                updateUserRoleMetricListWithoutCommit(app_user_id, muar_id, asgndMtrcList);
                            }
                            
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            msg = e.Message;
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }

            return msg;
        }

        //Removes a user role. Removes metric assignments associated with the role if applicable.
        private string removeUserRole(string raw_json)
        {
            string msg = "Success";

            try
            {
                //Parse JSON
                JObject parsed_result = JObject.Parse(raw_json);
                int app_user_id = (int)parsed_result["app_user_id"];
                int muar_id = (int)parsed_result["muar_id"];

                //
                if (app_user_id == 0)
                {
                    msg = "User Id cannot be 0.";
                }
                else if (muar_id == 0)
                {
                    msg = "Metric User App Role Id cannot be 0.";
                }
                else
                {
                    //Begin Add Transaction
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var roleRow = db.MTRC_USER_APP_ROLES.Where(x => x.muar_id == muar_id).First();

                            //Delete Muar records that are currently being unassigned and whose effective start date is today.
                            if (ModelState.IsValid && DateTime.Today == roleRow.muar_eff_start_dt)
                            {
                                db.MTRC_USER_APP_ROLES.Remove(roleRow);
                            }
                            //Update Muar records that are currently being unassigned and whose effective start date is NOT today.
                            //Set effective end date to yesterday's date
                            else if (ModelState.IsValid && DateTime.Today != roleRow.muar_eff_start_dt)
                            {
                                roleRow.muar_eff_end_dt = DateTime.Today.AddDays(-1);

                                if (ModelState.IsValid)
                                {
                                    db.Entry(roleRow).State = EntityState.Modified;
                                }
                            }

                            //Delete/Effective end date metric assignments associated with the Muar record. 
                            //Check to see if building or metric assignments are necessary.
                            List<int> emptyList = new List<int>();
                            updateUserRoleMetricListWithoutCommit(app_user_id, muar_id, emptyList);

                            db.SaveChanges();
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            msg = e.Message;
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }

            return msg;
        }

        //-------------------------------------------------------------------------------------------------------------------
        #region Auto generated
        //-------------------------------------------------------------------------------------------------------------------

        // GET: UserMgmt/List
        public ActionResult List()
        {
            var dSC_APP_USER = db.DSC_APP_USER.Include(d => d.DSC_EMPLOYEE);
            return View(dSC_APP_USER.ToList());
        }

        // GET: UserMgmt/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_APP_USER dSC_APP_USER = db.DSC_APP_USER.Find(id);
            if (dSC_APP_USER == null)
            {
                return HttpNotFound();
            }
            return View(dSC_APP_USER);
        }

        // GET: UserMgmt/Create
        public ActionResult Create()
        {
            ViewBag.dsc_emp_id = new SelectList(db.DSC_EMPLOYEE, "dsc_emp_id", "dsc_emp_perm_id");
            return View();
        }

        // POST: UserMgmt/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "app_user_id,dsc_emp_id,app_user_sso_id,app_user_sso_system,app_user_email_addr,app_user_full_name,app_user_disabled_yn,app_user_disabled_on_dtm")] DSC_APP_USER dSC_APP_USER)
        {
            if (ModelState.IsValid)
            {
                db.DSC_APP_USER.Add(dSC_APP_USER);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.dsc_emp_id = new SelectList(db.DSC_EMPLOYEE, "dsc_emp_id", "dsc_emp_perm_id", dSC_APP_USER.dsc_emp_id);
            return View(dSC_APP_USER);
        }

        // GET: UserMgmt/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_APP_USER dSC_APP_USER = db.DSC_APP_USER.Find(id);
            if (dSC_APP_USER == null)
            {
                return HttpNotFound();
            }
            ViewBag.dsc_emp_id = new SelectList(db.DSC_EMPLOYEE, "dsc_emp_id", "dsc_emp_perm_id", dSC_APP_USER.dsc_emp_id);
            return View(dSC_APP_USER);
        }

        // POST: UserMgmt/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "app_user_id,dsc_emp_id,app_user_sso_id,app_user_sso_system,app_user_email_addr,app_user_full_name,app_user_disabled_yn,app_user_disabled_on_dtm")] DSC_APP_USER dSC_APP_USER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dSC_APP_USER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.dsc_emp_id = new SelectList(db.DSC_EMPLOYEE, "dsc_emp_id", "dsc_emp_perm_id", dSC_APP_USER.dsc_emp_id);
            return View(dSC_APP_USER);
        }

        // GET: UserMgmt/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_APP_USER dSC_APP_USER = db.DSC_APP_USER.Find(id);
            if (dSC_APP_USER == null)
            {
                return HttpNotFound();
            }
            return View(dSC_APP_USER);
        }

        // POST: UserMgmt/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DSC_APP_USER dSC_APP_USER = db.DSC_APP_USER.Find(id);
            db.DSC_APP_USER.Remove(dSC_APP_USER);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
