//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MetricDM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MTRC_MPG
    {
        public int mpg_id { get; set; }
        public int mtrc_period_id { get; set; }
        public short prod_id { get; set; }
        public Nullable<decimal> mpg_less_val { get; set; }
        public Nullable<decimal> mpg_less_eq_val { get; set; }
        public Nullable<decimal> mpg_greater_val { get; set; }
        public Nullable<decimal> mpg_greater_eq_val { get; set; }
        public string mpg_equal_val { get; set; }
        public int mpg_score { get; set; }
        public string mpg_display_text { get; set; }
        public string mpg_allow_bldg_override { get; set; }
        public System.DateTime mpg_start_eff_dtm { get; set; }
        public System.DateTime mpg_end_eff_dtm { get; set; }
    
        public virtual MTRC_METRIC_PERIOD MTRC_METRIC_PERIOD { get; set; }
        public virtual MTRC_PRODUCT MTRC_PRODUCT { get; set; }
    }
}
