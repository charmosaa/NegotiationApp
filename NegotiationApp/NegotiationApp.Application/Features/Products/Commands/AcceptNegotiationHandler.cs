using MediatR;
using NegotiationApp.Application.DTOs;
using NegotiationApp.Domain.Repositories;
using NegotiationApp.Domain.Exceptions;

namespace NegotiationApp.Application.Features.Negotiations.Commands
{
    public class AcceptNegotiationHandler : IRequestHandler<AcceptNegotiationCommand, NegotiationDto>
    {
        private readonly INegotiationRepository _negotiationRepository;

        public AcceptNegotiationHandler(INegotiationRepository negotiationRepository)
        {
            _negotiationRepository = negotiationRepository;
        }

        public async Task<NegotiationDto> Handle(AcceptNegotiationCommand request, CancellationToken cancellationToken)
        {
            var negotiation = await _negotiationRepository.GetByIdAsync(request.NegotiationId);
            if (negotiation == null)
            {
                throw new NegotiationNotFoundException($"Negotiation with ID: {request.NegotiationId} not found");
            }

            negotiation.AcceptOffer();
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