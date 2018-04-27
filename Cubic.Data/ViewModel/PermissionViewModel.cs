using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cubic.Data.ViewModel
{
    public class PermissionViewModel
    {
        public Int64 PermissionId { get; set; }

        [Display(Name = "Permission Name")]
        public string PermissionName { get; set; }


        [Display(Name = "Permission Code")]
        public string PermissionCode { get; set; }
    }
}
