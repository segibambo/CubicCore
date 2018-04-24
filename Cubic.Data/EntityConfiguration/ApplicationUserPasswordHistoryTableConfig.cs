using System.Data.Entity.ModelConfiguration;
using Cubic.Data.IdentityModel;

namespace Cubic.Data.EntityConfiguration
{
    public class ApplicationUserPasswordHistoryTableConfig : EntityTypeConfiguration<ApplicationUserPasswordHistory>
    {
        public ApplicationUserPasswordHistoryTableConfig()
        {
            this.ToTable(tableName: "PasswordHistory");
            //ToTable("AspNetUserRoles");
            // Primary Key
            HasKey(t => t.Id);
            this.Property(e => e.UserId).IsRequired().HasColumnName("AspNetUserId");
            this.Property(m => m.HashPassword).IsRequired();
            this.Property(m => m.DateCreated).IsRequired();
            this.Property(m => m.PasswordSalt).IsOptional();
            



        }
     }
}
