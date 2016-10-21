using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MetricDM.Models
{
    //Metadata for MTRC_METRIC
    [MetadataType(typeof(DSC_APP_USERMetaData))]
    public partial class DSC_APP_USER { }

    public class DSC_APP_USERMetaData
    {
        [Display(Name = "App User Id")]
        public int app_user_id { get; set; }
        [Display(Name = "Employee Id")]
        public Nullable<int> dsc_emp_id { get; set; }
        [Display(Name = "SSO Id")]
        public string app_user_sso_id { get; set; }
        [Display(Name = "SSO System")]
        public string app_user_sso_system { get; set; }
        [Display(Name = "Email")]
        public string app_user_email_addr { get; set; }
        [Display(Name = "Full Name")]
        public string app_user_full_name { get; set; }
        [Display(Name = "Disabled (Y/N)")]
        public string app_user_disabled_yn { get; set; }
        [Display(Name = "Disabled Datetime")]
        public Nullable<System.DateTime> app_user_disabled_on_dtm { get; set; }

    }




    public class UserMgmtViewModel
    {
        public DSC_APP_USER appUserDetails { get; set; }
        public List<DSC_MTRC_LC_BLDG> userBldgList { get; set; }
        public List<UserAppProduct> userProductRoleList { get; set; }

        public UserMgmtViewModel()
        {
            appUserDetails = new DSC_APP_USER();
            userBldgList = new List<DSC_MTRC_LC_BLDG>();
        }

    }


    public class UserAppProduct
    {
        public string productId { get; set; }
        public string productName { get; set; }
        public List<UserAppRole> userRoles { get; set; }

        public UserAppProduct()
        {
            userRoles = new List<UserAppRole>();
        }
    }

    public class UserAppRole
    {
        public string userAppRoleId { get; set; }
        public string appRoleId { get; set; }
        public string appRoleName { get; set; }
        public string appRoleDesc { get; set; }
        public string reqBldgAuth { get; set; }
        public string reqMtrcAuth { get; set; }
        public List<RoleMetricAuthority> roleMetrics { get; set; }

        public UserAppRole()
        {
            roleMetrics = new List<RoleMetricAuthority>();
        }
    }

    public class RoleMetricAuthority
    {
        public string userAppRoleId { get; set; }
        public string mtrcPeriodId { get; set; }
        public string mtrcPeriodName { get; set; }
    }

}