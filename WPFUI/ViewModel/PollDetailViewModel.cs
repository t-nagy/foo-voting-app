using ClientLib;
using SharedLibrary;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.DisplayModels;

namespace WPFUI.ViewModel
{
    class PollDetailViewModel : ViewModelBase
    {
        private readonly IPollManager _pollManager;
        private readonly ParticipantModel? _currUserAsParticipant;
        public PollModel Poll { get; }

        public DelegateCommand AccountSettingsCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }
        public DelegateCommand PrimaryActionCommand { get; private set; }
        public DelegateCommand ShowParticipantsCommand { get; private set; }

        public event EventHandler? ShowAccountSettingsPage;
        public event EventHandler? ClosePollDetails;
        public event EventHandler<PollModel>? ShowParticipants;

        public PollDetailViewModel(IPollManager pollManager, PollModel poll)
        {
            _pollManager = pollManager;
            Poll = poll;
            ParticipantModel? currUserAsParticipant = poll.Participants?.Find(x => x.Username == _pollManager.LoggedInEmail);
            if (currUserAsParticipant == null && !Poll.IsPublic)
            {
                throw new ArgumentException("Poll participant list must contain at least the poll creator!");
            }
            _currUserAsParticipant = currUserAsParticipant;

            AccountSettingsCommand = new DelegateCommand((param) =>
            {
                ShowAccountSettingsPage?.Invoke(this, EventArgs.Empty);
            });

            CloseCommand = new DelegateCommand((param) =>
            {
                ClosePollDetails?.Invoke(this, EventArgs.Empty);
            });

            PrimaryActionCommand = new DelegateCommand((param) =>
            {
                switch (Poll.Status)
                {
                    case SharedLibrary.PollStatus.Vote:
                        break;
                    case SharedLibrary.PollStatus.Validate:
                        break;
                    case SharedLibrary.PollStatus.Closed:
                        break;
                    default:
                        break;
                }
            });

            ShowParticipantsCommand = new DelegateCommand((param) =>
            {
                ShowParticipants?.Invoke(this, Poll);
            });

            Options = new ObservableCollection<DisplayOptionModel>();
            Poll.PollOptions?.ForEach(x =>
            {
                var displayOption = new DisplayOptionModel(x);
                displayOption.OptionSelected += OptionSelected;
                Options.Add(displayOption);
            });
        }

        public ObservableCollection<DisplayOptionModel> Options { get; }

        public bool HasDescription
        {
            get { return Poll.Description != null; }
        }

        public string PollCreatedText
        {
            get { return Poll.CreatedDate.ToLocalTime().ToString(CultureInfo.CurrentCulture); }
        }

        public string VoteCollectionEndText
        {
            get { return Poll.VoteCollectionEndDate.ToLocalTime().ToString(CultureInfo.CurrentCulture); }
        }

        public string VoteValidationEndText
        {
            get { return Poll.VoteValidationEndDate.ToLocalTime().ToString(CultureInfo.CurrentCulture); }
        }

        public string PollStatusText
        {
            get
            {
                switch (Poll.Status)
                {
                    case SharedLibrary.PollStatus.Vote:
                        return "Voting open";
                    case SharedLibrary.PollStatus.Validate:
                        return "Validation";
                    case SharedLibrary.PollStatus.Closed:
                        return "Concluded";
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public string PollCreatorName
        {
            get
            {
                return Poll.OwnerName!;
            }
        }

        public string PrimaryButtonText
        {
            get
            {
                switch (Poll.Status)
                {
                    case SharedLibrary.PollStatus.Vote:
                        return "Submit Vote";
                    case SharedLibrary.PollStatus.Validate:
                        return "Validate Vote";
                    case SharedLibrary.PollStatus.Closed:
                        return "Validate Vote";
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public bool IsPrimaryButtonEnabled
        {
            get
            {
                switch (Poll.Status)
                {
                    case PollStatus.Vote:
                        if (!Poll.IsPublic)
                        {
                            return !_currUserAsParticipant!.HasVoted;
                        }
                        return true;
                    case PollStatus.Validate:
                        // TODO - Check if app has key required to validate
                        return true;
                    case PollStatus.Closed:
                        return false;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public bool IsParticipantsButtonVisible
        {
            get { return !Poll.IsPublic && _currUserAsParticipant!.Role == PollRole.Owner; }
        }

        private bool _isErrorTextVisible = false;

        public bool IsErrorTextVisible
        {
            get { return _isErrorTextVisible; }
            set { _isErrorTextVisible = value; OnPropertyChanged(); }
        }


        private void OptionSelected(object? sender, DisplayOptionModel e)
        {
            foreach (var option in Options)
            {
                if (option.IsSelected && option != e)
                {
                    option.IsSelected = false;
                }
            }
        }
    }
}
