using MediatR;
using NegotiationApp.Application.DTOs;
using NegotiationApp.Domain.Repositories;

namespace NegotiationApp.Application.Features.Products.Queries
{
    public class GetProductsListHandler : IRequestHandler<GetProductsListQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductsListHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetProductsListQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => new ProductDto { Id = p.Id, Name = p.Name, BasePrice = p.BasePrice });
        }
    }
}