using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace imageComputing
{
    abstract class SimpleFilters
    {

        static bool saveFile = true;
        static string savePath;

        public static void doFilters(ImageData toCompute, string convolutionWindowDiameter, string convolutionWindoFilter){
            ConvolutionWindow window = CreateConvolutionWindow.doCreate(convolutionWindowDiameter, convolutionWindoFilter);
            chooseConvolution(window, toCompute);
            if (saveFile == true) {
                toCompute.saveBitmap(savePath);
            }
        }

        static void chooseConvolution(ConvolutionWindow window, ImageData toCompute) {
            if (window.processingFilter == "median") {
                doMedianFilter(window, toCompute);
                savePath = "results/medianFilterResult.bmp";
            }
            else if (window.processingFilter == "dilation") {
                doDilation(window, toCompute);
                savePath = "results/dilationResult.bmp";
            }
            else if (window.processingFilter == "erosion") {
                doErosion(window, toCompute);
                savePath = "results/erosionResult.bmp";
            }
            else if (window.processingFilter == "closing") {
                doDilation(window, toCompute);
                doErosion(window, toCompute);
                savePath = "results/closingResult.bmp";
            }
            else if (window.processingFilter == "opening") {
                doErosion(window, toCompute);
                doDilation(window, toCompute);
                savePath = "results/openingResult.bmp";
            }
        }

        static void doMedianFilter(ConvolutionWindow window, ImageData toCompute) {
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

        static void doDilation(ConvolutionWindow window, ImageData toCompute) {
            List<Color> colorList = new List<Color>();
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    window.pixelsCounter = 0;
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

        static void doErosion(ConvolutionWindow window, ImageData toCompute) {
            List<Color> colorList = new List<Color>();
            for (int Yindex=0; Yindex<toCompute.image.Height; Yindex++) {
                for (int Xindex=0; Xindex<toCompute.image.Width; Xindex++) {
                    window.pixelsCounter = 0;
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