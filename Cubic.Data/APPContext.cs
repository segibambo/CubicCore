using Cubic.Data.Entities;
using Cubic.Data.IdentityModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Data
{

    public class APPContext : IdentityDbContext<ApplicationUser, ApplicationRole, long, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, IdentityRoleClaim<long>, IdentityUserToken<long>>
    {
        #region MyDBSetRegion

        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationUserClaim> ApplicationUserClaims { get; set; }
        public DbSet<ApplicationUserLogin> ApplicationUserLogins { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }
        public DbSet<ApplicationUserPasswordHistory> ApplicationUserPasswordHistorys { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<PortalVersion> PortalVersions { get; set; }


        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<EmailToken> EmailTokens { get; set; }
        public DbSet<EmailAttachment> EmailAttachments { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        #endregion


        public APPContext(DbContextOptions<APPContext> options)
          : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        
        #region MyDateCreated&DateUpdateRegion
        //public override int SaveChanges()
        //{

        //    try
        //    {
        //        //Audits();
        //        return base.SaveChanges();
        //    }
        //    catch (DbEntityValidationException filterContext)
        //    {
        //        if (typeof(DbEntityValidationException) == filterContext.GetType())
        //        {
        //            foreach (var validationErrors in filterContext.EntityValidationErrors)
        //            {
        //                foreach (var validationError in validationErrors.ValidationErrors)
        //                {
        //                    Debug.WriteLine("Property: {0} Error: {1}",
        //                        validationError.PropertyName,
        //                        validationError.ErrorMessage);

        //                }
        //            }
        //        }
        //        throw;
        //    }

        //}

        //public override async Task<int> SaveChangesAsync()
        //{
        //    try
        //    {
        //        // Audits();
        //        return await base.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException filterContext)
        //    {
        //        Debug.WriteLine("Concurrency Error: {0}", filterContext.Message);
        //        return await Task.FromResult(0);

        //    }

        //}





        //private void Audits()
        //{

        //    var entities = ChangeTracker.Entries().Where(x => (x.Entity is IEntity || x.Entity is IAduit || x.Entity is Entity || x.Entity is BaseEntityWithAudit) && (x.State == EntityState.Added || x.State == EntityState.Modified));
        //    int userId = 0;
        //    try
        //    {
        //        userId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
        //    }
        //    catch { }
        //    foreach (var entity in entities)
        //    {
        //        if (entity.State == EntityState.Added)
        //        {

        //            if (entity.Entity is IAduit)
        //                ((IAduit)entity.Entity).DateCreated = DateTime.Now;
        //            ((IAduit)entity.Entity).CreatedBy = userId;
        //            ((IAduit)entity.Entity).IsActive = true;
        //            ((IAduit)entity.Entity).IsDeleted = false;
        //            if (entity.Entity is IEntity)
        //                ((IEntity)entity.Entity).DateCreated = DateTime.Now;
        //            ((IEntity)entity.Entity).IsActive = true;
        //            ((IEntity)entity.Entity).IsDeleted = false;
        //            if (entity.Entity is Entity)
        //                //((Entity)entity.Entity).DateCreated = DateTime.Now;
        //                //((Entity)entity.Entity).IsActive = true;
        //                //((Entity)entity.Entity).IsDeleted = false;
        //                if (entity.Entity is BaseEntityWithAudit)
        //                    ((BaseEntityWithAudit)entity.Entity).DateCreated = DateTime.Now;
        //            ((BaseEntityWithAudit)entity.Entity).CreatedBy = userId;
        //            ((BaseEntityWithAudit)entity.Entity).IsActive = true;
        //            ((BaseEntityWithAudit)entity.Entity).IsDeleted = false;

        //        }
        //        else if (entity.State == EntityState.Modified)
        //        {
        //            if (entity.Entity is IAduit)
        //                ((IAduit)entity.Entity).DateUpdated = DateTime.Now;
        //            ((IAduit)entity.Entity).UpdatedBy = userId;
        //            if (entity.Entity is BaseEntityWithAudit)
        //                ((BaseEntityWithAudit)entity.Entity).DateUpdated = DateTime.Now;
        //            ((BaseEntityWithAudit)entity.Entity).UpdatedBy = userId;
        //        }
        //        else if (entity.State == EntityState.Deleted)
        //        {
        //            if (entity.Entity is IAduit)
        //                ((IAduit)entity.Entity).IsDeleted = true;
        //            if (entity.Entity is IEntity)
        //                ((IEntity)entity.Entity).IsDeleted = true;
        //            if (entity.Entity is Entity)
        //                //((Entity)entity.Entity).IsDeleted = true;
        //                if (entity.Entity is BaseEntityWithAudit)
        //                    ((BaseEntityWithAudit)entity.Entity).IsDeleted = true;
        //        }
        //    }
        //}

        //#region MyAuditTrailRegion
        //private void AppAuditLogs()
        //{
        //    var entities = ChangeTracker.Entries().Where(x => (x.Entity is IEntity || x.Entity is IAduit || x.Entity is Entity || x.Entity is BaseEntityWithAudit) && (x.State == EntityState.Added || x.State == EntityState.Modified));
        //    int userId = 0;
        //    try
        //    {
        //        userId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
        //    }
        //    catch { }
        //    // For each changed record, get the audit record entries and add them
        //    foreach (var entity in entities)
        //    {
        //        foreach (AuditLog x in GetAuditRecordsForChange(entity, userId))
        //        {
        //            this.AuditLogs.Add(x);
        //        }
        //    }
        //}
        //private List<AuditLog> GetAuditRecordsForChange(DbEntityEntry dbEntry, int userId)
        //{
        //    List<AuditLog> result = new List<AuditLog>();
        //    DateTime changeTime = DateTime.Now;
        //    String IPAddress = HttpContext.Current.Request.UserHostAddress;
        //    // Get the Table() attribute, if one exists
        //    TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

        //    // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
        //    string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;
        //    string jsonstring = string.Empty;
        //    try
        //    {
        //        jsonstring = Newtonsoft.Json.JsonConvert.SerializeObject(dbEntry.Entity);
        //    }
        //    catch { }
        //    // Get primary key value 
        //    //string keyName = dbEntry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).Name;

        //    if (dbEntry.State == EntityState.Added)
        //    {
        //        try
        //        {
        //            ((AuditedEntity)dbEntry.Entity).DateCreated = DateTime.Now;
        //        }
        //        catch { }

        //        // For Inserts, just add the whole record
        //        result.Add(new AuditLog()
        //        {
        //            UserID = userId,
        //            EventDate = changeTime,
        //            EventType = Convert.ToInt32(AuditActionType.Create),
        //            TableName = tableName,
        //            //RecordID = dbEntry.CurrentValues.GetValue<object>(keyName).ToString(),
        //            ColumnName = "*ALL",
        //            NewValue = jsonstring,
        //            IPAddress = IPAddress
        //        });
        //    }
        //    else if (dbEntry.State == EntityState.Deleted)
        //    {
        //        try
        //        {
        //            ((AuditedEntity)dbEntry.Entity).DateModified = DateTime.Now;
        //        }
        //        catch { }


        //        // Same with deletes, do the whole record
        //        result.Add(new AuditLog()
        //        {
        //            UserID = userId,
        //            EventDate = changeTime,
        //            EventType = Convert.ToInt32(AuditActionType.Delete),
        //            TableName = tableName,
        //            // RecordID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
        //            ColumnName = "*ALL",
        //            NewValue = jsonstring,
        //            IPAddress = IPAddress
        //        });

        //    }
        //    else if (dbEntry.State == EntityState.Modified)
        //    {
        //        try
        //        {
        //            ((AuditedEntity)dbEntry.Entity).DateModified = DateTime.Now;
        //        }
        //        catch { }


        //        foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
        //        {
        //            // For updates, we only want to capture the columns that actually changed
        //            if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
        //            {
        //                result.Add(new AuditLog()
        //                {
        //                    UserID = userId,
        //                    EventDate = changeTime,
        //                    EventType = Convert.ToInt32(AuditActionType.Edit),
        //                    TableName = tableName,
        //                    //RecordID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
        //                    ColumnName = propertyName,
        //                    OldValue = jsonstring,
        //                    NewValue = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString(),
        //                    IPAddress = IPAddress
        //                });
        //            }
        //        }
        //    }
        //    return result;
        //}
        //#endregion


        #endregion
    }
  }
