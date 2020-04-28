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
        public List<float> sumColoredElements;


        public ConvolutionWindow(int diameter) { //Constructor with default filter
            this.windowDiameter = diameter;
            this.processingFilter = "median";
            this.totalPixel = diameter*diameter;
            this.pixelsCounter =0;
            this.sumElements = new List<int>();
            this.sumColoredElements = new List<float>();
        }

        public ConvolutionWindow(int diameter, string filter) { //Constructor with custom filter
            this.windowDiameter = diameter;
            this.processingFilter = filter;
            this.totalPixel = diameter*diameter;
            this.pixelsCounter =0;
            this.sumElements = new List<int>();
            this.sumColoredElements = new List<float>();
        }

        public void ConvolutionWindowPixelsSum(FasterBitmap toCompute, int centerPositionX, int centerPositionY) {
            for (int yIndex = centerPositionY - (this.windowDiameter - 1); yIndex< centerPositionY + (this.windowDiameter -1); yIndex++) {
                for (int xIndex = centerPositionX - (this.windowDiameter - 1); xIndex<centerPositionX + (this.windowDiameter -1); xIndex++) {
                    if ((yIndex < 0)||(yIndex >= toCompute.Height)||(xIndex < 0)||(xIndex >= toCompute.Width)) continue;
                    else {
                        this.sumElements.Add(toCompute.GetPixel(xIndex, yIndex).B); 
                        this.pixelsCounter++;
                    }
                }
            }
            return;
        }

        public void ConvolutionWindowPixelsSumForColoredFilters(FasterBitmap toCompute, int centerPositionX, int centerPositionY) {
            for (int yIndex = centerPositionY - (this.windowDiameter - 1); yIndex< centerPositionY + (this.windowDiameter -1); yIndex++) {
                for (int xIndex = centerPositionX - (this.windowDiameter - 1); xIndex<centerPositionX + (this.windowDiameter -1); xIndex++) {
                    if ((yIndex < 0)||(yIndex >= toCompute.Height)||(xIndex < 0)||(xIndex >= toCompute.Width)) continue;
                    else {
                        this.sumColoredElements.Add(HSVColor.RGBtoHSV(toCompute.GetPixel(xIndex, yIndex)).value); 
                        this.pixelsCounter++;
                    }
                }
            }
            return;
        }

    }

    public abstract class CreateConvolutionWindow 
    {

        public static ConvolutionWindow DoCreate(string diameter, string filter) { //Create a Window with the selected diameter and filter
            int diameterTemp = 0;
            if ((!int.TryParse(diameter, out diameterTemp))||(diameterTemp%2==0)) {
                Console.WriteLine("Given diameter is invalid!");
                return null;
            }
            if ((filter != "median")&&(filter != "dilation")&&(filter != "erosion")&&(filter != "opening")&&(filter != "closing")&&(filter != "colored")&&(filter != "negative")) {
                filter = "median";
            }
            ConvolutionWindow window = new ConvolutionWindow(diameterTemp, filter);
            if (window.totalPixel == 0) {
                Console.WriteLine("The chosen convolution window does not contain any pixels!");
                return null;
            }
            return(window);
        }

        public static ConvolutionWindow DoCreate(string diameter) { //Create a Median Filter Window with the selected diameter
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