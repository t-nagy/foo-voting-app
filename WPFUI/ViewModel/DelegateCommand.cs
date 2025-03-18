﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFUI.ViewModel
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<Object?> _execute;
        private readonly Func<Object?, Boolean>? _canExecute;

        public DelegateCommand(Action<Object?> execute) : this(null, execute) { }

        public DelegateCommand(Func<Object?, Boolean>? canExecute, Action<Object?> execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public Boolean CanExecute(Object? parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(Object? parameter)
        {
            if (!CanExecute(parameter))
            {
                throw new InvalidOperationException("Command execution is disabled.");
            }
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
