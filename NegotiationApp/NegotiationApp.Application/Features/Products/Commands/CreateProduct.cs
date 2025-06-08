using MediatR;
using NegotiationApp.Application.DTOs;

namespace NegotiationApp.Application.Features.Products.Commands
{
    public record CreateProductCommand(string Name, decimal BasePrice) : IRequest<ProductDto>;
}