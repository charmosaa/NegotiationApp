using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegotiationApp.Domain.Exceptions
{
    public class NegotiationNotFoundException : Exception
    {
        public NegotiationNotFoundException(string message) : base(message) { }
    }
}
