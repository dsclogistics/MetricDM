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
    public class MetricPeriodController : Controller
    {
        private DSC_MTRC_DEVEntities db = new DSC_MTRC_DEVEntities();

        //============================================================================================================
        // GET: MetricPeriod
        public ActionResult Index(int? id)
        {
            if (id == null || id == 0)
            {
                return View(db.MTRC_METRIC.Include(m => m.MTRC_DATA_TYPE).ToList());
            }
            else
            {
                List<MTRC_METRIC> mTRC_METRIC = new List<MTRC_METRIC>();
                    
                mTRC_METRIC.Add(db.MTRC_METRIC.Find(id));

                if (mTRC_METRIC == null)
                {
                    return HttpNotFound();
                }
                return View(mTRC_METRIC);
            }
        }
        //============================================================================================================
        // GET: MetricPeriod/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTRC_METRIC mTRC_METRIC = db.MTRC_METRIC.Find(id);
            if (mTRC_METRIC == null)
            {
                return HttpNotFound();
            }
            return View(mTRC_METRIC);
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
        public ActionResult Create([Bind(Include = "mtrc_id,data_type_id,mtrc_name,mtrc_token,mtrc_desc,mtrc_eff_start_dt,mtrc_eff_end_dt,mtrc_min_val,mtrc_max_val,mtrc_max_dec_places,mtrc_max_str_size,mtrc_na_allow_yn")] MTRC_METRIC mTRC_METRIC)
        {
            if (ModelState.IsValid)
            {
                db.MTRC_METRIC.Add(mTRC_METRIC);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.data_type_id = new SelectList(db.MTRC_DATA_TYPE, "data_type_id", "data_type_name", mTRC_METRIC.data_type_id);
            return View(mTRC_METRIC);
        }

        //============================================================================================================
        // GET: MetricPeriod/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTRC_METRIC mTRC_METRIC = db.MTRC_METRIC.Find(id);
            if (mTRC_METRIC == null)
            {
                return HttpNotFound();
            }
            ViewBag.data_type_id = new SelectList(db.MTRC_DATA_TYPE, "data_type_id", "data_type_name", mTRC_METRIC.data_type_id);
            return View(mTRC_METRIC);
        }

        //============================================================================================================
        // POST: MetricPeriod/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "mtrc_id,data_type_id,mtrc_name,mtrc_token,mtrc_desc,mtrc_eff_start_dt,mtrc_eff_end_dt,mtrc_min_val,mtrc_max_val,mtrc_max_dec_places,mtrc_max_str_size,mtrc_na_allow_yn")] MTRC_METRIC mTRC_METRIC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mTRC_METRIC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.data_type_id = new SelectList(db.MTRC_DATA_TYPE, "data_type_id", "data_type_name", mTRC_METRIC.data_type_id);
            return View(mTRC_METRIC);
        }

        //============================================================================================================
        // GET: MetricPeriod/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTRC_METRIC mTRC_METRIC = db.MTRC_METRIC.Find(id);
            if (mTRC_METRIC == null)
            {
                return HttpNotFound();
            }
            return View(mTRC_METRIC);
        }

        //============================================================================================================
        // POST: MetricPeriod/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MTRC_METRIC mTRC_METRIC = db.MTRC_METRIC.Find(id);
            db.MTRC_METRIC.Remove(mTRC_METRIC);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //============================================================================================================
        // GET: /MetricPeriod/Maintenance/5
        public ActionResult MetricMaintenance(int? id)
        {
            id = (id==null)?0:id;    //Assign a Zero value to Id if it's null
            ViewBag.metric_sel_list = new SelectList(db.MTRC_METRIC, "mtrc_id", "mtrc_name", id);

            //Load all the Metrics into a Drop down List for Selection
            MTRC_METRIC selectedMetric;
            if (id == 0) { selectedMetric = null;}
            else { 
                selectedMetric = db.MTRC_METRIC.Find(id);
                ViewBag.data_type_id = new SelectList(db.MTRC_DATA_TYPE, "data_type_id", "data_type_name", selectedMetric.data_type_id);
            }
            //if (selectedMetric == null)
            //{
            //    throw new Exception("The selected Metric does not exist");
            //}
            return View(selectedMetric);
        }

        // POST: /MetricPeriod/Maintenance/5
        [HttpPost]
        public ActionResult MetricMaintenance(MTRC_METRIC selectedMetric)
        {
            ViewBag.saveResult = "Data Saved Successfully.";
            return View(selectedMetric);
        }

        //============================================================================================================
        // GET: /MetricPeriod/AddMetricPeriod
        public ActionResult AddMetricPeriod(int? id)
        {
            ViewBag.mtrc_id = new SelectList(db.MTRC_METRIC, "mtrc_id", "mtrc_name", id);
            ViewBag.tpt_id = new SelectList(db.MTRC_TIME_PERIOD_TYPE, "tpt_id", "tpt_name");
            ViewBag.return_id = id;

            return View();
        }

        //============================================================================================================
        // POST: /MetricPeriod/AddMetricPeriod
        [HttpPost]
        public string AddMetricPeriod(MTRC_METRIC_PERIOD newMetricPeriod)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //db.MTRC_METRIC_PERIOD.Add(newMetricPeriod);
                    //db.SaveChanges();
                }
                catch(Exception e)
                {
                    return "Error: " + e.Message; 
                }

                return "Data saved successfully!";
            }

            return "Validation errors found. Please review your input.";

        }

        //============================================================================================================
        // GET: /MetricPeriod/Maintenance/5
        public ActionResult MetricPeriodMaintenance(int? id)
        {
            //id = (id == null) ? 0 : id;    //Assign a Zero value to Id if it's null
            //ViewBag.metric_sel_list = new SelectList(db.MTRC_METRIC, "mtrc_id", "mtrc_name", id);

            ////Load all the Metrics into a Drop down List for Selection
            //MTRC_METRIC selectedMetric = db.MTRC_METRIC.Find(id);
            ////if (selectedMetric == null)
            ////{
            ////    throw new Exception("The selected Metric does not exist");
            ////}
            return View();
        }
        //============================================================================================================
        public PartialViewResult _metricPeriodList(int id)
        {
            List<MTRC_METRIC_PERIOD> metricPeriods = db.MTRC_METRIC_PERIOD.Where(x => x.mtrc_id == id).ToList();

            return PartialView(metricPeriods);
        }
        //============================================================================================================
        // GET: /MetricPeriod/_metricPeriodDetails/5
        //[ChildActionOnly]
        public PartialViewResult _metricPeriodDetails(int? id, int? mtrcId)
        {
            MTRC_METRIC_PERIOD mPeriod;
            MTRC_METRIC mMetric;
            if (id == null)
            {
                if(mtrcId == null)
                {
                    mPeriod = null;
                }
                else
                {
                    //If creating a new metric period, use values from the upper level metric to populate some of the fields.
                    mPeriod = new MTRC_METRIC_PERIOD();
                    mMetric = db.MTRC_METRIC.Find(mtrcId);
                    mPeriod.mtrc_period_max_dec_places = mMetric.mtrc_max_dec_places;
                    mPeriod.mtrc_period_min_val = mMetric.mtrc_min_val;
                    mPeriod.mtrc_period_max_val = mMetric.mtrc_max_val;
                    mPeriod.mtrc_period_na_allow_yn = mMetric.mtrc_na_allow_yn;
                    mPeriod.mtrc_period_max_str_size = mMetric.mtrc_max_str_size;
                }

                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                ViewBag.tpt_id = new SelectList(db.MTRC_TIME_PERIOD_TYPE, "tpt_id", "tpt_name");
                ViewBag.createNewMetricPeriod = true;
            }
            else {
                mPeriod = db.MTRC_METRIC_PERIOD.Find(id);
                ViewBag.tpt_id = new SelectList(db.MTRC_TIME_PERIOD_TYPE, "tpt_id", "tpt_name", mPeriod.tpt_id);
                ViewBag.createNewMetricPeriod = false;
            }

            //
            //MTRC_TIME_PERIOD_TYPE selectedTpt;
            //if (mPeriod.tpt_id == 0) { selectedTpt = null; }
            //else {
            //    selectedTpt = db.MTRC_TIME_PERIOD_TYPE.Find(mPeriod.tpt_id);
            //    ViewBag.tpt_id = new SelectList(db.MTRC_TIME_PERIOD_TYPE, "tpt_id", "tpt_name", selectedTpt.tpt_id);
            //}

            return PartialView(mPeriod);
        }
        //============================================================================================================
        // POST: /MetricPeriod/_metricPeriodDetails
        [HttpPost] 
        //[ValidateAntiForgeryToken]
        public PartialViewResult _metricPeriodDetails(MTRC_METRIC_PERIOD mPeriod)
        {
            ViewBag.tpt_id = new SelectList(db.MTRC_TIME_PERIOD_TYPE, "tpt_id", "tpt_name", mPeriod.tpt_id);

            return PartialView(mPeriod);
        }
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
