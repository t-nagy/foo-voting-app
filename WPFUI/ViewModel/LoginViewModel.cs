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
        private readonly IAccountOperationManager _accountOperationManager;

        public DelegateCommand LoginCommand { get; private set; }
        public DelegateCommand RegisterCommand { get; private set; }
        public DelegateCommand ResendEmailCommand { get; private set; }
        public DelegateCommand ForgotPasswordCommand { get; private set; }


        public event EventHandler? ShowRegisterPage;
        public event EventHandler? ShowPollsPage;
        public event EventHandler<string>? ShowForgotPasswordPage;


        public LoginViewModel(IAccountOperationManager accountOperationManager)
        {
            _accountOperationManager = accountOperationManager;

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

            ResendEmailCommand = new DelegateCommand((param) =>
            {
                ResendEmailConfirmation();
            });

            ForgotPasswordCommand = new DelegateCommand((param) =>
            {
                ShowForgotPasswordPage?.Invoke(this, EmailAddress);
            });
        }

        private string _emailAddress = string.Empty;

        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; OnPropertyChanged(); }
        }

        private bool _isErrorTextVisible;

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

        private bool _isEmailResendVisible = false;

        public bool IsEmailResendVisible
        {
            get { return _isEmailResendVisible; }
            set { _isEmailResendVisible = value; OnPropertyChanged(); }
        }

        private bool _isForgotPasswordVisible = false;

        public bool IsForgotPasswordVisible
        {
            get { return _isForgotPasswordVisible; }
            set { _isForgotPasswordVisible = value; OnPropertyChanged(); }
        }

        private bool _buttonsEnabled = true;

        public bool ButtonsEnabled
        {
            get { return _buttonsEnabled; }
            set { _buttonsEnabled = value; OnPropertyChanged(); }
        }


        private async void Login(string password)
        {
            ButtonsEnabled = false;
            LoginResponse response = await _accountOperationManager.Login(EmailAddress, password);
            if (response == LoginResponse.Success)
            {
                ShowPollsPage?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                HandleLoginError(response);
            }
            ButtonsEnabled = true;
        }

        private async void ResendEmailConfirmation()
        {
            if (await _accountOperationManager.ResendEmailConfirmation(EmailAddress))
            {
                ErrorText = "A new confirmation link has been sent to your email address.";
                IsEmailResendVisible = false;
            }
            else
            {
                ErrorText = "An error occured while requesting your new confirmation email. Please try again later!";
            }
        }

        private void HandleLoginError(LoginResponse errorStatus)
        {
            ErrorText = string.Empty;
            IsEmailResendVisible = false;
            IsForgotPasswordVisible = false;
            switch (errorStatus)
            {
                case LoginResponse.Success:
                    throw new InvalidOperationException("Login was successful but UI started login error handling");
                case LoginResponse.InvalidCredentials:
                    ErrorText = "The provided email address or password was invalid. If you have forgotten your password use the button below to reset it!";
                    IsForgotPasswordVisible = true;
                    break;
                case LoginResponse.EmailNotConfirmed:
                    ErrorText = "Your email address has not yet been confirmed. Please confirm it or request a new confirmation link using the button below!";
                    IsEmailResendVisible = true;
                    break;
                case LoginResponse.UnknownFailure:
                    ErrorText = "An unknown failure has occured during login. Please try again later!";
                    break;
                default:
                    break;
            }
            IsErrorTextVisible = true;
        }

    }
}
