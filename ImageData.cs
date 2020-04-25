using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace imageComputing
{
    public class ImageData
    {
        public Bitmap bitmapImage { get; set; }
        public FasterBitmap image { get; set; }
        public int pixelNumber { get; private set; }
        public string fileName { get; private set; }
        public List<int> histogram { get; private set; } = null;
        public List<int> cumulativeHistogram {get; private set;} = null;
        public double entropy { get; private set; } = 0;
        public float[] dynamicExpansionParameters {get; private set; } = null;

        public ImageData(string imagePath) { //Constructor
            image = CreateBitmap(imagePath);
            fileName = Path.GetFileNameWithoutExtension(imagePath);
            pixelNumber = image.Height*image.Width;
        }

        private void CreateHistogram() { //Initialize the histogram
            this.histogram = new List<int>();
            this.histogram.AddRange(Enumerable.Repeat(0, 256));
        }

        private void CreateCumulativeHistogram() { //Initialize the cumulative histogram
            this.cumulativeHistogram = new List<int>();
            this.cumulativeHistogram.AddRange(Enumerable.Repeat(0, 256));
        }

        public int GetIndexMax() { //Get the index of the hightest peak of an histogram
            int maxTempo = 0;
            int maxIndex = 0;
            for (int index = 0; index < 256; index++) {
                if (this.histogram[index] >= maxTempo) {
                    maxTempo = this.histogram[index];
                    maxIndex = index;
                }
            }
            return (maxIndex);
        }

        public List<int> GetNMaxIndex(int indexNumber) { //get the indexes of the n highest peaks of the histogram, except the white one
            int[] histogramCopy = new int[256];
            List<int> maxIndexes = new List<int>();
            this.histogram.CopyTo(histogramCopy);
            while ((maxIndexes.Count != indexNumber)&&(histogramCopy.Max() != 0)) {
                int index = histogram.IndexOf(histogramCopy.Max());
                if (index == 255) {
                    histogramCopy[index] = 0;
                }
                else {
                    maxIndexes.Add(index);
                    histogramCopy[index] = 0;
                }
            }
            return maxIndexes;
        }

        private FasterBitmap CreateBitmap(string imagePath) { //Initialization of the Bitmap and FasterBitmap entities
            try {
                this.bitmapImage = new Bitmap(imagePath);
            }
            catch(FileNotFoundException) {
                Console.WriteLine("File not found!");
                return(null);
            }
            catch(ArgumentException) {
                Console.WriteLine("The given argument is not a path!");
                return(null);
            }
            FasterBitmap imageFast = new FasterBitmap(bitmapImage);
            imageFast.LockBits();
            return(imageFast);
        }

        public void SaveBitmap(string savePath) {  //Save the final Bitmap
            this.image.UnlockBits();
            this.bitmapImage.Save(savePath);
        }

        public void GetHistogramFromImage() { //Calculate the image's histogram
            int greyLevel; 
            CreateHistogram();
            for (int yIndex=0; yIndex<this.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<this.image.Width; xIndex++) {
                    Color pixelColor = this.image.GetPixel(xIndex, yIndex);
                    greyLevel = (int)(0.299*pixelColor.R + 0.587*pixelColor.G + 0.114*pixelColor.B);
                    this.histogram[greyLevel]++;
                }
            }
        }

        public void GetHistogramFromColoredImage() { //Calculate the histogram from a colored image
            CreateHistogram();
            List<HSVColor> colorList = new List<HSVColor>();
            for (int yIndex = 0; yIndex <this.image.Height; yIndex++) {
                for (int xIndex=0; xIndex<this.image.Width; xIndex++) {
                    colorList.Add(HSVColor.RGBtoHSV(this.image.GetPixel(xIndex, yIndex)));
                }
            }
            foreach (HSVColor element in colorList) {
                int greyLevel = HSVColor.HSVtoGreyLevelRGB(element).B;
                this.histogram[greyLevel]++;
            }
        }

        public void GetCumulativeHistogram() { //Calculate the image's cumulative histogram
            int sum = 0;
            CreateCumulativeHistogram();
            for (int index=0; index<256; index++) {
                sum = sum + this.histogram[index];
                this.cumulativeHistogram[index] = sum;
            }
        }

        public void GetEntropy() { //Calculate the image entropy
            double entropySum = 0;
            double tempValue = 0;
            double maxPixel = (double) this.image.Height*this.image.Width ;
            for(int index=0; index<256; index++) {
                if (this.histogram[index] != 0) {
                    tempValue = (double)((double)this.histogram[index]/maxPixel);
                    entropySum = entropySum + tempValue*(Math.Log(tempValue));
                }
            }
            this.entropy = -entropySum;
        }

        public void GetExpansionParameters() { //Calculate the two parameters for dynamic expansion
            float [] parameters = {0,0};
            int h0 = 0, h1 = 255;
            h0 = GetMinHistogram();
            h1 = GetMaxHistogram();
            parameters[0] = (float)(255/(h1-h0));
            parameters[1] = (float)((-255*h0)/(h1-h0));
            this.dynamicExpansionParameters = parameters;
        }

        private int GetMinHistogram() {
            int h0 = 0;
            for (int index=1; index<256; index++) {
                if (this.histogram[index]!=0) {
                    h0 = index;
                    break;
                }
            }
            return(h0);
        }

        private int GetMaxHistogram() {
            int h1 = 255;
            for (int index=254; index>=0; index--) {
                if (this.histogram[index]!=0) {
                    h1 = index;
                    break;
                }
            }
            return(h1);
        }

    }
}