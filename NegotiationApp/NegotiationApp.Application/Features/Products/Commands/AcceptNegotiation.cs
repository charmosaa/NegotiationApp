using MediatR;
using NegotiationApp.Application.DTOs;

namespace NegotiationApp.Application.Features.Negotiations.Commands
{
    public record AcceptNegotiationCommand(Guid NegotiationId) : IRequest<NegotiationDto>;
}