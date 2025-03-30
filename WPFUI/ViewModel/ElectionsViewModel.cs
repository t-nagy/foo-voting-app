using ClientLib.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPFUI.ViewModel
{
    class ElectionsViewModel : ViewModelBase
    {

        public DelegateCommand AccountSettingsCommand { get; set; }

        public event EventHandler? ShowAccountSettingsPage;

        public ElectionsViewModel()
        {
            AccountSettingsCommand = new DelegateCommand((param) =>
            {
                ShowAccountSettingsPage?.Invoke(this, EventArgs.Empty);
            });
        }


    }
}
