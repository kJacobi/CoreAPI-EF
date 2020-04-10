using CoreAPI_EF.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreAPI_EF.Data
{
	public class DataContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
	{
		public DataContext(DbContextOptions<DataContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
		}

        // For automatic trimming of all strings on ef core save method
        public void OnBeforeSaving(DbContext context)
        {
            var entries = context?.ChangeTracker?.Entries();

            if (entries is null)
            {
                return;
            }

            foreach (var entry in entries)
            {
                // get all the properties and are of type string
                var propertyValues = entry.CurrentValues.Properties.Where(p => p.ClrType == typeof(string));

                foreach (var prop in propertyValues)
                {
                    // access the correct column by it's name and trim the value if it's not null
                    if (entry.CurrentValues[prop.Name] != null) entry.CurrentValues[prop.Name] = entry.CurrentValues[prop.Name].ToString().Trim();
                }
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            // do our custom rules
            OnBeforeSaving(this);
            return base.SaveChanges(acceptAllChangesOnSuccess);

        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
        {
            // do our custom rules
            OnBeforeSaving(this);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        }

        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Tbl_Reference> Tbl_References { get; set; }
        public DbSet<TransactionImage> TransactionImages { get; set; }
        public DbSet<TransactionLog> TransactionLogs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }



    }
}
