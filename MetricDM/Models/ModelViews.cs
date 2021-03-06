﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MetricDM.Models
{
    //Metadata for MTRC_METRIC
    [MetadataType(typeof(MTRC_METRICMetaData))]
    public partial class MTRC_METRIC { }

    public class MTRC_METRICMetaData
    {
        [Display(Name = "Metric Id")]
        public int mtrc_id { get; set; }
        [Display(Name = "Data Type")]
        public short data_type_id { get; set; }
        [Required(ErrorMessage = "The Metric Name is Mandatory!")][Display(Name = "Metric Name")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        public string mtrc_name { get; set; }
        [Display(Name = "Token")]
        public string mtrc_token { get; set; }
        [Display(Name = "Description")] [Required] [DataType(DataType.MultilineText)]
        public string mtrc_desc { get; set; }
        [Display(Name = "Effective Start Date")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime mtrc_eff_start_dt { get; set; }
        [Display(Name = "Effective End Date")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime mtrc_eff_end_dt { get; set; }
        [Display(Name = "Minimum Value")]
        public Nullable<decimal> mtrc_min_val { get; set; }
        [Display(Name = "Maximum Value")]
        public Nullable<decimal> mtrc_max_val { get; set; }
        [Display(Name = "Maximum Decimal Places")]
        public Nullable<short> mtrc_max_dec_places { get; set; }
        [Display(Name = "Maximum String Size")]
        public Nullable<short> mtrc_max_str_size { get; set; }
        [Display(Name = "N/A Allowed (Y/N)")]
        public string mtrc_na_allow_yn { get; set; }
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    }

    //Metadata for MTRC_METRIC_PERIOD
    [MetadataType(typeof(MTRC_METRIC_PERIODMetaData))]
    public partial class MTRC_METRIC_PERIOD { }

    public class MTRC_METRIC_PERIODMetaData
    {
        [Display(Name = "Metric Period")]
        public int mtrc_period_id { get; set; }
        [Display(Name = "Metric Id")]
        [Required]
        public int mtrc_id { get; set; }
        [Display(Name = "Time Period")]
        [Required]
        public short tpt_id { get; set; }
        [Display(Name = "Metric Period Name")]
        [Required]
        public string mtrc_period_name { get; set; }
        [Display(Name = "Token")]
        public string mtrc_period_token { get; set; }
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string mtrc_period_desc { get; set; }
        [Display(Name = "Calc (Y/N)")]
        [Required]
        public string mtrc_period_calc_yn { get; set; }
        [Display(Name = "Min Val")]
        public Nullable<decimal> mtrc_period_min_val { get; set; }
        [Display(Name = "Max Val")]
        public Nullable<decimal> mtrc_period_max_val { get; set; }
        [Display(Name = "Max Dec Places")]
        public Nullable<short> mtrc_period_max_dec_places { get; set; }
        [Display(Name = "Max String Size")]
        public Nullable<short> mtrc_period_max_str_size { get; set; }
        [Display(Name = "N/A Allowed")]
        [Required(ErrorMessage = "The field is mandatory!")]
        [StringLength(1, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string mtrc_period_na_allow_yn { get; set; }
        [Display(Name = "Can Import Y/N")]
        [Required(ErrorMessage = "The field is mandatory!")]
        [StringLength(1, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string mtrc_period_can_import_yn { get; set; }
        [Display(Name = "Auto Y/N")]
        [Required(ErrorMessage = "The field is mandatory!")]
        [StringLength(1, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string mtrc_period_is_auto_yn { get; set; }
    }

    //Metadata for MTRC_METRIC_PERIOD_TYPE
    [MetadataType(typeof(MTRC_TIME_PERIOD_TYPEMetaData))]
    public partial class MTRC_TIME_PERIOD_TYPE { }

    public class MTRC_TIME_PERIOD_TYPEMetaData
    {
        [Display(Name = "Time Period Id")]
        public short tpt_id { get; set; }
        [Display(Name = "Time Period Name")]
        public string tpt_name { get; set; }
    }

    //Metadata for DSC_MTRC_LC_BLDG
    [MetadataType(typeof(DSC_MTRC_LC_BLDGMetaData))]
    public partial class DSC_MTRC_LC_BLDG { }

    public class DSC_MTRC_LC_BLDGMetaData
    {
        [Display(Name = "Building Id")]
        public short dsc_mtrc_lc_bldg_id { get; set; }
        [Display(Name = "Logistics Center")]
        public int dsc_lc_id { get; set; }
        [Required(ErrorMessage = "The Building Name is Mandatory!")]
        [Display(Name = "Building Name")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        public string dsc_mtrc_lc_bldg_name { get; set; }
        [Display(Name = "Building Code")]
        public string dsc_mtrc_lc_bldg_code { get; set; }
        [Display(Name = "Effective Start Date")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime dsc_mtrc_lc_bldg_eff_start_dt { get; set; }
        [Display(Name = "Effective End Date")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime dsc_mtrc_lc_bldg_eff_end_dt { get; set; }
    }

    [MetadataType(typeof(MTRC_BLDG_MTRC_PERIODMetaData))]
    public partial class MTRC_BLDG_MTRC_PERIOD { }

    public class MTRC_BLDG_MTRC_PERIODMetaData
    {
        [Display(Name = "Building Metric Period Id")]
        public int bmp_id { get; set; }
        [Display(Name = "Data Source")]
        public Nullable<short> data_src_id { get; set; }
        [Display(Name = "Building")]
        public Nullable<short> dsc_mtrc_lc_bldg_id { get; set; }
        [Display(Name = "Metric Period")]
        public int mtrc_period_id { get; set; }
        [Display(Name = "Is Editable (Y/N)")]
        public string bmp_is_editable_yn { get; set; }
        [Display(Name = "Is Manual (Y/N)")]
        public string bmp_is_manual_yn { get; set; }
        [Display(Name = "N/A Allowed (Y/N)")]
        public string bmp_na_allow_yn { get; set; }
    }

}