using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace imageComputing

/*TODO: 
Reconnaissance de forme:
    Toutes les fonctions Slides partie 5
    Toutes les fonctions Slides partie 6
    Pour quadtree, croissance de formes et les graines: interface graphique rapide pour choisir ou mettre les graines initiales
Fonctions de machine learning
    Réseau de Neurones
    (...)
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
            SimpleFilters.doFilters(toCompute, "3", "colored");
        }

    }
}