using FastImageCombine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastImageCombine.MVVM.ViewModels
{
    class MainViewModel:ObservableObject
    {

        public RelayCommand HomeViewCommand {  get; set; }

        public RelayCommand SettingsViewCommand { get; set; }

        public RelayCommand ImageCombineViewCommand { get; set; }




        public HomeViewModel HomeVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }

        public ImageCombineViewModel ImageCombineVM { get; set; }

        private object? _currentView;
        public object? CurrentView 
        {  
            get => _currentView;  
            set 
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel() 
        {
            HomeVM = new HomeViewModel();
            SettingsVM = new SettingsViewModel();
            ImageCombineVM = new ImageCombineViewModel();
            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(obj =>
            {
                CurrentView = HomeVM;
            });
            SettingsViewCommand = new RelayCommand(obj =>
            {
                CurrentView = SettingsVM;
            });
            ImageCombineViewCommand = new RelayCommand(obj =>
            {
                CurrentView = ImageCombineVM;
            });
        }
    }
}
