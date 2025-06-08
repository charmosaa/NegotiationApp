using MediatR;
using NegotiationApp.Application.DTOs;
using NegotiationApp.Domain.Repositories;

namespace NegotiationApp.Application.Features.Negotiations.Queries
{
    public class GetNegotiationsByProductIdHandler : IRequestHandler<GetNegotiationsByProductIdQuery, IEnumerable<NegotiationDto>>
    {
        private readonly INegotiationRepository _negotiationRepository;

        public GetNegotiationsByProductIdHandler(INegotiationRepository negotiationRepository)
        {
            _negotiationRepository = negotiationRepository;
        }

        public async Task<IEnumerable<NegotiationDto>> Handle(GetNegotiationsByProductIdQuery request, CancellationToken cancellationToken)
        {
            var negotiations = await _negotiationRepository.GetByProductIdAsync(request.ProductId);
            return negotiations.Select(n => new NegotiationDto
            {
                Id = n.Id,
                ProductId = n.ProductId,
                InitialPrice = n.InitialPrice,
                CurrentProposedPrice = n.CurrentProposedPrice,
                Status = n.Status,
                AttemptsLeft = n.AttemptsLeft,
                NegotiationStartedDate = n.NegotiationStartedDate,
                LastOfferDate = n.LastOfferDate,
                EmployeeResponseDate = n.EmployeeResponseDate
            });
        }
    }
}