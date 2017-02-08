﻿using System;
using System.Windows.Input;

namespace TaskManagerApplication
{
    public class RelayCommand : ICommand
    {
        private Action<object> _action;

        public RelayCommand(Action<object> action)
        {
            _action = action;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if(CanExecute(parameter))
                _action(parameter);
        }

        #endregion
    }
}
