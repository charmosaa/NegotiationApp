using MediatR;
using NegotiationApp.Application.DTOs;
using NegotiationApp.Domain.Repositories;
using NegotiationApp.Domain.Exceptions;

namespace NegotiationApp.Application.Features.Negotiations.Commands
{
    public class CancelNegotiationHandler : IRequestHandler<CancelNegotiationCommand, NegotiationDto>
    {
        private readonly INegotiationRepository _negotiationRepository;

        public CancelNegotiationHandler(INegotiationRepository negotiationRepository)
        {
            _negotiationRepository = negotiationRepository;
        }

        public async Task<NegotiationDto> Handle(CancelNegotiationCommand request, CancellationToken cancellationToken)
        {
            var negotiation = await _negotiationRepository.GetByIdAsync(request.NegotiationId);
            if (negotiation == null)
            {
                throw new NegotiationNotFoundException($"Negotiation with ID: {request.NegotiationId} not found.");
            }

            negotiation.CancelNegotiation();
            await _negotiationRepository.UpdateAsync(negotiation);

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