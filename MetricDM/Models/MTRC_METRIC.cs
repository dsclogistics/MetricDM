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
    
    public partial class MTRC_METRIC
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MTRC_METRIC()
        {
            this.MTRC_METRIC_PERIOD = new HashSet<MTRC_METRIC_PERIOD>();
        }
    
        public int mtrc_id { get; set; }
        public short data_type_id { get; set; }
        public string mtrc_name { get; set; }
        public string mtrc_token { get; set; }
        public string mtrc_desc { get; set; }
        public System.DateTime mtrc_eff_start_dt { get; set; }
        public System.DateTime mtrc_eff_end_dt { get; set; }
        public Nullable<decimal> mtrc_min_val { get; set; }
        public Nullable<decimal> mtrc_max_val { get; set; }
        public Nullable<short> mtrc_max_dec_places { get; set; }
        public Nullable<short> mtrc_max_str_size { get; set; }
        public string mtrc_na_allow_yn { get; set; }
    
        public virtual MTRC_DATA_TYPE MTRC_DATA_TYPE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTRC_METRIC_PERIOD> MTRC_METRIC_PERIOD { get; set; }
    }
}
