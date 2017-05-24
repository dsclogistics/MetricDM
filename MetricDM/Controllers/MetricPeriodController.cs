using MetricDM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

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
                    MTRC_METRIC mMetric = db.MTRC_METRIC.Find(mtrcId);
                    if (mMetric == null) { mPeriod = null; }
                    else
                    {
                        mPeriod = new MTRC_METRIC_PERIOD { 
                            mtrc_id = mMetric.mtrc_id,
                            mtrc_period_max_dec_places = mMetric.mtrc_max_dec_places,
                            mtrc_period_min_val = mMetric.mtrc_min_val,
                            mtrc_period_max_val = mMetric.mtrc_max_val,
                            mtrc_period_na_allow_yn = mMetric.mtrc_na_allow_yn.ToUpper(),
                            mtrc_period_max_str_size = mMetric.mtrc_max_str_size
                        };
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

            
            //Check if the Metric Period Can be Deleted
            int valueRecCount = db.MTRC_METRIC_PERIOD_VALUE.Where(x => x.mtrc_period_id == mPeriod.mtrc_period_id).Count();
            if (mPeriod.mtrc_period_id > 0 && valueRecCount == 0) { ViewBag.DeleteAllowed = "Y"; }
            else { ViewBag.DeleteAllowed = "N"; }

            ViewBag.ErrorMsg = "";
            ViewBag.alertCSS = "";

            return PartialView(mPeriod);
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 

        // POST: /MetricPeriod/_metricPeriodDetails
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public PartialViewResult _metricPeriodDetails(MTRC_METRIC_PERIOD mPeriod)
        {
            ViewBag.tpt_id = new SelectList(db.MTRC_TIME_PERIOD_TYPE, "tpt_id", "tpt_name", mPeriod.tpt_id);
            ViewBag.createNewMetricPeriod = false;
            ViewBag.alertCSS = "alert-danger";

            //Get the Product Id for this Metric Period
            int prod_Id = db.MTRC_METRIC_PRODUCTS.FirstOrDefault(x => x.mtrc_period_id == mPeriod.mtrc_period_id).prod_id;
            //Check if the Metric Period Can be Deleted
            int valueRecCount = db.MTRC_METRIC_PERIOD_VALUE.Where(x => x.mtrc_period_id == mPeriod.mtrc_period_id).Count();
            if (mPeriod.mtrc_period_id > 0 && valueRecCount == 0) { ViewBag.DeleteAllowed = "Y"; }
            else { ViewBag.DeleteAllowed = "N"; }


            //Perform Basic Validation
            if (prod_Id == 1 && mPeriod.tpt_id != 6)
            {  //For Product '1' (Red Zone), a monthly metric type is required
                ViewBag.ErrorMsg = "Red Zone Metrics require a 'Month' type Time Period. Data not Saved";
                return PartialView(mPeriod);
            }

            try {
                if (ModelState.IsValid)
                {
                    db.Entry(mPeriod).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.ErrorMsg = "Data Saved Successfully!";
                    ViewBag.alertCSS = "alert-success";
                }
                else
                {
                    ViewBag.ErrorMsg = "Invalid Data. Please review your input";
                }            
            }
            catch(Exception ex) {
                ViewBag.ErrorMsg = "ERROR: " + ex.Message;
            }

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
            ViewBag.prod_id = new SelectList(db.MTRC_PRODUCT, "prod_id", "prod_name");

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
        [ValidateAntiForgeryToken]
        public string AddMetricPeriod(string jsonData)
        {
            //This "Post" Action will add a new Metric Period  on all Tables that hold "Metric Period" related information.
            // - Add a record to the "MTRC_METRIC_PERIOD" table for the New Metric Period
            // - Add a record to the "MTRC_METRIC_PRODUCTS" table. New record to be used as the column identifier in the dashboards
            // - Add a record to the "MTRC_MPG" table. This is Metric Period Goal to be used for this new MP
            // - Add multiple records to the "MTRC_BLDG_MTRC_PERIOD" table. One records for each active building that will be part of the metric
            // - If the New Metric Period has a start date prior to the Current Period; then the Time Period that will be affected must be created:
            //     * Create a record in "MTRC_TM_PERIODS" for each period in the past (From the Effective Start Date until the current period)
            //     * For Each record added in "MTRC_TM_PERIODS", also create an associated record in the "RZ_MTRC_PERIOD_STATUS" table
            //       and initialize the field "rz_mps_status" = "Open". 
            //       [Future Time Periods will be opened automatically at the begining of next period]

            JavaScriptSerializer jsParser = new JavaScriptSerializer();
            var newMP = jsParser.Deserialize<newMetricPeriod>(jsonData);
            //dynamic newMP = jss.Deserialize<dynamic>(jsonData);
            
            //Perform Basic Validation
            //Red ZOne Metric Periods are monthly Periods and should start at the end of the month
            if (newMP.effStartDate.Day != 1) { return "The Effective Start Day Must always be the first of the Month";}

            //Verify that the Effective Start Day of this Metric Period is greater than the Effective Start Date of the Metric from which it is derived
            int metricId = Int32.Parse(newMP.mtrc_id);
            DateTime metricStartDate = db.MTRC_METRIC.FirstOrDefault(x => x.mtrc_id == metricId).mtrc_eff_start_dt;
            if (metricStartDate == null || metricStartDate < new DateTime(2000, 1, 1)) {
                return "ERROR: The Effective Metric Period Start Date or its associated Metric Start Date is not valid.";
            }

            if (newMP.effStartDate < metricStartDate)
            {
                return "ERROR: The Metric Period Start Date cannot be before the Effective Start Date of it associated Metric ('" + metricStartDate.ToString("MMM dd, yyyy") + "')";
            }

            try
            {
                // Collect all data for each of the tables to Save. And save them as part of a single transaction so the update is on all tables or none
                // First create the "MTRC_METRIC_PERIOD" Table record to Add
                MTRC_METRIC_PERIOD mp = db.MTRC_METRIC_PERIOD.FirstOrDefault(x => x.mtrc_period_token == newMP.mtrc_period_token);
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (mp != null) { 
                            throw new Exception("The current Metric Period cannot be created.<br/>A Metric Period with Token '" + newMP.mtrc_period_token + "' already Exists.");
                        }
                        mp = new MTRC_METRIC_PERIOD
                        {
                            mtrc_id = Int16.Parse(newMP.mtrc_id),
                            tpt_id = short.Parse(newMP.tpt_id),
                            mtrc_period_name = newMP.mtrc_period_name,
                            mtrc_period_token = newMP.mtrc_period_token,
                            mtrc_period_desc = newMP.mtrc_period_desc,
                            mtrc_period_calc_yn = newMP.mtrc_period_calc_yn,
                            mtrc_period_min_val = Decimal.Parse(newMP.mtrc_period_min_val),
                            mtrc_period_max_val = Decimal.Parse(newMP.mtrc_period_max_val),
                            mtrc_period_max_dec_places = Int16.Parse(newMP.mtrc_period_max_dec_places),
                            mtrc_period_max_str_size = String.IsNullOrEmpty(newMP.mtrc_period_max_str_size) ? short.Parse("0") : (short.Parse(newMP.mtrc_period_max_str_size)),
                            mtrc_period_na_allow_yn = newMP.mtrc_period_na_allow_yn,
                            mtrc_period_can_import_yn = newMP.mtrc_period_can_import_yn,
                            mtrc_period_is_auto_yn = newMP.mtrc_period_is_auto_yn
                        };
                        if (mp.mtrc_period_max_str_size == 0) { mp.mtrc_period_max_str_size = null; }
                        //Save Metric Period Record
                        db.MTRC_METRIC_PERIOD.Add(mp);
                        db.SaveChanges();

                        //Retrieve the Metric Period Id of the record that was recently generated
                        int mp_Id = db.MTRC_METRIC_PERIOD.SingleOrDefault(item => item.mtrc_period_token == newMP.mtrc_period_token).mtrc_period_id;

                        //Resequence the order of the existing columns if needed
                        foreach (var column in newMP.ColumnsOrder) {
                            MTRC_METRIC_PRODUCTS mmp = db.MTRC_METRIC_PRODUCTS.FirstOrDefault(x => x.mtrc_prod_display_text == column.mtrc_prod_display_text && x.mtrc_prod_display_order == column.old_Order);
                            mmp.mtrc_prod_display_order = column.mtrc_prod_display_order;
                            db.SaveChanges();
                        }

                        //Add the New Metric Product Record with the new order sequence
                        MTRC_METRIC_PRODUCTS mproduct = new MTRC_METRIC_PRODUCTS
                        {
                            prod_id = 1,
                            mtrc_period_id = mp_Id,
                            mtrc_prod_top_lvl_parent_yn = "Y",
                            mtrc_prod_display_text = newMP.mtrc_prod_display_text,
                            mtrc_prod_display_order = Int16.Parse(newMP.mtrc_prod_display_order),
                            mtrc_prod_eff_start_dt = newMP.effStartDate,
                            mtrc_prod_eff_end_dt = newMP.effEndDate
                        };
                        db.MTRC_METRIC_PRODUCTS.Add(mproduct);                       
                        //db.SaveChanges();     /////////// Needed Individually???????????????????????????

                        //Collect all data for the "MTRC_MPG" Table record to Add
                        //Add the Metric Perios Goal and associated goal condition(s) for the new metric product
                        MTRC_MPG mpg = new MTRC_MPG
                        {
                            mtrc_period_id = mp_Id,
                            prod_id = 1,
                            mpg_score = 1,
                            mpg_display_text = newMP.goal_disp_text,
                            mpg_allow_bldg_override = newMP.mtrc_period_na_allow_yn,
                            mpg_start_eff_dtm = newMP.effStartDate,
                            mpg_end_eff_dtm = newMP.effEndDate
                        };
                        foreach (var goalCondition in newMP.goalConditions) {
                            decimal dValue = 0;
                            if (!goalCondition.condition.Trim().Equals("=") &&  !Decimal.TryParse(goalCondition.value, out dValue)) {
                                throw new Exception("Input Goal Value Conditions are not valid");
                            }
                            switch (goalCondition.condition.Trim())
                            {
                                case "<":
                                    mpg.mpg_less_val = dValue;
                                    break;
                                case "<=":
                                    mpg.mpg_less_eq_val = dValue;
                                    break;
                                case ">":
                                    mpg.mpg_greater_val = dValue;
                                    break;
                                case ">=":
                                    mpg.mpg_greater_eq_val = dValue;
                                    break;
                                case "=":
                                    mpg.mpg_equal_val = goalCondition.value;
                                    break;
                                default:
                                    throw new Exception("Input Goal Value Condition was not Recognized. Valid values are >, >=, <, <=, =");
                            }                            
                        }
                        
                        db.MTRC_MPG.Add(mpg);   //  -- Save the new MPG
                        //db.SaveChanges();     /////////// Needed Individually???????????????????????????

                        //Collect all data for the "MTRC_BLDG_MTRC_PERIOD" Table records to Add
                        //Insert into the "MTRC_BLDG_MTRC_PERIOD" one new record for each active building
                        
                        //List<MTRC_BLDG_MTRC_PERIOD> incBMPs = db.DSC_MTRC_LC_BLDG
                        //    .Where(x => x.dsc_mtrc_lc_bldg_eff_start_dt <= newMP.effStartDate && x.dsc_mtrc_lc_bldg_eff_end_dt >= newMP.effEndDate)
                        //    .Select(y => new MTRC_BLDG_MTRC_PERIOD{
                        //        data_src_id = 1,
                        //        dsc_mtrc_lc_bldg_id = y.dsc_mtrc_lc_bldg_id,
                        //        mtrc_period_id = mp_Id,
                        //        bmp_is_editable_yn = "Y",
                        //        bmp_is_manual_yn = "Y",
                        //        bmp_na_allow_yn = newMP.mtrc_period_na_allow_yn
                        //    })
                        //    .ToList();

                        List<DSC_MTRC_LC_BLDG> buildingList = db.DSC_MTRC_LC_BLDG.Where(x => x.dsc_mtrc_lc_bldg_eff_start_dt <= newMP.effStartDate && x.dsc_mtrc_lc_bldg_eff_end_dt >= newMP.effEndDate).ToList();
                        List<MTRC_BLDG_MTRC_PERIOD> buildingMPList = new List<MTRC_BLDG_MTRC_PERIOD>();
                        foreach (var lcBuilding in buildingList)
                        {
                            MTRC_BLDG_MTRC_PERIOD buildingMP = new MTRC_BLDG_MTRC_PERIOD
                            {
                                data_src_id = 1,
                                dsc_mtrc_lc_bldg_id = lcBuilding.dsc_mtrc_lc_bldg_id,
                                mtrc_period_id = mp_Id,
                                bmp_is_editable_yn = "Y",
                                bmp_is_manual_yn = "Y",
                                bmp_na_allow_yn = newMP.mtrc_period_na_allow_yn
                            };
                            buildingMPList.Add(buildingMP);
                        }
                        db.MTRC_BLDG_MTRC_PERIOD.AddRange(buildingMPList);

                        //Check if the Metric Period Start Date is less than today, to create all previous Time Periods if required
                        DateTime timePeriodStart = newMP.effStartDate;
                        bool timePeriodRequired = (timePeriodStart < DateTime.Now);
                        while (timePeriodRequired)
                        {
                            // Check if a monthly (type 6) Time Period already exist, else create one
                            int time_P_Id = 0;
                            MTRC_TM_PERIODS timePeriodNew = db.MTRC_TM_PERIODS.FirstOrDefault(x => x.tm_per_start_dtm == timePeriodStart && x.tpt_id == 6);
                            if (timePeriodNew == null)
                            {
                                timePeriodNew = new MTRC_TM_PERIODS
                                {
                                    tpt_id = Int16.Parse(newMP.tpt_id),
                                    tm_per_start_dtm = timePeriodStart,
                                    tm_per_end_dtm = timePeriodStart.AddMonths(1).AddDays(-1)
                                };
                                db.MTRC_TM_PERIODS.Add(timePeriodNew);
                                time_P_Id = db.MTRC_TM_PERIODS.SingleOrDefault(tp => tp.tm_per_start_dtm == timePeriodStart && tp.tpt_id == 6).tm_period_id;
                            }
                            else {                                
                                time_P_Id = (int)timePeriodNew.tm_period_id;
                            }

                            //Add a Time Period Status Record for the time period just created
                            RZ_MTRC_PERIOD_STATUS metricPeriodStatus = new RZ_MTRC_PERIOD_STATUS
                            {
                                tm_period_id = time_P_Id,
                                mtrc_period_id = mp_Id,  // Id of Metric Period creater earlier
                                rz_mps_status = "Open",
                                rz_mps_opened_on_dtm = DateTime.Now
                            };
                            db.RZ_MTRC_PERIOD_STATUS.Add(metricPeriodStatus);

                            timePeriodStart = timePeriodStart.AddMonths(1);       //Increase the Time period to the following month to create new if needed
                            timePeriodRequired = (timePeriodStart < DateTime.Now);
                        };                        

                        db.SaveChanges();     // Save all Transaction changes to the Database
                        transaction.Commit();
                        //------------ Successfully completed all Action that take part of a Transaction DB Update ---------------
                    }//end of try
                    catch (Exception ex)
                    {
                        // On any error rollback all changes and report an error
                        transaction.Rollback();
                        return "ERROR: " + ex.Message;
                    }
                }//end of  using (var transaction = db.Database.BeginTransaction())    
            }
            catch(Exception ex) {
                return "ERROR: " + ex.Message;
            }

            return "Data Saved Successfully!";
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
                        dataValueInt = (int)y.mtrc_prod_display_order,
                        dataValue2 = y.mtrc_prod_display_order.ToString()
                    })
                    .OrderBy(o => o.dataValueInt)
                );
            }
            catch { }

            return PartialView(columns);
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 
        [HttpPost]
        public string _DeleteMP(string mp_id)
        {
            //int iMetricId = 0;
            int iMetricPeriodId = 0;
            
            //if (!Int32.TryParse(mtrc_id,out iMetricId)){  return "Invalid Metric Id. Metric Period Cannot be Deleted."; }
            if (!Int32.TryParse(mp_id,out iMetricPeriodId)){  return "Invalid Metric Period Id. Metric Period Cannot be Deleted.";  }
            if (iMetricPeriodId == 0) { return "Invalid Metric Period Id. Review your input and try again."; }
            
            //Check if the Metric Period Can be Deleted
            int valueRecCount = db.MTRC_METRIC_PERIOD_VALUE.Where(x => x.mtrc_period_id == iMetricPeriodId).Count();
            if (valueRecCount > 0) { //Metric Period cannot Be Deleted
                return "Metric Period Id '" + mp_id + "' is already in use and it cannot be deleted. Try changing its Effective End Date to disable it.";
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //Retrieve and Delete all Associate Metric Period Status Entries
                    List<RZ_MTRC_PERIOD_STATUS> mpStatuses = db.RZ_MTRC_PERIOD_STATUS.Where(x => x.mtrc_period_id == iMetricPeriodId).ToList();
                    if (mpStatuses != null){db.RZ_MTRC_PERIOD_STATUS.RemoveRange(mpStatuses);}

                    //Delete All Building Period Entries
                    var buildingPeriods = db.MTRC_BLDG_MTRC_PERIOD.Where(x => x.mtrc_period_id == iMetricPeriodId).ToList();
                    if (buildingPeriods != null) { db.MTRC_BLDG_MTRC_PERIOD.RemoveRange(buildingPeriods); }

                    //Delete the Metric Period Goal Entry
                    var mPeriodGoal = db.MTRC_MPG.FirstOrDefault(x => x.mtrc_period_id == iMetricPeriodId);
                    if (mPeriodGoal != null) { db.MTRC_MPG.Remove(mPeriodGoal); }

                    //Delete the Metric Product Entry
                    var mProduct = db.MTRC_METRIC_PRODUCTS.FirstOrDefault(x => x.mtrc_period_id == iMetricPeriodId);
                    if (mProduct != null) { db.MTRC_METRIC_PRODUCTS.Remove(mProduct); }

                    //Delete the Metric Period Entry
                    var mPeriod = db.MTRC_METRIC_PERIOD.FirstOrDefault(x => x.mtrc_period_id == iMetricPeriodId);
                    if (mPeriod != null) { db.MTRC_METRIC_PERIOD.Remove(mPeriod); }

                    ////Delete the Metric Entry
                    //var mMetric = db.MTRC_METRIC.FirstOrDefault(x => x.mtrc_id == iMetricId);
                    //if (mMetric != null) { db.MTRC_METRIC.Remove(mMetric); }

                    db.SaveChanges();     // Save and commit all Transaction changes to the Database
                    transaction.Commit();
                }//------------ Successfully completed all Action that take part of a Transaction DB Update ---------------
                catch (Exception ex)
                {  // On any error rollback all changes and report an error
                    transaction.Rollback();
                    return "ERROR: " + ex.Message;
                }
            }//end of  using (var transaction = db.Database.BeginTransaction())  

            return "Metric Period Id '" + mp_id + "' Was Successfully Deleted!";
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 


        //= = = = = = = = = = = = = =     HELPER CLASES  = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 
        //The "newMetricPeriod" class is used to hold all the values that are posted from the "addMetricPeriod" page.
        //These object properties are used to populate all the affected tables directly related to adding a new Metric Period
        public class newMetricPeriod
        {
            public string mtrc_id { get; set; }
            public string mtrc_period_name { get; set; }
            public string mtrc_period_desc { get; set; }
            public string tpt_id { get; set; }
            public string mtrc_period_token { get; set; }
            public string mtrc_period_max_dec_places { get; set; }
            public string mtrc_period_max_str_size { get; set; }            
            public string mtrc_period_calc_yn { get; set; }
            public string mtrc_period_min_val { get; set; }
            public string mtrc_period_max_val { get; set; }
            public string mtrc_period_na_allow_yn { get; set; }
            public string mtrc_period_can_import_yn { get; set; }
            public string mtrc_period_is_auto_yn { get; set; }
            public string mtrc_prod_display_text { get; set; }
            public DateTime effStartDate { get; set; }
            public DateTime effEndDate { get; set; }
            public string flgBldgOverride { get; set; }
            public string mtrc_prod_display_order { get; set; }
            public string goal_disp_text { get; set; }
            public goalCondition[] goalConditions { get; set; }
            public dashboardColumn[] ColumnsOrder { get; set; }
        }

        public class goalCondition
        {
            public string condition { get; set; }
            public string value { get; set; }
        }

        public class dashboardColumn
        {
            public string prod_id { get; set; }
            public string mtrc_prod_display_text { get; set; }
            public Int16 old_Order { get; set; }
            public Int16 mtrc_prod_display_order { get; set; }
        }

    }
}