using ClientLib.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFUI.ViewModel
{
    internal class RegisterViewModel : ViewModelBase
    {
        private AccountOperationManager _accountOperations;
        private ISessionManager _sessionManager;

        private string? _password;
        private string? _confirmPassword;

        private string? _emailAddress;

        public string? EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; OnPropertyChanged(); }
        }

        public DelegateCommand RegisterCommand { get; private set; }
        public DelegateCommand LoginCommand { get; private set; }

        public event EventHandler<EventArgs>? ShowLoginPage;

        public RegisterViewModel(AccountOperationManager accountOperations, ISessionManager sessionManager)
        {
            _accountOperations = accountOperations;
            _sessionManager = sessionManager;
            RegisterCommand = new DelegateCommand((param) =>
            {
                if (param is PasswordBoxPair pws)
                {
                    Register(pws.Password!.Password, pws.ConfirmPassword!.Password);

                }
            });

            LoginCommand = new DelegateCommand((param) =>
            {
                ShowLoginPage?.Invoke(this, EventArgs.Empty);
            });
        }

        private async void Register(string? password, string? confirmPassword)
        {
            if (!ValidateInput(password, confirmPassword))
            {
                return;
            }

            if (await _accountOperations.Register(EmailAddress!, password!))
            {
                string? token = (await _sessionManager.GetAuthenticationToken()) ?? "";
                MessageBox.Show($"Successful registration and login.\nToken: {token}");
            }
            else
            {
                MessageBox.Show("Unsuccessful registration.");
            }
        }

        private bool ValidateInput(string? password, string? confirmPassword)
        {
            if (string.IsNullOrEmpty(EmailAddress) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                return false;
            }
            if (password != confirmPassword)
            {
                return false;
            }
            return true;
        }
    }
}
