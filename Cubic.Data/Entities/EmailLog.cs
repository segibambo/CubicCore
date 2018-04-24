﻿using Cubic.Data.EntityBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Data.Entities
{
    public  class EmailLog : Entity<long>
    {
        [Required]
        [StringLength(1000)]
        public string Sender { get; set; }
        [Required]
        [StringLength(1000)]
        public string Receiver { get; set; }
       
        [StringLength(1000)]
        public string CC { get; set; }

        public string BCC { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string MessageBody { get; set; }

        public bool HasAttachment { get; set; }

        public int Retires { get; set; }
        public bool IsSent { get; set; }

        public DateTime? DateSent { get; set; }

        public DateTime DateToSend { get; set; }

        [NotMapped]
        public virtual ICollection<EmailAttachment> EmailAttachments { get; } = new List<EmailAttachment>();
    }
}
