using ClientLib.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.DataManagers
{
    public abstract class AdminApiAuthCaller : AdminApiCaller, ILoginRequester
    {
        protected readonly ISessionManager _sessionManager;

        public event EventHandler? LoginRequired;

        protected AdminApiAuthCaller(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            _sessionManager.LoginRequiredEvent += PropagateLoginRequiredEvent;
        }

        public string? LoggedInEmail
        {
            get { return _sessionManager.LoggedInEmail; }
        }

        private void PropagateLoginRequiredEvent(object? sender, EventArgs e)
        {
            LoginRequired?.Invoke(sender, e);
        }
    }
}
