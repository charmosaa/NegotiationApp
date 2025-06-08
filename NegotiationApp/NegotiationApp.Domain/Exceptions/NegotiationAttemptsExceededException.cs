
namespace NegotiationApp.Domain.Exceptions
{
    public class NegotiationAttemptsExceededException : Exception
    {
        public NegotiationAttemptsExceededException (string message) : base(message) { }
    }
}
