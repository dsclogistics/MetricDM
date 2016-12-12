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
    public class GoalMgmtController : Controller
    {
        private DSC_MTRC_DEVEntities db = new DSC_MTRC_DEVEntities();

        // GET: GoalMgmt
        public ActionResult Index()
        {

            var mTRC_MPG = db.MTRC_MPG.Include(m => m.MTRC_METRIC_PERIOD).Include(m => m.MTRC_PRODUCT);

            var query = from a in mTRC_MPG
                        join b in db.MTRC_PRODUCT on a.prod_id equals b.prod_id
                        join c in db.MTRC_METRIC_PRODUCTS on new { a.prod_id, a.mtrc_period_id } equals new { c.prod_id, c.mtrc_period_id }
                        where b.prod_token == "RED_ZONE_WHS"
                        select a;

            return View(mTRC_MPG.ToList());




        }











        // GET: GoalMgmt/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTRC_MPG mTRC_MPG = db.MTRC_MPG.Find(id);
            if (mTRC_MPG == null)
            {
                return HttpNotFound();
            }
            return View(mTRC_MPG);
        }

        // GET: GoalMgmt/Create
        public ActionResult Create()
        {
            ViewBag.mtrc_period_id = new SelectList(db.MTRC_METRIC_PERIOD, "mtrc_period_id", "mtrc_period_name");
            ViewBag.prod_id = new SelectList(db.MTRC_PRODUCT, "prod_id", "prod_name");
            return View();
        }

        // POST: GoalMgmt/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "mpg_id,mtrc_period_id,prod_id,mpg_less_val,mpg_less_eq_val,mpg_greater_val,mpg_greater_eq_val,mpg_equal_val,mpg_score,mpg_display_text,mpg_allow_bldg_override,mpg_start_eff_dtm,mpg_end_eff_dtm")] MTRC_MPG mTRC_MPG)
        {
            if (ModelState.IsValid)
            {
                db.MTRC_MPG.Add(mTRC_MPG);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.mtrc_period_id = new SelectList(db.MTRC_METRIC_PERIOD, "mtrc_period_id", "mtrc_period_name", mTRC_MPG.mtrc_period_id);
            ViewBag.prod_id = new SelectList(db.MTRC_PRODUCT, "prod_id", "prod_name", mTRC_MPG.prod_id);
            return View(mTRC_MPG);
        }

        // GET: GoalMgmt/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTRC_MPG mTRC_MPG = db.MTRC_MPG.Find(id);
            if (mTRC_MPG == null)
            {
                return HttpNotFound();
            }
            ViewBag.mtrc_period_id = new SelectList(db.MTRC_METRIC_PERIOD, "mtrc_period_id", "mtrc_period_name", mTRC_MPG.mtrc_period_id);
            ViewBag.prod_id = new SelectList(db.MTRC_PRODUCT, "prod_id", "prod_name", mTRC_MPG.prod_id);
            return View(mTRC_MPG);
        }

        // POST: GoalMgmt/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "mpg_id,mtrc_period_id,prod_id,mpg_less_val,mpg_less_eq_val,mpg_greater_val,mpg_greater_eq_val,mpg_equal_val,mpg_score,mpg_display_text,mpg_allow_bldg_override,mpg_start_eff_dtm,mpg_end_eff_dtm")] MTRC_MPG mTRC_MPG)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mTRC_MPG).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.mtrc_period_id = new SelectList(db.MTRC_METRIC_PERIOD, "mtrc_period_id", "mtrc_period_name", mTRC_MPG.mtrc_period_id);
            ViewBag.prod_id = new SelectList(db.MTRC_PRODUCT, "prod_id", "prod_name", mTRC_MPG.prod_id);
            return View(mTRC_MPG);
        }

        // GET: GoalMgmt/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTRC_MPG mTRC_MPG = db.MTRC_MPG.Find(id);
            if (mTRC_MPG == null)
            {
                return HttpNotFound();
            }
            return View(mTRC_MPG);
        }

        // POST: GoalMgmt/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MTRC_MPG mTRC_MPG = db.MTRC_MPG.Find(id);
            db.MTRC_MPG.Remove(mTRC_MPG);
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
