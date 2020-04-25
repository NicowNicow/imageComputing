using System;
using System.Collections.Generic;

namespace imageComputing
{
    public class AdvancedConvolutionWindow : ConvolutionWindow
    {
        public int[][] valueMatrix {get; private set;}
        public int[][] horizontalValueMatrix {get; private set;}
        public int[][] verticalValueMatrix {get; private set;}
        public int gaussianWeightedSum {get; private set;}
        public string norm {get; private set;}

        public AdvancedConvolutionWindow(int diameter): base(diameter, "gaussian") { //Constructor
            SetValueMatrix(diameter);
        }

        public AdvancedConvolutionWindow(string filter, string norm): base(3, filter) {
            SetValueMatrix(3);
            if (this.processingFilter == "robert") {
                this.windowDiameter = 2;
            }
            this.norm = norm;
        }

        public int DoNorm(int[] values) {
            int result = 0;
            if (this.norm == "L1") {
                result = (int) (values[0] + values[1]);
            }
            if (this.norm == "L2") {
                result = (int)Math.Sqrt(Math.Pow(values[0], 2) + Math.Pow(values[1], 2));
            }
            if (this.norm == "infinite") {
                result = (int)(Math.Max(values[0], values[1]));
            }
            return(result);
        }

        private void SetValueMatrix(int diameter) { //Set the Value Matrix depending on the filter and the diameter
            switch(this.processingFilter) {
                case "gaussian":
                    SetGaussian(diameter);
                    break;
                case "prewitt":
                    SetSobelPrewitt(1);
                    break;
                case "robert":
                    SetRoberts();
                    break;
                case "sobel":
                    SetSobelPrewitt(2);
                    break;
                case "laplacian4":
                    SetLaplacian(4);
                    break;
                case "laplacian8":
                    SetLaplacian(8);
                    break;
            }
        }

        private void SetGaussian(int diameter) {
            int weightedSum = 0;
            int[][] values = new int[diameter][];
            for (int index=0; index< diameter; index++) {
                values[index] = new int[diameter];
            }
            int yIndexFromCenter = (int)(-diameter/2);
            for (int yIndex = 0; yIndex < diameter; yIndex++) {
                int xIndexFromCenter = (int) (-diameter/2);
                for (int xIndex = 0; xIndex < diameter; xIndex++) {
                    double firstTerm =  Math.Pow( ((Math.Pow( diameter, 2) - 1)/4), 2);
                    double secondTerm =  Math.Pow( (double)((double)(diameter-1)/(double)(diameter+1)), (Math.Pow(xIndexFromCenter, 2) + Math.Pow(yIndexFromCenter, 2)));
                    values[yIndex][xIndex] = (int)(firstTerm*secondTerm);
                    xIndexFromCenter++;
                    weightedSum = weightedSum + values[yIndex][xIndex];
                }
                yIndexFromCenter++;
            }
            this.gaussianWeightedSum = weightedSum;
            this.valueMatrix = values;
        }

        private void SetSobelPrewitt(int selector) {
            this.horizontalValueMatrix = new int[3][];
            this.horizontalValueMatrix[0] = new int[3]{-1, 0, 1};
            this.horizontalValueMatrix[1] = new int[3]{-selector, 0, selector};
            this.horizontalValueMatrix[2] = new int[3]{-1, 0, 1};
            this.verticalValueMatrix = new int[3][];
            this.verticalValueMatrix[0] = new int[3]{-1, -selector, -1};
            this.verticalValueMatrix[1] = new int[3]{0, 0, 0};
            this.verticalValueMatrix[2] = new int[3]{1, selector, 1};
        }

        private void SetRoberts() {
            this.horizontalValueMatrix = new int[2][];
            this.horizontalValueMatrix[0] = new int[2]{1, 0};
            this.horizontalValueMatrix[1] = new int[2]{0, -1};
            this.verticalValueMatrix = new int[2][];
            this.verticalValueMatrix[0] = new int[2]{0,1};
            this.verticalValueMatrix[1] = new int[2]{-1,0};
        }

        private void SetLaplacian(int selector) {
            if (selector == 8) {
                this.valueMatrix = new int[3][];
                this.valueMatrix[0] = new int[3]{-1, -1, -1};
                this.valueMatrix[1] = new int[3]{-1, selector, -1};
                this.valueMatrix[2] = new int[3]{-1, -1,-1};
            }
            else if (selector == 4) {
                this.valueMatrix = new int[3][];
                this.valueMatrix[0] = new int[3]{0, -1, 0};
                this.valueMatrix[1] = new int[3]{-1, selector, -1};
                this.valueMatrix[2] = new int[3]{0, -1, 0};
            }
        }

    } 

    public abstract class CreateAdvancedConvolutionWindow 
    {

        public static AdvancedConvolutionWindow DoCreateDiameter(string diameter) { //Create a Window for gaussian filter with the given diameter
            int diameterTemp = 0;
            if ((!int.TryParse(diameter, out diameterTemp))||(diameterTemp%2==0)) {
                Console.WriteLine("Given diameter is invalid!");
                return null;
            }
            AdvancedConvolutionWindow window = new AdvancedConvolutionWindow(diameterTemp);
            if (window.totalPixel == 0) {
                Console.WriteLine("The chosen convolution window does not contain any pixels!");
                return null;
            }
            return(window);
        }

        public static AdvancedConvolutionWindow DoCreateFilter(string filter) { //Create a 3x3  window for the given filter
            if ((filter != "gaussian")&&(filter != "prewitt")&&(filter != "robert")&&(filter != "sobel")&&(filter != "laplacian4")&&(filter != "laplacian8")) {
                Console.WriteLine("Unknown filter!");
               return null;
            }
            AdvancedConvolutionWindow window = new AdvancedConvolutionWindow(filter, "L2");
            return(window);
        }

        public static AdvancedConvolutionWindow DoCreateFilter(string filter, string norm) { //Create a 3x3  window for the given filter
            if ((filter != "gaussian")&&(filter != "prewitt")&&(filter != "robert")&&(filter != "sobel")&&(filter != "laplacian4")&&(filter != "laplacian8")) {
                Console.WriteLine("Unknown filter!");
               return null;
            }
            if ((norm != "L1")&&(norm!="L2")&&(norm!="infinite")) {
                norm = "L2";
            }
            AdvancedConvolutionWindow window = new AdvancedConvolutionWindow(filter, norm);
            return(window);
        }
    }

}