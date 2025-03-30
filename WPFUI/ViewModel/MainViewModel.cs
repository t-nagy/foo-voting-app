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
        private ISessionManager _sessionManager;
        private IAccountOperationManager _accountManager;
        #endregion

        private Page? _activePage;

        public Page? ActivePage
        {
            get { return _activePage; }
            set { _activePage = value; OnPropertyChanged(); }
        }


        #region Pages and ViewmMdels
        private LoginPage? _loginPage;
        private LoginViewModel? _loginViewModel;

        private RegisterPage? _registerPage;
        private RegisterViewModel? _registerViewModel;

        private RegistrationCompletePage? _registrationCompletePage;
        private RegistrationCompleteViewModel? _registrationCompleteViewModel;

        private ElectionsPage? _electionsPage;
        private ElectionsViewModel? _electionsViewModel;

        private ForgotPasswordPage? _forgotPasswordPage;
        private ForgotPasswordViewModel? _forgotPasswordViewModel;
        #endregion


        public MainViewModel(ISessionManager sessionManager, IAccountOperationManager accountManager)
        {
            _sessionManager = sessionManager;
            _accountManager = accountManager;
            _sessionManager.LoginRequiredEvent += LoginRequiredEvent;

            //_activePage = new ElectionsPage();

            ShowLogin(true);
        }


        #region Show page methods
        private void ShowLogin(bool clear)
        {
            if (_loginPage == null || _loginViewModel == null || clear)
            {
                _loginViewModel = new LoginViewModel(_sessionManager, _accountManager);
                _loginPage = new LoginPage();
                _loginViewModel.ShowRegisterPage += ShowRegisterEvent;
                _loginViewModel.ShowElectionsPage += ShowElectionsPageEvent;
                _loginViewModel.ShowForgotPasswordPage += ShowForgotPasswordEvent;
                _loginPage.DataContext = _loginViewModel;
            }

            ActivePage = _loginPage;
        }

        private void ShowRegister()
        {
            _registerPage = new RegisterPage();
            _registerViewModel = new RegisterViewModel(_accountManager);
            _registerViewModel.ShowLoginPage += ShowLoginEvent;
            _registerViewModel.RegistrationComplete += ShowRegistrationCompleteEvent;
            _registerPage.DataContext = _registerViewModel;

            ActivePage = _registerPage;

        }

        private void ShowRegistrationComplete()
        {
            if (_registrationCompletePage == null || _registrationCompleteViewModel == null)
            {
                _registrationCompletePage = new RegistrationCompletePage();
                _registrationCompleteViewModel = new RegistrationCompleteViewModel();
                _registrationCompleteViewModel.ShowLoginPage += ShowLoginEvent;
                _registrationCompletePage.DataContext = _registrationCompleteViewModel;
            }

            ActivePage = _registrationCompletePage;
        }

        private void ShowElectionsPage()
        {
            if (_electionsPage == null || _registerViewModel == null)
            {
                _electionsPage = new ElectionsPage();
                _electionsViewModel = new ElectionsViewModel(_sessionManager);
                _electionsPage.DataContext = _electionsViewModel;
            }

            ActivePage = _electionsPage;
        }

        private void ShowForgotPassword(string email = "")
        {
            if (_forgotPasswordPage == null || _forgotPasswordViewModel == null)
            {
                _forgotPasswordPage = new ForgotPasswordPage();
                _forgotPasswordViewModel = new ForgotPasswordViewModel(_accountManager);
                _forgotPasswordViewModel.EmailAddress = email;
                _forgotPasswordViewModel.ShowLoginPage += ShowLoginEvent;
                _forgotPasswordPage.DataContext = _forgotPasswordViewModel;
            }

            ActivePage = _forgotPasswordPage;
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
            ShowLogin(true);
        }

        private void ShowForgotPasswordEvent(object? sender, string email)
        {
            ShowForgotPassword(email);
        }
        #endregion
    }
}
