
namespace NegotiationApp.Domain.Exceptions
{
    public class InvalidProposedPriceException : Exception
    {
        public InvalidProposedPriceException (string message) : base(message) { }
    }
}
