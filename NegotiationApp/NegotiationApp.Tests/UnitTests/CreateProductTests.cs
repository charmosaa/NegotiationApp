using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper; 
using NegotiationApp.Application.Features.Products.Commands;
using NegotiationApp.Application.Validators;

namespace NegotiationApp.Tests.UnitTests.Application.Validators
{
    public class CreateProductCommandValidatorTests
    {
        private readonly CreateProductCommandValidator _validator;

        public CreateProductCommandValidatorTests()
        {
            _validator = new CreateProductCommandValidator();
        }

        [Fact] 
        public void ShouldHaveError_WhenNameIsEmpty()
        {
            // Arrange 
            var command = new CreateProductCommand("", 100.00m); 

            // Act 
            var result = _validator.TestValidate(command); 

            // Assert 
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Product name can not be empty"); 
        }

        [Fact]
        public void ShouldNotHaveError_WhenNameIsProvided()
        {
            // Arrange
            var command = new CreateProductCommand("Valid Product Name", 100.00m);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void ShouldHaveError_WhenBasePriceIsZero()
        {
            // Arrange
            var command = new CreateProductCommand("Test Product", 0m); 

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.BasePrice)
                  .WithErrorMessage("Base price has to be greater than 0");
        }

        [Fact]
        public void ShouldHaveError_WhenBasePriceIsNegative()
        {
            // Arrange
            var command = new CreateProductCommand("Test Product", -10m); 

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.BasePrice)
                  .WithErrorMessage("Base price has to be greater than 0");
        }

        [Fact]
        public void ShouldNotHaveError_WhenBasePriceIsValid()
        {
            // Arrange
            var command = new CreateProductCommand("Test Product", 0.01m); // minimal price
            var command2 = new CreateProductCommand("Test Product", 100.00m); // normal price

            // Act
            var result = _validator.TestValidate(command);
            var result2 = _validator.TestValidate(command2);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.BasePrice);
            result2.ShouldNotHaveValidationErrorFor(x => x.BasePrice);
        }

        [Fact]
        public void ShouldNotHaveAnyValidationErrors_WhenCommandIsValid()
        {
            // Arrange
            var command = new CreateProductCommand("Fully Valid Product", 123.45m);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }
}