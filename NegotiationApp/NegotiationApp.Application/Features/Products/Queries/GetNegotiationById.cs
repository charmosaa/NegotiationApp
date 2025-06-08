using MediatR;
using NegotiationApp.Application.DTOs;

namespace NegotiationApp.Application.Features.Negotiations.Queries
{
    public record GetNegotiationByIdQuery(Guid Id) : IRequest<NegotiationDto?>;
}