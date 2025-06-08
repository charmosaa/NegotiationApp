using NegotiationApp.Domain.Entities;

namespace NegotiationApp.Domain.Repositories
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task UpdateAsync(Product product);
    }
}
