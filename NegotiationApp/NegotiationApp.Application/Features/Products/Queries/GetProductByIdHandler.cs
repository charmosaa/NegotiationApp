using MediatR;
using NegotiationApp.Application.DTOs;
using NegotiationApp.Domain.Repositories;

namespace NegotiationApp.Application.Features.Products.Queries
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductRepository _productRepository;

        public GetProductByIdHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null) return null;
            return new ProductDto { Id = product.Id, Name = product.Name, BasePrice = product.BasePrice };
        }
    }
}