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

        public static void doFilters(ImageData toCompute, string convolutionWindowFilter) {
            AdvancedConvolutionWindow window = CreateAdvancedConvolutionWindow.doCreateFilter(convolutionWindowFilter);
            chooseConvolution(window, toCompute);
            toCompute.saveBitmap(savePath);
        }

        public static void doFilters(ImageData toCompute, string convolutionWindowFilter, string windowNorm) {
            AdvancedConvolutionWindow window = CreateAdvancedConvolutionWindow.doCreateFilter(convolutionWindowFilter, windowNorm);
            chooseConvolution(window, toCompute);
            toCompute.saveBitmap(savePath);
        }

        public static void doGaussianFilter(ImageData toCompute, string windowDiameter) {
            AdvancedConvolutionWindow window = CreateAdvancedConvolutionWindow.doCreateDiameter(windowDiameter);
            chooseConvolution(window, toCompute);
            toCompute.saveBitmap(savePath);
        }

        static void chooseConvolution(AdvancedConvolutionWindow window, ImageData toCompute) {
            if (window.processingFilter == "gaussian") {
                doGaussian(window, toCompute);
                savePath = "results/" + toCompute.fileName + "GaussianFilterResult.bmp";
            }
            else if (window.processingFilter == "prewitt") {
                doSobelPrewitt(window, toCompute);
                savePath = "results/" + toCompute.fileName + "PrewittFilterResult.bmp";
            }
            else if (window.processingFilter == "sobel") {
                doSobelPrewitt(window, toCompute);
                savePath = "results/" + toCompute.fileName + "SobelFilterResult.bmp";
            }
            else if (window.processingFilter == "robert") {
                doRobert(window, toCompute);
                savePath = "results/" + toCompute.fileName + "RobertsFilterResult.bmp";
            }
            else if (window.processingFilter == "laplacian4") {
                doLaplacian(window, toCompute);
                savePath = "results/" + toCompute.fileName + "Laplacian4Result.bmp";
            }
            else if (window.processingFilter == "laplacian8") {
                doLaplacian(window, toCompute);
                savePath = "results/" + toCompute.fileName + "Laplacian8Result.bmp";
            }
        }


        static void doGaussian(AdvancedConvolutionWindow window, ImageData toCompute) {
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

        static void doSobelPrewitt(AdvancedConvolutionWindow window, ImageData toCompute) {
            List<Color> colorList = new List<Color>();
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    window.sumElements.Clear();
                    int[] values = SobelPrewittConvolution(window, toCompute.image, xIndex, yIndex);
                    int value = window.doNorm(values);
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

        static void doRobert(AdvancedConvolutionWindow window, ImageData toCompute) {
            List<Color> colorList = new List<Color>();
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    window.sumElements.Clear();
                    int[] values = RobertConvolution(window, toCompute.image, xIndex, yIndex);
                    int value = window.doNorm(values);
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

        static void doLaplacian(AdvancedConvolutionWindow window, ImageData toCompute) {
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
                    try {
                        int yValueIndex = yIndex - centerPositionY + (window.windowDiameter/2);
                        int xValueIndex = xIndex - centerPositionX + (window.windowDiameter/2);
                        window.sumElements.Add((toCompute.GetPixel(xIndex, yIndex).B)*(window.valueMatrix[yValueIndex][xValueIndex])); 
                    }
                    catch (ArgumentOutOfRangeException) {
                        continue;
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
                    try {
                        xComputation = xComputation + (toCompute.GetPixel(xIndex, yIndex).B * window.horizontalValueMatrix[yIndex - centerPositionY][xIndex - centerPositionX]);
                        yComputation = yComputation + (toCompute.GetPixel(xIndex, yIndex).B * window.verticalValueMatrix[yIndex - centerPositionY][xIndex - centerPositionX]);
                    }
                    catch (ArgumentOutOfRangeException) {
                        continue;
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
                    try {
                        xComputation = xComputation + (toCompute.GetPixel(xIndex, yIndex).B * window.horizontalValueMatrix[yIndexFromZero][xIndexFromZero]);
                        yComputation = yComputation + (toCompute.GetPixel(xIndex, yIndex).B * window.verticalValueMatrix[yIndexFromZero][xIndexFromZero]);
                    }
                    catch (ArgumentOutOfRangeException) {
                        continue;
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