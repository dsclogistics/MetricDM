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

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 
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

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 

        // GET: /MetricPeriod/_metricPeriodList/
        public PartialViewResult _metricPeriodList(int id)
        {
            List<MTRC_METRIC_PERIOD> metricPeriods = db.MTRC_METRIC_PERIOD.Where(x => x.mtrc_id == id).ToList();

            return PartialView(metricPeriods);
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 

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
                    else
                    {
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
        
        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 

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

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 

        //Get:  /MetricPeriod/AddMetricPeriod/'id'
        [HttpGet]
        public ActionResult AddMetricPeriod(int? id)
        {
            //Parameter "id" is the Metric Id for which a new Metric Period will be built for
            MTRC_METRIC_PERIOD mPeriod = new MTRC_METRIC_PERIOD();
            MTRC_METRIC mMetric = null;
            int metricId = id ?? 0;
            ViewBag.metricId = metricId;        //Default Value
            ViewBag.metricName = "Not Found";   //Default Value
            ViewBag.createNewMetricPeriod = true;    //Not Sure how this is being used. Leave alone for now
            ViewBag.tpt_id = new SelectList(db.MTRC_TIME_PERIOD_TYPE, "tpt_id", "tpt_name");
            ViewBag.mtrc_id = new SelectList(db.MTRC_METRIC, "mtrc_id", "mtrc_name", metricId);


            if (metricId > 0) { mMetric = db.MTRC_METRIC.Find(metricId); }

            //Check if Metric Id was not Specified or if it was not valid
            if (mMetric == null)
            {
                ViewBag.metricId = 0;
                ViewBag.metricType = "";
            }
            else
            {
                ViewBag.metricName = mMetric.mtrc_name;
                ViewBag.metricType = mMetric.MTRC_DATA_TYPE.data_type_token;
                string Type1 = mMetric.MTRC_DATA_TYPE.data_type_token;
                //Create New Metric Period with values from the parent metric.
                mPeriod = new MTRC_METRIC_PERIOD
                {
                    mtrc_id = mMetric.mtrc_id,
                    mtrc_period_token = mMetric.mtrc_token + "_",
                    mtrc_period_desc = mMetric.mtrc_desc,
                    mtrc_period_max_dec_places = mMetric.mtrc_max_dec_places,
                    mtrc_period_min_val = mMetric.mtrc_min_val,
                    mtrc_period_max_val = mMetric.mtrc_max_val,
                    mtrc_period_na_allow_yn = mMetric.mtrc_na_allow_yn.ToUpper(),
                    mtrc_period_max_str_size = mMetric.mtrc_max_str_size
                };
            }

            return View(mPeriod);
        }


        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 

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


        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 
        
        [ChildActionOnly]
        public ActionResult _productMetricColumns()
        {
            List<dataPair> columns = new List<dataPair>();
            try
            {
                columns.AddRange(
                    db.MTRC_METRIC_PRODUCTS.Where(x => x.prod_id == 1 && x.mtrc_prod_display_order != null)
                    .Select(
                    y => new dataPair
                    {
                        id = y.mtrc_prod_id,
                        dataValue = y.mtrc_prod_display_text,
                        dataValue2 = y.mtrc_prod_display_order.ToString()
                    })
                    .OrderBy(o => o.dataValue2)
                );
            }
            catch { }

            return PartialView(columns);
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 


        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 




    }
}