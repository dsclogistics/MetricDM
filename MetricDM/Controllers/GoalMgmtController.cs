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
    public class GoalMgmtController : Controller
    {
        private DSC_MTRC_DEVEntities db = new DSC_MTRC_DEVEntities();

        // GET: GoalMgmt
        public ActionResult Index(int? prodId, int? mpId)
        {
            GoalMgmtViewModel goalMgmtViewModel = new GoalMgmtViewModel();
            MetricPeriodGoal enterpriseGoal = new MetricPeriodGoal();
            List<MetricPeriodBuildingGoal> buildingGoalList = new List<MetricPeriodBuildingGoal>();
            List<MetricPeriodGoal> enterpriseGoalList = new List<MetricPeriodGoal>();

            prodId = prodId ?? 0;     //Assign a Zero value to prodId if it's null
            mpId = mpId ?? 0;         //Assign a Zero value to mpId if it's null

            ViewBag.prodId = prodId;
            ViewBag.mpId = mpId;

            ViewBag.prod_sel_list = new SelectList(db.MTRC_PRODUCT, "prod_id", "prod_name", prodId);
            ViewBag.mp_sel_list = new SelectList(Enumerable.Empty<SelectListItem>());

            if (prodId == 0)
            {
                //Don't display mp detail grid
            }
            else
            {
                // GET all currently active product metric period goals for the specifid prodId.
                var query = from a in db.MTRC_MPG
                            join b in db.MTRC_PRODUCT on a.prod_id equals b.prod_id
                            join c in db.MTRC_METRIC_PRODUCTS on new { a.prod_id, a.mtrc_period_id } equals new { c.prod_id, c.mtrc_period_id }
                            where a.prod_id == prodId
                                && DateTime.Now >= DbFunctions.TruncateTime(a.mpg_start_eff_dtm)
                                && DateTime.Now <= DbFunctions.TruncateTime(a.mpg_end_eff_dtm)
                            select new MetricPeriodGoal
                            {
                                mTRC_MPG = a,
                                mtrc_period_id = a.mtrc_period_id.ToString(),
                                mtrc_prod_display_text = c.mtrc_prod_display_text
                            };

                ViewBag.mp_sel_list = new SelectList(query, "mtrc_period_id", "mtrc_prod_display_text", mpId);

                goalMgmtViewModel.enterpriseGoalList = query.ToList();

                if (mpId == 0)
                {

                }
                else
                {
                    // Display single product metric period goal
                    var query2 = from a in db.MTRC_MPG
                                 join b in db.MTRC_PRODUCT on a.prod_id equals b.prod_id
                                 join c in db.MTRC_METRIC_PRODUCTS on new { a.prod_id, a.mtrc_period_id } equals new { c.prod_id, c.mtrc_period_id }
                                 where a.prod_id == prodId
                                     && a.mtrc_period_id == mpId
                                     && DateTime.Now >= DbFunctions.TruncateTime(a.mpg_start_eff_dtm)
                                     && DateTime.Now <= DbFunctions.TruncateTime(a.mpg_end_eff_dtm)
                                 select new MetricPeriodGoal
                                 {
                                     mTRC_MPG = a,
                                     mtrc_period_id = a.mtrc_period_id.ToString(),
                                     mtrc_prod_display_text = c.mtrc_prod_display_text
                                 };

                    goalMgmtViewModel.enterpriseGoal = query2.ToList().First();

                    var emptyMTRC_MPBG = new MTRC_MPBG();

                    //Get all currently active product metric period building goals for the specified prodId and mpId.
                    var query3 = (from d in db.DSC_MTRC_LC_BLDG
                                  join a in db.MTRC_MPBG on d.dsc_mtrc_lc_bldg_id equals a.dsc_mtrc_lc_bldg_id into group1
                                  from g1 in group1.Where(x => x.prod_id == prodId
                                                               && x.mtrc_period_id == mpId
                                                               && DateTime.Now >= DbFunctions.TruncateTime(x.mpbg_start_eff_dtm)
                                                               && DateTime.Now <= DbFunctions.TruncateTime(x.mpbg_end_eff_dtm))
                                                               .DefaultIfEmpty()
                                  join b in db.MTRC_PRODUCT on g1.prod_id equals b.prod_id into group2
                                  from g2 in group2.DefaultIfEmpty()
                                  join c in db.MTRC_METRIC_PRODUCTS on new { g1.prod_id, g1.mtrc_period_id } equals new { c.prod_id, c.mtrc_period_id } into group3
                                  from g3 in group3.DefaultIfEmpty()
                                      //where g1.prod_id == prodId
                                      //    && g1.mtrc_period_id == mpId
                                      //    && DateTime.Now >= DbFunctions.TruncateTime(g1.mpbg_start_eff_dtm)
                                      //    && DateTime.Now <= DbFunctions.TruncateTime(g1.mpbg_end_eff_dtm)
                                  select new MetricPeriodBuildingGoal
                                  {
                                      mTRC_MPBG = g1,
                                      mtrc_prod_display_text = String.IsNullOrEmpty(g3.mtrc_prod_display_text) ? "" : g3.mtrc_prod_display_text,
                                      bldgName = d.dsc_mtrc_lc_bldg_name,
                                      bldgId = d.dsc_mtrc_lc_bldg_id.ToString()
                                  });

                    goalMgmtViewModel.buildingGoalList = query3.ToList();

                    foreach (var mpbg in goalMgmtViewModel.buildingGoalList)
                    {
                        if (mpbg.mTRC_MPBG == null) {
                            mpbg.mTRC_MPBG = new MTRC_MPBG();
                        }
                    }
                }
            }

            return View(goalMgmtViewModel);
        }

        // GET: GoalMgmt/_Details
        public ActionResult _Detail(int? prodId, int? mpId, int? bldgId)
        {
            GoalDetailViewModel goalDetailViewModel = new GoalDetailViewModel();
            prodId = prodId ?? 0;    //Assign a Zero value to Id if it's null
            mpId = mpId ?? 0;
            bldgId = bldgId ?? 0;
            ViewBag.prodId = prodId;
            ViewBag.mpId = mpId;
            ViewBag.bldgId = bldgId;

            if (prodId == 0 || mpId == 0)
            {
                //Display error
            }
            else
            {
                //Display mpg details
                var query = from a in db.MTRC_MPG
                            join b in db.MTRC_PRODUCT on a.prod_id equals b.prod_id
                            join c in db.MTRC_METRIC_PRODUCTS on new { a.prod_id, a.mtrc_period_id } equals new { c.prod_id, c.mtrc_period_id }
                            where b.prod_id == prodId
                                && a.mtrc_period_id == mpId
                            orderby a.mpg_start_eff_dtm descending
                            select new MetricPeriodGoal
                            {
                                mTRC_MPG = a,
                                mtrc_prod_display_text = c.mtrc_prod_display_text,
                                mtrc_period_id = a.mtrc_period_id.ToString()
                            };

                goalDetailViewModel.enterpriseGoalHistory = query.ToList();

                if (bldgId == 0)
                {

                }
                else
                {
                    //Get history of product metric period building goals for the specified prodId, mpId, and bldgId.

                    var query2 = from a in db.MTRC_MPBG
                                 join b in db.MTRC_PRODUCT on a.prod_id equals b.prod_id
                                 join c in db.MTRC_METRIC_PRODUCTS on new { a.prod_id, a.mtrc_period_id } equals new { c.prod_id, c.mtrc_period_id }
                                 join d in db.DSC_MTRC_LC_BLDG on a.dsc_mtrc_lc_bldg_id equals d.dsc_mtrc_lc_bldg_id
                                 where b.prod_id == prodId
                                     && a.mtrc_period_id == mpId
                                     && a.dsc_mtrc_lc_bldg_id == bldgId
                                 orderby a.mpbg_start_eff_dtm descending
                                 select new MetricPeriodBuildingGoal
                                 {
                                     mTRC_MPBG = a,
                                     mtrc_prod_display_text = c.mtrc_prod_display_text,
                                     bldgName = d.dsc_mtrc_lc_bldg_name,
                                     bldgId = d.dsc_mtrc_lc_bldg_id.ToString()
                                 };

                    goalDetailViewModel.buildingGoalHistory = query2.ToList();
                }
            }
            return PartialView(goalDetailViewModel);
        }

        //POST: _Detail
        [HttpPost]
        public string _UserRoleAssign(string raw_json)
        {
            string msg = addEnterpriseGoal(raw_json);

            return msg;
        }

        private string addEnterpriseGoal(string raw_json)
        {
            string msg = "Success";

            try
            {
                //Parse JSON
                JObject parsed_result = JObject.Parse(raw_json);

                // Get parameters from parsed JSON
                // - Metric Period
                // - Prod Id
                // - (Bldg Id)
                // - New interpretted rule
                // - New Eff Start Date
                // - New Eff End Date
                //int mpId = 1;
                //int prodId = 1;
                //double mpg_less_val = 1.00;
                string start = "2016-01-01";
                string end = "2060-12-31";

                DateTime startDtm = DateTime.ParseExact(start, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                DateTime endDtm = DateTime.ParseExact(end, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);



                // Enforce New Eff Start Date as greater than or equal to current month (depends on the time period type).
                // - Return error if violated

                // If New Eff Date range leaves a date range where there is no active enterprise goal, return error

                // If overlaps with a goal with the same rule, return message.

                // Check if datetime conflicts with any future goals
                // - If so, indicate which future goals will be affected and ask user whether it's okay to affect the future records.
                // - If user says No, cancel. If user says Yes, modify effective date or delete record for future records.

                // If overwriting current month, check to see if current goal was used to generate rows in the RZ_MTRC_PERIOD_VAL_GOAL table
                // - If so, return error
                // - Else, overwrite effective end date of current goal. 

                // If never used, overwrite old end effective date. Do this for ALL conflicts (including currently defined goal).

                // Write new record

                // 
                if (msg == "Success")
                {
                    msg = "";
                }
                else
                {
                    //Begin Add Transaction
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {

                            if (ModelState.IsValid)
                            {
                                //db.MTRC_USER_APP_ROLES.Add(addRow);
                            }

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

        public string _addSaveBuildingGoal(string prodId, string mpId, string bldgId, string mpGoal, string startDate, string endDate)
        {
            //Param "mpGoal" is a comma delimited field (Goal Condition, Goal value1, Goal Value2, bool 'include lower limit', bool 'include higuer limit'.
            //Perform parameter Input Validation
            string validationText = "";
            int iprodId = 0;
            int impId = 0;
            int ibldgId = 0;
            String[] mpGoalValues = mpGoal.Split(',');
            DateTime goalStartDate = new DateTime();
            DateTime goalEndDate = new DateTime();
            validationText = Int32.TryParse(prodId, out iprodId) ? "" : "The Product Id Parameter is invalid\n";
            validationText += Int32.TryParse(mpId, out impId) ? "" : "The Metric Period Id Parameter is invalid\n";
            validationText += Int32.TryParse(bldgId, out ibldgId) ? "" : "The Building Id Parameter is invalid\n";
            validationText += DateTime.TryParse(startDate, out goalStartDate) ? "" : "The Effective Goal Start Date is not valid.\n";
            validationText += DateTime.TryParse(endDate, out goalEndDate) ? "" : "The Effective Goal End Date is not valid.\n";
            if (validationText != "") { return validationText; }      //No need to continue if parameters passed are not valid

            //Verify that this building is allowed to override the Enterprise Metric Goal
            string overrideFlag = db.MTRC_MPG.FirstOrDefault(x => x.mtrc_period_id == impId && x.prod_id == iprodId).mpg_allow_bldg_override;
            overrideFlag = overrideFlag ?? "N";
            validationText += overrideFlag.Equals("N") ? "\nThe Selected Metric Cannot be Overriden at the Building Level." : "";

            //Verify tha the Effective Start date of the New goal is valid. It cannot be on or before the last closed period.
            var lastClosedMP = db.RZ_MTRC_PERIOD_STATUS.Include("MTRC_TM_PERIODS").Where(x => x.mtrc_period_id == impId && x.rz_mps_status == "Closed").OrderByDescending(o => o.tm_period_id).FirstOrDefault();
            DateTime earliestGoalDate = DateTime.Today;

            if (lastClosedMP == null) { earliestGoalDate = new DateTime(1900, 1, 1); }  //There are no previously closed periods for this Metric Period. The Start date can be any date after Jan 1, 1900
            earliestGoalDate = lastClosedMP.MTRC_TM_PERIODS.tm_per_end_dtm;
            if (goalStartDate <= earliestGoalDate) {
                validationText += String.Format( "Invalid Goal Start Date.<br/>It Must be after the last closed period's end date:  [{0}]", earliestGoalDate);
            }

            //Verify that a valid goal syntax is received
            if (mpGoalValues.Length < 2)
            {
                validationText += "The Goal Condition Symtax is not valid.\n";
            }
            else {
                mpGoalValues[0] = mpGoalValues[0].Trim().ToUpper();
                if (mpGoalValues[0].Equals("BETWEEN") && mpGoalValues.Length < 5) {
                    validationText += "The Goal Condition Symtax is not valid.\n";
                }
            } 

            if (validationText != "") { return validationText; }     
            // ------------- Stop Here if there are any validation Errors

            string displayText = "";
            double value01 = 0;
            double value02 = -999;
            if (!Double.TryParse(mpGoalValues[1], out value01)){ return "The Specified Value is not valid. Data must be Numeric"; }

            switch (mpGoalValues[0])
            {
                case "IS GREATER THAN":
                    displayText += "> X";
                    break;
                case "IS GREATER THAN OR EQUAL TO":
                    displayText += ">= X";
                    break;
                case "IS LESS THAN":
                    displayText += "< X";
                    break;
                case "IS LESS THAN OR EQUAL TO":
                    displayText += "<= X";
                    break;
                case "IS EQUAL TO":
                    displayText += " = X";
                    break;
                case "BETWEEN":
                    if (!Double.TryParse(mpGoalValues[2], out value02)){ return "The Specified Maximum Value is not valid. Data must be Numeric"; }
                    if (mpGoalValues[3].Equals("true")) {
                        displayText += ">= X";
                    } else {
                        displayText += "> X";
                    }
                    if (mpGoalValues[4].Equals("true")) { displayText += " and <= Y"; }
                    else { displayText += " and < Y"; }
                    break;
                default:
                    validationText += "Goal Condition Syntax is Invalid";
                    break;
            }

            if (validationText != "") { return validationText; }     


            //Retrieve the current Metric Periods Metadata
            MTRC_METRIC_PERIOD currentMP = db.MTRC_METRIC_PERIOD.Find(impId);
            if (currentMP == null) { return "Metric Period No Found in Database."; }

            double minValue = (currentMP.mtrc_period_min_val == null) ? (double)currentMP.MTRC_METRIC.mtrc_min_val : (double)currentMP.mtrc_period_min_val;
            double maxValue = (currentMP.mtrc_period_max_val == null) ? (double)currentMP.MTRC_METRIC.mtrc_max_val : (double)currentMP.mtrc_period_max_val;
            int decimals = (currentMP.mtrc_period_max_dec_places == null) ? (int)currentMP.MTRC_METRIC.mtrc_max_dec_places : (int)currentMP.mtrc_period_max_dec_places;
            string valueTypeToken = currentMP.MTRC_METRIC.MTRC_DATA_TYPE.data_type_token;  //Possible Values: str,char,int,dec,cur,pct

            //Verify that the New Values are withing the accepted Metric or Metric Period Min/Max Range
            if (value01 < minValue || value01 > maxValue ) { return "Specified Goal Value is not within the Acceptable Metric Values Range"; }
            if (value02 != -999 &&  (value02 < minValue || value02 > maxValue) ) { return "One or more of the Specified Goal Values is not within the Acceptable Metric Values Range"; }


            //Retrieve all current MP building goals that are still active and set their end effective dates accordingly then add the new goal record
            List<MTRC_MPBG> activeGoals = db.MTRC_MPBG.Where(x => x.dsc_mtrc_lc_bldg_id == ibldgId && x.mtrc_period_id == impId && x.prod_id == iprodId && x.mpbg_end_eff_dtm > goalStartDate).ToList();
            activeGoals.ForEach(x => x.mpbg_end_eff_dtm = goalStartDate.AddDays(-1));

            MTRC_MPBG newGoal = new MTRC_MPBG
            {
                dsc_mtrc_lc_bldg_id = (short)ibldgId,
                mtrc_period_id = impId,
                prod_id = (short)iprodId,
                mpbg_score = 1,
                mpbg_start_eff_dtm = goalStartDate,
                mpbg_end_eff_dtm = new DateTime(2060, 12, 31)
            };

            //displayText
            string formatingString = "";
            switch(valueTypeToken){
                case "str":
                case "char":
                    displayText = displayText.Replace("X", value01.ToString()).Replace("Y", value02.ToString());                    
                    break;
                case "int":
                    formatingString = "N0";
                    break;
                case "dec":
                    formatingString = "N" + decimals.ToString();
                    break;
                case "cur":
                    formatingString = "C" + decimals.ToString();
                    break;
                case "pct":
                    formatingString = "P" + decimals.ToString();
                    break;
                default:
                    break;            
            }
            displayText = displayText.Replace("X", value01.ToString(formatingString)).Replace("Y", value02.ToString(formatingString));

            newGoal.mpbg_display_text = displayText;

            switch (mpGoalValues[0])
            {
                case "IS GREATER THAN":
                    newGoal.mpbg_greater_val = (decimal)value01;
                    break;
                case "IS GREATER THAN OR EQUAL TO":
                    newGoal.mpbg_greater_eq_val = (decimal)value01;
                    break;
                case "IS LESS THAN":
                    newGoal.mpbg_less_val = (decimal)value01;
                    break;
                case "IS LESS THAN OR EQUAL TO":
                    newGoal.mpbg_less_eq_val = (decimal)value01;
                    break;
                case "IS EQUAL TO":
                    newGoal.mpbg_equal_val = value01.ToString();
                    break;
                case "BETWEEN":
                    if (mpGoalValues[3].Equals("true")) { newGoal.mpbg_greater_eq_val = (decimal)value01; }
                    else { newGoal.mpbg_greater_val = (decimal)value01; }
                    if (mpGoalValues[4].Equals("true")) { newGoal.mpbg_less_eq_val = (decimal)value02; }
                    else { newGoal.mpbg_less_val = (decimal)value02; }
                    break;
                default:
                    validationText += "Goal Condition Syntax is Invalid";
                    break;
            }


            db.MTRC_MPBG.Add(newGoal);
            try { db.SaveChanges(); }
            catch (Exception ex) { return "Failed to Save Changes to Database:\n" + ex.Message; }

            return "Changes Saved Successfully!";    //Debug Checkpoint

            
            //string periodType = lastClosedMP.MTRC_TM_PERIODS.MTRC_TIME_PERIOD_TYPE.tpt_name;
            
           

            //string response = "Save Functionality is not enabled yet: \nProdId:" + prodId + "\nMPid: " + mpId + "\nBuilding Id: " + bldgId;
            //response += "\nGoal: " + mpGoal + "\nFrom: " + goalStartDate.ToString("MMM dd, yyyy") + "\nUntil: " + goalEndDate.ToString("MMM dd, yyyy");
            //return "SUCCESS:Response from Server: " + response + "\nMetric Level Allow Override?: " + overrideFlag;
        }
        

    }
}
