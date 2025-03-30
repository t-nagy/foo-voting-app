using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Authentication
{
    public abstract class ApiCaller
    {
        protected readonly ISessionManager _sessionManager;

        public event EventHandler? LoginRequired;

        protected ApiCaller(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            _sessionManager.LoginRequiredEvent += PropagateLoginRequiredEvent;
        }

        private void PropagateLoginRequiredEvent(object? sender, EventArgs e)
        {
            LoginRequired?.Invoke(sender, e);
        }
    }
}
