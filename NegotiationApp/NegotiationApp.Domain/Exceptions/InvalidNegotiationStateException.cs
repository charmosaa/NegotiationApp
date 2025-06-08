
namespace NegotiationApp.Domain.Exceptions
{
    public class InvalidNegotiationStateException : Exception
    {
        public InvalidNegotiationStateException(string message) : base(message) { }
    }
}
