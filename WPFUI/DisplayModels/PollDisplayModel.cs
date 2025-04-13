using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.DisplayModels
{
    internal class PollDisplayModel
    {
        private readonly PollModel _poll;

        public PollDisplayModel(PollModel poll)
        {
            _poll = poll;
        }

        public int Id
        {
            get { return _poll.Id; }
        }

        public string OwnerName
        {
            get { return _poll.OwnerName!; }
        }

        public string Title
        {
            get { return _poll.Title; }
        }

        public string CreatedDate
        {
            get { return _poll.CreatedDate.ToLocalTime().ToString(CultureInfo.CurrentCulture); }
        }

        public string VoteCollectionEndDate
        {
            get { return _poll.VoteCollectionEndDate.ToLocalTime().ToString(CultureInfo.CurrentCulture); }
        }

        public string VoteValidationEndDate
        {
            get { return _poll.VoteValidationEndDate.ToLocalTime().ToString(CultureInfo.CurrentCulture); }
        }

        public string Status
        {
            get
            {
                switch (_poll.Status)
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

        public string IsPublic
        {
            get { return _poll.IsPublic ? "Yes" : "No"; }
        }
    }
}
