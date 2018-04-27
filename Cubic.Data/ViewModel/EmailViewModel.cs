using Cubic.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cubic.Data.ViewModel
{

    public class EmailListViewModel
    {
        public long EmailID { get; set; }
        public string EmailName { get; set; }
        public string EmailCode { get; set; }
    }

    public class EmailViewModel
    {
        public long EmailID { get; set; }
        public string EmailSubject{ get; set; }
        public string EmailCode { get; set; }
        public string EmailText { get; set; }

        public List<EmailToken> EmailToken { get; set; }
    }
}
