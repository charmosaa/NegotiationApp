using NegotiationApp.Domain.Entities;
using NegotiationApp.Domain.Repositories;
using System.Collections.Concurrent;

namespace NegotiationApp.Infrastructure.Persistence.InMemory
{
    public class InMemoryProductRepository : IProductRepository
    {
        private readonly ConcurrentDictionary<Guid, Product> _products = new ConcurrentDictionary<Guid, Product>();

        public Task AddAsync(Product product)
        {
            _products[product.Id] = product;
            return Task.CompletedTask;
        }

        public Task<Product?> GetByIdAsync(Guid id)
        {
            _products.TryGetValue(id, out var product);
            return Task.FromResult(product);
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            return Task.FromResult(_products.Values.AsEnumerable());
        }

        public Task UpdateAsync(Product product)
        {
            _products[product.Id] = product; 
            return Task.CompletedTask;
        }
    }
}