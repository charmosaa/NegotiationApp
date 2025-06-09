
namespace NegotiationApp.Domain.Exceptions
{
    public class NegotiationNotFoundException : Exception
    {
        public NegotiationNotFoundException(string message) : base(message) { }
    }
}
