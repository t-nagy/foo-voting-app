using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib
{
    public class ServerUnreachableException : Exception
    {
        public ServerUnreachableException() : base()
        {
        }

        public ServerUnreachableException(string? message) : base(message)
        {
        }

        public ServerUnreachableException(string? message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
