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
    
    public partial class RZ_MTRC_PERIOD_VAL_GOAL
    {
        public int rz_mpvg_id { get; set; }
        public long mtrc_period_val_id { get; set; }
        public string rz_mpvg_goal_met_yn { get; set; }
        public System.DateTime rz_mpvg_created_dtm { get; set; }
    
        public virtual MTRC_METRIC_PERIOD_VALUE MTRC_METRIC_PERIOD_VALUE { get; set; }
    }
}
