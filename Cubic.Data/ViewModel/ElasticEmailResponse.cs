using System;
using System.Collections.Generic;
using System.Text;

namespace Cubic.Data.ViewModel
{

    public class messageResponse
    {
        public string transactionid { get; set; }
        public string messageid { get; set; }
    }
    public class ElasticEmailResponse
    {
        public bool success { get; set; }
        public messageResponse data { get; set; }
    }
}
