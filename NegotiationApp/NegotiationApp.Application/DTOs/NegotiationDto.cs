using NegotiationApp.Domain.Enums;

namespace NegotiationApp.Application.DTOs
{
    public class NegotiationDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public decimal InitialPrice { get; set; }
        public decimal CurrentProposedPrice { get; set; }
        public NegotiationStatus Status { get; set; }
        public int AttemptsLeft { get; set; }
        public DateTime NegotiationStartedDate { get; set; }
        public DateTime? LastOfferDate { get; set; }
        public DateTime? EmployeeResponseDate { get; set; }
    }
}