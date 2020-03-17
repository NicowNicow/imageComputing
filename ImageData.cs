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
        public List<int> histogram { get; private set; }
        public double entropy { get; private set; }
        public float[] dynamicExpansionParameters {get; private set; }

        public ImageData(string imagePath) { //Constructor
            image = createBitmap(imagePath);
            getHistogramFromImage();
            getEntropy();
            getExpansionParameters();
        }
        
        private void createHistogram() { //Initialize the histogram
            this.histogram = new List<int>();
            histogram.AddRange(Enumerable.Repeat(0, 256));
        }

        private FasterBitmap createBitmap(string imagePath) { //Initialization of the Bitmap and FasterBitmap entities
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

        public void saveBitmap(string savePath) {  //Save the final Bitmap
            this.image.UnlockBits();
            this.bitmapImage.Save(savePath);
        }

        private void getHistogramFromImage() { //Calculate the image's histogram
            int greyLevel; 
            createHistogram();
            for (int Yindex=0; Yindex<this.image.Height; Yindex++) {
                for (int Xindex=0; Xindex<this.image.Width; Xindex++) {
                    Color pixelColor = this.image.GetPixel(Xindex, Yindex);
                    greyLevel = (int)(0.299*pixelColor.R + 0.587*pixelColor.G + 0.114*pixelColor.B);
                    this.histogram[greyLevel]++;
                }
            }
        }

        private void getEntropy() { //Calculate the image entropy
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

        private void getExpansionParameters() { //Calculate the two parameters for dynamic expansion
            float [] parameters = {0,0};
            int h0 = 0, h1 = 255;
            h0 = getMinHistogram();
            h1 = getMaxHistogram();
            parameters[0] = (float)(255/(h1-h0));
            parameters[1] = (float)((-255*h0)/(h1-h0));
            this.dynamicExpansionParameters = parameters;
        }

        private int getMinHistogram() {
            int h0 = 0;
            for (int index=1; index<256; index++) {
                if (this.histogram[index]!=0) {
                    h0 = index;
                    break;
                }
            }
            return(h0);
        }

        private int getMaxHistogram() {
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