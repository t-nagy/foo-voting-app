using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Authentication
{
    public interface ILoginRequester
    {
        event EventHandler? LoginRequired;
    }
}
