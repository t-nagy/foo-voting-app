using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.ViewModel
{
    class RegistrationCompleteViewModel : ViewModelBase
    {
        public DelegateCommand ShowLoginCommand { get; set; }

        public event EventHandler<bool>? ShowLoginPage;

        public RegistrationCompleteViewModel()
        {
            ShowLoginCommand = new DelegateCommand((param) =>
            {
                ShowLoginPage?.Invoke(this, true);
            });
        }
    }
}
