using System;
using System.Collections.Generic;
using System.Drawing;

namespace imageComputing 
{

    public class Watershed {
        public int markNumber {get; private set;}
        public int[][] zonesMap {get; private set;}

        public Watershed() {
            this.markNumber = 0;
        }

        public void DoWatershed(ImageData toCompute, int isOkAltitude) {
            this.zonesMap = Watershed.AllocZoneMap(toCompute.image.Width, toCompute.image.Height);
            this.FindZero(toCompute);
            for (int greyLevelIndex = 0; greyLevelIndex < 256; greyLevelIndex++) {
                for (int yIndex = 0; yIndex < toCompute.image.Height; yIndex++) {
                    for (int xIndex = 0; xIndex < toCompute.image.Width; xIndex++) {
                        int pixelGreyScale = HSVColor.HSVtoGreyLevelRGB(HSVColor.RGBtoHSV(toCompute.image.GetPixel(xIndex, yIndex))).B;
                        if ((this.zonesMap[yIndex][xIndex] != 0)&&(pixelGreyScale >= greyLevelIndex)&&(pixelGreyScale <= greyLevelIndex + isOkAltitude)) {
                            for (int yWindow = yIndex - 1; yWindow < yIndex + 2; yWindow++) {
                                for (int xWindow = xIndex - 1; xWindow < xIndex + 2; xWindow++) {
                                    if ((yIndex < 0)||(yIndex >= toCompute.image.Height)||(xIndex < 0)||(xIndex >= toCompute.image.Width)) continue;
                                    else if (this.zonesMap[yWindow][xWindow] == 0) this.zonesMap[yWindow][xWindow] = this.zonesMap[yIndex][xIndex]; 
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DrawAllShapes (ImageData toCompute) {
            Random random = new Random();
            List<Color> colorList = new List<Color>();
            for (int index = 1; index < this.markNumber + 1; index++) {
                colorList.Add(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)));
            }
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    if (this.zonesMap[yIndex][xIndex] == 0) toCompute.image.SetPixel(xIndex, yIndex, Color.FromName("White"));
                    else toCompute.image.SetPixel(xIndex, yIndex, colorList[this.zonesMap[yIndex][xIndex] - 1]);
                }
            }
            toCompute.SaveBitmap("results/" + toCompute.fileName + "WatershedAllShapes.bmp");
        }

        public void DrawASingleShape(ImageData toCompute, int zoneNumber) {
            Random random = new Random();
            Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    if (this.zonesMap[yIndex][xIndex] == zoneNumber) toCompute.image.SetPixel(xIndex, yIndex, color);
                    else toCompute.image.SetPixel(xIndex, yIndex, Color.FromName("White"));
                }
            }
            toCompute.SaveBitmap("results/" + toCompute.fileName + "WatershedShape" + zoneNumber + ".bmp");
        }

        private static int[][] AllocZoneMap(int xLimit, int yLimit) {
            int[][] map = new int[yLimit][];
            for (int yIndex = 0; yIndex < yLimit; yIndex++) {
                map[yIndex] = new int[xLimit];
            }
            return map;
        }

        private void FindZero(ImageData toCompute) {
            int minimumGreyValue = 0;
            for (int indexGrey = 0; indexGrey < 256; indexGrey++) {
                if (toCompute.histogram[indexGrey] != 0) {
                    minimumGreyValue = indexGrey;
                    break;
                }
            }
            for (int yIndex = 0; yIndex < toCompute.image.Height; yIndex++) {
                for (int xIndex = 0; xIndex < toCompute.image.Width; xIndex++) {
                    if (HSVColor.HSVtoGreyLevelRGB(HSVColor.RGBtoHSV(toCompute.image.GetPixel(xIndex, yIndex))).B == minimumGreyValue) {
                        int notAround = 0;
                        for (int yWindow = yIndex - 1; yWindow < yIndex + 2; yWindow++) {
                            for (int xWindow = xIndex - 1; xWindow < xIndex + 2; xWindow++) {
                                if ((yIndex < 0)||(yIndex >= toCompute.image.Height)||(xIndex < 0)||(xIndex >= toCompute.image.Width)) continue;
                                else if (this.zonesMap[yWindow][xWindow] != 0) notAround = this.zonesMap[yWindow][xWindow]; 
                            }
                        }
                        if (notAround == 0) {
                            markNumber++;
                            this.zonesMap[yIndex][xIndex] = markNumber;
                        }
                        else this.zonesMap[yIndex][xIndex] = markNumber;
                    }
                }
            }
        }

    }
}