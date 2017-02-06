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
    public class BuildingController : Controller
    {
        private DSC_MTRC_DEVEntities db = new DSC_MTRC_DEVEntities();

        // GET: DSC_BUILDING
        public ActionResult Index()
        {
            var dSC_MTRC_LC_BLDG = db.DSC_MTRC_LC_BLDG.Include(d => d.DSC_LC);
            return View(dSC_MTRC_LC_BLDG.ToList());
        }

        // GET: DSC_BUILDING/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_MTRC_LC_BLDG dSC_MTRC_LC_BLDG = db.DSC_MTRC_LC_BLDG.Find(id);
            if (dSC_MTRC_LC_BLDG == null)
            {
                return HttpNotFound();
            }
            return View(dSC_MTRC_LC_BLDG);
        }


        // GET: DSC_BUILDING/Create
        public ActionResult Create()
        {
            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name");
            return View();
        }

        // POST: DSC_BUILDING/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "dsc_mtrc_lc_bldg_id,dsc_lc_id,dsc_mtrc_lc_bldg_name,dsc_mtrc_lc_bldg_code,dsc_mtrc_lc_bldg_eff_start_dt,dsc_mtrc_lc_bldg_eff_end_dt")] DSC_MTRC_LC_BLDG dSC_MTRC_LC_BLDG)
        {
            if (ModelState.IsValid)
            {
                db.DSC_MTRC_LC_BLDG.Add(dSC_MTRC_LC_BLDG);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", dSC_MTRC_LC_BLDG.dsc_lc_id);
            return View(dSC_MTRC_LC_BLDG);
        }

        // GET: DSC_BUILDING/Edit/5
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_MTRC_LC_BLDG dSC_MTRC_LC_BLDG = db.DSC_MTRC_LC_BLDG.Find(id);
            if (dSC_MTRC_LC_BLDG == null)
            {
                return HttpNotFound();
            }
            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", dSC_MTRC_LC_BLDG.dsc_lc_id);
            return View(dSC_MTRC_LC_BLDG);
        }

        // POST: DSC_BUILDING/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "dsc_mtrc_lc_bldg_id,dsc_lc_id,dsc_mtrc_lc_bldg_name,dsc_mtrc_lc_bldg_code,dsc_mtrc_lc_bldg_eff_start_dt,dsc_mtrc_lc_bldg_eff_end_dt")] DSC_MTRC_LC_BLDG dSC_MTRC_LC_BLDG)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dSC_MTRC_LC_BLDG).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", dSC_MTRC_LC_BLDG.dsc_lc_id);
            return View(dSC_MTRC_LC_BLDG);
        }

        // GET: DSC_BUILDING/Delete/5
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_MTRC_LC_BLDG dSC_MTRC_LC_BLDG = db.DSC_MTRC_LC_BLDG.Find(id);
            if (dSC_MTRC_LC_BLDG == null)
            {
                return HttpNotFound();
            }
            return View(dSC_MTRC_LC_BLDG);
        }

        // POST: DSC_BUILDING/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            DSC_MTRC_LC_BLDG dSC_MTRC_LC_BLDG = db.DSC_MTRC_LC_BLDG.Find(id);
            db.DSC_MTRC_LC_BLDG.Remove(dSC_MTRC_LC_BLDG);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //============================================================================================================
        // GET: /Building/BuildingMaintenance/5
        public ActionResult BuildingMaintenance(int? id)
        {
            id = (id == null) ? 0 : id;    //Assign a Zero value to Id if it's null
            ViewBag.building_sel_list = new SelectList(db.DSC_MTRC_LC_BLDG, "dsc_mtrc_lc_bldg_id", "dsc_mtrc_lc_bldg_name", id);

            //Load all the Metrics into a Drop down List for Selection
            DSC_MTRC_LC_BLDG selectedBuilding;
            if (id == 0) { selectedBuilding = null; }
            else
            {
                selectedBuilding = db.DSC_MTRC_LC_BLDG.Find(id);
                ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", selectedBuilding.dsc_lc_id);
            }
            //if (selectedMetric == null)
            //{
            //    throw new Exception("The selected Metric does not exist");
            //}
            return View(selectedBuilding);
        }

        //============================================================================================================
        // GET: /MetricPeriod/_buildingMetricPeriodList/
        public PartialViewResult _buildingMetricPeriodList(int id)
        {
            List<MTRC_BLDG_MTRC_PERIOD> buildingMetricPeriods = db.MTRC_BLDG_MTRC_PERIOD.Where(x => x.dsc_mtrc_lc_bldg_id == id).ToList();

            return PartialView(buildingMetricPeriods);
        }
        //============================================================================================================
        // GET: /MetricPeriod/_buildingMetricPeriodDetails/
        //[ChildActionOnly]
        public PartialViewResult _buildingMetricPeriodDetails(int? id, int? bldgId)
        {
            MTRC_BLDG_MTRC_PERIOD bldgMetricPeriod;
            DSC_MTRC_LC_BLDG bldg;
            if (id == null)
            {
                if (bldgId == null)
                {
                    bldgMetricPeriod = null;
                }
                else
                {
                    //If creating a new building metric period, populate building id and that's it.
                    bldgMetricPeriod = new MTRC_BLDG_MTRC_PERIOD();
                    bldg = db.DSC_MTRC_LC_BLDG.Find(bldgId);
                    bldgMetricPeriod.dsc_mtrc_lc_bldg_id = bldg.dsc_mtrc_lc_bldg_id;
                }

                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                ViewBag.data_src_id = new SelectList(db.MTRC_DATA_SRC, "data_src_id", "data_src_name");
                ViewBag.mtrc_period_id = new SelectList(db.MTRC_METRIC_PERIOD, "mtrc_period_id", "mtrc_period_name");
                ViewBag.createNewBuildingMetricPeriod = true;
            }
            else
            {
                bldgMetricPeriod = db.MTRC_BLDG_MTRC_PERIOD.Find(id);

                ViewBag.data_src_id = new SelectList(db.MTRC_DATA_SRC, "data_src_id", "data_src_name", bldgMetricPeriod.data_src_id);
                ViewBag.mtrc_period_id = new SelectList(db.MTRC_METRIC_PERIOD, "mtrc_period_id", "mtrc_period_name", bldgMetricPeriod.mtrc_period_id);
                ViewBag.createNewBuildingMetricPeriod = false;
            }

            ViewBag.dsc_mtrc_lc_bldg_id = new SelectList(db.DSC_MTRC_LC_BLDG, "dsc_mtrc_lc_bldg_id", "dsc_mtrc_lc_bldg_name", bldgMetricPeriod.dsc_mtrc_lc_bldg_id);

            return PartialView(bldgMetricPeriod);
        }
        //============================================================================================================
        // POST: /Building/_buildingMetricPeriodDetails
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public PartialViewResult _buildingMetricPeriodDetails(MTRC_BLDG_MTRC_PERIOD bldgMetricPeriod)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bldgMetricPeriod).State = EntityState.Modified;
                db.SaveChanges();

                //ViewBag.data_src_id = new SelectList(db.MTRC_DATA_SRC, "data_src_id", "data_src_name", bldgMetricPeriod.data_src_id);
                //ViewBag.mtrc_period_id = new SelectList(db.MTRC_METRIC_PERIOD, "mtrc_period_id", "mtrc_period_name", bldgMetricPeriod.mtrc_period_id);
                //ViewBag.createNewMetricPeriod = false;
                //return PartialView(bldgMetricPeriod);
            }
            ViewBag.data_src_id = new SelectList(db.MTRC_DATA_SRC, "data_src_id", "data_src_name", bldgMetricPeriod.data_src_id);
            ViewBag.mtrc_period_id = new SelectList(db.MTRC_METRIC_PERIOD, "mtrc_period_id", "mtrc_period_name", bldgMetricPeriod.mtrc_period_id);
            ViewBag.dsc_mtrc_lc_bldg_id = new SelectList(db.DSC_MTRC_LC_BLDG, "dsc_mtrc_lc_bldg_id", "dsc_mtrc_lc_bldg_name", bldgMetricPeriod.dsc_mtrc_lc_bldg_id);
            ViewBag.createNewMetricPeriod = false;
            return PartialView(bldgMetricPeriod);
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
