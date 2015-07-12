using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sudoku
{
    /// <summary>
    /// Logique d'interaction pour ResolutionWindow.xaml
    /// </summary>
    public partial class ResolutionWindow : Window
    {
        public ResolutionWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Sélection d'un fichier de Sudoku
            var openFileDialog = new Microsoft.Win32.OpenFileDialog() { Filter = "Sudoku Files (*.sud, *.sudoku)|*.sud;*.sudoku|Text Files (*.txt)|*.txt|All Files (*.*)|*.*" };
            var result = openFileDialog.ShowDialog();

            if (result != false)
            {
                // TODO Appel de la méthode de parsing du fichier
                // TODO Remplissage de la DataGrid

                Grid[] grids = new SudokuSpliter().split(openFileDialog.FileName);

                foreach (Grid g in grids)
                {
                    //g.printGrid();

                    //if (g.IsValid)
                    //{
                    //    Console.WriteLine("Grid " + g.Name + " is valid");
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Grid " + g.Name + " isn't valid");
                    //}
                    Console.WriteLine();
                    //Console.WriteLine("before");
                    //g.printGrid();
                    g.resolve();
                    Console.WriteLine();
                    //Console.WriteLine("after");
                    g.printGrid();
                    Console.WriteLine();
                    //Console.WriteLine( g.validate() );
                    Console.WriteLine();
                }
            }
        }
    }
}
