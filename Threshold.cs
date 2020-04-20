using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

namespace imageComputing
{
    public abstract class Threshold
    {

        public static void ManualThreshold(ImageData toCompute) {
            int threshold = 0;
            Console.WriteLine("Enter a threshold value: ");
            if (!int.TryParse(Console.ReadLine(), out threshold)) {
                Console.WriteLine("Invalid value !");
                return;
            }
            else {
                doSimpleThreshold(toCompute, threshold, "results/" + toCompute.fileName + "ManualThresholdResult.bmp");
            }
        }

        private static void doSimpleThreshold(ImageData toCompute, int threshold, string savePath) {
            Color currentColor; 
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                    for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                        Color pixelColor = toCompute.image.GetPixel(xIndex, yIndex);
                        int currentGreyLevel = (int)(0.299*pixelColor.R + 0.587*pixelColor.G + 0.114*pixelColor.B);
                        if (currentGreyLevel >= threshold) {
                            currentColor = Color.FromName("White");
                        } 
                        else {
                            currentColor = Color.FromName("Black"); 
                        }
                        toCompute.image.SetPixel(xIndex,yIndex, currentColor);
                    }
                }
                toCompute.saveBitmap(savePath);
        }

        public static void simpleVarianceThreshold(ImageData toCompute) {
            double maxVariance = 0;
            int maxThreshold = 0;
            for (int threshold = 0; threshold < 256; threshold++) {
                int countPixelsInf = 0, countWeightedPixelsInf = 0;
                int countPixelsSup = 0, countWeightedPixelsSup = 0;
                double variance = 0;
                double averageInf = 0, averageSup = 0, average = 0;
                for (int greyLevelIndex = 0; greyLevelIndex < 256; greyLevelIndex++) {
                    if (greyLevelIndex <= threshold) {
                        countPixelsInf = countPixelsInf + toCompute.histogram[greyLevelIndex];
                        countWeightedPixelsInf = countWeightedPixelsInf + toCompute.histogram[greyLevelIndex]*greyLevelIndex;
                    }
                    if (greyLevelIndex > threshold) {
                        countPixelsSup = countPixelsSup + toCompute.histogram[greyLevelIndex];
                        countWeightedPixelsSup = countWeightedPixelsSup + toCompute.histogram[greyLevelIndex]*greyLevelIndex;
                    }
                }
                if (countPixelsInf != 0) {
                    averageInf = (double)(countWeightedPixelsInf/countPixelsInf);
                }
                if (countPixelsSup != 0) {
                    averageSup = (double)(countWeightedPixelsSup/countPixelsSup);
                }
                average = (double)((countWeightedPixelsInf + countWeightedPixelsSup)/toCompute.pixelNumber);
                variance = (double)((double)countPixelsInf/(double)toCompute.pixelNumber)*(double)(average-averageInf)*(double)(average-averageInf) + (double)((double)countPixelsSup/(double)toCompute.pixelNumber)*(double)(average-averageSup)*(double)(average-averageSup);
                if (maxVariance <= variance) {
                    maxVariance = variance;
                    maxThreshold = threshold;
                }
            }
            doSimpleThreshold(toCompute, maxThreshold, "results/" + toCompute.fileName + "SimpleVarianceThresholdResult.bmp");
        }

        public static void simpleEntropyThreshold(ImageData toCompute) {
            double maxEntropy = 0;
            int maxThreshold = 0;
            for (int threshold = 0; threshold < 256; threshold++) {
                int countPixelsInf = 0;
                double entropy = 0;
                for (int greyLevelIndex = 0; greyLevelIndex <= threshold; greyLevelIndex++) {
                    countPixelsInf = countPixelsInf + toCompute.histogram[greyLevelIndex];
                }
                for (int greyLevelIndex = 0; greyLevelIndex < 256; greyLevelIndex++) {
                    if ((greyLevelIndex <= threshold)&&(toCompute.histogram[greyLevelIndex]!=0)) {
                        entropy = entropy + (double)((1/(double)countPixelsInf)*toCompute.histogram[greyLevelIndex]*Math.Log(toCompute.histogram[greyLevelIndex]));
                    }
                    if ((greyLevelIndex > threshold)&&(toCompute.histogram[greyLevelIndex]!=0)) {
                        entropy = entropy + (double)((1/(double)(toCompute.pixelNumber - countPixelsInf))*toCompute.histogram[greyLevelIndex]*Math.Log(toCompute.histogram[greyLevelIndex]));
                    }
                }
                if (countPixelsInf!=0) {
                    entropy = -entropy + Math.Log((double)(countPixelsInf*toCompute.pixelNumber));
                }
                else {
                    entropy = -entropy;
                }
                if (maxEntropy < entropy) {
                    maxEntropy = entropy;
                    maxThreshold = threshold;
                }
            }
            doSimpleThreshold(toCompute, maxThreshold, "results/" + toCompute.fileName + "SimpleEntropyThresholdResult.bmp");
        }

    }
}