using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace imageComputing

/*TODO: 
Histogram: Toutes les fonctions Slides Partie 3:
    Seuillage automatique Multi-niveau: Variance, Entropie, Manuel
Gestion des images en couleurs: Pas en RGB du coup mais en HSV; faire pour SimpleFilters.cs (filtre médian uniquement) et Histogram
Reconnaissance de forme:
    Toutes les fonctions Slides partie 5
    Toutes les fonctions Slides partie 6
    Pour quadtree, croissance de formes et les graines: interface graphique rapide pour choisir ou mettre les graines initiales?
Créer les dll
Faire une doc propre (Pour chaque fonction de la librairie)
Optimization:
    Utiliser la librairie Cudafy.NET pour passer de la charge au GPU ?
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
            AdvancedFilters.doFilters(toCompute, "robert", "L2");
        }

    }
}