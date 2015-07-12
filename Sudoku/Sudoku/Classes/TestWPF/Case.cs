using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    public class Case
    {
        public char Valeur { get; set; }
        public string Symboles { get; set; }

        public Case(char val, string Symboles)
        {
            // TODO: Complete member initialization
            this.Valeur = val;
            this.Symboles = Symboles;
        }

        public object Hypoteses { get; set; }
    }
}
