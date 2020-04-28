using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace imageComputing
{
    abstract class AdvancedFilters
    {
        static string savePath;

        public static void DoFilters(ImageData toCompute, string convolutionWindowFilter, bool saveResult) {
            AdvancedConvolutionWindow window = CreateAdvancedConvolutionWindow.DoCreateFilter(convolutionWindowFilter);
            ChooseConvolution(window, toCompute);
            if (saveResult == true) toCompute.SaveBitmap(savePath);
        }

        public static void DoFilters(ImageData toCompute, string convolutionWindowFilter, string windowNorm, bool saveResult) {
            AdvancedConvolutionWindow window = CreateAdvancedConvolutionWindow.DoCreateFilter(convolutionWindowFilter, windowNorm);
            ChooseConvolution(window, toCompute);
            if (saveResult == true) toCompute.SaveBitmap(savePath);
        }

        public static void DoGaussianFilter(ImageData toCompute, string windowDiameter, bool saveResult) {
            AdvancedConvolutionWindow window = CreateAdvancedConvolutionWindow.DoCreateDiameter(windowDiameter);
            ChooseConvolution(window, toCompute);
            if (saveResult == true) toCompute.SaveBitmap(savePath);
        }

        static void ChooseConvolution(AdvancedConvolutionWindow window, ImageData toCompute) {
            if (window.processingFilter == "gaussian") {
                DoGaussian(window, toCompute);
                savePath = "results/" + toCompute.fileName + "GaussianFilterResult.bmp";
            }
            else if (window.processingFilter == "prewitt") {
                DoSobelPrewitt(window, toCompute);
                savePath = "results/" + toCompute.fileName + "PrewittFilterResult.bmp";
            }
            else if (window.processingFilter == "sobel") {
                DoSobelPrewitt(window, toCompute);
                savePath = "results/" + toCompute.fileName + "SobelFilterResult.bmp";
            }
            else if (window.processingFilter == "robert") {
                DoRobert(window, toCompute);
                savePath = "results/" + toCompute.fileName + "RobertsFilterResult.bmp";
            }
            else if (window.processingFilter == "laplacian4") {
                DoLaplacian(window, toCompute);
                savePath = "results/" + toCompute.fileName + "Laplacian4Result.bmp";
            }
            else if (window.processingFilter == "laplacian8") {
                DoLaplacian(window, toCompute);
                savePath = "results/" + toCompute.fileName + "Laplacian8Result.bmp";
            }
        }


        static void DoGaussian(AdvancedConvolutionWindow window, ImageData toCompute) {
            List<Color> colorList = new List<Color>();
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    window.sumElements.Clear();
                    StandardConvolution(window, toCompute.image, xIndex, yIndex);
                    int sum = window.sumElements.Sum()/window.gaussianWeightedSum;
                    if (sum < 0) sum = 0;
                    if (sum > 255) sum = 255;
                    colorList.Add(Color.FromArgb(sum, sum, sum));
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

        static void DoSobelPrewitt(AdvancedConvolutionWindow window, ImageData toCompute) {
            List<Color> colorList = new List<Color>();
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    int[] values = SobelPrewittConvolution(window, toCompute.image, xIndex, yIndex);
                    int value = window.DoNorm(values);
                    if (value < 0) value = 0;
                    if (value > 255) value = 255;
                    colorList.Add(Color.FromArgb(value, value, value));
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

        static void DoRobert(AdvancedConvolutionWindow window, ImageData toCompute) {
            List<Color> colorList = new List<Color>();
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    int[] values = RobertConvolution(window, toCompute.image, xIndex, yIndex);
                    int value = window.DoNorm(values);
                    if (value < 0) value = 0;
                    if (value > 255) value = 255;
                    colorList.Add(Color.FromArgb(value, value, value));
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

        static void DoLaplacian(AdvancedConvolutionWindow window, ImageData toCompute) {
            List<Color> colorList = new List<Color>();
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    window.sumElements.Clear();
                    StandardConvolution(window, toCompute.image, xIndex, yIndex);
                    int sum = window.sumElements.Sum();
                    if (sum < 0) sum = 0;
                    if (sum > 255) sum = 255;
                    colorList.Add(Color.FromArgb(sum, sum, sum));
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

        static void StandardConvolution(AdvancedConvolutionWindow window, FasterBitmap toCompute, int centerPositionX, int centerPositionY) {
            for (int yIndex = centerPositionY - (window.windowDiameter/2); yIndex< centerPositionY + (window.windowDiameter/2) + 1; yIndex++) {
                for (int xIndex = centerPositionX - (window.windowDiameter/2); xIndex<centerPositionX + (window.windowDiameter/2) + 1; xIndex++) {
                    if ((yIndex < 0)||(yIndex >= toCompute.Height)||(xIndex < 0)||(xIndex >= toCompute.Width)) continue;
                    else {
                        int yValueIndex = yIndex - centerPositionY + (window.windowDiameter/2);
                        int xValueIndex = xIndex - centerPositionX + (window.windowDiameter/2);
                        window.sumElements.Add((toCompute.GetPixel(xIndex, yIndex).B)*(window.valueMatrix[yValueIndex][xValueIndex])); 
                    }
                }
            } 
            return;
        }

        static int[] RobertConvolution(AdvancedConvolutionWindow window, FasterBitmap toCompute, int centerPositionX, int centerPositionY) {
            int[] values = new int[2];
            int xComputation = 0;
            int yComputation = 0;
            for (int yIndex = centerPositionY; yIndex< centerPositionY + 2 ; yIndex++) {
                for (int xIndex = centerPositionX; xIndex<centerPositionX + 2; xIndex++) {
                    if ((yIndex < 0)||(yIndex >= toCompute.Height)||(xIndex < 0)||(xIndex >= toCompute.Width)) continue;
                    else {
                        xComputation = xComputation + (toCompute.GetPixel(xIndex, yIndex).B * window.horizontalValueMatrix[yIndex - centerPositionY][xIndex - centerPositionX]);
                        yComputation = yComputation + (toCompute.GetPixel(xIndex, yIndex).B * window.verticalValueMatrix[yIndex - centerPositionY][xIndex - centerPositionX]);
                    }
                }
            } 
            values[0] = xComputation;
            values[1] = yComputation;
            return(values);
        }

        static int[] SobelPrewittConvolution(AdvancedConvolutionWindow window, FasterBitmap toCompute, int centerPositionX, int centerPositionY) {
            int[] values = new int[2];
            int xComputation = 0;
            int yComputation = 0;
            int yIndexFromZero = 0;
            for (int yIndex = centerPositionY - (window.windowDiameter/2); yIndex< centerPositionY + (window.windowDiameter/2) + 1; yIndex++) {
                int xIndexFromZero = 0;
                for (int xIndex = centerPositionX - (window.windowDiameter/2); xIndex<centerPositionX + (window.windowDiameter/2) + 1; xIndex++) {
                    if ((yIndex < 0)||(yIndex >= toCompute.Height)||(xIndex < 0)||(xIndex >= toCompute.Width)) continue;
                    else {
                        xComputation = xComputation + (toCompute.GetPixel(xIndex, yIndex).B * window.horizontalValueMatrix[yIndexFromZero][xIndexFromZero]);
                        yComputation = yComputation + (toCompute.GetPixel(xIndex, yIndex).B * window.verticalValueMatrix[yIndexFromZero][xIndexFromZero]);
                    }
                    xIndexFromZero++;
                }
                yIndexFromZero++;
            } 
            values[0] = xComputation;
            values[1] = yComputation;
            return(values);
        }

    }
}