using MediatR;
using NegotiationApp.Application.DTOs;

namespace NegotiationApp.Application.Features.Negotiations.Commands
{
    public record ProposePriceCommand(Guid NegotiationId, decimal ProposedPrice) : IRequest<NegotiationDto>;
}