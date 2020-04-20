using System;
using System.Collections.Generic;

namespace imageComputing
{
    public class ConvolutionWindow
    {
        public int windowDiameter { get; protected set; }
        public int totalPixel { get; private set; }
        public string processingFilter { get; private set; }
        public int pixelsCounter { get; set; }
        public List<int> sumElements;


        public ConvolutionWindow(int diameter) { //Constructor with default filter
            this.windowDiameter = diameter;
            this.processingFilter = "median";
            this.totalPixel = diameter*diameter;
            this.pixelsCounter =0;
            this.sumElements = new List<int>();
        }

        public ConvolutionWindow(int diameter, string filter) { //Constructor with custom filter
            this.windowDiameter = diameter;
            this.processingFilter = filter;
            this.totalPixel = diameter*diameter;
            this.pixelsCounter =0;
            this.sumElements = new List<int>();
        }

        public void ConvolutionWindowPixelsSum(FasterBitmap toCompute, int centerPositionX, int centerPositionY) {
            for (int Yindex = centerPositionY - (this.windowDiameter - 1); Yindex< centerPositionY + (this.windowDiameter -1); Yindex++) {
                for (int Xindex = centerPositionX - (this.windowDiameter - 1); Xindex<centerPositionX + (this.windowDiameter -1); Xindex++) {
                    try {
                        this.sumElements.Add(toCompute.GetPixel(Xindex, Yindex).B); 
                        this.pixelsCounter++;
                    }
                    catch (ArgumentOutOfRangeException) {
                        continue;
                    }
                }
            }
            return;
        }

    }

    public abstract class CreateConvolutionWindow 
    {

        public static ConvolutionWindow doCreate(string diameter, string filter) { //Create a Window with the selected diameter and filter
            int diameterTemp = 0;
            if ((!int.TryParse(diameter, out diameterTemp))||(diameterTemp%2==0)) {
                Console.WriteLine("Given diameter is invalid!");
                return null;
            }
            if ((filter != "median")&&(filter != "dilation")&&(filter != "erosion")&&(filter != "opening")&&(filter != "closing")) {
                filter = "median";
            }
            ConvolutionWindow window = new ConvolutionWindow(diameterTemp, filter);
            if (window.totalPixel == 0) {
                Console.WriteLine("The chosen convolution window does not contain any pixels!");
                return null;
            }
            return(window);
        }

        public static ConvolutionWindow doCreate(string diameter) { //Create a Median Filter Window with the selected diameter
            int diameterTemp = 0;
            if ((!int.TryParse(diameter, out diameterTemp))||(diameterTemp%2==0)) {
                Console.WriteLine("Given diameter is invalid!");
                return null;
            }
            ConvolutionWindow window = new ConvolutionWindow(diameterTemp);
            if (window.totalPixel == 0) {
                Console.WriteLine("The chosen convolution window does not contain any pixels!");
                return null;
            }
            return(window);
        }
    }
}