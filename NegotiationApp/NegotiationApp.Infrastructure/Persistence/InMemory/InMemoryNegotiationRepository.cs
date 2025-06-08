using NegotiationApp.Domain.Entities;
using NegotiationApp.Domain.Repositories;
using System.Collections.Concurrent;

namespace NegotiationApp.Infrastructure.Persistence.InMemory
{
    public class InMemoryNegotiationRepository : INegotiationRepository
    {
        private readonly ConcurrentDictionary<Guid, Negotiation> _negotiations = new ConcurrentDictionary<Guid, Negotiation>();

        public Task AddAsync(Negotiation negotiation)
        {
            _negotiations[negotiation.Id] = negotiation;
            return Task.CompletedTask;
        }

        public Task<Negotiation?> GetByIdAsync(Guid id)
        {
            _negotiations.TryGetValue(id, out var negotiation);
            return Task.FromResult(negotiation);
        }

        public Task UpdateAsync(Negotiation negotiation)
        {
            _negotiations[negotiation.Id] = negotiation;
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Negotiation>> GetByProductIdAsync(Guid productId)
        {
            return Task.FromResult(_negotiations.Values.Where(n => n.ProductId == productId).AsEnumerable());
        }
    }
}