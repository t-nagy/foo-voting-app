using ClientLib.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPFUI.ViewModel
{
    class PollsViewModel : ViewModelBase
    {

        public DelegateCommand AccountSettingsCommand { get; set; }

        public event EventHandler? ShowAccountSettingsPage;

        public PollsViewModel()
        {
            AccountSettingsCommand = new DelegateCommand((param) =>
            {
                ShowAccountSettingsPage?.Invoke(this, EventArgs.Empty);
            });
        }


    }
}
