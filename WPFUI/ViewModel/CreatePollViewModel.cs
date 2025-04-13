using ClientLib;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFUI.ViewModel
{
    public class CreatePollViewModel : ViewModelBase
    {
        private readonly IPollManager _pollManager;

        public DelegateCommand AccountSettingsCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }
        public DelegateCommand AddOptionCommand { get; private set; }
        public DelegateCommand DeleteOptionCommand { get; private set; }
        public DelegateCommand CreatePollCommand { get; set; }


        public event EventHandler? ShowAccountSettingsPage;
        public event EventHandler? CancelPollCreation;
        public event EventHandler<PollModel>? PollCreated;

        public CreatePollViewModel(IPollManager pollManager)
        {
            _pollManager = pollManager;

            AccountSettingsCommand = new DelegateCommand((param) =>
            {
                ShowAccountSettingsPage?.Invoke(this, EventArgs.Empty);
            });

            CloseCommand = new DelegateCommand((param) =>
            {
                CancelPollCreation?.Invoke(this, EventArgs.Empty);
            });

            AddOptionCommand = new DelegateCommand((param) =>
            {
                if (ValidateAddOption())
                {
                    OptionsList.Add(new OptionModel { OptionText = AddOptionText });
                    AddOptionText = string.Empty;
                    IsErrorTextVisible = false;
                }
            });

            DeleteOptionCommand = new DelegateCommand((param) =>
            {
                if (param is OptionModel option)
                {
                    OptionsList.Remove(option);
                }
            });

            CreatePollCommand = new DelegateCommand((param) =>
            {
                if (ValidatePoll())
                {
                    CreatePoll();
                }
            });
        }

        public ObservableCollection<OptionModel> OptionsList { get; set; } = new ObservableCollection<OptionModel>();

        #region Binding properties

        private string _title = string.Empty;

        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(); }
        }

        private string _description = string.Empty;

        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged(); }
        }

        private DateTime _voteCollectionEnd;

        public DateTime VoteCollectionEnd
        {
            get { return _voteCollectionEnd; }
            set { _voteCollectionEnd = value.Date.AddHours(23).AddMinutes(59).AddSeconds(59); OnPropertyChanged(); }
        }

        private DateTime _voteValidationEnd;

        public DateTime VoteValidationEnd
        {
            get { return _voteValidationEnd; }
            set { _voteValidationEnd = value.Date.AddHours(23).AddMinutes(59).AddSeconds(59); OnPropertyChanged(); }
        }

        public DateTime DisplayDateStartCollection
        {
            get
            {
                return DateTime.Now.AddDays(2);
            }
        }

        public DateTime DisplayDateStartValidation
        {
            get
            {
                return DateTime.Now.AddDays(3);
            }
        }

        private bool _isPublic = false;

        public bool IsPublic
        {
            get { return _isPublic; }
            set { _isPublic = value; OnPropertyChanged(); }
        }

        private string _addOptionText = string.Empty;

        public string AddOptionText
        {
            get { return _addOptionText; }
            set { _addOptionText = value; OnPropertyChanged(); }
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

        private bool _isCreateButtonEnabled = true;

        public bool IsCreateButtonEnabled
        {
            get { return _isCreateButtonEnabled; }
            set { _isCreateButtonEnabled = value; OnPropertyChanged(); }
        }

        #endregion

        private async void CreatePoll()
        {
            PollModel poll = new PollModel()
            {
                Title = this.Title,
                IsPublic = this.IsPublic,
                VoteCollectionEndDate = this.VoteCollectionEnd.ToUniversalTime(),
                VoteValidationEndDate = this.VoteValidationEnd.ToUniversalTime(),
            };

            poll.Description = !string.IsNullOrWhiteSpace(this.Description) ? this.Description : null;
            poll.PollOptions = OptionsList.ToList();

            IsCreateButtonEnabled = false;
            PollModel? pollWithId;
            try
            {
                pollWithId = await _pollManager.CreatePoll(poll);
            }
            catch (ServerUnreachableException ex)
            {
                ErrorText = ex.Message;
                IsErrorTextVisible = true;
                IsCreateButtonEnabled = true;
                return;
            }
            IsCreateButtonEnabled = true;

            if (pollWithId == null)
            {
                ErrorText = "An unknown error has occoured while trying to create your poll. Please try again!\n";
                IsErrorTextVisible = true;
                return;
            }
            Console.WriteLine(poll);
            PollCreated?.Invoke(this, pollWithId);
        }

        private bool ValidateAddOption()
        {
            bool isValid = true;
            ErrorText = string.Empty;
            if (string.IsNullOrWhiteSpace(AddOptionText))
            {
                ErrorText += "Cannot add a poll option without any text!\n";
                isValid = false;
            }
            if (AddOptionText.Length > 200)
            {
                ErrorText += "Poll options must be at most 200 characters!\n";
                isValid = false;
            }
            if (OptionsList.Any(x => x.OptionText == AddOptionText))
            {
                ErrorText += "You have already added this poll option!\n";
                isValid = false;
            }

            IsErrorTextVisible = !isValid;
            return isValid;
        }

        private bool ValidatePoll()
        {
            bool isValid = true;
            ErrorText = string.Empty;
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorText += "Poll title cannot be empty!\n";
                isValid = false;
            }
            if (Title.Length < 4 || Title.Length > 200)
            {
                ErrorText += "Title must be between 4 and 200 characters!\n";
                isValid = false;
            }
            if (Description.Length > 500)
            {
                ErrorText += "The description cannot be more than 500 characters!\n";
                isValid = false;
            }
            if (DateTime.Now.AddHours(48) > VoteCollectionEnd)
            {
                ErrorText += "The end date of the vote collection period cannot be within the next 2 days!\n";
                isValid = false;
            }
            if (VoteCollectionEnd.AddHours(24) > VoteValidationEnd)
            {
                ErrorText += "The end date of the vote validation period cannot must be at least 24 hours after the vote collection period has ended!\n";
                isValid = false;
            }
            if (OptionsList.Count < 2)
            {
                ErrorText += "You must create least two poll options to create the poll!\n";
                isValid = false;
            }

            IsErrorTextVisible = !isValid;
            return isValid;
        }
    }
}
