using MediatR;
using NegotiationApp.Application.DTOs;
using NegotiationApp.Domain.Repositories;

namespace NegotiationApp.Application.Features.Negotiations.Queries
{
    public class GetNegotiationByIdHandler : IRequestHandler<GetNegotiationByIdQuery, NegotiationDto?>
    {
        private readonly INegotiationRepository _negotiationRepository;

        public GetNegotiationByIdHandler(INegotiationRepository negotiationRepository)
        {
            _negotiationRepository = negotiationRepository;
        }

        public async Task<NegotiationDto?> Handle(GetNegotiationByIdQuery request, CancellationToken cancellationToken)
        {
            var negotiation = await _negotiationRepository.GetByIdAsync(request.Id);
            if (negotiation == null) return null;

            return new NegotiationDto
            {
                Id = negotiation.Id,
                ProductId = negotiation.ProductId,
                InitialPrice = negotiation.InitialPrice,
                CurrentProposedPrice = negotiation.CurrentProposedPrice,
                Status = negotiation.Status,
                AttemptsLeft = negotiation.AttemptsLeft,
                NegotiationStartedDate = negotiation.NegotiationStartedDate,
                LastOfferDate = negotiation.LastOfferDate,
                EmployeeResponseDate = negotiation.EmployeeResponseDate
            };
        }
    }
}