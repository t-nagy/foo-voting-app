using ClientLib.Authentication;
using ClientLib.DataManagers;
using ClientLib.Persistance;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WPFUI.View;

namespace WPFUI.ViewModel
{
    internal class MainViewModel : ViewModelBase
    {
        #region Private fields
        private readonly IAccountOperationManager _accountManager;
        private readonly IPollManager _pollManager;
        private readonly IParticipantManager _participantManager;
        private readonly IVoteAdministrationManager _adminManager;
        private readonly IKeyManager _keyManager;
        private readonly IVoteManager _voteManager;
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

        private readonly PollsPage _pollsPage;
        private PollsViewModel? _pollsViewModel;

        private readonly ForgotPasswordPage _forgotPasswordPage;
        private ForgotPasswordViewModel? _forgotPasswordViewModel;

        private readonly AccountSettingsPage _accountSettingsPage;
        private AccountSettingsViewModel? _accountSettingsViewModel;

        private CreatePollPage _createPollPage;
        private CreatePollViewModel? _createPollViewModel;

        private readonly PollDetailPage _pollDetailPage;
        private PollDetailViewModel? _pollDetailViewModel;

        private readonly ParticipantsPage _participantsPage;
        private ParticipantsViewModel? _participantsViewModel;

        private readonly JoinWithCodePage _joinWithCodePage;
        private JoinWithCodeViewModel? _joinWithCodeViewModel;
        #endregion


        public MainViewModel(IAccountOperationManager accountManager, 
                                IPollManager pollManager, 
                                IParticipantManager participantManager, 
                                IVoteAdministrationManager adminManager, 
                                IKeyManager keyManager,
                                IVoteManager voteManager)
        {
            _accountManager = accountManager;
            _pollManager = pollManager;
            _participantManager = participantManager;
            _adminManager = adminManager;
            _keyManager = keyManager;
            _voteManager = voteManager;

            _accountManager.LoginRequired += LoginRequiredEvent;
            _pollManager.LoginRequired += LoginRequiredEvent;
            _participantManager.LoginRequired += LoginRequiredEvent;
            _adminManager.LoginRequired += LoginRequiredEvent;

            _loginPage = new LoginPage();
            _registerPage = new RegisterPage();
            _registrationCompletePage = new RegistrationCompletePage();
            _pollsPage = new PollsPage();
            _forgotPasswordPage = new ForgotPasswordPage();
            _accountSettingsPage = new AccountSettingsPage();
            _createPollPage = new CreatePollPage();
            _pollDetailPage = new PollDetailPage();
            _participantsPage = new ParticipantsPage();
            _joinWithCodePage = new JoinWithCodePage();

            ShowLogin(true);
            
        }

        private void ResetViewModels()
        {
            _loginViewModel = null;
            _registerViewModel = null;
            _registrationCompleteViewModel = null;
            _pollsViewModel = null;
            _forgotPasswordViewModel = null;
            _accountSettingsViewModel = null;
            _createPollViewModel = null;
            _pollDetailViewModel = null;
            _participantsViewModel = null;
            _joinWithCodeViewModel = null;
        }

        #region Show page methods
        private void ShowLogin(bool clear)
        {
            if (_loginViewModel == null || clear)
            {
                _loginViewModel = new LoginViewModel(_accountManager);
                _loginViewModel.ShowRegisterPage += ShowRegisterEvent;
                _loginViewModel.ShowPollsPage += ShowPollsPageEvent;
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

        private void ShowPollsPage()
        {
            _pollsViewModel = new PollsViewModel(_pollManager);
            _pollsViewModel.ShowAccountSettingsPage += ShowAccountSettingsPageEvent;
            _pollsViewModel.ShowCreateNewPollPage += ShowCreatePollPageEvent;
            _pollsViewModel.ShowJoinWithCodePage += ShowJoinWithCodePageEvent;
            _pollsViewModel.ShowPollDetailPage += ShowPollDetailPageEvent;
            _pollsPage.DataContext = _pollsViewModel;

            ActivePage = _pollsPage;
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

        private void ShowCreatePollPage()
        {
            // Date picker control refuses to reset like other controls so reloading ONLY this page is neccessary
            _createPollPage = new CreatePollPage();
            _createPollViewModel = new CreatePollViewModel(_pollManager);
            _createPollViewModel.ShowAccountSettingsPage += ShowAccountSettingsPageEvent;
            _createPollViewModel.CancelPollCreation += ShowPollsPageEvent;
            _createPollViewModel.PollCreated += ShowPollDetailPageEvent;
            _createPollPage.DataContext = _createPollViewModel;

            ActivePage = _createPollPage;
        }

        private void ShowPollDetailPage(PollModel poll)
        {
            _pollDetailViewModel = new PollDetailViewModel(_pollManager, _voteManager, _keyManager,  poll);
            _pollDetailViewModel.ShowAccountSettingsPage += ShowAccountSettingsPageEvent;
            _pollDetailViewModel.ClosePollDetails += ShowPollsPageEvent;
            _pollDetailViewModel.ShowParticipants += ShowParticipantsPageEvent;
            _pollDetailPage.DataContext = _pollDetailViewModel;

            ActivePage = _pollDetailPage;
        }

        private void ShowParticipantsPage(PollModel poll)
        {
            _participantsViewModel = new ParticipantsViewModel(_participantManager, poll);
            _participantsViewModel.ShowAccountSettingsPage += ShowAccountSettingsPageEvent;
            _participantsViewModel.ShowPollDetailPage += ShowPollDetailPageEvent;
            _participantsPage.DataContext = _participantsViewModel;

            ActivePage = _participantsPage;
        }

        private void ShowJoinWithCodePage()
        {
            _joinWithCodeViewModel = new JoinWithCodeViewModel(_participantManager);
            _joinWithCodeViewModel.ShowAccountSettingsPage += ShowAccountSettingsPageEvent;
            _joinWithCodeViewModel.ShowPollsPageEvent += ShowPollsPageEvent;
            _joinWithCodeViewModel.PollJoinedEvent += ShowPollDetailPageEvent;
            _joinWithCodePage.DataContext = _joinWithCodeViewModel;

            ActivePage = _joinWithCodePage;
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

        private void ShowPollsPageEvent(object? sender, EventArgs e)
        {
            ShowPollsPage();
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

        private void ShowCreatePollPageEvent(object? sender, EventArgs e)
        {
            ShowCreatePollPage();
        }

        private void ShowPollDetailPageEvent(object? sender, PollModel e)
        {
            ShowPollDetailPage(e);
        }

        private void ShowParticipantsPageEvent(object? sender, PollModel e)
        {
            ShowParticipantsPage(e);
        }

        private void ShowJoinWithCodePageEvent(object? sender, EventArgs e)
        {
            ShowJoinWithCodePage();
        }
        #endregion
    }
}
