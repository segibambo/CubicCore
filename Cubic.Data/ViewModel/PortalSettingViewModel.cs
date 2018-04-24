using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cubic.Data.ViewModel
{
    public class PortalSettingViewModel
    {
        public Int64 Id { get; set; }

        public bool HasAdminUserConfigured { get; set; }
        [Required(ErrorMessage = "* Required")]
        [StringLength(100)]
        [DisplayName("Portal Title")]
        public string PortalTitle { get; set; }

        [Required(ErrorMessage = "* Required")]
        [DisplayName("Portal Description")]
        public string PortalDescription { get; set; }

        [StringLength(200)]
        [DisplayName("Terms & Condition")]
        public string TermsAndConditionPath { get; set; }

        [DisplayName("Display Picture Icon?")]
        public bool DisplayPictureBesideUserName
        {
            get { return PictureBesideUsername > 0; }
            set { PictureBesideUsername = value ? 1 : 0; }
        }

        [ScaffoldColumn(false)]
        [Range(0, 1)]
        public int PictureBesideUsername { get; set; }

    }
}
