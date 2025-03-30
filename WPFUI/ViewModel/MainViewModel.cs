using ClientLib.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFUI.View;

namespace WPFUI.ViewModel
{
    internal class MainViewModel : ViewModelBase
    {
        #region Private fields
        private readonly IAccountOperationManager _accountManager;
        private Page? _returnPage;
        #endregion

        private Page? _activePage;

        public Page? ActivePage
        {
            get { return _activePage; }
            set { _activePage = value; OnPropertyChanged(); }
        }


        #region Pages and ViewmMdels
        private readonly LoginPage _loginPage;
        private LoginViewModel? _loginViewModel;

        private readonly RegisterPage _registerPage;
        private RegisterViewModel? _registerViewModel;

        private readonly RegistrationCompletePage _registrationCompletePage;
        private RegistrationCompleteViewModel? _registrationCompleteViewModel;

        private readonly ElectionsPage _electionsPage;
        private ElectionsViewModel? _electionsViewModel;

        private readonly ForgotPasswordPage _forgotPasswordPage;
        private ForgotPasswordViewModel? _forgotPasswordViewModel;

        private readonly AccountSettingsPage _accountSettingsPage;
        private AccountSettingsViewModel? _accountSettingsViewModel;
        #endregion


        public MainViewModel(IAccountOperationManager accountManager)
        {
            _accountManager = accountManager;
            _accountManager.LoginRequired += LoginRequiredEvent;

            _loginPage = new LoginPage();
            _registerPage = new RegisterPage();
            _registrationCompletePage = new RegistrationCompletePage();
            _electionsPage = new ElectionsPage();
            _forgotPasswordPage = new ForgotPasswordPage();
            _accountSettingsPage = new AccountSettingsPage();

            ShowLogin(true);
        }

        private void ResetViewModels()
        {
            _loginViewModel = null;
            _registerViewModel = null;
            _registrationCompleteViewModel = null;
            _electionsViewModel = null;
            _forgotPasswordViewModel = null;
            _accountSettingsViewModel = null;
        }

        #region Show page methods
        private void ShowLogin(bool clear)
        {
            if (_loginViewModel == null || clear)
            {
                _loginViewModel = new LoginViewModel(_accountManager);
                _loginViewModel.ShowRegisterPage += ShowRegisterEvent;
                _loginViewModel.ShowElectionsPage += ShowElectionsPageEvent;
                _loginViewModel.ShowForgotPasswordPage += ShowForgotPasswordEvent;
                _loginPage.DataContext = _loginViewModel;
            }

            ActivePage = _loginPage;
        }

        private void ShowRegister()
        {
            _registerViewModel = new RegisterViewModel(_accountManager);
            _registerViewModel.ShowLoginPage += ShowLoginEvent;
            _registerViewModel.RegistrationComplete += ShowRegistrationCompleteEvent;
            _registerPage.DataContext = _registerViewModel;

            ActivePage = _registerPage;

        }

        private void ShowRegistrationComplete()
        {
            if (_registrationCompleteViewModel == null)
            {
                _registrationCompleteViewModel = new RegistrationCompleteViewModel();
                _registrationCompleteViewModel.ShowLoginPage += ShowLoginEvent;
                _registrationCompletePage.DataContext = _registrationCompleteViewModel;
            }

            ActivePage = _registrationCompletePage;
        }

        private void ShowElectionsPage()
        {
            if (_registerViewModel == null)
            {
                _electionsViewModel = new ElectionsViewModel();
                _electionsViewModel.ShowAccountSettingsPage += ShowAccountSettingsPageEvent;
                _electionsPage.DataContext = _electionsViewModel;
            }

            ActivePage = _electionsPage;
        }

        private void ShowForgotPassword(string email = "")
        {
            _forgotPasswordViewModel = new ForgotPasswordViewModel(_accountManager);
            _forgotPasswordViewModel.EmailAddress = email;
            _forgotPasswordViewModel.ShowLoginPage += ShowLoginEvent;
            _forgotPasswordPage.DataContext = _forgotPasswordViewModel;

            ActivePage = _forgotPasswordPage;
        }

        private void ShowAccountSettingsPage()
        {
            _returnPage = ActivePage;
            _accountSettingsViewModel = new AccountSettingsViewModel(_accountManager);
            _accountSettingsViewModel.CloseAccountSettingsPageEvent += CloseAccountSettingsPageEvent;
            _accountSettingsPage.DataContext = _accountSettingsViewModel;
            ActivePage = _accountSettingsPage;
        }
        #endregion


        #region Show page events
        private void ShowLoginEvent(object? sender, bool clear)
        {
            ShowLogin(clear);
        }

        private void ShowRegisterEvent(object? sender, EventArgs e)
        {
            ShowRegister();
        }

        private void ShowRegistrationCompleteEvent(object? sender, EventArgs e)
        {
            _registerViewModel = null;
            ShowRegistrationComplete();
        }

        private void ShowElectionsPageEvent(object? sender, EventArgs e)
        {
            ShowElectionsPage();
        }

        private void LoginRequiredEvent(object? sender, EventArgs e)
        {
            ResetViewModels();
            _returnPage = null;

            ShowLogin(true);
        }

        private void ShowForgotPasswordEvent(object? sender, string email)
        {
            ShowForgotPassword(email);
        }

        private void ShowAccountSettingsPageEvent(object? sender, EventArgs e)
        {
            ShowAccountSettingsPage();
        }

        private void CloseAccountSettingsPageEvent(object? sender, EventArgs e)
        {
            _accountSettingsViewModel = null;
            ActivePage = _returnPage;
        }
        #endregion
    }
}
