using MediatR;
using NegotiationApp.Application.DTOs;

namespace NegotiationApp.Application.Features.Negotiations.Commands
{
    public record CancelNegotiationCommand(Guid NegotiationId) : IRequest<NegotiationDto>;
}