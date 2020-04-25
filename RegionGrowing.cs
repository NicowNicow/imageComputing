using System;
using System.Drawing;
using System.Collections.Generic;

namespace imageComputing
{
    public class Point {
        public int x {get; private set;}
        public int y {get; private set;}
        public int mark {get; set;}

        public Point(int x, int y) {
            this.x = x;
            this.y = y;
            this.mark = -1;
        }

        public Point(int x, int y, int mark) {
            this.x = x;
            this.y = y;
            this.mark = mark;
        }

        public void Print() {
            Console.WriteLine("x = " + this.x + "; y = " + this.y + "; region : " + this.mark);
        }
    }

    public class RegionGrowing : TailRecursionProcess {
        public int markNumber {get; private set;}
        public List<Point> seeds {get; private set;}
        public int[][] zonesMap {get; private set;}

        public RegionGrowing() {
            this.markNumber = 0;
            this.seeds = new List<Point>();
        }

        public void DoRegionGrowing(ImageData toCompute) {
            this.AllocZoneMap(toCompute.image.Width, toCompute.image.Height);
            this.GenerateSeeds(toCompute, 2);
            this.zonesMap = DoRecursion<int[][]> (() => DoDilation(toCompute, this.seeds, this.zonesMap));
        }

        
        public void DrawAllShapes (ImageData toCompute) {
            Random random = new Random();
            List<Color> colorList = new List<Color>();
            for (int index = 1; index < this.markNumber + 1; index++) {
                colorList.Add(Color.FromArgb(index*random.Next((int)(256/index)), index*random.Next((int)(256/index)), index*random.Next((int)(256/index))));
            }
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {
                    if (this.zonesMap[yIndex][xIndex] == 0) toCompute.image.SetPixel(xIndex, yIndex, Color.FromName("White"));
                    else toCompute.image.SetPixel(xIndex, yIndex, colorList[this.zonesMap[yIndex][xIndex] - 1]);
                }
            }
            toCompute.SaveBitmap("results/" + toCompute.fileName + "RegionGrowingAllShapes.bmp");
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
            toCompute.SaveBitmap("results/" + toCompute.fileName + "RegionGrowingShape" + zoneNumber + ".bmp");
        }

        private void AllocZoneMap(int xLimit, int yLimit) {
            this.zonesMap = new int[yLimit][];
            for (int yIndex = 0; yIndex < yLimit; yIndex++) {
                this.zonesMap[yIndex] = new int[xLimit];
            }
        }

        private void GenerateSeeds(ImageData toCompute, int seedsNumberMax) {
            List<int> greyValue =  toCompute.GetNMaxIndex(seedsNumberMax);
            foreach (int element in greyValue) {
                Point seed = CheckPoint(toCompute, element);
                if (seed != null) {
                    this.markNumber++;
                    seed.mark = this.markNumber;
                    this.zonesMap[seed.y][seed.x] = this.markNumber;
                    this.seeds.Add(seed);
                }
            }
        }

        private static Point CheckPoint(ImageData toCompute, int element) {
            List<Point> possiblePoints = new List<Point>();
            Random random = new Random();
            for (int yIndex = 0; yIndex < toCompute.image.Height; yIndex++) {
                for (int xIndex = 0; xIndex < toCompute.image.Width; xIndex++) {
                    Color pixelColor = toCompute.image.GetPixel(xIndex, yIndex);
                    if (element == HSVColor.HSVtoGreyLevelRGB(HSVColor.RGBtoHSV(pixelColor)).B) {
                        possiblePoints.Add(new Point(xIndex, yIndex));
                    }
                }
            }
            if (possiblePoints.Count == 0) return null;
            else return possiblePoints[random.Next(possiblePoints.Count)];
        }

        private TailRecursionResult<int[][]> DoDilation(ImageData toCompute, List<Point> seedsList, int[][] map) {
           int isNotPossible = 0;
           foreach (Point point in seedsList) {
               int sum = 0;
               int pixelCounter = 0;
               for (int yIndex = point.y - 1; yIndex < point.y + 2; yIndex++) {
                   for (int xIndex = point.x - 1; xIndex < point.x + 2; xIndex++) {
                       try {
                           if (zonesMap[yIndex][xIndex] == 0) {
                               sum = sum + HSVColor.HSVtoGreyLevelRGB(HSVColor.RGBtoHSV(toCompute.image.GetPixel(xIndex, yIndex))).R;
                               pixelCounter++;
                           }
                       }
                       catch (IndexOutOfRangeException) {
                           continue;
                       }
                   }
               }
               if (pixelCounter == 0) isNotPossible++;
               else {
                    int mean = (int)(sum/pixelCounter);
                    int greyLevel = HSVColor.HSVtoGreyLevelRGB(HSVColor.RGBtoHSV(toCompute.image.GetPixel(point.x, point.y))).R;
                    if ((mean < greyLevel - 8)||(mean > greyLevel + 8)) isNotPossible++;
                    else {
                        for (int yIndex = point.y - 1; yIndex < point.y + 2; yIndex++) {
                            for (int xIndex = point.x - 1; xIndex < point.x + 2; xIndex++) {
                                try {
                                    zonesMap[yIndex][xIndex] =  point.mark;
                                }
                                catch (IndexOutOfRangeException) {
                                    continue;
                                }
                            }
                        }
                    }
               }
           }
           if (isNotPossible == seedsList.Count) return End(zonesMap);
           else {
               seedsList = RefillSeedsFromZonesMap(seedsList, zonesMap);
               return Next(() => DoDilation(toCompute, seedsList, zonesMap));
           }
        }

        private List<Point> RefillSeedsFromZonesMap(List<Point> seeds, int[][] zonesMap) {
            seeds.Clear();
            for (int yIndex = 0; yIndex < zonesMap.Length; yIndex++) {
                for (int xIndex = 0; xIndex < zonesMap[yIndex].Length; xIndex++) {
                    if (zonesMap[yIndex][xIndex] != 0) {
                        seeds.Add(new Point(xIndex, yIndex, zonesMap[yIndex][xIndex]));
                    }
                }
            }
            return seeds;
        }

        public T DoRecursion<T>(Func<TailRecursionResult<T>> Tfunction) {
            while (true) {
                var TailRecursionResult = Tfunction();
                if (TailRecursionResult.isFinalResult) return TailRecursionResult.Tresult;
                Tfunction = TailRecursionResult.TnextStep;
            }
        }

        public TailRecursionResult<T> End<T>(T Tresult) {
            return new TailRecursionResult<T>(true, Tresult, null);
        }

        public TailRecursionResult<T> Next<T>(Func<TailRecursionResult<T>> TnextStep) {
            return new TailRecursionResult<T>(false, default(T), TnextStep);
        }
    }
}