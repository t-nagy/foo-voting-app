using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public enum Role
    {
        Owner,
        Voter
    }

    public enum ElectionStatus
    {
        Vote,
        Validate,
        Closed
    }
}
