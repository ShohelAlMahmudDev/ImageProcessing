using System;
using System.Windows.Input;

namespace FastImageCombine.Helpers
{
     
    public class RelayCommand(Action<object> execute, Func<object, bool>? canExecute = null) : ICommand
    {
        private readonly Action<object> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? arg)
        {
            // Handle null argument if _canExecute is not null
            return canExecute == null || canExecute(arg ?? new object());
        }

        public void Execute(object? arg)
        {
            if (arg == null)
            { 
                // throw new ArgumentNullException(nameof(arg), "Argument cannot be null");
            }

            _execute(arg ?? new object());
        }
    }


}
