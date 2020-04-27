using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace imageComputing

/*TODO: 
Reconnaissance de forme:
    Snakes
    Quadtree
    Boite englobante (Feret)
    Perimètre (Feret)
    Périmètre (Crofton)
    Connexité
    Circularité radiale
    Convexité
    Diametres inscrits & conscrits
    Blaschke
Débugger le multiseuillage par Variance 
Faire une doc propre (Pour chaque fonction de la librairie)
Optimisation a fond (virer les listes et les try-catch, ...)
Suppresion de ce fichier, creation d'un imageComputing.dll selfcontained 
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
            Shape shape = new Shape(test.zonesMap, 1);
            shape.DrawBoundingBox(toCompute, 1);
                watch.Stop();
                Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms"); 
            Console.WriteLine(shape.geometricalCircularity);
        }

    }
}