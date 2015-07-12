using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SudokuSolver
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static SudokuViewModel ViewModelSudoku { get; set; }

        static App()
        {
            ViewModelSudoku = new SudokuViewModel();
        }
    }
}
