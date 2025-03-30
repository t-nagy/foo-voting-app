using ClientLib.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.ViewModel
{
    class ElectionsViewModel : ViewModelBase
    {
        private readonly ISessionManager _sessionManager;

        public ElectionsViewModel(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }


        public string? UserEmail
        {
            get { return $"{_sessionManager.LoggedInEmail}!"; }
        }

    }
}
