using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NegotiationApp.Domain.Exceptions
{
    public class ClientResponseTimeExceededException : Exception
    {
        public ClientResponseTimeExceededException(string message) : base(message) { }
    }
}
