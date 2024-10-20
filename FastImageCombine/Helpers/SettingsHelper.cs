using System;
using System.Windows;

namespace FastImageCombine.Helpers
{
    public static class SettingsHelper
    {
        public static int GetOutlineThickness()
        {
            try
            {
                return Properties.Settings.Default.OutLineThickness;
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error retrieving outline thickness.");
                throw;
            }
        }

        public static string GetOutlineColor()
        {
            try
            {
                return Properties.Settings.Default.OutLineColor;
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error retrieving outline color.");
                throw;
            }
        }
        public static string GetOutputFormat()
        {
            try
            {
                return Properties.Settings.Default.OutputFormat;
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error retrieving outline color.");
                throw;
            }
        }
        public static void SaveOutlineThickness(int thickness)
        {
            try
            {
                Properties.Settings.Default.OutLineThickness = thickness;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error saving outline thickness.");
                throw;  
            }
        }

        public static void SaveOutlineColor(string? color)
        {
            try
            {
                Properties.Settings.Default.OutLineColor = color;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error saving outline color.");
                throw;  
            }
        }
        public static void  SaveOutputFormat(string? outputformat)
        {
            try
            {
                Properties.Settings.Default.OutputFormat = outputformat;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error saving output format.");
                throw; 
            }
        }
        private static void HandleException(Exception ex, string customMessage)
        { 
            Console.WriteLine($"{customMessage}\nException: {ex.Message}"); 
            MessageBox.Show($"{customMessage}\nPlease check the details.");
        }
    }
}
