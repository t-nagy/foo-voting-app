using ClientLib;
using ClientLib.DataManagers;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFUI.ViewModel
{
    class ParticipantsViewModel : ViewModelBase
    {
        private readonly IParticipantManager _participantManager;

        public PollModel Poll { get; }

        public DelegateCommand AccountSettingsCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }
        public DelegateCommand CopyCommand { get; private set; }
        public DelegateCommand AddParticipantCommand { get; private set; }


        public event EventHandler? ShowAccountSettingsPage;
        public event EventHandler<PollModel>? ShowPollDetailPage;

        public ParticipantsViewModel(IParticipantManager participantManager, PollModel poll)
        {
            _participantManager = participantManager;
            Poll = poll;
            
            AccountSettingsCommand = new DelegateCommand((param) =>
            {
                ShowAccountSettingsPage?.Invoke(this, EventArgs.Empty);
            });

            CloseCommand = new DelegateCommand((param) =>
            {
                ShowPollDetailPage?.Invoke(this, Poll);
            });

            CopyCommand = new DelegateCommand((param) =>
            {
                Clipboard.SetText(Poll.JoinCode);
            }); 

            AddParticipantCommand = new DelegateCommand(async (param) =>
            {
                if (ValidateEmail())
                {
                    await AddParticipant();
                }
            });

            RefreshParticipantsList();
        }

        private void RefreshParticipantsList()
        {
            ParticipantsList = new ObservableCollection<ParticipantModel>(Poll.Participants!);
            OnPropertyChanged("ParticipantsList");
        }

        public ObservableCollection<ParticipantModel> ParticipantsList { get; set; } = new ObservableCollection<ParticipantModel>();

        private string _addEmail = string.Empty;

        public string AddEmail
        {
            get { return _addEmail; }
            set { _addEmail = value; OnPropertyChanged(); }
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


        public string JoinCode
        {
            get { return Poll.JoinCode!; }
        }


        private async Task AddParticipant()
        {
            var participantToAdd = new ParticipantModel { Username = AddEmail, PollId = Poll.Id, Role = SharedLibrary.PollRole.Voter };
            bool addResult;
            try
            {
                addResult = await _participantManager.AddParticipant(participantToAdd);
            }
            catch (ServerUnreachableException ex)
            {
                ErrorText = ex.Message;
                IsErrorTextVisible = true;
                return;
            }

            if (addResult)
            {
                Poll.Participants!.Add(participantToAdd);
                AddEmail = string.Empty;
                RefreshParticipantsList();
            }
            else
            {
                ErrorText = "Could not add participant. Please make sure the email address you entered is correct!";
                IsErrorTextVisible = true;
            }
            
        }

        private bool ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(AddEmail))
            {
                ErrorText = "Participant email address cannot be empty. Please enter a valid email address!";
                IsErrorTextVisible = true;
                return false;
            }
            if (Poll.Participants!.Any(x => x.Username == AddEmail))
            {
                ErrorText = "This participant already has access to this poll!";
                IsErrorTextVisible = true;
                return false;
            }

            IsErrorTextVisible = false;
            return true;
        }

    }
}
