
using Cubic.Data.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Cubic.Data.IdentityService
{
    ///---------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines ApplicationUserStore which implement ASP.Net Identity 2  UserStore
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


    public class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, APPContext, long, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, IdentityUserToken<long>, IdentityRoleClaim<long>>
    {
        public ApplicationUserStore(APPContext context) : base(context)
        {
        }
    }
    
}
