using Xunit;
using FluentAssertions;
using NegotiationApp.Domain.Entities;
using NegotiationApp.Domain.Enums;
using NegotiationApp.Domain.Exceptions;
using System.Reflection;

namespace NegotiationApp.Tests.UnitTests.Domain.Entities
{
    public class NegotiationTests
    {
        private readonly Guid _testProductId = Guid.NewGuid();
        private const decimal InitialPrice = 100.00m;
        private const int TestClientResponseDays = 7;

        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            var negotiation = new Negotiation(_testProductId, InitialPrice);

            // Assert
            negotiation.Id.Should().NotBeEmpty();
            negotiation.ProductId.Should().Be(_testProductId);
            negotiation.InitialPrice.Should().Be(InitialPrice);
            negotiation.CurrentProposedPrice.Should().Be(InitialPrice);
            negotiation.Status.Should().Be(NegotiationStatus.PendingClientOffer);
            negotiation.AttemptsLeft.Should().Be(3); // MaxAttempts = 3
            negotiation.NegotiationStartedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            negotiation.LastOfferDate.Should().BeNull();
            negotiation.EmployeeResponseDate.Should().BeNull();
        }

        // --- ProposePrice Method ---
        [Fact]
        public void ProposePrice_ShouldUpdatePriceAndStatus_WhenPendingClientOffer()
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);
            var newPrice = 90.00m;

            // Act
            negotiation.ProposePrice(newPrice);

            // Assert
            negotiation.CurrentProposedPrice.Should().Be(newPrice);
            negotiation.Status.Should().Be(NegotiationStatus.ClientOffered);
            negotiation.LastOfferDate.Should().NotBeNull();
            negotiation.AttemptsLeft.Should().Be(3); 
        }

        [Fact]
        public void ProposePrice_ShouldUpdatePriceAndStatus_WhenRejectedByEmployee()
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);
            negotiation.ProposePrice(95.00m); // Client offers
            negotiation.RejectOffer(); // Employee rejects
            var newPrice = 85.00m;

            // Act
            negotiation.ProposePrice(newPrice);

            // Assert
            negotiation.CurrentProposedPrice.Should().Be(newPrice);
            negotiation.Status.Should().Be(NegotiationStatus.ClientOffered);
            negotiation.LastOfferDate.Should().NotBeNull();
            negotiation.AttemptsLeft.Should().Be(2); 
        }

        [Fact]
        public void ProposePrice_ShouldThrowException_WhenAttemptsExceeded()
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);
            // 3 tries max
            negotiation.ProposePrice(90m); // Client offers
            negotiation.RejectOffer(); // Employee rejects, AttemptsLeft = 2
            negotiation.ProposePrice(80m); // Client offers
            negotiation.RejectOffer(); // Employee rejects, AttemptsLeft = 1
            negotiation.ProposePrice(70m); // Client offers
            negotiation.RejectOffer(); // Employee rejects, AttemptsLeft = 0

            // Act & Assert
            // with 4th negotiation we get exception 
            Action act = () => negotiation.ProposePrice(60m);
            act.Should().Throw<NegotiationAttemptsExceededException>()
               .WithMessage("You cannot negotiate more");
            negotiation.Status.Should().Be(NegotiationStatus.Cancelled); //out of tries = status cancelled
        }

        [Theory] // different status scenarios 
        [InlineData(NegotiationStatus.AcceptedByEmployee)]
        [InlineData(NegotiationStatus.Cancelled)]
        [InlineData(NegotiationStatus.ClientOffered)] 
        public void ProposePrice_ShouldThrowException_WhenInvalidStatus(NegotiationStatus invalidStatus)
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);
            
            if (invalidStatus == NegotiationStatus.ClientOffered)
            {
                negotiation.ProposePrice(90m);
            }
            else if (invalidStatus == NegotiationStatus.AcceptedByEmployee)
            {
                negotiation.ProposePrice(90m);
                negotiation.AcceptOffer();
            }
            else if (invalidStatus == NegotiationStatus.Cancelled)
            {
                negotiation.CancelNegotiation();
            }

            // Act & Assert
            Action act = () => negotiation.ProposePrice(80m);
            act.Should().Throw<InvalidNegotiationStateException>()
               .WithMessage($"Offer status is {invalidStatus}, you can not negotiate now");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void ProposePrice_ShouldThrowException_WhenNewPriceIsInvalid(decimal invalidPrice)
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);

            // Act & Assert
            Action act = () => negotiation.ProposePrice(invalidPrice);
            act.Should().Throw<InvalidProposedPriceException>()
               .WithMessage("Proposed price has to be more than 0");
        }

        [Fact]
        public void ProposePrice_ShouldThrowExceptionAndCancel_WhenClientResponseTimeExceededAfterReject()
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);
            negotiation.ProposePrice(90m);
            negotiation.RejectOffer();
            // more than one week response time 
            SetEmployeeResponseDate(negotiation, DateTime.UtcNow.AddDays(-(TestClientResponseDays+1))); // response was 8 days ago

            // Act & Assert
            Action act = () => negotiation.ProposePrice(80m);
            act.Should().Throw<ClientResponseTimeExceededException>()
               .WithMessage("Client has exceeded response time, negotiation has been canceled");
            negotiation.Status.Should().Be(NegotiationStatus.Cancelled); // check status
        }


        // --- AcceptOffer Method ---
        [Fact]
        public void AcceptOffer_ShouldUpdateStatusToAcceptedByEmployee()
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);
            negotiation.ProposePrice(90.00m); 

            // Act
            negotiation.AcceptOffer();

            // Assert
            negotiation.Status.Should().Be(NegotiationStatus.AcceptedByEmployee);
            negotiation.EmployeeResponseDate.Should().NotBeNull();
        }

        [Theory]
        [InlineData(NegotiationStatus.PendingClientOffer)]
        [InlineData(NegotiationStatus.RejectedByEmployee)]
        [InlineData(NegotiationStatus.AcceptedByEmployee)] 
        [InlineData(NegotiationStatus.Cancelled)]
        public void AcceptOffer_ShouldThrowException_WhenInvalidStatus(NegotiationStatus invalidStatus)
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);
            if (invalidStatus == NegotiationStatus.RejectedByEmployee)
            {
                negotiation.ProposePrice(90m);
                negotiation.RejectOffer();
            }
            else if (invalidStatus == NegotiationStatus.AcceptedByEmployee)
            {
                negotiation.ProposePrice(90m);
                negotiation.AcceptOffer();
            }
            else if (invalidStatus == NegotiationStatus.Cancelled)
            {
                negotiation.CancelNegotiation();
            }

            // Act & Assert
            Action act = () => negotiation.AcceptOffer();
            act.Should().Throw<InvalidNegotiationStateException>()
               .WithMessage($"Can not accept, negotiation state is {invalidStatus}");
        }

        // --- RejectOffer Method ---
        [Fact]
        public void RejectOffer_ShouldUpdateStatusToRejectedByEmployeeAndDecrementAttempts()
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);
            negotiation.ProposePrice(90.00m); 
            var initialAttempts = negotiation.AttemptsLeft;

            // Act
            negotiation.RejectOffer();

            // Assert
            negotiation.Status.Should().Be(NegotiationStatus.RejectedByEmployee);
            negotiation.AttemptsLeft.Should().Be(initialAttempts - 1);
            negotiation.EmployeeResponseDate.Should().NotBeNull();
        }

        [Fact]
        public void RejectOffer_ShouldCancelNegotiation_WhenAttemptsReachZero()
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);
            negotiation.ProposePrice(90m);
            negotiation.RejectOffer(); // AttemptsLeft = 2
            negotiation.ProposePrice(80m);
            negotiation.RejectOffer(); // AttemptsLeft = 1
            negotiation.ProposePrice(70m);

            // Act
            negotiation.RejectOffer(); // AttemptsLeft = 0, cancelled

            // Assert
            negotiation.Status.Should().Be(NegotiationStatus.Cancelled);
            negotiation.AttemptsLeft.Should().Be(0);
        }

        [Theory]
        [InlineData(NegotiationStatus.PendingClientOffer)]
        [InlineData(NegotiationStatus.AcceptedByEmployee)]
        [InlineData(NegotiationStatus.RejectedByEmployee)] 
        [InlineData(NegotiationStatus.Cancelled)]
        public void RejectOffer_ShouldThrowException_WhenInvalidStatus(NegotiationStatus invalidStatus)
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);
            if (invalidStatus == NegotiationStatus.AcceptedByEmployee)
            {
                negotiation.ProposePrice(90m);
                negotiation.AcceptOffer();
            }
            else if (invalidStatus == NegotiationStatus.RejectedByEmployee)
            {
                negotiation.ProposePrice(90m);
                negotiation.RejectOffer();
            }
            else if (invalidStatus == NegotiationStatus.Cancelled)
            {
                negotiation.CancelNegotiation();
            }

            // Act & Assert
            Action act = () => negotiation.RejectOffer();
            act.Should().Throw<InvalidNegotiationStateException>()
               .WithMessage($"Can not reject, negotiation state is {invalidStatus}");
        }

        // --- CancelNegotiation Method ---
        [Fact]
        public void CancelNegotiation_ShouldChangeStatusToCancelled()
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);

            // Act
            negotiation.CancelNegotiation();

            // Assert
            negotiation.Status.Should().Be(NegotiationStatus.Cancelled);
        }

        [Fact]
        public void CancelNegotiation_ShouldChangeStatusToCancelled_FromClientOffered()
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);
            negotiation.ProposePrice(90.00m); 

            // Act
            negotiation.CancelNegotiation();

            // Assert
            negotiation.Status.Should().Be(NegotiationStatus.Cancelled);
        }

        [Theory]
        [InlineData(NegotiationStatus.AcceptedByEmployee)]
        [InlineData(NegotiationStatus.Cancelled)]
        public void CancelNegotiation_ShouldThrowException_WhenAlreadyFinalized(NegotiationStatus finalStatus)
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);
            if (finalStatus == NegotiationStatus.AcceptedByEmployee)
            {
                negotiation.ProposePrice(90m);
                negotiation.AcceptOffer();
            }
            else if (finalStatus == NegotiationStatus.Cancelled)
            {
                negotiation.CancelNegotiation();
            }

            // Act & Assert
            Action act = () => negotiation.CancelNegotiation();
            act.Should().Throw<InvalidNegotiationStateException>()
               .WithMessage("Negotiation has already been accepted or canceled");
        }

        // --- CanClientProposeNewPrice Method ---
        [Theory]
        [InlineData(NegotiationStatus.PendingClientOffer, true)]
        [InlineData(NegotiationStatus.RejectedByEmployee, true)]
        [InlineData(NegotiationStatus.ClientOffered, false)]
        [InlineData(NegotiationStatus.AcceptedByEmployee, false)]
        [InlineData(NegotiationStatus.Cancelled, false)]
        public void CanClientProposeNewPrice_ShouldReturnCorrectlyBasedOnStatus(NegotiationStatus status, bool expected)
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);
            SetNegotiationStatus(negotiation, status);

            // Act
            var result = negotiation.CanClientProposeNewPrice();

            // Assert
            result.Should().Be(expected);
        }

        // --- IsExpiredForClientResponse Method ---
        [Fact]
        public void IsExpiredForClientResponse_ShouldReturnFalse_WhenNotRejectedByEmployee()
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice); // PendingClientOffer status

            // Act
            var result = negotiation.IsExpiredForClientResponse();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsExpiredForClientResponse_ShouldReturnFalse_WhenRejectedButNotExpired()
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);
            negotiation.ProposePrice(90m);
            negotiation.RejectOffer(); 

            // Act
            var result = negotiation.IsExpiredForClientResponse();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsExpiredForClientResponse_ShouldReturnTrue_WhenRejectedAndExpired()
        {
            // Arrange
            var negotiation = new Negotiation(_testProductId, InitialPrice);
            negotiation.ProposePrice(90m);
            negotiation.RejectOffer();

            SetEmployeeResponseDate(negotiation, DateTime.UtcNow.AddDays(-(TestClientResponseDays + 1))); // response was n+1 days ago

            // Act
            var result = negotiation.IsExpiredForClientResponse();

            // Assert
            result.Should().BeTrue();
        }


        // reflection for testing
        private void SetNegotiationStatus(Negotiation negotiation, NegotiationStatus status)
        {
            var property = typeof(Negotiation).GetProperty(nameof(Negotiation.Status));
            property.SetValue(negotiation, status);
        }

        private void SetEmployeeResponseDate(Negotiation negotiation, DateTime? date)
        {
            var property = typeof(Negotiation).GetProperty(nameof(Negotiation.EmployeeResponseDate));
            property.SetValue(negotiation, date);
        }
    }
}