using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    public class Grille
    {
        public string Nom { get; set; }
        public string Date { get; set; }
        public string Symboles { get; set; }
        public int Taille { get { return Symboles.Length;} }

        public Case[,] Tab { get; set; }

        public void InitTabCases()
        {
            Tab = new Case[Taille,Taille];
            Random rand = new Random();
            int x = rand.Next();
            for (int i = 0; i < Taille; i++)
            {
                for (int j = 0; j < Taille; j++)
                {
                    x = rand.Next(0, Taille + 1);
                    if (x == Taille)
                        Tab[i, j] = new Case('.', Symboles);
                    else
                        Tab[i, j] = new Case(Symboles[x], Symboles);
                }
            }

        }
    }
}
