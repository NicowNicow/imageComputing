using System;
using System.Collections.Generic;
using System.Drawing;

namespace imageComputing
{

    public class Shape {
        public List<Point> inShape {get; private set;}
        public BoundingBox boundingBox {get; private set;}
        public float inscribedDiameter {get; private set;}
        public float circumscribedDiameter {get; private set;}
        public float croftonPerimeter {get; private set;}
        public int surface {get; private set;}
        public float radialCircularity {get; private set;}
        public float geometricalCircularity {get; private set;}
        public float symmetry {get; private set;}
        public float connectivity {get; private set;}
        public float convexity {get; private set;}

        public Shape(int[][] zonesList, int zoneNumber) {
            this.inShape = GetPixels(zonesList, zoneNumber);
            this.surface = this.inShape.Count;
            this.boundingBox = GetBounding();
        }

        private List<Point> GetPixels(int[][] zonesList, int zoneNumber) {
            List<Point> shape = new List<Point>();
            for (int yIndex = 0; yIndex < zonesList.Length; yIndex++) {
                for (int xIndex = 0; xIndex < zonesList[yIndex].Length; xIndex++) {
                    if (zonesList[yIndex][xIndex] == zoneNumber) {
                        shape.Add(new Point(xIndex, yIndex, zoneNumber));
                    }
                }
            }
            return shape;
        }

        private BoundingBox GetBounding() {
            int tempoXmin = -1;
            int tempoXmax = -1;
            int tempoYmin = -1;
            int tempoYmax = -1;
            for (int index = 0; index < this.inShape.Count; index++) {
                if ((inShape[index].x <= tempoXmin)||(tempoXmin == -1)) tempoXmin = inShape[index].x;
                if ((inShape[index].x >= tempoXmax)||(tempoXmax == -1)) tempoXmax = inShape[index].x;
                if ((inShape[index].y <= tempoYmin)||(tempoYmin == -1)) tempoYmin = inShape[index].y;
                if ((inShape[index].y >= tempoYmax)||(tempoYmax == -1)) tempoYmax = inShape[index].y;
            }
            return new BoundingBox(tempoXmin, tempoXmax, tempoYmin, tempoYmax);
        }

        public void DrawBoundingBox(ImageData toCompute, int zoneNumber) {
            Random random = new Random();
            for (int yIndex=0; yIndex<toCompute.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<toCompute.image.Width; xIndex++) {toCompute.image.SetPixel(xIndex, yIndex, Color.FromName("White"));
                }
            }
            for (int index = 0; index < inShape.Count; index++) {
                toCompute.image.SetPixel(this.inShape[index].x, this.inShape[index].y, Color.FromName("Black"));
            }
            for (int yIndex = this.boundingBox.yMin; yIndex <= this.boundingBox.yMax; yIndex++) {
                try {
                    toCompute.image.SetPixel(this.boundingBox.xMin, yIndex, Color.FromName("Blue"));
                    toCompute.image.SetPixel(this.boundingBox.xMax, yIndex, Color.FromName("Blue"));
                }
                catch (IndexOutOfRangeException) {
                    continue;
                }
            }
            for (int xIndex = this.boundingBox.xMin; xIndex <= this.boundingBox.xMax; xIndex++) {
                try {
                    toCompute.image.SetPixel(xIndex, this.boundingBox.yMin, Color.FromName("Blue"));
                    toCompute.image.SetPixel(xIndex, this.boundingBox.yMax, Color.FromName("Blue"));
                }
                catch (IndexOutOfRangeException) {
                    continue;
                }
            }
            toCompute.SaveBitmap("results/" + toCompute.fileName + "BoundingBoxShape" + zoneNumber + ".bmp");
        }

    }

    public class BoundingBox {
        public int xMin {get; private set;}
        public int xMax {get; private set;}
        public int yMin {get; private set;}
        public int yMax {get; private set;}

        public BoundingBox(int xMin, int xMax, int yMin, int yMax) {
            this.xMin = xMin;
            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;
        }
    }


}