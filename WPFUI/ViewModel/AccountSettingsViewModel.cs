using ClientLib.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPFUI.ViewModel
{
    class AccountSettingsViewModel : ViewModelBase
    {
        private readonly IAccountOperationManager _accountManager;

        public DelegateCommand ChangePasswordCommand { get; set; }
        public DelegateCommand CloseAccountSettingsCommand { get; set; }
        public DelegateCommand LogoutCommand { get; set; }


        public event EventHandler? CloseAccountSettingsPageEvent;

        public AccountSettingsViewModel(IAccountOperationManager accountOperationManager)
        {
            _accountManager = accountOperationManager;

            ChangePasswordCommand = new DelegateCommand((param) =>
            {
                if (param is PasswordBoxTriple pws)
                {
                    ChangePassword(pws.oldPassword!.Password, pws.newPassword!.Password, pws.ConfirmNewPassword!.Password);
                }
            });

            CloseAccountSettingsCommand = new DelegateCommand((param) =>
            {
                CloseAccountSettingsPageEvent?.Invoke(this, EventArgs.Empty);
            });

            LogoutCommand = new DelegateCommand((param) =>
            {
                _accountManager.Logout();
            });
        }

        public string? LoggedInText
        {
            get { return $"You are logged in as: {_accountManager.LoggedInEmail}"; }
        }


        private bool _isPasswordChangeTextVisible = false;

        public bool IsPasswordChangeTextVisible
        {
            get { return _isPasswordChangeTextVisible; }
            set { _isPasswordChangeTextVisible = value; OnPropertyChanged(); }
        }

        private string _passwordChangeText = string.Empty;

        public string PasswordChangeText
        {
            get { return _passwordChangeText; }
            set { _passwordChangeText = value; OnPropertyChanged(); }
        }

        private string _passwordChangeColor = "Red";

        public string PasswordChangeColor
        {
            get { return _passwordChangeColor; }
            set { _passwordChangeColor = value; OnPropertyChanged(); }
        }



        private async void ChangePassword(string? oldPassword, string? newPassword, string? newPasswordAgain)
        {
            
            if (!ValidateInput(oldPassword, newPassword, newPasswordAgain))
            {
                PasswordChangeText = "The password fields must not be empty and the new passwords must match!";
                PasswordChangeColor = "Red";
                IsPasswordChangeTextVisible = true;
                return;
            }

            string? errors = await _accountManager.ChangePassword(oldPassword!, newPassword!);
            if (errors == null)
            {
                PasswordChangeText = "Your password change was successful!";
                PasswordChangeColor = "Green";
                IsPasswordChangeTextVisible = true;
            }
            else
            {
                PasswordChangeText = errors.TrimEnd();
                PasswordChangeColor = "Red";
                IsPasswordChangeTextVisible = true;
            }
        }

        private bool ValidateInput(string? oldPassword, string? newPassword, string? newPasswordAgain)
        {
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(newPasswordAgain))
            {
                return false;
            }
            if (newPassword != newPasswordAgain)
            {
                return false;
            }
            return true;
        }
    }
}
