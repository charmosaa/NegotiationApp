using MediatR;
using NegotiationApp.Application.DTOs;
using NegotiationApp.Domain.Entities;
using NegotiationApp.Domain.Repositories;

namespace NegotiationApp.Application.Features.Products.Commands
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product(request.Name, request.BasePrice);
            await _productRepository.AddAsync(product);
            return new ProductDto { Id = product.Id, Name = product.Name, BasePrice = product.BasePrice };
        }
    }
}