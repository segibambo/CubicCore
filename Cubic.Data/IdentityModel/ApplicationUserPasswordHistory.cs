﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubic.Data.EntityBase;

namespace Cubic.Data.IdentityModel
{

    ///---------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines ApplicationUserPasswordHistory  which is used to manage User password history change
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
    public class ApplicationUserPasswordHistory: BaseEntityWithAudit<long>
    {
        public long UserId { get; set; }
        public string HashPassword { get; set; }

        public string PasswordSalt { get; set; }
    }
}
