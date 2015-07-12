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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SudokuSolver
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ComboBox_Difficulty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBoxItem)ComboBox_menu.SelectedValue).Content.ToString().Equals("Générer un Sudoku"))
            {
                MessageBox.Show(((ComboBoxItem)ComboBox_menu.SelectedValue).Content.ToString());
            }
            else if (((ComboBoxItem)ComboBox_menu.SelectedValue).Content.ToString().Equals("Résoudre un Sudoku"))
            {
                ResolutionWindow rw = new ResolutionWindow();
                rw.Show();
                this.Hide();
            }
            else if (((ComboBoxItem)ComboBox_menu.SelectedValue).Content.ToString().Equals("Vérifier un Sudoku"))
            {
                MessageBox.Show(((ComboBoxItem)ComboBox_menu.SelectedValue).Content.ToString());
            }
        }
    }
}
