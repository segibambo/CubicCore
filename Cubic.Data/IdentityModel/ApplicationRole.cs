using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubic.Data.EntityContract;
using Microsoft.AspNetCore.Identity;

namespace Cubic.Data.IdentityModel
{

    ///---------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines ApplicationRole which implement ASP.Net Identity 2 interface and override it.
    /// </summary>
    /// <copyright>
    /// *****************************************************************************
    ///     ----- Fadipe Wasiu Ayobami . All Rights Reserved. Copyright (c) 2017
    /// *****************************************************************************
    /// </copyright>
    /// <remarks>
    /// *****************************************************************************
    ///     ---- Created For: Public Use (All Products)
    ///     ---- Created By: Fadipe Wasiu Ayobami
    ///     ---- Original Language: C#
    ///     ---- Current Version: v1.0.0.0.1
    ///     ---- Current Language: C#
    /// *****************************************************************************
    /// </remarks>
    /// <history>
    /// *****************************************************************************
    ///     --- Date First Created : 08 - 11 - 2017
    ///     --- Author: Fadipe Wasiu Ayobami
    ///     --- Date First Reviewed: 
    ///     --- Date Last Reviewed:
    /// *****************************************************************************
    /// </history>
    /// <usage>
    /// 
    /// -- Fadipe Wasiu Ayobami
    /// </usage>
    /// ----------------------------------------------------------------------------------------------
    ///
    /// 
    public class ApplicationRole : IdentityRole<long>, IEntity<Int64>
    {
        public ApplicationRole()
        {
            DateCreated = DateTime.Now;
            IsActive = true;
            IsDeleted = false;
        }
        public DateTime DateCreated { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsTransient()
        {
            return EqualityComparer<Int64>.Default.Equals(Id, default(Int64));
        }

        [NotMapped]
        public virtual ICollection<ApplicationUserRole> Users { get; } = new List<ApplicationUserRole>();
    }
}
