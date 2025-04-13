using ClientLib;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace WPFUI.ViewModel
{
    class JoinWithCodeViewModel : ViewModelBase
    {
        private readonly IParticipantManager _participantManager;

        public DelegateCommand CloseCommand { get; }
        public DelegateCommand AccountSettingsCommand { get; }
        public DelegateCommand JoinWithCodeCommand { get; set; }


        public event EventHandler? ShowAccountSettingsPage;
        public event EventHandler? ShowPollsPageEvent;
        public event EventHandler<PollModel>? PollJoinedEvent;

        public JoinWithCodeViewModel(IParticipantManager participantManager)
        {
            _participantManager = participantManager;

            AccountSettingsCommand = new DelegateCommand((param) =>
            {
                ShowAccountSettingsPage?.Invoke(this, EventArgs.Empty);
            });

            CloseCommand = new DelegateCommand((param) =>
            {
                ShowPollsPageEvent?.Invoke(this, EventArgs.Empty);
            });

            JoinWithCodeCommand = new DelegateCommand(async (param) =>
            {
                if (ValidateCode())
                {
                    await JoinPoll();
                }
            });
        }

        private string _joinCode = string.Empty;

        public string JoinCode
        {
            get { return _joinCode; }
            set { _joinCode = value; OnPropertyChanged(); }
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



        private bool ValidateCode()
        {
            if (string.IsNullOrWhiteSpace(JoinCode))
            {
                ErrorText = "Invite code is invalid!";
                IsErrorTextVisible = true;
                return false;
            }

            return true;
        }

        private async Task JoinPoll()
        {
            PollModel? poll;
            try
            {
                poll = await _participantManager.JoinPollWithCode(JoinCode);
            }
            catch (ServerUnreachableException ex)
            {
                ErrorText = ex.Message;
                IsErrorTextVisible = true;
                return;
            }
            if (poll == null)
            {
                ErrorText = "Invite code is invalid or you are already a participant in that poll!";
                IsErrorTextVisible = true;
                return;
            }

            PollJoinedEvent?.Invoke(this, poll);
        }

    }
}
