using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Authentication
{
    public enum LoginResponse
    {
        Success,
        InvalidCredentials,
        EmailNotConfirmed,
        UnknownFailure
    }
}
