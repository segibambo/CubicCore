using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cubic.Data.ViewModel
{
    public  class changepasswordViewModel
    {
        [Required(ErrorMessage = "* Required")]
        [DataType(DataType.Password)]
        [DisplayName("OldPassword")]
        public string OldPassword { get; set; }

        
        [Required(ErrorMessage = "* Required")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "* Required")]
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
