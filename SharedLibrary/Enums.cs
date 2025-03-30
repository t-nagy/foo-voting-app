using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public enum PollRole
    {
        Owner,
        Voter
    }

    public enum PollStatus
    {
        Vote,
        Validate,
        Closed
    }
}
