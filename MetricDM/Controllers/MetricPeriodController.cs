using MetricDM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MetricDM.Controllers
{
    public class MetricPeriodController : Controller
    {
        private DSC_MTRC_DEVEntities db = new DSC_MTRC_DEVEntities();
        //===========================================================================
        // GET: MetricPeriod
        public ActionResult Index(int? id)
        {
            id = (id == null) ? 0 : id;    //Assign a Zero value to Id if it's null
            ViewBag.metric_sel_list = new SelectList(db.MTRC_METRIC, "mtrc_id", "mtrc_name", id);

            //Load all the Metrics into a Drop down List for Selection
            MTRC_METRIC selectedMetric = null;
            if (id > 0) 
            {
                selectedMetric = db.MTRC_METRIC.Find(id);
                if (selectedMetric != null)
                ViewBag.data_type_id = new SelectList(db.MTRC_DATA_TYPE, "data_type_id", "data_type_name", selectedMetric.data_type_id);
            }
            return View(selectedMetric);
        }

        //============================================================================================================
        // GET: /MetricPeriod/_metricPeriodDetails/
        //[ChildActionOnly]
        public PartialViewResult _metricPeriodDetails(int? id, int? mtrcId)
        {
            MTRC_METRIC_PERIOD mPeriod;            
            if (id == null)
            {
                if (mtrcId == null)
                {
                    mPeriod = null;
                }
                else
                {
                    //If creating a new metric period, use values from the upper level metric to populate some of the fields.
                    mPeriod = new MTRC_METRIC_PERIOD();
                    MTRC_METRIC mMetric = db.MTRC_METRIC.Find(mtrcId);
                    if (mMetric == null) { mPeriod = null; }
                    else {
                        mPeriod.mtrc_id = mMetric.mtrc_id;
                        mPeriod.mtrc_period_max_dec_places = mMetric.mtrc_max_dec_places;
                        mPeriod.mtrc_period_min_val = mMetric.mtrc_min_val;
                        mPeriod.mtrc_period_max_val = mMetric.mtrc_max_val;
                        mPeriod.mtrc_period_na_allow_yn = mMetric.mtrc_na_allow_yn.ToUpper();
                        mPeriod.mtrc_period_max_str_size = mMetric.mtrc_max_str_size;
                    }
                }

                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                ViewBag.tpt_id = new SelectList(db.MTRC_TIME_PERIOD_TYPE, "tpt_id", "tpt_name");
                ViewBag.createNewMetricPeriod = true;
            }
            else
            {
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














        public ActionResult AddMetricPeriod(int? id)
        {
            //Id received as a parameter is the Metric Id for which a new Metric Period will be built for
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
                    db.MTRC_METRIC_PERIOD.Add(newMetricPeriod);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return "Error: " + e.Message;
                }

                return "Data saved successfully!";
            }

            return "Validation errors found. Please review your input.";

        }

        //============================================================================================================





















        // GET: /MetricPeriod/_metricPeriodList/
        public PartialViewResult _metricPeriodList(int id)
        {
            List<MTRC_METRIC_PERIOD> metricPeriods = db.MTRC_METRIC_PERIOD.Where(x => x.mtrc_id == id).ToList();

            return PartialView(metricPeriods);
        }
        //============================================================================================================

        // POST: /MetricPeriod/_metricPeriodDetails
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public PartialViewResult _metricPeriodDetails(MTRC_METRIC_PERIOD mPeriod)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mPeriod).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.tpt_id = new SelectList(db.MTRC_TIME_PERIOD_TYPE, "tpt_id", "tpt_name", mPeriod.tpt_id);
                ViewBag.createNewMetricPeriod = false;
                return PartialView(mPeriod);
            }
            ViewBag.tpt_id = new SelectList(db.MTRC_TIME_PERIOD_TYPE, "tpt_id", "tpt_name", mPeriod.tpt_id);
            ViewBag.createNewMetricPeriod = false;
            return PartialView(mPeriod);
        }
        //============================================================================================================



        // POST: /MetricPeriod/Maintenance/5
        //[HttpPost]
        //public ActionResult MetricMaintenance(MTRC_METRIC selectedMetric)
        //{
        //    ViewBag.saveResult = "Data Saved Successfully.";
        //    return View(selectedMetric);
        //}

    }
}