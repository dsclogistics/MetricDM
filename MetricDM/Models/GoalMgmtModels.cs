using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MetricDM.Models
{
    // Functionality and metadata extension for MTRC_MPG
    public class MTRC_MPGMetaData
    {
        [Display(Name = "Metric Period Goal Id")]
        public int mpg_id { get; set; }
        public int mtrc_period_id { get; set; }
        public short prod_id { get; set; }
        public Nullable<decimal> mpg_less_val { get; set; }
        public Nullable<decimal> mpg_less_eq_val { get; set; }
        public Nullable<decimal> mpg_greater_val { get; set; }
        public Nullable<decimal> mpg_greater_eq_val { get; set; }
        public string mpg_equal_val { get; set; }
        public int mpg_score { get; set; }
        [Display(Name = "Display Text")]
        public string mpg_display_text { get; set; }
        [Display(Name = "Allow Building Override (Y/N)")]
        public string mpg_allow_bldg_override { get; set; }
        [Display(Name = "Effective Start Datetime")]
        public System.DateTime mpg_start_eff_dtm { get; set; }
        [Display(Name = "Effective End Datetime")]
        public System.DateTime mpg_end_eff_dtm { get; set; }
    }
    [MetadataType(typeof(MTRC_MPGMetaData))]
    public partial class MTRC_MPG {
        public MTRC_MPG()
        {

        }
    }

    // Functionality and metadata extension for MTRC_MPBG
    public class MTRC_MPBGMetaData
    {
        public int mpbg_id { get; set; }
        public short dsc_mtrc_lc_bldg_id { get; set; }
        public int mtrc_period_id { get; set; }
        public short prod_id { get; set; }
        public Nullable<decimal> mpbg_less_val { get; set; }
        public Nullable<decimal> mpbg_less_eq_val { get; set; }
        public Nullable<decimal> mpbg_greater_val { get; set; }
        public Nullable<decimal> mpbg_greater_eq_val { get; set; }
        public string mpbg_equal_val { get; set; }
        public int mpbg_score { get; set; }
        [Display(Name = "Display Text")]
        public string mpbg_display_text { get; set; }
        [Display(Name = "Effective Start Datetime")]
        public System.DateTime mpbg_start_eff_dtm { get; set; }
        [Display(Name = "Effective End Datetime")]
        public System.DateTime mpbg_end_eff_dtm { get; set; }
    }
    [MetadataType(typeof(MTRC_MPBGMetaData))]
    public partial class MTRC_MPBG { }

    // Model for enterprise level Metric Period Goals
    public class MetricPeriodGoalMetaData
    {
        [Display(Name = "Metric")]
        public string mtrc_prod_display_text { get; set; }
        [Display(Name = "Rule")]
        public string displayRule { get; set; }
        [Display(Name = "Start Datetime")]
        public string startDtm { get; set; }
        [Display(Name = "End Datetime")]
        public string endDtm { get; set; }
    }

    [MetadataType(typeof(MetricPeriodGoalMetaData))]
    public class MetricPeriodGoal
    {
        public MTRC_MPG mTRC_MPG { get; set; }
        public string mtrc_period_id { get; set; }
        public string mtrc_prod_display_text { get; set; }
        public string displayRule
        {
            //Displays the text interpretation of the rules contained within this Metric Period Goal record
            get
            {
                string returnVal = "";

                if (this.mTRC_MPG.mpg_less_val != null)
                {
                    if (this.mTRC_MPG.mpg_less_eq_val == null && this.mTRC_MPG.mpg_greater_val == null && this.mTRC_MPG.mpg_greater_eq_val == null && this.mTRC_MPG.mpg_equal_val == null)
                    {
                        returnVal = "x < " + this.mTRC_MPG.mpg_less_val;
                    }
                    else if (this.mTRC_MPG.mpg_greater_val != null && this.mTRC_MPG.mpg_less_eq_val == null && this.mTRC_MPG.mpg_greater_eq_val == null && this.mTRC_MPG.mpg_equal_val == null)
                    {
                        returnVal = this.mTRC_MPG.mpg_greater_val + " < x < " + this.mTRC_MPG.mpg_less_val;
                    }
                    else if (this.mTRC_MPG.mpg_greater_eq_val != null && this.mTRC_MPG.mpg_less_eq_val == null && this.mTRC_MPG.mpg_greater_val == null && this.mTRC_MPG.mpg_equal_val == null)
                    {
                        returnVal = this.mTRC_MPG.mpg_greater_eq_val + " <= x < " + this.mTRC_MPG.mpg_less_val;
                    }
                    else
                    {
                        returnVal = "Invalid Rule";
                    }
                }
                else if (this.mTRC_MPG.mpg_less_eq_val != null)
                {
                    if (this.mTRC_MPG.mpg_less_val == null && this.mTRC_MPG.mpg_greater_val == null && this.mTRC_MPG.mpg_greater_eq_val == null && this.mTRC_MPG.mpg_equal_val == null)
                    {
                        returnVal = "x <= " + this.mTRC_MPG.mpg_less_eq_val;
                    }
                    else if (this.mTRC_MPG.mpg_greater_val != null && this.mTRC_MPG.mpg_less_val == null && this.mTRC_MPG.mpg_greater_eq_val == null && this.mTRC_MPG.mpg_equal_val == null)
                    {
                        returnVal = this.mTRC_MPG.mpg_greater_val + "< x <= " + this.mTRC_MPG.mpg_less_eq_val;
                    }
                    else if (this.mTRC_MPG.mpg_greater_eq_val != null && this.mTRC_MPG.mpg_less_val == null && this.mTRC_MPG.mpg_greater_val == null && this.mTRC_MPG.mpg_equal_val == null)
                    {
                        returnVal = this.mTRC_MPG.mpg_greater_eq_val + " <= x <= " + this.mTRC_MPG.mpg_less_eq_val;
                    }
                    else
                    {
                        returnVal = "Invalid Rule";
                    }
                }
                else if (this.mTRC_MPG.mpg_greater_val != null)
                {
                    if (this.mTRC_MPG.mpg_less_val == null && this.mTRC_MPG.mpg_less_eq_val == null && this.mTRC_MPG.mpg_greater_eq_val == null && this.mTRC_MPG.mpg_equal_val == null)
                    {
                        returnVal = "x > " + this.mTRC_MPG.mpg_greater_val;
                    }
                    else
                    {
                        returnVal = "Invalid Rule";
                    }
                }
                else if (this.mTRC_MPG.mpg_greater_eq_val != null)
                {
                    if (this.mTRC_MPG.mpg_less_val == null && this.mTRC_MPG.mpg_less_eq_val == null && this.mTRC_MPG.mpg_greater_val == null && this.mTRC_MPG.mpg_equal_val == null)
                    {
                        returnVal = "x >= " + this.mTRC_MPG.mpg_greater_eq_val;
                    }
                    else
                    {
                        returnVal = "Invalid Rule";
                    }
                }
                else if (this.mTRC_MPG.mpg_equal_val != null)
                {
                    if (this.mTRC_MPG.mpg_less_val == null && this.mTRC_MPG.mpg_less_eq_val == null && this.mTRC_MPG.mpg_greater_val == null && this.mTRC_MPG.mpg_equal_val == null)
                    {
                        returnVal = "x = " + this.mTRC_MPG.mpg_equal_val;
                    }
                    else
                    {
                        returnVal = "Invalid Rule";
                    }
                }
                else
                {
                    returnVal = "Invalid Rule";
                }
                return returnVal;
            }
        }
        public string startDtm {
            get
            {
                string returnString = this.mTRC_MPG.mpg_start_eff_dtm.ToString();
                if (returnString == "1/1/0001 12:00:00 AM")
                {
                    returnString = "-";
                }
                return returnString;
            }
        }
        public string endDtm
        {
            get
            {
                string returnString = this.mTRC_MPG.mpg_end_eff_dtm.ToString();
                if (returnString == "12/31/2060 12:00:00 AM")
                {
                    returnString = "-";
                }
                else if (returnString == "1/1/0001 12:00:00 AM")
                {
                    returnString = "-";
                }
                return returnString;
            }
        }

        public MetricPeriodGoal()
        {
            mTRC_MPG = new MTRC_MPG();
        }
    }

    // Model for building level Metric Period Goals
    public class MetricPeriodBuildingGoalMetaData
    {
        [Display(Name = "Metric")]
        public string mtrc_prod_display_text { get; set; }
        [Display(Name = "Building")]
        public string bldgName { get; set; }
        [Display(Name = "Rule")]
        public string displayRule { get; set; }
        [Display(Name = "Start Datetime")]
        public string startDtm { get; set; }
        [Display(Name = "End Datetime")]
        public string endDtm { get; set; }
    }
    [MetadataType(typeof(MetricPeriodBuildingGoalMetaData))]
    public class    MetricPeriodBuildingGoal
    {
        public MTRC_MPBG mTRC_MPBG { get; set; }
        public string mtrc_prod_display_text { get; set; }
        public string bldgName { get; set; }
        public string bldgId { get; set; }
        public string displayRule
        {
            //Displays the text interpretation of the rules contained within this Metric Period Goal record
            get
            {
                string returnVal = "";

                if (this.mTRC_MPBG.mpbg_less_val != null)
                {
                    if (this.mTRC_MPBG.mpbg_less_eq_val == null && this.mTRC_MPBG.mpbg_greater_val == null && this.mTRC_MPBG.mpbg_greater_eq_val == null && this.mTRC_MPBG.mpbg_equal_val == null)
                    {
                        returnVal = "x < " + this.mTRC_MPBG.mpbg_less_val;
                    }
                    else if (this.mTRC_MPBG.mpbg_greater_val != null && this.mTRC_MPBG.mpbg_less_eq_val == null && this.mTRC_MPBG.mpbg_greater_eq_val == null && this.mTRC_MPBG.mpbg_equal_val == null)
                    {
                        returnVal = this.mTRC_MPBG.mpbg_greater_val + " < x < " + this.mTRC_MPBG.mpbg_less_val;
                    }
                    else if (this.mTRC_MPBG.mpbg_greater_eq_val != null && this.mTRC_MPBG.mpbg_less_eq_val == null && this.mTRC_MPBG.mpbg_greater_val == null && this.mTRC_MPBG.mpbg_equal_val == null)
                    {
                        returnVal = this.mTRC_MPBG.mpbg_greater_eq_val + " <= x < " + this.mTRC_MPBG.mpbg_less_val;
                    }
                    else
                    {
                        returnVal = "Invalid Rule";
                    }
                }
                else if (this.mTRC_MPBG.mpbg_less_eq_val != null)
                {
                    if (this.mTRC_MPBG.mpbg_less_val == null && this.mTRC_MPBG.mpbg_greater_val == null && this.mTRC_MPBG.mpbg_greater_eq_val == null && this.mTRC_MPBG.mpbg_equal_val == null)
                    {
                        returnVal = "x <= " + this.mTRC_MPBG.mpbg_less_eq_val;
                    }
                    else if (this.mTRC_MPBG.mpbg_greater_val != null && this.mTRC_MPBG.mpbg_less_val == null && this.mTRC_MPBG.mpbg_greater_eq_val == null && this.mTRC_MPBG.mpbg_equal_val == null)
                    {
                        returnVal = this.mTRC_MPBG.mpbg_greater_val + "< x <= " + this.mTRC_MPBG.mpbg_less_eq_val;
                    }
                    else if (this.mTRC_MPBG.mpbg_greater_eq_val != null && this.mTRC_MPBG.mpbg_less_val == null && this.mTRC_MPBG.mpbg_greater_val == null && this.mTRC_MPBG.mpbg_equal_val == null)
                    {
                        returnVal = this.mTRC_MPBG.mpbg_greater_eq_val + " <= x <= " + this.mTRC_MPBG.mpbg_less_eq_val;
                    }
                    else
                    {
                        returnVal = "Invalid Rule";
                    }
                }
                else if (this.mTRC_MPBG.mpbg_greater_val != null)
                {
                    if (this.mTRC_MPBG.mpbg_less_val == null && this.mTRC_MPBG.mpbg_less_eq_val == null && this.mTRC_MPBG.mpbg_greater_eq_val == null && this.mTRC_MPBG.mpbg_equal_val == null)
                    {
                        returnVal = "x > " + this.mTRC_MPBG.mpbg_greater_val;
                    }
                    else
                    {
                        returnVal = "Invalid Rule";
                    }
                }
                else if (this.mTRC_MPBG.mpbg_greater_eq_val != null)
                {
                    if (this.mTRC_MPBG.mpbg_less_val == null && this.mTRC_MPBG.mpbg_less_eq_val == null && this.mTRC_MPBG.mpbg_greater_val == null && this.mTRC_MPBG.mpbg_equal_val == null)
                    {
                        returnVal = "x >= " + this.mTRC_MPBG.mpbg_greater_eq_val;
                    }
                    else
                    {
                        returnVal = "Invalid Rule";
                    }
                }
                else if (this.mTRC_MPBG.mpbg_equal_val != null)
                {
                    if (this.mTRC_MPBG.mpbg_less_val == null && this.mTRC_MPBG.mpbg_less_eq_val == null && this.mTRC_MPBG.mpbg_greater_val == null && this.mTRC_MPBG.mpbg_equal_val == null)
                    {
                        returnVal = "x = " + this.mTRC_MPBG.mpbg_equal_val;
                    }
                    else
                    {
                        returnVal = "Invalid Rule";
                    }
                }
                else if(this.mTRC_MPBG.mpbg_less_val == null && this.mTRC_MPBG.mpbg_less_eq_val == null && this.mTRC_MPBG.mpbg_greater_val == null && this.mTRC_MPBG.mpbg_greater_eq_val == null && this.mTRC_MPBG.mpbg_equal_val == null)
                {
                    returnVal = "Enterprise Default";
                }
                else
                {
                    returnVal = "Invalid Rule";
                }
                return returnVal;
            }
        }
        public string startDtm
        {
            get
            {
                string returnString = this.mTRC_MPBG.mpbg_start_eff_dtm.ToString();
                if (returnString == "1/1/0001 12:00:00 AM")
                {
                    returnString = "-";
                }
                return returnString;
            }
        }
        public string endDtm
        {
            get
            {
                string returnString = this.mTRC_MPBG.mpbg_end_eff_dtm.ToString();
                if (returnString == "12/31/2060 12:00:00 AM")
                {
                    returnString = "-";
                }
                else if(returnString == "1/1/0001 12:00:00 AM")
                {
                    returnString = "-";
                }
                return returnString;
            }
        }

        public MetricPeriodBuildingGoal()
        {
            mTRC_MPBG = new MTRC_MPBG();
        }
    }

    // Goal Management View Model
    public class GoalMgmtViewModel
    {
        public MetricPeriodGoal enterpriseGoal { get; set; }
        public List<MetricPeriodBuildingGoal> buildingGoalList { get; set; }
        public List<MetricPeriodGoal> enterpriseGoalList { get; set; }

        public GoalMgmtViewModel(){
            enterpriseGoal = new MetricPeriodGoal();
            buildingGoalList = new List<MetricPeriodBuildingGoal>();
            enterpriseGoalList = new List<MetricPeriodGoal>();
        }
    }

    public class GoalDetailViewModel
    {
        public MetricPeriodGoal enterpriseGoal { get; set; }
        public List<MetricPeriodGoal> enterpriseGoalHistory { get; set; }
        public List<MetricPeriodBuildingGoal> buildingGoalHistory { get; set; }

        public GoalDetailViewModel()
        {
            enterpriseGoal = new MetricPeriodGoal();
            enterpriseGoalHistory = new List<MetricPeriodGoal>();
            buildingGoalHistory = new List<MetricPeriodBuildingGoal>();
        }
    }
}