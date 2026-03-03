using Microsoft.EntityFrameworkCore;
using WebInventario.Application.Abstractions.Repositories;
using WebInventario.Domain.Entities;
using WebInventario.Infrastructure.Persistence;

namespace WebInventario.Infrastructure.Repositories
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly InventraDbContext _db;

        public ProductRepository(InventraDbContext db) => _db = db;

        public async Task<IReadOnlyList<Product>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
        {
            IQueryable<Product> q = _db.Products.AsNoTracking();

            // Si el DbContext tiene HasQueryFilter(p => !p.IsDeleted),
            // entonces por defecto NO trae eliminados.
            // Para incluir eliminados debemos ignorar el filtro.
            if (includeDeleted)
                q = q.IgnoreQueryFilters();

            return await q
                .OrderBy(p => p.Name)
                .ToListAsync(ct);
        }

        public async Task<Product?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken ct = default)
        {
            IQueryable<Product> q = _db.Products;

            if (includeDeleted)
                q = q.IgnoreQueryFilters();

            return await q.FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public Task AddAsync(Product product, CancellationToken ct = default)
            => _db.Products.AddAsync(product, ct).AsTask();

        public Task SaveChangesAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);

        public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct = default)
        {
            // Para soft delete, normalmente solo tiene sentido sobre "activos".
            // Pero si quieres permitir "soft delete" aunque ya esté eliminado,
            // puedes usar IgnoreQueryFilters. Aquí lo permitimos para ser robustos.
            var product = await _db.Products
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id, ct);

            if (product is null) return false;

            product.IsDeleted = true;  // método en EntityBase (Domain)
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> HardDeleteAsync(Guid id, CancellationToken ct = default)
        {
            // Hard delete debe poder borrar incluso si ya está soft-deleted,
            // por eso ignoramos filtros.
            var product = await _db.Products
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id, ct);

            if (product is null) return false;

            _db.Products.Remove(product);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
