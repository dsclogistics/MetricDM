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

            prodId = (prodId == null) ? 0 : prodId;    //Assign a Zero value to Id if it's null
            mpId = (mpId == null) ? 0 : mpId;    //Assign a Zero value to Id if it's null

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
            prodId = (prodId == null) ? 0 : prodId;    //Assign a Zero value to Id if it's null
            mpId = (mpId == null) ? 0 : mpId;
            bldgId = (bldgId == null) ? 0 : bldgId;
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
                                mtrc_prod_display_text = c.mtrc_prod_display_text
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

    }
}
