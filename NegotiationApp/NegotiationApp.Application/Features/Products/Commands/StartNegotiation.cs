using MediatR;
using NegotiationApp.Application.DTOs;

namespace NegotiationApp.Application.Features.Negotiations.Commands
{
    public record StartNegotiationCommand(Guid ProductId) : IRequest<NegotiationDto>;
}