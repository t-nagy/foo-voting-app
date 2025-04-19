using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.DataManagers
{
    public interface ILoggedInUserManager
    {
        public string? LoggedInEmail { get; }
    }
}
