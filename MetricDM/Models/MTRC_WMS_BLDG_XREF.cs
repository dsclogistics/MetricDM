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
    
    public partial class MTRC_WMS_BLDG_XREF
    {
        public short dsc_mtrc_lc_bldg_id { get; set; }
        public string bldg_xref_lc_code { get; set; }
        public string lc_bldg_physical_id { get; set; }
        public System.DateTime bldg_xref_eff_start_dt { get; set; }
        public System.DateTime bldg_xref_eff_end_dt { get; set; }
        public string bldg_xref_invalid_yn { get; set; }
    
        public virtual DSC_MTRC_LC_BLDG DSC_MTRC_LC_BLDG { get; set; }
    }
}
