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
        private IAccountOperationManager _accountOperations;


        public DelegateCommand RegisterCommand { get; private set; }
        public DelegateCommand LoginCommand { get; private set; }


        public event EventHandler<bool>? ShowLoginPage;
        public event EventHandler? RegistrationComplete;


        public RegisterViewModel(IAccountOperationManager accountOperations)
        {
            _accountOperations = accountOperations;
            RegisterCommand = new DelegateCommand((param) =>
            {
                if (param is PasswordBoxPair pws)
                {
                    Register(pws.Password!.Password, pws.ConfirmPassword!.Password);
                }
            });

            LoginCommand = new DelegateCommand((param) =>
            {
                ShowLoginPage?.Invoke(this, false);
            });
        }


        private string? _emailAddress;

        public string? EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; OnPropertyChanged(); }
        }

        private bool _isErrorTextVisible = false;

        public bool IsErrorTextVisible
        {
            get { return _isErrorTextVisible; }
            set { _isErrorTextVisible = value; OnPropertyChanged(); }
        }

        private string _errorText = string.Empty;

        public string ErrorText
        {
            get { return _errorText; }
            set { _errorText = value; OnPropertyChanged(); }
        }



        private async void Register(string? password, string? confirmPassword)
        {
            if (!ValidateInput(password, confirmPassword))
            {
                ErrorText = "The passwords do not match!";
                IsErrorTextVisible = true;
                return;
            }

            string? errors = await _accountOperations.Register(EmailAddress!, password!);
            if (errors == null)
            {
                RegistrationComplete?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ErrorText = errors.TrimEnd();
                IsErrorTextVisible = true;
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
