using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace imageComputing

/*args :  dotnet run $image_path $convolution_window_diameter $simple_filter_type*/

/* $simple_filter_type availables : median, dilation, erosion, opening, closing */

/*TODO: 
Histogram: Toutes les fonctions Slides Partie 3:
    Seuillage automatique
    Maximisation d’entropie
    Maximisation de la variance interclasse
    Look-up table
    Egalisation d’histogramme
Filtres:
    Implémente un héritage de Convolution Windows qui crée une liste avec les valeurs requises pour le gradient et/ou Gauss
    Filtre Gaussien, Prewitt, Robert, Sobel, Laplaciens 4 et 8 (Slides Partie 4)
    Toutes les fonctions Slides partie 5
    Toutes les fonctions Slides partie 6
    Gestion des images en couleurs: Pas en RGB du coup mais en HSV; faire pour SimpleFilters.cs (filtre médian uniquement) et Histogram (egalisation, seuillage)
    Pour les gradients: Norme L1: Addition, Norme L2: Norme Euclidienne, Norme Infinie: Maximum des deux valeurs
Créer les dll
Optimization:
    Utiliser la librairie Cudafy.NET pour passer de la charge au GPU
    Utiliser des threads peut être aussi
*/

{
    class Program
    {

        static void Main(string[] args) {
            ImageData toCompute;
            string convolutionWindowDiameter = args[1];
            string convolutionWindowFilter = args[2];
            try {
                 toCompute = new ImageData(args[0]);
            }
            catch (IndexOutOfRangeException) {
                 Console.WriteLine("Please give the path to a bitmap image!");
                 return;
            }
            toCompute.getHistogramFromImage();
            Threshold.simpleEntropyThreshold(toCompute);
        }

    }
}