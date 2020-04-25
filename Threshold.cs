using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

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
                DoSimpleThreshold(toCompute, threshold, "results/" + toCompute.fileName + "ManualThresholdResult.bmp");
            }
        }

        public static void ManualMultiThreshold(ImageData toCompute) {
            List<int> thresholds = new List<int>();
            int tempoValue = 0;
            int failParseCounter  = 0;
            Console.WriteLine("Enter thresholds values: ");
            var thresholdValues = Console.ReadLine().Split(" ");
            foreach (var value in thresholdValues)
            {
                if (!int.TryParse(value , out tempoValue)) {
                    failParseCounter++;
                }
                else {
                    thresholds.Add(tempoValue);
                }
            }
            if (failParseCounter != 0) Console.WriteLine("An invalid threshold value was given. It will be ignored.");
            if (thresholds.Count == 0) {
                Console.WriteLine("No valid threshold given. Process cancelled.");
                return;
            }
            else {
                DoMultiThreshold(toCompute, thresholds, "results/" + toCompute.fileName + "ManualMultiThresholdResult.bmp");
            }
        }

        private static void DoSimpleThreshold(ImageData toCompute, int threshold, string savePath) {
            Color currentColor; 
            for (int yIndex=0; yIndex < toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex < toCompute.image.Width; xIndex++) {
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
            toCompute.SaveBitmap(savePath);
        }

        private static void DoMultiThreshold(ImageData toCompute, List<int> thresholds, string savePath) {
            Random random = new Random();
            int colorIndex = 0;
            List<Color> newPixelColor = new List<Color>();
            List<Color> possibleColor = new List<Color>();
            int indexMaxThreshold = thresholds.IndexOf(thresholds.Max());
            for (int index =0; index < thresholds.Count; index++) {
                if (index == indexMaxThreshold) possibleColor.Add(Color.FromName("White"));
                else possibleColor.Add(Color.FromArgb(random.Next(0,256), random.Next(0,256), random.Next(0,256)));
            }
            for (int yIndex=0; yIndex < toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex < toCompute.image.Width; xIndex++) {
                    int currentThreshold = 0;
                    foreach (int threshold in thresholds) {
                        Color pixelColor = toCompute.image.GetPixel(xIndex, yIndex);
                        int currentGreyLevel = (int)(0.299*pixelColor.R + 0.587*pixelColor.G + 0.114*pixelColor.B);
                        if ((currentGreyLevel >= threshold)&&(threshold > currentThreshold)) currentThreshold = threshold;
                    }
                    int indexOfThreshold = thresholds.IndexOf(currentThreshold);
                    if (indexOfThreshold == -1) newPixelColor.Add(Color.FromName("Black"));
                    else newPixelColor.Add(possibleColor[indexOfThreshold]);
                }
            }
            for (int yIndex=0; yIndex < toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex < toCompute.image.Width; xIndex++) {
                    toCompute.image.SetPixel(xIndex,yIndex, newPixelColor[colorIndex]);
                    colorIndex++;
                }
            }
            toCompute.SaveBitmap(savePath);
        }

        public static void SimpleVarianceThreshold(ImageData toCompute) {
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
            DoSimpleThreshold(toCompute, maxThreshold, "results/" + toCompute.fileName + "SimpleVarianceThresholdResult.bmp");
        }

        public static void SimpleEntropyThreshold(ImageData toCompute) {
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
            DoSimpleThreshold(toCompute, maxThreshold, "results/" + toCompute.fileName + "SimpleEntropyThresholdResult.bmp");
        }

        public static void MultiVarianceThreshold(ImageData toCompute, int thresholdsNumber) {
            Random random = new Random();
            double maxVariance = 0;
            List<int> thresholds = new List<int>();
            List<int> possibleThresholds = new List<int>();
            possibleThresholds.AddRange(Enumerable.Repeat(0, thresholdsNumber));
            for (long index = 0; index < (long)Math.Pow(256, thresholdsNumber); index++) {
                double currentVariance = CalculateMultiVariance(toCompute, possibleThresholds, thresholdsNumber);
                if (currentVariance >= maxVariance) {
                    thresholds.Clear();
                    thresholds.AddRange(Enumerable.Repeat(0, thresholdsNumber));
                    maxVariance = currentVariance;
                    for (int index2 = 0; index2 < thresholdsNumber; index2++) {
                        thresholds[index2] = possibleThresholds[index2];
                    }
                }
                possibleThresholds[0]++;
                for (int changeIndex = 0; changeIndex < thresholdsNumber; changeIndex++) {
                    if (possibleThresholds[changeIndex] == 256) {
                        try {
                            possibleThresholds[changeIndex] = 0;
                            possibleThresholds[changeIndex+1] = possibleThresholds[changeIndex+1] + 1;
                        }
                        catch (ArgumentOutOfRangeException) {
                            continue;
                        }
                    }
                }
            }
             for (int index =0; index <thresholdsNumber; index++) {
                Console.WriteLine(thresholds[index]);
             }
            DoMultiThreshold(toCompute, thresholds, "results/" + toCompute.fileName + "MultiVarianceThresholdResult.bmp");
        }

        private static double CalculateMultiVariance(ImageData toCompute, List<int> thresholds, int thresholdsNumber) {
            double variance = 0;
            List<int> countPixelsInf = new List<int>();
            List<int> countWeightedPixelsInf = new List<int>();
            List<double> averageInf = new List<double>();
            double average = 0;
            for (int greyLevelIndex = 0; greyLevelIndex < 255; greyLevelIndex++) {
                average = average + toCompute.histogram[greyLevelIndex]*greyLevelIndex;
            }
            average = (double)(average/toCompute.pixelNumber);
            thresholds = thresholds.OrderBy(threshold => threshold).ToList();
            for (int thresholdIndex = 0; thresholdIndex < thresholdsNumber; thresholdIndex++) {
                int countPixelsInfTempo = 0;
                int countWeightedPixelsInfTempo = 0;
                for (int greyLevelIndex = 0; greyLevelIndex <= thresholds[thresholdIndex]; greyLevelIndex++) {
                    countPixelsInfTempo = countPixelsInfTempo + toCompute.histogram[greyLevelIndex];
                    countWeightedPixelsInfTempo = countWeightedPixelsInfTempo + toCompute.histogram[greyLevelIndex]*greyLevelIndex;
                }
                countPixelsInf.Add(countPixelsInfTempo);
                countWeightedPixelsInf.Add(countWeightedPixelsInfTempo);
                if (countPixelsInf[thresholdIndex] != 0) {
                    averageInf.Add((double)(countWeightedPixelsInf[thresholdIndex]/countPixelsInf[thresholdIndex]));
                }
                else averageInf.Add(0);
            }
            for (int varianceIndex = 0; varianceIndex < thresholdsNumber; varianceIndex++) {
                if (countPixelsInf[varianceIndex] != 0) {
                    variance = variance + (double)((double)(countWeightedPixelsInf[varianceIndex]/countPixelsInf[varianceIndex])*(double)(average - averageInf[varianceIndex])*(double)(average - averageInf[varianceIndex]));
                }
            }
            return(variance);
        }

        public static void MultiEntropyThreshold(ImageData toCompute, int thresholdsNumber) {
            Random random = new Random();
            double maxEntropy = 0;
            List<int> thresholds = new List<int>();
            List<int> possibleThresholds = new List<int>();
            possibleThresholds.AddRange(Enumerable.Repeat(0, thresholdsNumber));
            for (long index = 0; index < (long)Math.Pow(256, thresholdsNumber); index++) {
                double currentEntropy = CalculateMultiEntropy(toCompute, possibleThresholds, thresholdsNumber);
                if (currentEntropy >= maxEntropy) {
                    thresholds.Clear();
                    thresholds.AddRange(Enumerable.Repeat(0, thresholdsNumber));
                    maxEntropy = currentEntropy;
                    for (int index2 = 0; index2 < thresholdsNumber; index2++) {
                        thresholds[index2] = possibleThresholds[index2];
                    }
                }
                possibleThresholds[0]++;
                for (int changeIndex = 0; changeIndex < thresholdsNumber; changeIndex++) {
                    if (possibleThresholds[changeIndex] == 256) {
                        try {
                            possibleThresholds[changeIndex] = 0;
                            possibleThresholds[changeIndex+1] = possibleThresholds[changeIndex+1] + 1;
                        }
                        catch (ArgumentOutOfRangeException) {
                            continue;
                        }
                    }
                }
            }
            DoMultiThreshold(toCompute, thresholds, "results/" + toCompute.fileName + "MultiEntropyThresholdResult.bmp");
        }

        private static double CalculateMultiEntropy(ImageData toCompute, List<int> thresholds, int thresholdsNumber) { 
            double entropy = 0;
            double log = 0;
            int productCountPixelInf = 1;
            List<int> countPixelsInf = new List<int>();
            List<double> sums = new List<double>();
            thresholds = thresholds.OrderBy(threshold => threshold).ToList();
            for (int thresholdIndex = 0; thresholdIndex < thresholdsNumber; thresholdIndex++) {
                int countPixelsInfTempo = 0;
                double countSumsTempo = 0;
                for (int greyLevelIndex = 0; greyLevelIndex <= thresholds[thresholdIndex]; greyLevelIndex++) {
                    countPixelsInfTempo = countPixelsInfTempo + toCompute.histogram[greyLevelIndex];
                }
                countPixelsInf.Add(countPixelsInfTempo);
                if (thresholdIndex == 0) {
                    for (int greyLevelIndex = 1; greyLevelIndex <= thresholds[thresholdIndex]; greyLevelIndex++) {
                        if (toCompute.histogram[greyLevelIndex] > 0) {
                            countSumsTempo = countSumsTempo + (double)(toCompute.histogram[greyLevelIndex]*Math.Log(toCompute.histogram[greyLevelIndex]));
                        }
                        else {
                            countSumsTempo = countSumsTempo + (double)(toCompute.histogram[greyLevelIndex]);
                        }
                    }
                }
                else {
                    for (int greyLevelIndex = thresholds[thresholdIndex - 1] + 1; greyLevelIndex <= thresholds[thresholdIndex]; greyLevelIndex++) {
                        if (toCompute.histogram[greyLevelIndex] > 0) {
                            countSumsTempo = countSumsTempo + (double)(toCompute.histogram[greyLevelIndex]*Math.Log(toCompute.histogram[greyLevelIndex]));
                        }
                        else {
                            countSumsTempo = countSumsTempo + (double)(toCompute.histogram[greyLevelIndex]);
                        }
                    }
                }
                sums.Add(countSumsTempo);
            }
            for (int index = 0; index < thresholdsNumber; index++) {
                entropy = entropy + (double)((1/countPixelsInf[index])*sums[index]);
                productCountPixelInf = productCountPixelInf * countPixelsInf[index];
            }
            if (productCountPixelInf >= 0) log = Math.Log(productCountPixelInf);
            entropy = -entropy + log;
            return(entropy);
        }

    }
}