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
        public int boundingBoxPerimeter {get; private set;}
        public int feretBoudingBoxPerimeter {get; private set;}
        public float croftonPerimeter {get; private set;}
        public int surface {get; private set;}
        public float radialCircularity {get; private set;}
        public float geometricalCircularity {get; private set;}
        public float symmetry {get; private set;}
        public float connectivity {get; private set;}
        public float convexity {get; private set;}

        public Shape(int[][] zonesList, int zoneNumber) {
            Initialize();
            this.inShape = GetPixels(zonesList, zoneNumber);
            this.surface = this.inShape.Count;
            this.boundingBox = GetBounding();
            this.boundingBoxPerimeter = GetBoundingBoxPerimeter();
            this.geometricalCircularity = GetGeometricalCircularity();
        }

        private void Initialize() {
            this.inShape = null;
            this.boundingBox = null;
            this.inscribedDiameter = 0;
            this.circumscribedDiameter = 0;
            this.boundingBoxPerimeter = 0;
            this.feretBoudingBoxPerimeter = 0;
            this.croftonPerimeter = 0;
            this.surface = 0;
            this.radialCircularity = 0;
            this.geometricalCircularity = 0;
            this.symmetry = 0;
            this.connectivity = 0;
            this.convexity = 0;
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

        private int GetBoundingBoxPerimeter() {
            return 2*(this.boundingBox.yMax - this.boundingBox.yMin) + 2*(this.boundingBox.xMax - this.boundingBox.xMin);
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
                if ((yIndex < 0)||(yIndex >= toCompute.image.Height)) continue;
                else {
                    toCompute.image.SetPixel(this.boundingBox.xMin, yIndex, Color.FromName("Blue"));
                    toCompute.image.SetPixel(this.boundingBox.xMax, yIndex, Color.FromName("Blue"));
                }
            }
            for (int xIndex = this.boundingBox.xMin; xIndex <= this.boundingBox.xMax; xIndex++) {
                if ((xIndex < 0)||(xIndex >= toCompute.image.Width)) continue;
                else {
                    toCompute.image.SetPixel(xIndex, this.boundingBox.yMin, Color.FromName("Blue"));
                    toCompute.image.SetPixel(xIndex, this.boundingBox.yMax, Color.FromName("Blue"));
                }
            }
            toCompute.SaveBitmap("results/" + toCompute.fileName + "BoundingBoxShape" + zoneNumber + ".bmp");
        }

        private float GetGeometricalCircularity() {
            float circularity = 0;
            if (this.croftonPerimeter != 0) {
                circularity = (float)(4*Math.PI*this.surface)/(float)(this.croftonPerimeter*this.croftonPerimeter);
            }
            else if (this.feretBoudingBoxPerimeter != 0) {
                circularity = (float)(4*Math.PI*this.surface)/(float)(this.feretBoudingBoxPerimeter*this.feretBoudingBoxPerimeter);
            }
            else {
                circularity = (float)(4*Math.PI*this.surface)/(float)(this.boundingBoxPerimeter*this.boundingBoxPerimeter);
            }
            return circularity;
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