////*************************************
////Class has been Refactored by Shohel
////*************************************

using FastImageCombine.Helpers;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using FastImageCombine.MVVM.Models;
using System.Runtime.InteropServices;

namespace FastImageCombine.MVVM.ViewModels
{
    class ImageCombineViewModel : ObservableObject, INotifyPropertyChanged
    {
        public static int OutLineThickness => Properties.Settings.Default.OutLineThickness;
        public static string OutLineColor => Properties.Settings.Default.OutLineColor;
        public static string OutputFormat => Properties.Settings.Default.OutputFormat;

        private int _progress;
        private string? _progressText;

        // Properties
        private Bitmap? grayscaleImage;

        private Bitmap? _image1;
        private Bitmap? _image2;

        private BitmapSource? _processedLayer1;
        private BitmapSource? _processedLayer2;
        private BitmapSource? _combinedImage;
        private BitmapSource? _outlinedImage;

        private BitmapImage? _selectedImage1;
        private BitmapImage? _selectedImage2;

        // Properties

        public int Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public string? ProgressText
        {
            get => _progressText;
            set => SetProperty(ref _progressText, value);
        }

        public BitmapSource? Image1 { get; private set; }
        public BitmapSource? Image2 { get; private set; }

        public BitmapSource? ProcessedLayer1
        {
            get => _processedLayer1;
            set => SetProperty(ref _processedLayer1, value);
        }

        public BitmapSource? ProcessedLayer2
        {
            get => _processedLayer2;
            set => SetProperty(ref _processedLayer2, value);
        }

        public BitmapSource? CombinedImage
        {
            get => _combinedImage;
            set => SetProperty(ref _combinedImage, value);
        }

        public BitmapSource? OutlinedImage
        {
            get => _outlinedImage;
            set => SetProperty(ref _outlinedImage, value);
        }

        public BitmapImage? SelectedImage1
        {
            get => _selectedImage1;
            set => SetProperty(ref _selectedImage1, value);
        }

        public BitmapImage? SelectedImage2
        {
            get => _selectedImage2;
            set => SetProperty(ref _selectedImage2, value);
        }

        public ICommand? LoadImage1Command { get; }
        public ICommand? LoadImage2Command { get; }
        public ICommand? ProcessImagesCommand { get; }
        public ICommand? SaveGrayscaleImageCommand { get; }

        public ImageCombineViewModel()
        {
            try
            {
                LoadImage1Command = new RelayCommand(_ => LoadImage1());
                LoadImage2Command = new RelayCommand(_ => LoadImage2());
                ProcessImagesCommand = new RelayCommand(async _ => await ProcessImagesAsync());
                SaveGrayscaleImageCommand = new RelayCommand(async _ => await SaveGrayscaleImageAsync());
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error initializing commands.");
            }
        }

        private static BitmapImage LoadImage(string filePath)
        {
            var bitmap = new BitmapImage();
            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = null;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze(); // Freeze to make it cross-thread accessible
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load image '{filePath}': {ex.Message}", ex);
            }
            return bitmap;
        }

        private void LoadImage1()
        {
            try
            {
                string? filePath = FileDialogHelper.OpenImageFileDialog();
                if (filePath == null) return;

                DisposeImage(ref _image1);
                _image1 = new Bitmap(filePath);
                SelectedImage1 = LoadImage(filePath);
                Image1 = ImageConversionHelper.BitmapToBitmapSource(_image1);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error loading image 1.");
            }
        }

        private void LoadImage2()
        {
            try
            {
                string? filePath = FileDialogHelper.OpenImageFileDialog();
                if (filePath == null) return;

                DisposeImage(ref _image2);
                _image2 = new Bitmap(filePath);
                SelectedImage2 = LoadImage(filePath);
                Image2 = ImageConversionHelper.BitmapToBitmapSource(_image2);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error loading image 2.");
            }
        }

        private static void DisposeImage(ref Bitmap? image)
        {
            if (image != null)
            {
                image.Dispose();
                image = null;
            }
        }

        private async Task ProcessImagesAsync()
        {
            try
            {
                if (_image1 == null || _image2 == null)
                {
                    MessageBox.Show("Both images must be loaded before processing.");
                    return;
                }

                Progress = 0;
                ProgressText = "Starting...";

                IProgress<int> progress = new Progress<int>(value =>
                {
                    Progress = value;
                    ProgressText = $"{value}% completed";
                });


                // Parallelize independent image processing tasks
                var processedLayer1Task = Task.Run(() => ImageCombineModel.ProcessImageAsync(_image1));
                progress.Report(20);
                var processedLayer2Task = Task.Run(() => ImageCombineModel.ProcessImageAsync(_image2));
                progress.Report(40);
                // Await both tasks in parallel
                var processedLayers = await Task.WhenAll(processedLayer1Task, processedLayer2Task);
                var processedLayer1 = processedLayers[0];
                var processedLayer2 = processedLayers[1];
                 

                // Combine and Processed images
                var combinedImage = await Task.Run(() => ImageCombineModel.CombineImages(processedLayer1, processedLayer2));
                progress.Report(60);
                //Outline Black Area
                var outlinedImage = await Task.Run(() => ImageCombineModel.OutlineBlackAreasAsync(combinedImage, OutLineThickness, ColorConversionHelper.StringToColor(OutLineColor)));
                progress.Report(80);
                 
                //Convert to BitmapSource
                ProcessedLayer1 = ImageConversionHelper.BitmapToBitmapSource(processedLayer1);
                ProcessedLayer2 = ImageConversionHelper.BitmapToBitmapSource(processedLayer2);
                CombinedImage = ImageConversionHelper.BitmapToBitmapSource(combinedImage);
                OutlinedImage = ImageConversionHelper.BitmapToBitmapSource(outlinedImage);

                if (OutputFormat.Contains("8Bit"))
                {
                    grayscaleImage = await ImageCombineModel.ConvertTo8BitGrayscaleAsync(outlinedImage);
                   
                }
                else
                {
                    grayscaleImage = await ImageCombineModel.ConvertTo32BitGrayscaleAsync(outlinedImage);
                }

                progress.Report(100);

                //********************************************* Only to save the image for Test Purposes*******************************************
                //string outputPath = "C:\\Temp\\FastImageCombine_Output";
                //if (!Directory.Exists(outputPath))
                //{
                //    Directory.CreateDirectory(outputPath);
                //}
                //ImageFormat format = ImageConversionHelper.GetImageFormatByExtension(".png");
                //await Task.Run(() =>
                //{
                //    processedLayer1.Save(Path.Combine(outputPath, "Task1A.png"),format);
                //    processedLayer2.Save(Path.Combine(outputPath, "Task1B.png"), format);
                //    combinedImage.Save(Path.Combine(outputPath, "Task2.png"), format);
                //    outlinedImage.Save(Path.Combine(outputPath, "Task3.png"), format);
                //    grayscaleImage.Save(Path.Combine(outputPath, "Task4.png"), format);
                //});
                //ProgressText = "Processing completed!";
                //*********************************************************************************************************************************

            }
            catch (Exception ex)
            {
                HandleException(ex, "Error processing images.");
            }
            finally
            { 
                DisposeImage(ref grayscaleImage);
            }
        }

        private async Task SaveGrayscaleImageAsync()
        {
            try
            {
                if (OutlinedImage == null)
                {
                    MessageBox.Show("Process images before saving grayscale.");
                    return;
                }

                if (OutputFormat.Contains("8Bit"))
                {
                    grayscaleImage = await ImageCombineModel.ConvertTo8BitGrayscaleAsync(ImageConversionHelper.BitmapSourceToBitmap(OutlinedImage));
                }
                else
                {
                    grayscaleImage = await ImageCombineModel.ConvertTo32BitGrayscaleAsync(ImageConversionHelper.BitmapSourceToBitmap(OutlinedImage));
                }

                string? filePath = FileDialogHelper.SaveImageFileDialog();
                if (filePath == null) return;

                ImageFormat format = ImageConversionHelper.GetImageFormatByExtension(filePath);
                grayscaleImage.Save(filePath, format);

                MessageBox.Show($"File {filePath} has been saved.");
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error saving grayscale image.");
            }
            finally
            {
                DisposeImage(ref grayscaleImage);
            }
        }

        private static void HandleException(Exception ex, string customMessage)
        {
            Console.WriteLine($"{customMessage}\nException: {ex.Message}");
            MessageBox.Show($"{customMessage}\nPlease check the details.");
        }

        public new event PropertyChangedEventHandler? PropertyChanged;

        private bool SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected new virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}