using MediatR;
using NegotiationApp.Application.DTOs;

namespace NegotiationApp.Application.Features.Products.Queries
{
    public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;
}