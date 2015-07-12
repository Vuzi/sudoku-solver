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

namespace SudokuSolver
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

            List<Sudoku> sudokuList = Sudoku.InitFromFile(openFileDialog.FileName);

            foreach (Sudoku sudoku in sudokuList)
            {
                sudoku.Solve();
                sudoku.DisplaySudoku();
            }
            
        }
    }
}
