using MediatR;
using NegotiationApp.Application.DTOs;

namespace NegotiationApp.Application.Features.Negotiations.Commands
{
    public record RejectNegotiationCommand(Guid NegotiationId) : IRequest<NegotiationDto>;
}