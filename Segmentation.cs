using System;
using System.Collections.Generic;
using System.Drawing;

namespace imageComputing
{

    public class Segmentation {

        static string savePath;

        public static void DoSegmentation(ImageData toCompute, int[][] zonesMap, string segmentationType, bool saveResult) {
            zonesMap = ChooseSegmentation(toCompute.fileName, zonesMap, segmentationType);
            ApplyNewZoneMap(toCompute, zonesMap);
            if (saveResult == true) toCompute.SaveBitmap(savePath);
        }

        static void ApplyNewZoneMap(ImageData toCompute, int[][] zonesMap) {
            int zoneNumber = countZone(zonesMap);
            Random random = new Random();
            List<Color> colorList = new List<Color>();
            for (int index = 1; index < zoneNumber + 1; index++) {
                colorList.Add(Color.FromArgb(index*random.Next((int)(256/index)), index*random.Next((int)(256/index)), index*random.Next((int)(256/index))));
            }
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    if (zonesMap[yIndex][xIndex] == 0) toCompute.image.SetPixel(xIndex, yIndex, Color.FromName("White"));
                    else toCompute.image.SetPixel(xIndex, yIndex, colorList[zonesMap[yIndex][xIndex] - 1]);
                }
            }
        }

        static int[][] ChooseSegmentation(string filename, int[][] zonesMap, string segmentationType) {
            if (segmentationType == "erosion") {
                savePath = "results/" + filename + "ErosionSegResult.bmp";
                return DoErosion(zonesMap);
            }
            else if (segmentationType == "dilation") {
                savePath = "results/" + filename + "DilationSegResult.bmp";
                return DoDilation(zonesMap);
            }
            else if (segmentationType == "opening") {
                savePath = "results/" + filename + "OpeningSegResult.bmp";
                return DoOpening(zonesMap);
            }
            else if (segmentationType == "closing") {
                savePath = "results/" + filename + "ClosingSegResult.bmp";
                return DoClosing(zonesMap);
            }
            else if (segmentationType == "finalerosion") {
                savePath = "results/" + filename + "FinalErosionSegResult.bmp";
                return DoFinalErosion(zonesMap);
            }
            else Console.WriteLine(" Invalid segmentation choice!");
            return null;
        }

        public static int[][] DoErosion(int[][] zonesMap) {
            int[][] newZonesMap = new int[zonesMap.Length][];
            for (int index = 0; index < zonesMap.Length; index++) newZonesMap[index] = new int[zonesMap[index].Length];
            for (int yIndex = 0; yIndex < zonesMap.Length; yIndex++) {
                for (int xIndex = 0; xIndex < zonesMap[yIndex].Length; xIndex++) {
                    int pixelCounter = 0;
                    for (int yWindow = yIndex - 1; yWindow < yIndex + 2; yWindow++) {
                        for (int xWindow = xIndex - 1; xWindow < xIndex + 2; xWindow++) {
                            try {
                                if(zonesMap[yWindow][xWindow] == zonesMap[yIndex][xIndex]) pixelCounter++;
                            }
                            catch(IndexOutOfRangeException) {
                                continue;
                            }
                        }
                    }
                    if (pixelCounter != 9) newZonesMap[yIndex][xIndex] = 0;
                    else newZonesMap[yIndex][xIndex] = zonesMap[yIndex][xIndex];
                }
            }
            return newZonesMap;
        }

        public static int[][] DoDilation(int[][] zonesMap) {
            int[][] newZonesMap = new int[zonesMap.Length][];
            for (int index = 0; index < zonesMap.Length; index++) newZonesMap[index] = new int[zonesMap[index].Length];
            for (int yIndex = 0; yIndex < zonesMap.Length; yIndex++) {
                for (int xIndex = 0; xIndex < zonesMap[yIndex].Length; xIndex++) {
                    if (zonesMap[yIndex][xIndex] != 0) {
                        for (int yWindow = yIndex - 1; yWindow < yIndex + 2; yWindow++) {
                            for (int xWindow = xIndex - 1; xWindow < xIndex + 2; xWindow++) {
                                try {
                                    newZonesMap[yWindow][xWindow] = zonesMap[yIndex][xIndex];
                                }
                                catch(IndexOutOfRangeException) {
                                    continue;
                                }
                            }
                        }
                    }
                    else newZonesMap[yIndex][xIndex] = 0;
                }
            }
            return newZonesMap;
        }

        public static int[][] DoOpening(int[][] zonesMap) {
            return DoDilation(DoErosion(zonesMap));
        }

        public static int[][] DoClosing(int[][] zonesMap) {
            return DoErosion(DoDilation(zonesMap));
        }

        public static int[][] DoFinalErosion(int[][] zonesMap) {
            int[][] copyZonesMap = new int[zonesMap.Length][];
            for (int index = 0; index < zonesMap.Length; index++) {
                copyZonesMap[index] = new int[zonesMap[index].Length];
            }
            bool stop = false;
            while (!stop) {
                int pixelCount = 0;
                Array.Copy(zonesMap, copyZonesMap, zonesMap.Length);
                zonesMap = DoErosion(zonesMap);
                for (int yIndex = 0; yIndex < zonesMap.Length; yIndex++) {
                    for (int xIndex = 0; xIndex < zonesMap[yIndex].Length; xIndex++) {
                        if (zonesMap[yIndex][xIndex] != 0) pixelCount++;
                    }
                }
                if (pixelCount == 0) {
                    stop = true;
                }

            }
            return copyZonesMap;
        }

        private static int countZone(int[][] zonesMap) {
            int maximum = 0;
            for (int yIndex = 0; yIndex < zonesMap.Length; yIndex++) {
                for (int xIndex = 0; xIndex < zonesMap[yIndex].Length; xIndex++) {
                    if (zonesMap[yIndex][xIndex] >= maximum) maximum = zonesMap[yIndex][xIndex];
                }
            }
            return maximum;
        }

    }
}