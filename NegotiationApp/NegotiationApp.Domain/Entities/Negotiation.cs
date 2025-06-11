using NegotiationApp.Domain.Enums;
using NegotiationApp.Domain.Exceptions;

namespace NegotiationApp.Domain.Entities
{
    public class Negotiation
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public decimal InitialPrice { get; private set; } 
        public decimal CurrentProposedPrice { get; private set; } 
        public NegotiationStatus Status { get; private set; }
        public int AttemptsLeft { get; private set; } 
        public DateTime NegotiationStartedDate { get; private set; }
        public DateTime? LastOfferDate { get; private set; }
        public DateTime? EmployeeResponseDate { get; private set; }

        private const int MaxAttempts = 3;
        private const int ClientResponseDays = 7;

        private Negotiation() { }

        public Negotiation(Guid productId, decimal initialPrice)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            InitialPrice = initialPrice;
            CurrentProposedPrice = initialPrice; 
            Status = NegotiationStatus.PendingClientOffer;
            AttemptsLeft = MaxAttempts;
            NegotiationStartedDate = DateTime.UtcNow;
        }

        public void ProposePrice(decimal newPrice)
        {
            if (AttemptsLeft <= 0)
            {
                throw new NegotiationAttemptsExceededException("You cannot negotiate more");
            }

            if (Status != NegotiationStatus.PendingClientOffer && Status != NegotiationStatus.RejectedByEmployee)
            {
                throw new InvalidNegotiationStateException($"Offer status is {Status}, you can not negotiate now");
            }

            if (newPrice <= 0)
            {
                throw new InvalidProposedPriceException("Proposed price has to be more than 0");
            }

            if (Status == NegotiationStatus.RejectedByEmployee && LastOfferDate.HasValue &&
                (DateTime.UtcNow - EmployeeResponseDate.Value).TotalDays > ClientResponseDays)
            {
                CancelNegotiation("Client has exceeded response time");
                throw new ClientResponseTimeExceededException("Client has exceeded response time, negotiation has been canceled");
            }

            CurrentProposedPrice = newPrice;
            Status = NegotiationStatus.ClientOffered;
            LastOfferDate = DateTime.UtcNow;
        }

        public void AcceptOffer()
        {
            if (Status != NegotiationStatus.ClientOffered)
            {
                throw new InvalidNegotiationStateException($"Can not accept, negotiation state is {Status}");
            }
            Status = NegotiationStatus.AcceptedByEmployee;
            EmployeeResponseDate = DateTime.UtcNow;
        }

        public void RejectOffer()
        {
            if (Status != NegotiationStatus.ClientOffered)
            {
                throw new InvalidNegotiationStateException($"Can not reject, negotiation state is {Status}");
            }
            AttemptsLeft--;
            Status = NegotiationStatus.RejectedByEmployee;
            EmployeeResponseDate = DateTime.UtcNow;

            if (AttemptsLeft <= 0)
            {
                CancelNegotiation("Offer canceled and client is out of negotiation attempts");
            }
        }

        public void CancelNegotiation(string reason = "Megotiation canceled")
        {
            if (Status == NegotiationStatus.AcceptedByEmployee || Status == NegotiationStatus.Cancelled)
            {
                throw new InvalidNegotiationStateException("Negotiation has already been accepted or canceled");
            }
            Status = NegotiationStatus.Cancelled;
            // TODO event: NegotiationCancelledEvent
        }

        public bool CanClientProposeNewPrice()
        {
            return Status == NegotiationStatus.PendingClientOffer || Status == NegotiationStatus.RejectedByEmployee;
        }

        public bool IsExpiredForClientResponse()
        {
            return Status == NegotiationStatus.RejectedByEmployee && EmployeeResponseDate.HasValue &&
                   (DateTime.UtcNow - EmployeeResponseDate.Value).TotalDays > ClientResponseDays;
        }
    }
}