using Cubic.Data.Entities;
using Cubic.Data.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cubic.Data
{
    public class DataInitializer
    {
        public static void SeedData(APPContext context)
        {
            context.Database.EnsureCreated();
            
            context.ApplicationRoles.Add(
               new ApplicationRole
               {
                   Name = "PortalAdmin",
                   NormalizedName= "PortalAdmin".Trim().ToUpper(),
                   IsActive = true,
                   IsDeleted = false,
                   DateCreated = DateTime.Now,
               });
            context.SaveChanges();

            context.PortalVersions.Add(
               new PortalVersion
               {
                   FrameworkName = "Cubic ASP.Net Core Framework",
                   FrameworkDescription = "An MVC Customized Framework built on ASP.Net Identity 2.0 to aid fast application development with built in logger and activitylog",
                   FrameworkVersion = "2.0.0.0",
                   TargetServer = "MSSQL,Postgress,MangoDB,MYSQL",
                   PackagesUsed = "Microsoft.ASPNET.Identity,Microsoft.OWIN,Log4net,EntityFramework,JQuery DataTable,Select 2,Boostrap,Autofac,Autofac.MVC,Autofac.WebAPI2,CQRS RepositoryPattern",
                   DefaultDatabaseEngine = "MSSQL Server",
                   IOC = "Autofac",
                   DateCreated = DateTime.Now,
                   DevelopedBy = "Fadipe Ayobami  || ayfadipe@gmail.com",
                   UX = "Open Source AdminLTE2 Template"
               });
            context.SaveChanges();
            //if (context.PortalVersions.Find(typeof(PortalVersion),1)!= null)
            //{


            //}
        }
    }
}
