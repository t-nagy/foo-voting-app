using ClientLib.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib
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

        private void PropagateLoginRequiredEvent(object? sender, EventArgs e)
        {
            LoginRequired?.Invoke(sender, e);
        }
    }
}
