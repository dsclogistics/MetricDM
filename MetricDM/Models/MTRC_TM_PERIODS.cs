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
    
    public partial class MTRC_TM_PERIODS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MTRC_TM_PERIODS()
        {
            this.MTRC_METRIC_PERIOD_VALUE = new HashSet<MTRC_METRIC_PERIOD_VALUE>();
            this.RZ_MTRC_PERIOD_STATUS = new HashSet<RZ_MTRC_PERIOD_STATUS>();
        }
    
        public int tm_period_id { get; set; }
        public Nullable<short> tpt_id { get; set; }
        public System.DateTime tm_per_start_dtm { get; set; }
        public System.DateTime tm_per_end_dtm { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTRC_METRIC_PERIOD_VALUE> MTRC_METRIC_PERIOD_VALUE { get; set; }
        public virtual MTRC_TIME_PERIOD_TYPE MTRC_TIME_PERIOD_TYPE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RZ_MTRC_PERIOD_STATUS> RZ_MTRC_PERIOD_STATUS { get; set; }
    }
}