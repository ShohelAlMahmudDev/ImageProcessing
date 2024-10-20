using FastImageCombine.Helpers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace FastImageCombine.MVVM.ViewModels
{
    class SettingsViewModel : ObservableObject
    {
        private int _outlineThickness;
        private string? _outlineColor;
        private string? _outputFormat;
        public int OutLineThickness
        {
            get => _outlineThickness;
            set => SetProperty(ref _outlineThickness, value);
        }

        public string? OutLineColor
        {
            get => _outlineColor;
            set => SetProperty(ref _outlineColor, value);
        }
        public string? OutputFormat
        {
            get => _outputFormat;
            set => SetProperty(ref _outputFormat, value);
        }
        public ICommand? SaveSettingsCommand { get; }

        public SettingsViewModel()
        {
            try
            {
                // Load the settings from Properties
                LoadSettings();

                SaveSettingsCommand = new RelayCommand(_ =>  SaveSettings());
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error initializing SettingsViewModel.");
            }
        }

        private void LoadSettings()
        {
            try
            {
                OutLineThickness = SettingsHelper.GetOutlineThickness();
                OutLineColor = SettingsHelper.GetOutlineColor();
                OutputFormat = SettingsHelper.GetOutputFormat();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error loading settings.");
            }
        }

        private void SaveSettings()
        {
            try
            {
                SettingsHelper.SaveOutlineThickness(OutLineThickness);
                SettingsHelper.SaveOutlineColor(OutLineColor);
                SettingsHelper.SaveOutputFormat(OutputFormat);
                MessageBox.Show("New settings have been saved.");
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error saving settings.");
            }
        }




        private static void HandleException(Exception ex, string customMessage)
        {
            // Log the exception (implement a logging mechanism as needed)
            Console.WriteLine($"{customMessage}\nException: {ex.Message}");
            // Optionally, display an error message to the user
            MessageBox.Show($"{customMessage}\nPlease check the details.");
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public new event PropertyChangedEventHandler? PropertyChanged;

        protected new void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
