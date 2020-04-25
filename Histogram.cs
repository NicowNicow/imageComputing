using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imageComputing
{
    public abstract class Histogram //This class is used to draw the histogram from an ImageData item
    {
        private static int tempMax = 0;

        public static void DrawHistogram(ImageData toCompute) { //Draw the histogram on an output bmp file
            List<float> sticksHeight = GetSticksHeight(toCompute);
            Bitmap drawImage = DrawWhite(toCompute);
            drawImage = DrawSticks(drawImage, sticksHeight);
            drawImage.Save("results/" + toCompute.fileName + "HistogramResult.bmp");
        }

        public static void DrawCumulativeHistogram(ImageData toCompute) { //Draw the cumulative histogram on an output bmp file
            List<float> sticksHeight = GetSticksHeight(toCompute);
            Bitmap drawImage = DrawWhite(toCompute);
            drawImage = DrawCumulativeSticks(toCompute, drawImage);
            drawImage.Save("results/" + toCompute.fileName + "CumulativeHistogramResult.bmp");
        }

        public static List<float> GetSticksHeight(ImageData toCompute) { //Calculate a representative height for each "stick" of the histogram
            List<float> sticksHeight = new List<float>();
            sticksHeight.AddRange(Enumerable.Repeat(0f, 256));
            for (int index =0; index< 256; index++) {
                if (toCompute.histogram[index]>= tempMax) {
                    tempMax = toCompute.histogram[index];
                }
            }
            for (int index = 0; index<256; index++) {
                sticksHeight[index] = (float) ((float)toCompute.histogram[index]/(float)tempMax)*(toCompute.image.Height/2);
            }
            return(sticksHeight);
        }

        public static Bitmap DrawWhite(ImageData toCompute) { //Draw a white background
            Bitmap drawImage = new Bitmap(256, toCompute.image.Height);
            Color white = Color.FromName("White");
            for (int Xindex=0; Xindex<256; Xindex++) {
                for (int Yindex=0; Yindex<toCompute.image.Height; Yindex++) {
                    drawImage.SetPixel(Xindex, Yindex, white);
                }
            }
            return(drawImage);
        }

        public static Bitmap DrawSticks(Bitmap drawImage, List<float> sticksHeight) { //Draw the histogram "sticks"
            Color black = Color.FromName("Black");
            for (int Xindex=0; Xindex< 256; Xindex++) {
                for (int Yindex=(drawImage.Height - 1); Yindex>drawImage.Height- 1 -(int)sticksHeight[Xindex]; Yindex--) {
                    drawImage.SetPixel(Xindex, Yindex, black);
                }
            }
            return(drawImage);
        }

        public static Bitmap DrawCumulativeSticks(ImageData toCompute, Bitmap drawImage) { //Draw the histogram "sticks" for a cumulative histogram
            Color black = Color.FromName("Black");
            int cumulativeHeight = 0;
            int totalPixel = toCompute.image.Height*toCompute.image.Width;
            for (int Xindex=0; Xindex< 256; Xindex++) {
                cumulativeHeight = cumulativeHeight + (int)toCompute.histogram[Xindex];
                for (int Yindex=(drawImage.Height - 1); Yindex>drawImage.Height -1 -(int)(cumulativeHeight*drawImage.Height/totalPixel); Yindex--) {
                    drawImage.SetPixel(Xindex, Yindex, black);
                }
            }
            return(drawImage);
        }

        public static void DrawDynamicExpansion(ImageData toCompute) {
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    Color currentColor = toCompute.image.GetPixel(xIndex,yIndex);
                    int red = (int)(toCompute.dynamicExpansionParameters[0]*currentColor.R + toCompute.dynamicExpansionParameters[1]);
                    if (red >= 255) red = 255;
                    if (red < 0) red = 0;
                    int green = (int)(toCompute.dynamicExpansionParameters[0]*currentColor.G + toCompute.dynamicExpansionParameters[1]);
                    if (green >= 255) green = 255;
                    if (green < 0) green = 0;
                    int blue = (int)(toCompute.dynamicExpansionParameters[0]*currentColor.B + toCompute.dynamicExpansionParameters[1]);
                    if (blue >= 255) blue = 255;
                    if (blue < 0) blue = 0;
                    toCompute.image.SetPixel(xIndex, yIndex, Color.FromArgb(red, green, blue));
                }
            }
            toCompute.SaveBitmap("results/" + toCompute.fileName + "DynamicExpansionResult.bmp");
        }

        public static void DrawHistogramEqualization(ImageData toCompute) {
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    Color pixelColor = toCompute.image.GetPixel(xIndex, yIndex);
                    int currentGreyLevel = (int)(0.299*pixelColor.R + 0.587*pixelColor.G + 0.114*pixelColor.B);
                    int newGreyLevel = (255*toCompute.cumulativeHistogram[currentGreyLevel]/(toCompute.image.Height*toCompute.image.Width));
                    toCompute.image.SetPixel(xIndex, yIndex, Color.FromArgb(newGreyLevel, newGreyLevel, newGreyLevel));
                }
            }
            toCompute.SaveBitmap("results/" + toCompute.fileName + "HistogramEqualizationResult.bmp");
        }

    }
}
