using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace imageComputing
{
    abstract class SimpleFilters
    {
        static string savePath;

        public static void DoFilters(ImageData toCompute, string ConvolutionWindowDiameter, string convolutionWindowFilter, bool saveResult){
            ConvolutionWindow window = CreateConvolutionWindow.DoCreate(ConvolutionWindowDiameter, convolutionWindowFilter);
            ChooseConvolution(window, toCompute);
            if (saveResult == true) toCompute.SaveBitmap(savePath);
        }

        static void ChooseConvolution(ConvolutionWindow window, ImageData toCompute) {
            if (window.processingFilter == "negative") {
                DoNegativeFilter(toCompute);
                savePath = "results/" + toCompute.fileName + "NegativeFilterResult.bmp";
            }
            else if (window.processingFilter == "median") {
                DoMedianFilter(window, toCompute);
                savePath = "results/" + toCompute.fileName + "MedianFilterResult.bmp";
            }
            else if (window.processingFilter == "colored") {
                DoColoredMedianFilter(window, toCompute);
                savePath = "results/" + toCompute.fileName + "ColoredMedianFilterResult.bmp";
            }
            else if (window.processingFilter == "dilation") {
                DoDilation(window, toCompute);
                savePath = "results/" + toCompute.fileName + "DilationResult.bmp";
            }
            else if (window.processingFilter == "erosion") {
                DoErosion(window, toCompute);
                savePath = "results/" + toCompute.fileName + "ErosionResult.bmp";
            }
            else if (window.processingFilter == "closing") {
                DoDilation(window, toCompute);
                DoErosion(window, toCompute);
                savePath = "results/" + toCompute.fileName + "ClosingResult.bmp";
            }
            else if (window.processingFilter == "opening") {
                DoErosion(window, toCompute);
                DoDilation(window, toCompute);
                savePath = "results/" + toCompute.fileName + "OpeningResult.bmp";
            }
        }

        static void DoNegativeFilter(ImageData toCompute) {
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    Color currentPixelColor = toCompute.image.GetPixel(xIndex, yIndex);
                    Color newPixelColor = Color.FromArgb(255 - currentPixelColor.R, 255 - currentPixelColor.G, 255 - currentPixelColor.B);
                    toCompute.image.SetPixel(xIndex, yIndex, newPixelColor);
                }
            }
        }

        static void DoMedianFilter(ConvolutionWindow window, ImageData toCompute) {
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    window.pixelsCounter = 0;
                    window.sumElements.Clear();
                    window.ConvolutionWindowPixelsSum(toCompute.image, xIndex, yIndex);
                    int sum = (window.sumElements.Sum())/window.pixelsCounter;
                    Color pixelColor = Color.FromArgb(sum, sum, sum);
                    toCompute.image.SetPixel(xIndex, yIndex, pixelColor);
                }
            }
        }

        static void DoColoredMedianFilter(ConvolutionWindow window, ImageData toCompute) {
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    window.pixelsCounter = 0;
                    window.sumColoredElements.Clear();
                    window.ConvolutionWindowPixelsSumForColoredFilters(toCompute.image, xIndex, yIndex);
                    float sum = (window.sumColoredElements.Sum())/window.pixelsCounter;
                    HSVColor pixelHSVColor = HSVColor.RGBtoHSV(toCompute.image.GetPixel(xIndex, yIndex));
                    Color pixelColor = HSVColor.HSVtoRGB(new HSVColor(pixelHSVColor.hue, pixelHSVColor.saturation, sum));
                    toCompute.image.SetPixel(xIndex, yIndex, pixelColor);
                }
            }
        }

        static void DoDilation(ConvolutionWindow window, ImageData toCompute) {
            List<Color> colorList = new List<Color>();
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    window.sumElements.Clear();
                    window.ConvolutionWindowPixelsSum(toCompute.image, xIndex, yIndex);
                    int max = window.sumElements.Max();
                    colorList.Add(Color.FromArgb(max, max, max));
                }
            }
            int indexColor = 0;
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    toCompute.image.SetPixel(xIndex, yIndex, colorList[indexColor]);
                    indexColor++;
                }
            }
        }

        static void DoErosion(ConvolutionWindow window, ImageData toCompute) {
            List<Color> colorList = new List<Color>();
            for (int Yindex=0; Yindex<toCompute.image.Height; Yindex++) {
                for (int Xindex=0; Xindex<toCompute.image.Width; Xindex++) {
                    window.sumElements.Clear();
                    window.ConvolutionWindowPixelsSum(toCompute.image, Xindex, Yindex);
                    int min = window.sumElements.Min();
                    colorList.Add(Color.FromArgb(min, min, min));
                }
            }
            int indexColor = 0;
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    toCompute.image.SetPixel(xIndex, yIndex, colorList[indexColor]);
                    indexColor++;
                }
            }
        }
    
    }
}