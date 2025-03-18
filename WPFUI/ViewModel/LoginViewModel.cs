using ClientLib.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFUI.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private ISessionManager _sessionManager;
        public DelegateCommand LoginCommand { get; private set; }
        public DelegateCommand RegisterCommand { get; private set; }

        public event EventHandler<EventArgs>? ShowRegisterPage;

        public LoginViewModel(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;

            LoginCommand = new DelegateCommand((param) =>
            {
                if (param is PasswordBox pass && pass.Password != null)
                {
                    Login(pass.Password);
                }
            });

            RegisterCommand = new DelegateCommand((param) =>
            {
                ShowRegisterPage?.Invoke(this, EventArgs.Empty);
            });
        }

        private string? _emailAddress;

        public string? EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; OnPropertyChanged(); }
        }


        private async void Login(string password)
        {
            if (string.IsNullOrEmpty(EmailAddress))
            {
                return;
            }
            BearerSessionManager sessionManager = new BearerSessionManager();
            bool loginSuccess = await sessionManager.StartNewSession(EmailAddress, password);
            string? token = await sessionManager.GetAuthenticationToken();
            if (loginSuccess && token != null)
            {
                MessageBox.Show($"Successfully logged in as {EmailAddress}.\nRecieved token: {token}");
            }
            else
            {
                MessageBox.Show("Login unsuccessful");
            }
        }
    }
}
