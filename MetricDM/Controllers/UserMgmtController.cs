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
        public ActionResult Index()
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
