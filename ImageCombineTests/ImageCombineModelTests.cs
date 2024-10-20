
using System;
using System.Drawing;
using System.Threading.Tasks;
using Xunit;
using FastImageCombine.MVVM.Models;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using FastImageCombine.Helpers;

namespace ImageCombineTests
{
    public class ImageCombineModelTests
    {
        
        private readonly Bitmap layer1 = new(Path.Combine("ImputImage", "Layer1.bmp")); 
        private readonly Bitmap layer2 = new(Path.Combine("ImputImage", "Layer2.bmp")); 
        private readonly Bitmap Task1A = new("TestImage//Task1A.png");
        private readonly Bitmap Task1B = new("TestImage//Task1B.png");
        private readonly Bitmap Task2_Combined = new("TestImage//Task2.png");
        private readonly Bitmap Task3_OutLineColor = new("TestImage//Task3.png");
        private readonly Bitmap Task4_OutlineGray32bit = new("TestImage//Task4_32bit.png");
        private readonly Bitmap Task4_OutlineGray8bit = new("TestImage//Task4_8bit.png");




        [Fact]
        public async Task ProcessImage_ValidImage_ReturnsProcessedImage()
        { 
           Bitmap result =  await ImageCombineModel.ProcessImageAsync(layer1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(layer1.Width, result.Width);
            Assert.Equal(layer1.Height, result.Height);

            // Example check: Verify if the result is not the same as the input (depends on processing logic)
            Assert.Equal(Task1A.GetPixel(20, 20), result.GetPixel(20, 20));
            
        }

        [Fact]
        public void CombineImages_ValidImages_ReturnsCombinedImage()
        {
              
            // Act
            Bitmap result = ImageCombineModel.CombineImages(layer1, layer2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(layer1.Width, result.Width);
            Assert.Equal(layer1.Height, result.Height);

            Assert.Equal(Task2_Combined.GetPixel(20, 20), result.GetPixel(20, 20));
        }

        [Fact]
        public async Task OutlineBlackAreas_ValidImage_ReturnsOutlinedImage()
        {

            // Act

            
            Bitmap result = await ImageCombineModel.OutlineBlackAreasAsync(Task2_Combined, 5, Color.Red);
             
            // Assert
            Assert.NotNull(result);
            Assert.Equal(layer1.Width, result.Width);
            Assert.Equal(layer1.Height, result.Height); 
             
            Assert.Equal(Task3_OutLineColor.GetPixel(0,0), result.GetPixel(0, 0)); 

        }

        [Fact]
        public async Task ConvertTo32BitGrayscaleAsync_ValidImage_ReturnsGrayscaleImage()
        {
              
            // Act
            Bitmap result = await ImageCombineModel.ConvertTo32BitGrayscaleAsync(Task3_OutLineColor);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Task4_OutlineGray32bit.Width, result.Width);
            Assert.Equal(Task4_OutlineGray32bit.Height, result.Height);

            // Use Parallel.For to iterate over rows and compare each pixel in parallel
            CompareBitmaps(result, Task4_OutlineGray32bit);
             
        }

        [Fact]
        public async Task ConvertTo8BitGrayscaleAsync_ValidImage_Returns8BitGrayscaleImage()
        {
            
            // Act
            Bitmap result = await ImageCombineModel.ConvertTo8BitGrayscaleAsync(Task3_OutLineColor);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Task4_OutlineGray8bit.Width, result.Width);
            Assert.Equal(Task4_OutlineGray8bit.Height, result.Height);

            CompareBitmaps(result, Task4_OutlineGray8bit);
        }

        private void CompareBitmaps(Bitmap expected, Bitmap actual)
        {
            // Check the dimensions first
            Assert.Equal(expected.Width, actual.Width);
            Assert.Equal(expected.Height, actual.Height);

            // Lock the bits for both images
            var rect = new System.Drawing.Rectangle(0, 0, expected.Width, expected.Height);
            var expectedData = expected.LockBits(rect, ImageLockMode.ReadOnly, expected.PixelFormat);
            var actualData = actual.LockBits(rect, ImageLockMode.ReadOnly, actual.PixelFormat);

            try
            {
                // Get the pointer to the pixel data
                IntPtr expectedPtr = expectedData.Scan0;
                IntPtr actualPtr = actualData.Scan0;

                // Get the total number of bytes in the images
                int bytes = Math.Abs(expectedData.Stride) * expected.Height;

                // Create byte arrays to hold the pixel data
                byte[] expectedPixels = new byte[bytes];
                byte[] actualPixels = new byte[bytes];

                // Copy the pixel data into the arrays
                Marshal.Copy(expectedPtr, expectedPixels, 0, bytes);
                Marshal.Copy(actualPtr, actualPixels, 0, bytes);

                // Compare the byte arrays
                Assert.Equal(expectedPixels, actualPixels);
            }
            finally
            {
                // Unlock the bits when done
                expected.UnlockBits(expectedData);
                actual.UnlockBits(actualData);
            }
        }
         

    }
}
