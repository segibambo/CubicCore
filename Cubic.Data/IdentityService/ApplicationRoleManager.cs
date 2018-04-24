using System;
using System.Collections.Generic;
using Cubic.Data.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Cubic.Data.IdentityService
{
    ///---------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines ApplicationRoleManager which implement ASP.Net Identity 2  RoleManager
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
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {

        public ApplicationRoleManager(IRoleStore<ApplicationRole> store, IEnumerable<IRoleValidator<ApplicationRole>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<ApplicationRole>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }

        //public ApplicationRoleManager(IRoleStore<ApplicationRole> store) : base(store)
        //{
        //}

        //public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        //{
        //    var appRoleManager = new ApplicationRoleManager(new ApplicationRoleStore(context.Get<APPContext>()));
        //    return appRoleManager;
        //}
    }
}
