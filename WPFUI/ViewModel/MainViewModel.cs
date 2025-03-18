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
        private ISessionManager _sessionManager;
        private AccountOperationManager _accountOperations;

        private Page? _activePage;

        public Page? ActivePage
        {
            get { return _activePage; }
            set { _activePage = value; OnPropertyChanged(); }
        }


        private LoginPage? _loginPage;
        private LoginViewModel? _loginViewModel;

        private RegisterPage? _registerPage;
        private RegisterViewModel? _registerViewModel;

        public MainViewModel(ISessionManager sessionManager, AccountOperationManager accountOperations)
        {
            _sessionManager = sessionManager;
            _accountOperations = accountOperations;
            _sessionManager.LoginRequiredEvent += ShowLoginEvent;

            ShowLogin(true);
        }

        private void ShowLogin(bool clear)
        {
            if (_loginPage == null || _loginViewModel == null)
            {
                _loginViewModel = new LoginViewModel(_sessionManager);
                _loginPage = new LoginPage();
            }
            else if (clear)
            {
                _loginViewModel = new LoginViewModel(_sessionManager);
            }
            _loginViewModel.ShowRegisterPage += ShowRegisterEvent;
            _loginPage.DataContext = _loginViewModel;
            ActivePage = _loginPage;
        }

        private void ShowRegister()
        {
            if (_registerPage == null || _registerViewModel == null)
            {
                _registerPage = new RegisterPage();
                _registerViewModel = new RegisterViewModel(_accountOperations, _sessionManager);
                _registerViewModel.ShowLoginPage += ShowLoginEvent;
            }
            _registerPage.DataContext = _registerViewModel;
            ActivePage = _registerPage;

        }

        private void ShowLoginEvent(object? sender, EventArgs e)
        {
            ShowLogin(true);
        }

        private void ShowRegisterEvent(object? sender, EventArgs e)
        {
            ShowRegister();
        }
    }
}
