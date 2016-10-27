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
        [HttpGet]
        public ActionResult Index(string search)
        {
            var dSC_APP_USER = db.DSC_APP_USER.Include(d => d.DSC_EMPLOYEE);

            if (!String.IsNullOrWhiteSpace(search))
            {
                //Filters list where search string is contained in the full name, email address, or sso_id
                dSC_APP_USER = dSC_APP_USER.Where(x => x.app_user_full_name.Contains(search) 
                                                        || x.app_user_email_addr.Contains(search)
                                                        || x.app_user_sso_id.Contains(search));
            }
            
            return View(dSC_APP_USER.ToList());
        }

        //-------------------------------------------------------------------------------------------------------------------
        // GET: UserMaintenance
        public ActionResult UserMaintenance(int? id)
        {
            UserMgmtViewModel userMgmtViewModel = new UserMgmtViewModel();

            if (id == null || id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                ViewBag.appUserId = id;
                DSC_APP_USER dSC_APP_USER = db.DSC_APP_USER.Find(id);
                if (dSC_APP_USER == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    userMgmtViewModel.appUserDetails = dSC_APP_USER;
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

        //Get: _UserRoleMtrcAssign
        public ActionResult _UserRoleMtrcAssign(int? app_user_role_id)
        {
            MtrcAsgnViewModel mtrcAsgnViewModel = new MtrcAsgnViewModel();

            if (app_user_role_id == null || app_user_role_id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                mtrcAsgnViewModel.userRoleMtrcList = getUserRoleMtrcList(app_user_role_id);
                mtrcAsgnViewModel.unassignedMtrcList = getAllMetricList().Except(mtrcAsgnViewModel.userRoleMtrcList).ToList();
            }

            return PartialView(mtrcAsgnViewModel);
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
                int prodId = prod_id ?? 0;
                ViewBag.prod_sel_list = new SelectList(db.MTRC_PRODUCT, "prod_id", "prod_name", prodId);

                if(prodId == 0)
                {
                    ViewBag.role_sel_list_display = false;
                }
                else
                {
                    int marId = mar_id ?? 0;
                    ViewBag.role_sel_list_display = true;
                    ViewBag.role_sel_list = new SelectList(db.MTRC_APP_ROLE.Where(x => x.prod_id == prod_id), "mar_id", "mar_name", marId);

                    if(marId == 0)
                    {
                        ViewBag.role_display = false;
                    }
                    else
                    {
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


        //-------------------------------------------------------------------------------------------------------------------
        //------
        //"API"s
        //------
        //Return a list of buildings associated with a particular app user id
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

        //WIP WIP WIP WIP WIP WIP WIP WIP WIP WIP WIP WIP
        //WIP WIP WIP WIP WIP WIP WIP WIP WIP WIP WIP WIP
        private string updateUserBuildingList(int? appUserId, List<int> newBldgIdList)
        {
            List<DSC_MTRC_LC_BLDG> bldgList = new List<DSC_MTRC_LC_BLDG>();
            List<int> bldgIdsToAdd = new List<int>();
            List<int> bldgIdsToRemove = new List<int>();

            if (appUserId == null || appUserId == 0)
            {

            }
            else
            {
                var query2 =
                    from child in db.RZ_BLDG_AUTHORIZATION
                    where child.DSC_APP_USER.app_user_id == appUserId
                    select child.DSC_MTRC_LC_BLDG;

                bldgList = query2.ToList();

                foreach(DSC_MTRC_LC_BLDG bldg in bldgList)
                {
                    if (!newBldgIdList.Contains(bldg.dsc_mtrc_lc_bldg_id))
                    {
                        bldgIdsToRemove.Add(bldg.dsc_mtrc_lc_bldg_id);
                    }
                }
            }
            return "";
        }

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
                    select child.MTRC_METRIC_PERIOD;

                mtrcList = query2.ToList();
            }

            return mtrcList;
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





        //-------------------------------------------------------------------------------------------------------------------
        //--------------
        //Auto generated
        //--------------

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
    }
}
