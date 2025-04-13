using ClientLib;
using ClientLib.Authentication;
using SharedLibrary;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFUI.DisplayModels;

namespace WPFUI.ViewModel
{
    class PollsViewModel : ViewModelBase
    {
        private readonly IPollManager _pollManager;
        private List<PollModel>? _realPolls;

        public DelegateCommand AccountSettingsCommand { get; private set; }
        public DelegateCommand CreatePollCommand { get; private set; }
        public DelegateCommand JoinWithCodeCommand { get; private set; }
        public DelegateCommand ClearFiltersCommand { get; private set; }
        public DelegateCommand RefreshCommand { get; private set; }

        public event EventHandler? ShowAccountSettingsPage;
        public event EventHandler? ShowCreateNewPollPage;
        public event EventHandler? ShowJoinWithCodePage;
        public event EventHandler<PollModel>? ShowPollDetailPage;

        public PollsViewModel(IPollManager pollManager)
        {
            _pollManager = pollManager;
            StatusOptions = new List<string>();
            StatusOptions.Add("Any");
            StatusOptions.Add("Voting");
            StatusOptions.Add("Validation");
            StatusOptions.Add("Concluded");

            AccountSettingsCommand = new DelegateCommand((param) =>
            {
                ShowAccountSettingsPage?.Invoke(this, EventArgs.Empty);
            });

            CreatePollCommand = new DelegateCommand((param) =>
            {
                ShowCreateNewPollPage?.Invoke(this, EventArgs.Empty);
            });

            JoinWithCodeCommand = new DelegateCommand((param) =>
            {
                ShowJoinWithCodePage?.Invoke(this, EventArgs.Empty);
            });

            ClearFiltersCommand = new DelegateCommand((param) =>
            {
                OwnerFilter = string.Empty;
                TitleFilter = string.Empty;
                AllowPublic = true;
                SelectedStatusOption = "Any";
                StatusFilter = -1;
            });

            RefreshCommand = new DelegateCommand(async (param) =>
            {
                await RefreshPolls();
            });

            RefreshPolls();
        }

        public ObservableCollection<PollDisplayModel> Polls { get; private set; } = new ObservableCollection<PollDisplayModel>();

        private async Task RefreshPolls()
        {
            try
            {
                _realPolls = (await _pollManager.GetAllPollsMinimal())?.ToList();
            }
            catch (ServerUnreachableException ex)
            {
                ErrorText = ex.Message;
                IsErrorTextVisible = true;
                return;
            }

            Polls = new ObservableCollection<PollDisplayModel>();
            if (_realPolls != null)
            {
                foreach (var p in _realPolls)
                {
                    Polls.Add(new PollDisplayModel(p));
                } 
            }
            OnPropertyChanged("Polls");
            FilterPolls();
        }

        private PollDisplayModel? _selectedPoll;

        public PollDisplayModel? SelectedPoll
        {
            get { return _selectedPoll; }
            set 
            { 
                _selectedPoll = value;
                OpenPoll();
            }
        }

        private string _ownerFilter = string.Empty;

        public string OwnerFilter
        {
            get { return _ownerFilter; }
            set { _ownerFilter = value; OnPropertyChanged(); FilterPolls(); }
        }

        private string _titleFilter = string.Empty;

        public string TitleFilter
        {
            get { return _titleFilter; }
            set { _titleFilter = value; OnPropertyChanged(); FilterPolls(); }
        }

        private bool _allowPublic = true;

        public bool AllowPublic
        {
            get { return _allowPublic; }
            set { _allowPublic = value; OnPropertyChanged(); FilterPolls(); }
        }

        public List<string> StatusOptions { get; }

        private string _selectedStatusOption = "Any";

        public string SelectedStatusOption
        {
            get { return _selectedStatusOption; }
            set 
            { 
                _selectedStatusOption = value;
                switch (value)
                {
                    case "Any":
                        StatusFilter = -1;
                        break;
                    case "Voting":
                        StatusFilter = (int)PollStatus.Vote;
                        break;
                    case "Validation":
                        StatusFilter = (int)PollStatus.Validate;
                        break;
                    case "Concluded":
                        StatusFilter = (int)PollStatus.Closed;
                        break;
                    default:
                        throw new ArgumentException();
                }
                OnPropertyChanged();
            }
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


        private int _statusFilter = -1;

        public int StatusFilter
        {
            get { return _statusFilter; }
            set { _statusFilter = value; FilterPolls(); }
        }


        private void FilterPolls()
        {
            Polls = new ObservableCollection<PollDisplayModel>();
            if (_realPolls != null)
            {
                foreach (var p in _realPolls)
                {
                    if (p.OwnerName!.ToLower().Contains(OwnerFilter.ToLower()) && p.Title.ToLower().Contains(TitleFilter.ToLower()) && (!p.IsPublic || AllowPublic) && (StatusFilter == -1 || (int)p.Status == StatusFilter))
                    {
                        Polls.Add(new PollDisplayModel(p));
                    }
                }
            }
            OnPropertyChanged("Polls");
        }

        private async Task OpenPoll()
        {
            PollModel? pollToOpen;
            try
            {
                pollToOpen = await _pollManager.GetPollById(SelectedPoll!.Id);
            }
            catch (ServerUnreachableException ex)
            {
                ErrorText = ex.Message;
                IsErrorTextVisible = true;
                return;
            }

            if (pollToOpen == null)
            {
                throw new InvalidOperationException();
            }

            ShowPollDetailPage?.Invoke(this, pollToOpen);
        }
    }
}
