﻿
using Cubic.Data.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Data.Entities
{
    public  class EmailTemplate : Entity<long>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Body { get; set; }
 
        
    }
}
