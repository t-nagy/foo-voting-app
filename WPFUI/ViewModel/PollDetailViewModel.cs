using ClientLib;
using ClientLib.DataManagers;
using ClientLib.Persistance;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using SharedLibrary;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WPFUI.DisplayModels;

namespace WPFUI.ViewModel
{
    class PollDetailViewModel : ViewModelBase
    {
        private readonly IPollManager _pollManager;
        private readonly IVoteManager _voteManager;
        private readonly IKeyManager _keyManager;
        private ParticipantModel? _currUserAsParticipant;

        public PollModel Poll { get; }

        public DelegateCommand AccountSettingsCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }
        public DelegateCommand PrimaryActionCommand { get; private set; }
        public DelegateCommand ShowParticipantsCommand { get; private set; }

        public event EventHandler? ShowAccountSettingsPage;
        public event EventHandler? ClosePollDetails;
        public event EventHandler<PollModel>? ShowParticipants;

        public PollDetailViewModel(IPollManager pollManager, IVoteManager voteManager, IKeyManager keyManager, PollModel poll)
        {
            _pollManager = pollManager;
            _voteManager = voteManager;
            _keyManager = keyManager;
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

            PrimaryActionCommand = new DelegateCommand(async (param) =>
            {
                switch (Poll.Status)
                {
                    case SharedLibrary.PollStatus.Vote:
                        await SubmitVote();
                        break;
                    case SharedLibrary.PollStatus.Validate:
                        await ValidateVote();
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

            if (_currUserAsParticipant != null && _currUserAsParticipant.HasVoted)
            {
                if (_voteManager.TryGetVotedOptionId(Poll.Id, out int optToSelect) && (Poll.Status != PollStatus.Closed || _voteManager.GetValidatedState(Poll.Id) == ValidatedState.Yes))
                {
                    Options.First(x => x.Model.Id == optToSelect).IsSelected = true;
                }
                else
                {
                    if (Poll.Status == PollStatus.Closed)
                    {
                        ErrorText = "To view which option you voted for and to make sure your vote was counted in the poll please log in from the same device you have submitted your vote on!";
                        IsErrorTextVisible = true; 
                    }
                    else
                    {
                        ErrorText = "To view which option you voted for please log in from the same device you have submitted your vote on!";
                        IsErrorTextVisible = true;
                    }
                }
            }

            if (Poll.Status == PollStatus.Closed)
            {
                SetPercentages();
                IsPercentageVisible = true;
            }
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
                        if (_currUserAsParticipant != null)
                        {
                            return !_currUserAsParticipant!.HasVoted;
                        }
                        else
                        {
                            return true;
                        }
                    case PollStatus.Validate:
                        if (_currUserAsParticipant != null && _currUserAsParticipant.HasVoted)
                        {
                            switch (_voteManager.GetValidatedState(Poll.Id))
                            {
                                case ValidatedState.Yes:
                                    IsSuccessfulActionTextVisible = true;
                                    return false;
                                case ValidatedState.No:
                                    return true;
                                case ValidatedState.DifferentMachine:
                                    ErrorText = "You can only validate your vote using the same device you have submitted the vote from!";
                                    return false;
                                default:
                                    throw new InvalidOperationException();
                            } 
                        }
                        else
                        {
                            return false;
                        }
                    case PollStatus.Closed:
                        return false;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public bool CanChangeVoteOption
        {
            get
            {
                if (_currUserAsParticipant != null)
                {
                    return !_currUserAsParticipant.HasVoted;
                }
                else
                {
                    if (Poll.Status == PollStatus.Vote)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public bool IsParticipantsButtonVisible
        {
            get { return !Poll.IsPublic && _currUserAsParticipant!.Role == PollRole.Owner && Poll.Status == PollStatus.Vote; }
        }

        private bool _isErrorTextVisible = false;

        public bool IsErrorTextVisible
        {
            get { return _isErrorTextVisible; }
            set 
            { 
                _isErrorTextVisible = value;
                if (value == true)
                {
                    IsSuccessfulActionTextVisible = false;
                }
                OnPropertyChanged(); 
            }
        }

        private string _errorText = string.Empty;

        public string ErrorText
        {
            get { return _errorText; }
            set { _errorText = value; OnPropertyChanged(); }
        }

        public string SuccessfulActionText
        {
            get
            {
                switch (Poll.Status)
                {
                    case SharedLibrary.PollStatus.Vote:
                        return "Your vote was successfully submitted. Please return to validate your vote in the validation period!";
                    case SharedLibrary.PollStatus.Validate:
                        return "Your vote has been successfully validated. Nothing further to do until the end of the poll!";
                    case SharedLibrary.PollStatus.Closed:
                        return "";
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private bool _isSuccessfulActionTextVisible = false;

        public bool IsSuccessfulActionTextVisible
        {
            get { return _isSuccessfulActionTextVisible; }
            set 
            { 
                _isSuccessfulActionTextVisible = value;
                if (value == true)
                {
                    IsErrorTextVisible = false;
                }
                OnPropertyChanged(); 
            }
        }

        private bool _isPercentageVisible = false;

        public bool IsPercentageVisible
        {
            get { return _isPercentageVisible; }
            set { _isPercentageVisible = value; OnPropertyChanged(); }
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

        private async Task SubmitVote()
        {

            DisplayOptionModel? selectedOption = Options.FirstOrDefault(x => x.IsSelected);
            if (selectedOption == null)
            {
                ErrorText = "You must select an option before submitting your vote!";
                IsErrorTextVisible = true;
                return;
            }

            VoteSubmitResult result = VoteSubmitResult.UnknownFailure;
            try
            {
                result = await _voteManager.SubmitVote(selectedOption.Model.PollId, selectedOption.Model.Id);
            }
            catch (ServerUnreachableException ex)
            {
                ErrorText = ex.Message;
                IsErrorTextVisible = true;
                return;
            }

            switch (result)
            {
                case VoteSubmitResult.Success:
                    IsErrorTextVisible = false;
                    if (_currUserAsParticipant != null)
                    {
                        _currUserAsParticipant.HasVoted = true;
                    }
                    else
                    {
                        _currUserAsParticipant = new ParticipantModel { Username = _pollManager.LoggedInEmail!, HasVoted = true, PollId = selectedOption.Model.PollId, Role = PollRole.Voter };
                    }
                    IsSuccessfulActionTextVisible = true;
                    break;
                case VoteSubmitResult.UnknownFailure:
                    ErrorText = "An unknown error has occoured while submitting your vote. Please try again!";
                    IsErrorTextVisible = true;
                    break;
                case VoteSubmitResult.KeyRegistrationFailed:
                    ErrorText = "Unable to register your user signature key with the vote administration server. Please try again!";
                    IsErrorTextVisible = true;
                    break;
                case VoteSubmitResult.NoAdminVerificationKey:
                    ErrorText = "An error has occoured with the vote administration server. Please contact the developer!";
                    IsErrorTextVisible = true;
                    break;
                case VoteSubmitResult.AdminRefusedToSign:
                    ErrorText = "An error has occoured while validating your vote in the administration server. Please try again!";
                    IsErrorTextVisible = true;
                    break;
                case VoteSubmitResult.AdminSignatureInvalid:
                    ErrorText = "An unknown error has validating the administration servers signature. Please contact the developer!";
                    IsErrorTextVisible = true;
                    break;
                case VoteSubmitResult.TrasportEncryptionFailed:
                    ErrorText = "Failed to encrypt your vote before submitting it. Please try again later! If the issue persists please contact the developer!";
                    IsErrorTextVisible = true;
                    break;
                case VoteSubmitResult.ShufflerPostFailed:
                    ErrorText = "Your vote could not be submitted to the counting authority. Please contact the developer!";
                    IsErrorTextVisible = true;
                    break;
                default:
                    break;
            }

            OnPropertyChanged("IsPrimaryButtonEnabled");
            OnPropertyChanged("CanChangeVoteOption");
        }

        private async Task ValidateVote()
        {
            VoteValidationResult result;
            try
            {
                result = await _voteManager.ValidateVote(Poll.Id);
            }
            catch (ServerUnreachableException ex)
            {
                ErrorText = ex.Message;
                IsErrorTextVisible = true;
                return;
            }

            switch (result)
            {
                case VoteValidationResult.Success:
                    IsErrorTextVisible = false;
                    IsSuccessfulActionTextVisible = true;
                    break;
                case VoteValidationResult.UnknownFailure:
                    ErrorText = "An unknown error has occoured while submitting your vote. Please try again!";
                    IsErrorTextVisible = true;
                    break;
                case VoteValidationResult.ShufflerPostFailed:
                    ErrorText = "Your validation request could not be submitted to the counting authority. Please contact the developer!";
                    IsErrorTextVisible = true;
                    break;
                default:
                    break;
            }

            OnPropertyChanged("IsPrimaryButtonEnabled");
        }

        private async Task SetPercentages()
        {
            Dictionary<int, int>? voteResults;
            try
            {
                voteResults = await _voteManager.GetResults(Poll.Id);
            }
            catch (Exception ex)
            {
                if (ex is ServerUnreachableException)
                {
                    ErrorText = ex.Message;
                    IsErrorTextVisible = true;
                    return;
                }
                if (ex is VoteInvalidException)
                {
                    ErrorText = $"The integrity of this poll has been compromised. {ex.Message}";
                    IsErrorTextVisible = true;
                    return;
                }
                throw;
            }
            if (voteResults == null)
            {
                ErrorText = "An uknown error has occoured while loading the result of the poll!";
                IsErrorTextVisible = true;
                return;
            }

            int totalVoteCount = voteResults.Sum(x => x.Value);
            int maxVotes = voteResults.Max(x => x.Value);
            foreach (var opt in Options)
            {
                if (voteResults.ContainsKey(opt.Model.Id))
                {
                    int optionVoteCount = voteResults[opt.Model.Id];
                    opt.Percentage = optionVoteCount / (double)totalVoteCount;
                    if (optionVoteCount == maxVotes)
                    {
                        opt.IsWinner = true;
                    }
                }
                else
                {
                    opt.Percentage = 0;
                }
            }
        }

    }
}
