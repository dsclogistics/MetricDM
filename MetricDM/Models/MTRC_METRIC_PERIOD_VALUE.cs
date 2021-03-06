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
    
    public partial class MTRC_METRIC_PERIOD_VALUE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MTRC_METRIC_PERIOD_VALUE()
        {
            this.RZ_MTRC_PERIOD_VAL_GOAL = new HashSet<RZ_MTRC_PERIOD_VAL_GOAL>();
            this.RZ_BAP_METRICS = new HashSet<RZ_BAP_METRICS>();
            this.MTRC_MPV_REASONS = new HashSet<MTRC_MPV_REASONS>();
        }
    
        public long mtrc_period_val_id { get; set; }
        public int mtrc_period_id { get; set; }
        public short dsc_mtrc_lc_bldg_id { get; set; }
        public int tm_period_id { get; set; }
        public System.DateTime mtrc_period_val_added_dtm { get; set; }
        public string mtrc_period_val_added_by_usr_id { get; set; }
        public Nullable<System.DateTime> mtrc_period_val_upd_dtm { get; set; }
        public string mtrc_period_val_upd_by_user_id { get; set; }
        public string mtrc_period_val_is_na_yn { get; set; }
        public string mtrc_period_val_value { get; set; }
    
        public virtual DSC_MTRC_LC_BLDG DSC_MTRC_LC_BLDG { get; set; }
        public virtual MTRC_METRIC_PERIOD MTRC_METRIC_PERIOD { get; set; }
        public virtual MTRC_TM_PERIODS MTRC_TM_PERIODS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RZ_MTRC_PERIOD_VAL_GOAL> RZ_MTRC_PERIOD_VAL_GOAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RZ_BAP_METRICS> RZ_BAP_METRICS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTRC_MPV_REASONS> MTRC_MPV_REASONS { get; set; }
    }
}
