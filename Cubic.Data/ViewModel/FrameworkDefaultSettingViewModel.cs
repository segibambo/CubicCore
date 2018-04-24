using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Cubic.Data.ViewModel
{
    [Serializable]
    public class FrameworkDefaultSettingViewModel
    {
        public Int64 Id { get; set; }
        [DisplayName("Framework Name")]
        [ReadOnly(true)]
        public string FrameworkName { get; set; }

        [DisplayName("Framework Version")]
        [ReadOnly(true)]
        public string FrameworkVersion { get; set; }

        [DisplayName("Description")]
        [ReadOnly(true)]
        public string FrameworkDescription { get; set; }

        [DisplayName("Target Server")]
        [ReadOnly(true)]
        public string TargetServer { get; set; }

        [DisplayName("Default Database")]
        [ReadOnly(true)]
        public string DefaultDatabaseEngine { get; set; }


        [DisplayName("IOC Container")]
        [ReadOnly(true)]
        public string IOC { get; set; }

        [DisplayName("Developed By")]
        [ReadOnly(true)]
        public string Developed { get; set; }

        [DisplayName("UX/UI")]
        [ReadOnly(true)]
        public string Graphic { get; set; }

        [DisplayName("DateCreated")]
        [ReadOnly(true)]
        public DateTime DateCreated { get; set; }


      
    }
}
