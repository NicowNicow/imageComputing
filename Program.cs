using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace imageComputing

/*TODO: 
Reconnaissance de forme:
    Watershed
    Snakes
    Quadtree
    Final Erosion
    Toutes les fonctions Slides partie 7
Débugger le seuillage par Variance Multiniveaux (Possiblement juste selectionner les seuils correspondants au k plus hautes variances, mais c'est pas fou)
Créer les dll
Faire une doc propre (Pour chaque fonction de la librairie)
*/

{
    class Program
    {

        static void Main(string[] args) {
            ImageData toCompute;
            try {
                toCompute = new ImageData(args[0]);
            }
            catch (IndexOutOfRangeException) {
                Console.WriteLine("Please give the path to a bitmap image!");
                return;
            }
            toCompute.GetHistogramFromColoredImage();
            RegionGrowing test = new RegionGrowing();
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();
            test.DoRegionGrowing(toCompute);
                watch.Stop();
                Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms"); 
                watch.Reset();
                watch.Start();
            Segmentation.DoSegmentation(toCompute, test.zonesMap, "dilation", true);
                watch.Stop();
                Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms"); 
        }

    }
}