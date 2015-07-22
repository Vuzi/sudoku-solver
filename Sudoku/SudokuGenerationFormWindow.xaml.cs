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

namespace SudokuSolver {
    
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class SudokuGenerationFormWindow : Window {
        public SudokuGenerationFormWindow() {
            InitializeComponent();

            // Init size values
            SudokuSize.DisplayMemberPath = "Text";
            SudokuSize.Items.Add(new { Text = "9x9", Value = 9 });
            SudokuSize.Items.Add(new { Text = "16x16", Value = 16 });
            SudokuSize.Items.Add(new { Text = "25x25", Value = 25 });
            SudokuSize.SelectedValue = SudokuSize.Items[0];

            // Init difficulties
            SudokuDifficulty.DisplayMemberPath = "Text";
            SudokuDifficulty.Items.Add(new { Text = "1 - Très facile",    Value = 10 });
            SudokuDifficulty.Items.Add(new { Text = "2 - Facile",         Value = 20 });
            SudokuDifficulty.Items.Add(new { Text = "3 - Moyen",          Value = 35 });
            SudokuDifficulty.Items.Add(new { Text = "4 - Difficile",      Value = 60 });
            SudokuDifficulty.Items.Add(new { Text = "5 - Très difficile", Value = 80 });
            SudokuDifficulty.SelectedValue = SudokuDifficulty.Items[0];
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            
            // Get size
            dynamic size = SudokuSize.SelectedValue;
            int sizeValue = size.Value;

            // Get difficulty
            dynamic diff = SudokuDifficulty.SelectedValue;
            int diffValue = diff.Value;

            App.ViewModelSudoku.AddGrid(sizeValue, diffValue);
            this.Close();
        }

    }
}
