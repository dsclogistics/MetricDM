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
    
    public partial class MTRC_DATA_SRC
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MTRC_DATA_SRC()
        {
            this.MTRC_BLDG_MTRC_PERIOD = new HashSet<MTRC_BLDG_MTRC_PERIOD>();
        }
    
        public short data_src_id { get; set; }
        public string data_src_name { get; set; }
        public string data_src_desc { get; set; }
        public string data_src_type { get; set; }
        public string data_src_conn_str { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTRC_BLDG_MTRC_PERIOD> MTRC_BLDG_MTRC_PERIOD { get; set; }
    }
}
