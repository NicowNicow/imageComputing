using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace imageComputing
{

//Faster Bitmap Computation code, used to remplace the standard Bitmap Class and optimize the librairy processing time.
//Original source code from https://www.codeproject.com/Tips/240428/Work-with-Bitmaps-Faster-in-Csharp-3

    public class FasterBitmap
    {
        Bitmap sourceImage = null;
        private IntPtr Iptr = IntPtr.Zero;
        private BitmapData bitmapData = null;
        public byte[] Pixels { get; set; }
        public int Depth { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
    
        public FasterBitmap(Bitmap sourceBMPImage) { //Constructor from bmp file
            this.sourceImage = sourceBMPImage;
        }

        public FasterBitmap(int height, int width) { //Constructor from dimensions
            this.sourceImage = new Bitmap(height, width);
        }
        
        public void LockBits() { //Lock the bits of the considered Bitmap in the system's memory to read through them quickly
            try {
                Width = sourceImage.Width;
                Height = sourceImage.Height;
                int pixelCount = Width * Height;
                Rectangle lockRectangle = new Rectangle(0, 0, Width, Height);
                Depth = System.Drawing.Bitmap.GetPixelFormatSize(sourceImage.PixelFormat);
                if (Depth != 8 && Depth != 24 && Depth != 32) {
                    throw new ArgumentException("Only 8, 24 and 32 bpp images are supported!");
                }
                bitmapData = sourceImage.LockBits(lockRectangle, ImageLockMode.ReadWrite, sourceImage.PixelFormat);
                int step = Depth / 8;
                Pixels = new byte[pixelCount * step];
                Iptr = bitmapData.Scan0;
                Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
            }
            catch (Exception error) {
                throw error;
            }
        }

        public void UnlockBits() { //Unlock the bits from the system's memory
            try {
                Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);
                sourceImage.UnlockBits(bitmapData);
            }
            catch (Exception error) {
                throw error;
            }
        }

        public Color GetPixel(int xPosition, int yPosition) { //Quicker GetPixel Method
            Color color = Color.Empty;
            int colorCount = Depth / 8;
            uint indexPixel = (uint)(((yPosition * Width) + xPosition) * colorCount);
            if (indexPixel > Pixels.Length - colorCount) return color;
            if (Depth == 32) {
                byte blue = Pixels[indexPixel];
                byte green = Pixels[indexPixel + 1];
                byte red = Pixels[indexPixel + 2];
                byte alpha = Pixels[indexPixel + 3];
                color = Color.FromArgb(alpha, red, green, blue);
            }
            if (Depth == 24) {
                byte blue = Pixels[indexPixel];
                byte green = Pixels[indexPixel + 1];
                byte red = Pixels[indexPixel + 2];
                color = Color.FromArgb(red, green, blue);
            }
            if (Depth == 8) {
                byte chosenColor = Pixels[indexPixel];
                color = Color.FromArgb(chosenColor, chosenColor, chosenColor);
            }
            return color;
        }

        public void SetPixel(int xPosition, int yPosition, Color color) { //Quicker SetPixel Method
            int colorCount = Depth / 8;
            int indexPixel = ((yPosition * Width) + xPosition) * colorCount;
            if (Depth == 32) {
                Pixels[indexPixel] = color.B;
                Pixels[indexPixel + 1] = color.G;
                Pixels[indexPixel + 2] = color.R;
                Pixels[indexPixel + 3] = color.A;
            }
            if (Depth == 24) {
                Pixels[indexPixel] = color.B;
                Pixels[indexPixel + 1] = color.G;
                Pixels[indexPixel + 2] = color.R;
            }
            if (Depth == 8) {
                Pixels[indexPixel] = color.B;
            }
        }
    }

}
