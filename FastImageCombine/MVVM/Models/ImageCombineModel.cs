////*************************************
////Class has been Refactored by Shohel
////*************************************
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FastImageCombine.MVVM.Models
{
    public static class ImageCombineModel
    {

        public static async Task<Bitmap> ProcessImageAsync(Bitmap source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source), "Source image cannot be null.");

            try
            {
                Bitmap result = CreateBitmap(source.Width, source.Height);
                await Task.Run(() => ProcessImage(source, result));
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to process the image.", ex);
            }
        }
        private static void ProcessImage(Bitmap source, Bitmap result)
        {
            Rectangle rect = new(0, 0, source.Width, source.Height);

            BitmapData sourceData = source.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData resultData = result.LockBits(rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                int bytes = Math.Abs(sourceData.Stride) * source.Height;
                byte[] buffer = new byte[bytes];
                byte[] resultBuffer = new byte[bytes];

                Marshal.Copy(sourceData.Scan0, buffer, 0, bytes);

                Parallel.For(0, source.Height, y =>
                {
                    int offset = y * sourceData.Stride;
                    for (int x = 0; x < sourceData.Width; x++)
                    {
                        int idx = offset + x * 4;
                        ProcessPixel(buffer, resultBuffer, idx);
                    }
                });

                Marshal.Copy(resultBuffer, 0, resultData.Scan0, bytes);
            }
            finally
            {
                source.UnlockBits(sourceData);
                result.UnlockBits(resultData);
            }
        }

        private static void ProcessPixel(byte[] buffer, byte[] resultBuffer, int idx)
        {
            byte alpha = buffer[idx + 3];
            byte red = buffer[idx + 2];
            byte green = buffer[idx + 1];
            byte blue = buffer[idx];

            if (IsGreenHSV(red, green, blue))
            {
                resultBuffer[idx + 3] = 0;
                resultBuffer[idx + 2] = red;
                resultBuffer[idx + 1] = green;
                resultBuffer[idx] = blue;
            }
            else if (IsBlueHSV(red, green, blue))
            {
                resultBuffer[idx + 3] = 255;
                resultBuffer[idx + 2] = 255;
                resultBuffer[idx + 1] = 255;
                resultBuffer[idx] = 255;
            }
            else if (IsUnwantedColor(red, green, blue))
            {
                byte gray = (byte)((red + green + blue) / 3);
                resultBuffer[idx + 3] = 0;
                resultBuffer[idx + 2] = gray;
                resultBuffer[idx + 1] = gray;
                resultBuffer[idx] = gray;
            }
            else
            {
                resultBuffer[idx + 3] = alpha;
                resultBuffer[idx + 2] = red;
                resultBuffer[idx + 1] = green;
                resultBuffer[idx] = blue;
            }
        }

        private static Bitmap CreateBitmap(int width, int height)
        {
            return new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        private static bool IsGreenHSV(byte r, byte g, byte b)
        {
            var color = System.Drawing.Color.FromArgb(r, g, b);
            float hue = color.GetHue();
            float saturation = color.GetSaturation();
            float brightness = color.GetBrightness();

            return hue >= 80 && hue <= 160 &&
                   saturation > 0.35 &&
                   brightness > 0.25 && brightness < 0.9;
        }

        private static bool IsBlueHSV(byte r, byte g, byte b)
        {
            var color = System.Drawing.Color.FromArgb(r, g, b);
            float hue = color.GetHue();
            float saturation = color.GetSaturation();
            float brightness = color.GetBrightness();

            return hue >= 210 && hue <= 250 &&
                   saturation > 0.3 &&
                   brightness > 0.2 && brightness < 0.85;
        }

        private static bool IsUnwantedColor(byte r, byte g, byte b)
        {
            return g > r && g > b && g > 50;
        }

        public static Bitmap CombineImages(Bitmap image1, Bitmap image2)
        {
            // Validate arguments and provide parameter names
            if (image1 == null)
                throw new ArgumentNullException(nameof(image1), "Image 1 cannot be null.");

            if (image2 == null)
                throw new ArgumentNullException(nameof(image2), "Image 2 cannot be null.");

            try
            {
                Bitmap result = CreateBitmap(image1.Width, image1.Height);
                Combine(image1, image2, result);
                return result;
            }
            catch (Exception ex)
            {
                // Ensure the exception message is informative
                throw new InvalidOperationException("Failed to combine images due to an internal error.", ex);
            }
        }
        private static void Combine(Bitmap img1, Bitmap img2, Bitmap result)
        {
            Rectangle rect = new(0, 0, img1.Width, img1.Height);

            BitmapData data1 = img1.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData data2 = img2.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData resultData = result.LockBits(rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                int bytes = Math.Abs(data1.Stride) * img1.Height;
                byte[] buffer1 = new byte[bytes];
                byte[] buffer2 = new byte[bytes];
                byte[] resultBuffer = new byte[bytes];

                Marshal.Copy(data1.Scan0, buffer1, 0, bytes);
                Marshal.Copy(data2.Scan0, buffer2, 0, bytes);

                Parallel.For(0, img1.Height, y =>
                {
                    int offset = y * data1.Stride;
                    for (int x = 0; x < data1.Width; x++)
                    {
                        int idx = offset + x * 4;
                        CombinePixels(buffer1, buffer2, resultBuffer, idx);
                    }
                });

                Marshal.Copy(resultBuffer, 0, resultData.Scan0, bytes);
            }
            finally
            {
                img1.UnlockBits(data1);
                img2.UnlockBits(data2);
                result.UnlockBits(resultData);
            }
        }

        private static void CombinePixels(byte[] buffer1, byte[] buffer2, byte[] resultBuffer, int idx)
        {
            if (buffer1[idx + 3] > 0)
            {
                Array.Copy(buffer1, idx, resultBuffer, idx, 4);
            }
            else if (buffer2[idx + 3] > 0)
            {
                Array.Copy(buffer2, idx, resultBuffer, idx, 4);
            }
        }

        public static async Task<Bitmap> OutlineBlackAreasAsync(Bitmap source, int thickness, System.Drawing.Color outlineColor)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source), "Source image cannot be null.");

            if (thickness < 1)
                throw new ArgumentOutOfRangeException(nameof(thickness), "Thickness must be at least 1.");

            try
            {
                Bitmap result = CreateBitmap(source.Width, source.Height);

                await Task.Run(() => OutlineColoredAreas(source, result, thickness, System.Drawing.Color.Black, outlineColor));
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to outline black areas.", ex);
            }
        }
        private static void OutlineColoredAreas(Bitmap source, Bitmap result, int thickness, System.Drawing.Color targetColor, System.Drawing.Color outlineColor)
        {
            Rectangle rect = new(0, 0, source.Width, source.Height);

            BitmapData sourceData = source.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData resultData = result.LockBits(rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                int bytes = Math.Abs(sourceData.Stride) * source.Height;
                byte[] buffer = new byte[bytes];
                byte[] resultBuffer = new byte[bytes];

                Marshal.Copy(sourceData.Scan0, buffer, 0, bytes);
                buffer.CopyTo(resultBuffer, 0);

                // Convert target color to byte array for easy comparison
                byte targetBlue = targetColor.B;
                byte targetGreen = targetColor.G;
                byte targetRed = targetColor.R;
                byte targetAlpha = targetColor.A;

                Parallel.For(1, source.Height - 1, y =>
                {
                    int offset = y * sourceData.Stride;
                    for (int x = 1; x < sourceData.Width - 1; x++)
                    {
                        int idx = offset + x * 4;

                        // Check if the pixel matches the target color
                        if (IsColorMatch(buffer, idx, targetRed, targetGreen, targetBlue, targetAlpha))
                        {
                            // Draw the outline around the matched color
                            DrawOutline(resultBuffer, sourceData.Stride, x, y, sourceData.Width, sourceData.Height, thickness, outlineColor, targetColor);

                        }
                    }
                });

                Marshal.Copy(resultBuffer, 0, resultData.Scan0, bytes);
            }
            finally
            {
                source.UnlockBits(sourceData);
                result.UnlockBits(resultData);
            }
        }


        private static void DrawOutline(byte[] buffer, int stride, int x, int y, int width, int height, int thickness, System.Drawing.Color outlineColor, System.Drawing.Color targetColor)
        {
            for (int ty = -thickness; ty <= thickness; ty++)
            {
                for (int tx = -thickness; tx <= thickness; tx++)
                {
                    if (tx == 0 && ty == 0)
                        continue;

                    int nx = x + tx;
                    int ny = y + ty;

                    if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    {
                        int idx = ny * stride + nx * 4;

                        // Only draw the outline if the neighboring pixel is not the target color
                        if (!IsColorMatch(buffer, idx, targetColor.R, targetColor.G, targetColor.B, targetColor.A))
                        {
                            buffer[idx] = outlineColor.B;
                            buffer[idx + 1] = outlineColor.G;
                            buffer[idx + 2] = outlineColor.R;
                            buffer[idx + 3] = outlineColor.A;
                        }
                    }
                }
            }
        }

        private static bool IsColorMatch(byte[] buffer, int idx, byte red, byte green, byte blue, byte alpha)
        {
            // Compare the buffer's pixel color with the target color
            return buffer[idx + 0] == blue   // Blue channel
                && buffer[idx + 1] == green  // Green channel
                && buffer[idx + 2] == red    // Red channel
                && buffer[idx + 3] == alpha; // Alpha channel
        }

        public static async Task<Bitmap> ConvertTo32BitGrayscaleAsync(Bitmap source)
        {
            return await Task.Run(() =>
            {
                try
                {
                    ArgumentNullException.ThrowIfNull(source);

                    // Create a new 32-bit ARGB bitmap
                    Bitmap grayscaleBitmap = new(source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    // Lock the source bitmap's bits for read-only access
                    BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);

                    // Lock the grayscale bitmap's bits for read/write access
                    BitmapData grayscaleData = grayscaleBitmap.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    // Get the stride (the width in bytes of a single row of pixels, including any padding)
                    int sourceStride = sourceData.Stride;
                    int grayscaleStride = grayscaleData.Stride;

                    // Get pointers to the first pixel in each bitmap
                    IntPtr sourcePtr = sourceData.Scan0;
                    IntPtr grayscalePtr = grayscaleData.Scan0;

                    // Buffer arrays to hold pixel data
                    byte[] sourceBuffer = new byte[sourceStride * source.Height];
                    byte[] grayscaleBuffer = new byte[grayscaleStride * grayscaleBitmap.Height];

                    // Copy pixel data from the source bitmap to the buffer array
                    Marshal.Copy(sourcePtr, sourceBuffer, 0, sourceBuffer.Length);


                    Parallel.For(0, source.Height, y =>
                    {
                        for (int x = 0; x < sourceData.Width; x++)
                        {
                            int sourceIndex = y * sourceStride + x * 4;
                            int grayscaleIndex = y * grayscaleStride + x * 4;

                            byte b = sourceBuffer[sourceIndex];
                            byte g = sourceBuffer[sourceIndex + 1];
                            byte r = sourceBuffer[sourceIndex + 2];

                            byte gray = (byte)(0.299 * r + 0.587 * g + 0.114 * b);

                            grayscaleBuffer[grayscaleIndex] = gray;
                            grayscaleBuffer[grayscaleIndex + 1] = gray;
                            grayscaleBuffer[grayscaleIndex + 2] = gray;
                            grayscaleBuffer[grayscaleIndex + 3] = sourceBuffer[sourceIndex + 3];
                        }
                    });
                    // Copy the modified pixel data back into the grayscale bitmap
                    Marshal.Copy(grayscaleBuffer, 0, grayscalePtr, grayscaleBuffer.Length);

                    // Unlock the bits of both bitmaps
                    source.UnlockBits(sourceData);
                    grayscaleBitmap.UnlockBits(grayscaleData);

                    return grayscaleBitmap;
                }
                catch (Exception expr)
                {
                    Console.WriteLine($"Error during grayscale conversion: {expr.Message}");
                    throw;
                }

            });
        }
        public static async Task<Bitmap> ConvertTo8BitGrayscaleAsync(Bitmap source)
        {
            return await Task.Run(() =>
            {
                try
                {
                    ArgumentNullException.ThrowIfNull(source);

                    // Create a new 8-bit grayscale bitmap
                    Bitmap grayscaleBitmap = new(source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                    // Set up grayscale palette
                    ColorPalette palette = grayscaleBitmap.Palette;
                    for (int i = 0; i < 256; i++)
                    {
                        palette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
                    }
                    grayscaleBitmap.Palette = palette;

                    // Lock the source bitmap's bits for read-only access
                    BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);

                    // Lock the grayscale bitmap's bits for read/write access
                    BitmapData grayscaleData = grayscaleBitmap.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                    // Get the stride (the width in bytes of a single row of pixels, including any padding)
                    int sourceStride = sourceData.Stride;
                    int grayscaleStride = grayscaleData.Stride;

                    // Get pointers to the first pixel in each bitmap
                    IntPtr sourcePtr = sourceData.Scan0;
                    IntPtr grayscalePtr = grayscaleData.Scan0;

                    // Buffer arrays to hold pixel data
                    byte[] sourceBuffer = new byte[sourceStride * source.Height];
                    byte[] grayscaleBuffer = new byte[grayscaleStride * grayscaleBitmap.Height];

                    // Copy pixel data from the source bitmap to the buffer array
                    Marshal.Copy(sourcePtr, sourceBuffer, 0, sourceBuffer.Length);


                    Parallel.For(0, source.Height, y =>
                    {
                        for (int x = 0; x < sourceData.Width; x++)
                        {
                            int sourceIndex = y * sourceStride + x * 4;
                            int grayscaleIndex = y * grayscaleStride + x;

                            byte b = sourceBuffer[sourceIndex];
                            byte g = sourceBuffer[sourceIndex + 1];
                            byte r = sourceBuffer[sourceIndex + 2];

                            byte gray = (byte)(0.299 * r + 0.587 * g + 0.114 * b);

                            grayscaleBuffer[grayscaleIndex] = gray;
                        }
                    });

                    // Copy the modified pixel data back into the grayscale bitmap
                    Marshal.Copy(grayscaleBuffer, 0, grayscalePtr, grayscaleBuffer.Length);

                    // Unlock the bits of both bitmaps
                    source.UnlockBits(sourceData);
                    grayscaleBitmap.UnlockBits(grayscaleData);

                    return grayscaleBitmap;
                }
                catch (Exception expr)
                {
                    Console.WriteLine($"Error during grayscale conversion: {expr.Message}");
                    throw;
                }

            });
        }

         
    }
}
