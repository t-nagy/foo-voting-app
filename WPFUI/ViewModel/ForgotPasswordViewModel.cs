using ClientLib;
using ClientLib.Authentication;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WPFUI.ViewModel
{
    class ForgotPasswordViewModel : ViewModelBase
    {
        private readonly IAccountOperationManager _accountOperationManager;

        public DelegateCommand RequestResetTokenCommand { get; private set; }
        public DelegateCommand ResetPasswordCommand { get; private set; }
        public DelegateCommand LoginCommand { get; private set; }

        public event EventHandler<bool>? ShowLoginPage;
        

        public ForgotPasswordViewModel(IAccountOperationManager accountOperationManager)
        {
            _accountOperationManager = accountOperationManager;

            RequestResetTokenCommand = new DelegateCommand((param) =>
            {
                RequestResetToken();
            });

            ResetPasswordCommand = new DelegateCommand((param) =>
            {
                if (param is PasswordBoxPair pws)
                {
                    ResetPassword(pws.Password!.Password, pws.ConfirmPassword!.Password);
                }
            });

            LoginCommand = new DelegateCommand((param) =>
            {
                ShowLoginPage?.Invoke(this, false);
            });
        }


        private string _emailAddress = string.Empty;

		public string EmailAddress
		{
			get { return _emailAddress; }
			set { _emailAddress = value; OnPropertyChanged(); }
        }

        private string _resetToken = string.Empty;

        public string ResetToken
        {
            get { return _resetToken; }
            set { _resetToken = value; OnPropertyChanged(); }
        }


        private bool _isForgotNoticeVisible = false;

        public bool IsForgotNoticeVisible
        {
            get { return _isForgotNoticeVisible; }
            set { _isForgotNoticeVisible = value; OnPropertyChanged(); }
        }


        private string _forgotNoticeColor = "Red";

        public string ForgotNoticeColor
        {
            get { return _forgotNoticeColor; }
            set { _forgotNoticeColor = value; OnPropertyChanged(); }
        }

        private string _forgotNoticeText = "";

        public string ForgotNoticeText
        {
            get { return _forgotNoticeText; }
            set { _forgotNoticeText = value; OnPropertyChanged(); }
        }

        private bool _isErrorTextVisible = false;

        public bool IsErrorTextVisible
        {
            get { return _isErrorTextVisible; }
            set { _isErrorTextVisible = value; OnPropertyChanged(); }
        }

        private string _errorText = "";

        public string ErrorText
        {
            get { return _errorText; }
            set { _errorText = value; OnPropertyChanged(); }
        }

        private bool _buttonsEnabled = true;

        public bool ButtonsEnabled
        {
            get { return _buttonsEnabled; }
            set { _buttonsEnabled = value; OnPropertyChanged(); }
        }

        private async void RequestResetToken()
        {
            ButtonsEnabled = false;
            bool result;
            try
            {
                result = await _accountOperationManager.ForgotPassword(EmailAddress);
            }
            catch (ServerUnreachableException ex)
            {
                ErrorText = ex.Message;
                IsErrorTextVisible = true;
                ButtonsEnabled = true;
                return;
            }

            if (result)
            {
                ForgotNoticeText = "An email containing your reset code has been sent to your email adress.";
                ForgotNoticeColor = "Green";
            }
            else
            {
                ForgotNoticeText = "An error occured while requesting your new confirmation email. Please try again later!";
                ForgotNoticeColor = "Red";
            }
            IsForgotNoticeVisible = true;
            ButtonsEnabled = true;
        }

        private async void ResetPassword(string password, string confirmPassword)
        {
            ButtonsEnabled = false;
            if (!ValidateInput(password, confirmPassword))
            {
                ErrorText = "The passwords do not match!";
                IsErrorTextVisible = true;
                return;
            }

            string? errors;
            try
            {
                errors = await _accountOperationManager.ResetPassword(EmailAddress, ResetToken.Trim(), password);
            }
            catch (ServerUnreachableException ex)
            {
                ErrorText = ex.Message;
                IsErrorTextVisible = true;
                ButtonsEnabled = true;
                return;
            }

            if (errors == null)
            {
                IsErrorTextVisible = false;
                MessageBox.Show("Your password has been successfully reset!", "Password reset", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowLoginPage?.Invoke(this, true);
            }
            else
            {
                ErrorText = errors.TrimEnd();
                IsErrorTextVisible = true;
            }
            ButtonsEnabled = true;
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
