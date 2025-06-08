using MediatR;
using NegotiationApp.Application.DTOs;
using NegotiationApp.Domain.Entities;
using NegotiationApp.Domain.Repositories;
using NegotiationApp.Domain.Exceptions;

namespace NegotiationApp.Application.Features.Negotiations.Commands
{
    public class StartNegotiationHandler : IRequestHandler<StartNegotiationCommand, NegotiationDto>
    {
        private readonly INegotiationRepository _negotiationRepository;
        private readonly IProductRepository _productRepository;

        public StartNegotiationHandler(INegotiationRepository negotiationRepository, IProductRepository productRepository)
        {
            _negotiationRepository = negotiationRepository;
            _productRepository = productRepository;
        }

        public async Task<NegotiationDto> Handle(StartNegotiationCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                throw new ProductNotFoundException($"Product with ID: {request.ProductId} not found.");
            }

            // TODO
            // czy dla danego produktu nie ma już aktywnej negocjacji? 
            // Na potrzeby tego przykładu, zakładamy, że klient może rozpocząć nową negocjację.
            // sprawdzić czy istnieje już negocjacja, która nie jest zakończona i zwrócić ją zamiast tworzyć nową.

            var negotiation = new Negotiation(request.ProductId, product.BasePrice);
            await _negotiationRepository.AddAsync(negotiation);

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