using Microsoft.EntityFrameworkCore;
using WebInventario.Domain.Common;
using WebInventario.Domain.Entities;

namespace WebInventario.Infrastructure.Persistence
{
    public sealed class InventraDbContext : DbContext
    {
        public InventraDbContext(DbContextOptions<InventraDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Price)
                    .HasColumnType("decimal(18,2)");

                entity.Property(x => x.Stock)
                    .IsRequired();

                // Auditoría
                entity.Property(x => x.CreatedAtUtc)
                    .IsRequired();

                entity.Property(x => x.UpdatedAtUtc);

                entity.Property(x => x.IsDeleted).IsRequired();

                entity.HasQueryFilter(p => !p.IsDeleted);

                // Concurrencia optimista
                entity.Property(x => x.RowVersion)
                    .IsRowVersion();
            });
        }

        public override int SaveChanges()
        {
            ApplyAudit();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAudit();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAudit()
        {
            var utcNow = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<EntityBase>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAtUtc = utcNow;
                    entry.Entity.UpdatedAtUtc = null;
                }

                if (entry.State == EntityState.Modified)
                {
                    // Nunca permitas que se "re-edite" CreatedAt
                    entry.Property(x => x.CreatedAtUtc).IsModified = false;

                    entry.Entity.UpdatedAtUtc = utcNow;
                }
            }
        }
    }
}
