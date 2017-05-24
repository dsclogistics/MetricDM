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
    public class MetricController : Controller
    {
        private DSC_MTRC_DEVEntities db = new DSC_MTRC_DEVEntities();

        //============================================================================================================
        // GET: MetricPeriod
        public ActionResult Index(int? id)
        {
            if (id == null || id == 0)
            {  // return/Display all metrics
                return View(db.MTRC_METRIC.Include(m => m.MTRC_DATA_TYPE).ToList());
            }
            else
            {  //Retrieve a specific Metric
                List<MTRC_METRIC> dscMetricsList = db.MTRC_METRIC.Include(m => m.MTRC_DATA_TYPE).Where(y => y.mtrc_id == id).ToList();
                //MTRC_METRIC selectedMetric = db.MTRC_METRIC.Find(id);
                return View(dscMetricsList);
            }
        }
        //============================================================================================================
        // GET: MetricPeriod/Create
        public ActionResult Create()
        {
            ViewBag.data_type_id = new SelectList(db.MTRC_DATA_TYPE, "data_type_id", "data_type_name");
            return View();
        }

        //============================================================================================================
        // POST: MetricPeriod/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "mtrc_id,data_type_id,mtrc_name,mtrc_token,mtrc_desc,mtrc_eff_start_dt,mtrc_eff_end_dt,mtrc_min_val,mtrc_max_val,mtrc_max_dec_places,mtrc_max_str_size,mtrc_na_allow_yn")] MTRC_METRIC dscMetric)
        {
            if (ModelState.IsValid)
            {
                db.MTRC_METRIC.Add(dscMetric);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.data_type_id = new SelectList(db.MTRC_DATA_TYPE, "data_type_id", "data_type_name", dscMetric.data_type_id);
            return View(dscMetric);
        }

        //============================================================================================================
        // GET: MetricPeriod/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null || id == 0) { return RedirectToAction("Index", "Metric");  }

            MTRC_METRIC dscMetric = db.MTRC_METRIC.Find(id);
            if (dscMetric == null) { return RedirectToAction("Index","Metric");  }

            ViewBag.data_type_id = new SelectList(db.MTRC_DATA_TYPE, "data_type_id", "data_type_name", dscMetric.data_type_id);

            int metricPeriodCount = db.MTRC_METRIC_PERIOD.Where(x => x.mtrc_id == id).Count();
            if (metricPeriodCount > 0) { ViewBag.canBeDeleted = "N"; }
            else { ViewBag.canBeDeleted = "Y"; }
            
            return View(dscMetric);
        }

        //============================================================================================================
        // POST: MetricPeriod/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "mtrc_id,data_type_id,mtrc_name,mtrc_token,mtrc_desc,mtrc_eff_start_dt,mtrc_eff_end_dt,mtrc_min_val,mtrc_max_val,mtrc_max_dec_places,mtrc_max_str_size,mtrc_na_allow_yn")] MTRC_METRIC dscMetric)
        {
            if (!(dscMetric.mtrc_na_allow_yn == "Y" || dscMetric.mtrc_na_allow_yn == "N")) {
                ModelState.AddModelError("mtrc_na_allow_yn", "N/A Allowed must be 'Y' or 'N' - DATA NOT SAVED!!!");
            }
            if (ModelState.IsValid)
            {
                db.Entry(dscMetric).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.data_type_id = new SelectList(db.MTRC_DATA_TYPE, "data_type_id", "data_type_name", dscMetric.data_type_id);
            return View(dscMetric);
        }

        //============================================================================================================
        //GET: Metric/Delete/5
        public string Delete(int? id)
        {
            if (id == null) { return "Invalid Metric Id. Please review your input"; }
            
            MTRC_METRIC mTRC_METRIC = db.MTRC_METRIC.Find(id);
            if (mTRC_METRIC == null) { return "Metric '" + id + "' was not Found. PLease review your input."; }
            
            //Verify that the selected Metric does not have any Metric Periods associated to it
            int mpCount = db.MTRC_METRIC_PERIOD.Where(x => x.mtrc_id == id).Count();
            if (mpCount > 0)
            { //Metric Has Period Associated to it; It cannot Be Deleted
                return "Metric Id '" + id + "' is already in use and it cannot be deleted. Try changing its Effective End Date instead, to disable it.";
            }

            //Perform the Actual Metric Deletion
            var metric = db.MTRC_METRIC.FirstOrDefault(x => x.mtrc_id == id);
            if (metric != null) { 
                db.MTRC_METRIC.Remove(metric); 
            }
            db.SaveChanges();     // Save and commit all Transaction changes to the Database

            return "Metric Id '" + id + "' was Deleted Successfully";
        }

        //============================================================================================================
        //// POST: MetricPeriod/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    MTRC_METRIC mTRC_METRIC = db.MTRC_METRIC.Find(id);
        //    db.MTRC_METRIC.Remove(mTRC_METRIC);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //============================================================================================================
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
