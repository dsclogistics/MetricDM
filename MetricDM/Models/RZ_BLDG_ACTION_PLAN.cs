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
    
    public partial class RZ_BLDG_ACTION_PLAN
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RZ_BLDG_ACTION_PLAN()
        {
            this.RZ_BAP_METRICS = new HashSet<RZ_BAP_METRICS>();
        }
    
        public int rz_bap_id { get; set; }
        public short dsc_mtrc_lc_bldg_id { get; set; }
        public int tm_period_id { get; set; }
        public System.DateTime rz_bap_created_on_dtm { get; set; }
        public string rz_bap_comment { get; set; }
    
        public virtual DSC_MTRC_LC_BLDG DSC_MTRC_LC_BLDG { get; set; }
        public virtual MTRC_TM_PERIODS MTRC_TM_PERIODS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RZ_BAP_METRICS> RZ_BAP_METRICS { get; set; }
    }
}
