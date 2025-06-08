using MediatR;
using NegotiationApp.Application.DTOs;

namespace NegotiationApp.Application.Features.Negotiations.Queries
{
    public record GetNegotiationsByProductIdQuery(Guid ProductId) : IRequest<IEnumerable<NegotiationDto>>;
}