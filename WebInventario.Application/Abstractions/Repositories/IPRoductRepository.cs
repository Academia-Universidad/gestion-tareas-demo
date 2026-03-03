using WebInventario.Domain.Entities;

namespace WebInventario.Application.Abstractions.Repositories
{
    public interface IProductRepository
    {
        Task<IReadOnlyList<Product>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default);

        Task<Product?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken ct = default);

        Task AddAsync(Product product, CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);

        Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct = default);

        Task<bool> HardDeleteAsync(Guid id, CancellationToken ct = default);
    }
}
