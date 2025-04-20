using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib
{
    public class VoteInvalidException : Exception
    {
        public VoteInvalidException() : base()
        {
            
        }

        public VoteInvalidException(string message) : base(message)
        {

        }
    }
}
