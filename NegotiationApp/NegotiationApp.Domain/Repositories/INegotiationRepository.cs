using NegotiationApp.Domain.Entities;

namespace NegotiationApp.Domain.Repositories
{
    public interface INegotiationRepository
    {
        Task AddAsync(Negotiation negotiation);
        Task<Negotiation?> GetByIdAsync(Guid id);
        Task UpdateAsync(Negotiation negotiation);
        Task<IEnumerable<Negotiation>> GetByProductIdAsync(Guid productId);
    }
}