using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.ViewModel;

namespace WPFUI.DisplayModels
{
    class DisplayOptionModel : ViewModelBase
    {
        public OptionModel Model { get; private set; }

        public string DisplayText
        {
            get { return Model.OptionText; }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set 
            {
                _isSelected = value;
                if (value)
                {
                    OptionSelected?.Invoke(this, this);
                }
                OnPropertyChanged(); 
            }
        }

        public event EventHandler<DisplayOptionModel>? OptionSelected;

        public DisplayOptionModel(OptionModel model)
        {
            Model = model;
        }
    }
}
