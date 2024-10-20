using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastImageCombine.Helpers
{
    public static class FileDialogHelper
    {
        public static string? OpenImageFileDialog()
        {
            OpenFileDialog openFileDialog = new ()
            {
                Filter = "Bitmap Image (*.bmp)|*.bmp|JPEG Image (*.jpg;*.jpeg)|*.jpg;*.jpeg|GIF Image (*.gif)|*.gif|PNG Image (*.png)|*.png"
            };

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }

        public static string? SaveImageFileDialog()
        {
            SaveFileDialog saveFileDialog = new ()
            {
                Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg;*.jpeg)|*.jpg;*.jpeg|GIF Image (*.gif)|*.gif|Bitmap Image (*.bmp)|*.bmp",
                Title = "Save Grayscale Image"
            };

            return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : null;
        }
    }
}
