using System;

using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace FastImageCombine.Helpers
{
    class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
         
    }
}
