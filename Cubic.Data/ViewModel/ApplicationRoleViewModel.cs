using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cubic.Data.ViewModel
{
    public class ApplicationRoleViewModel
    {
        public Int64 Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You must enter a name for the Role.")]
        [StringLength(256, ErrorMessage = "The role name must be 256 characters or shorter.")]
        [Display(Name = "Role Name")]
        public string Name { get; set; }

        [Display(Name = "Permission(s)")]
        public IEnumerable<Int64> SelectedPermissionId { get; set; }
        public IEnumerable<SelectListItem> Permissions { get; set; }
    }
}
