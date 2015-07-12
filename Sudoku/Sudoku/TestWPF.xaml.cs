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
    public partial class TestWPF : Window
    {
        public TestWPF()
        {
            InitializeComponent();
            DataContext = App.ViewModelSudoku;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModelSudoku.AjouterGrille();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            App.ViewModelSudoku.SupprimerGrille();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FrontGrille.Children.Clear();
            FrontGrille.RowDefinitions.Clear();
            FrontGrille.ColumnDefinitions.Clear();

            Grille g = App.ViewModelSudoku.GrilleSelect;
            for (int i = 0; i < g.Taille; i++)
            {
                FrontGrille.RowDefinitions.Add(new RowDefinition());
                FrontGrille.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < g.Taille; i++)
            {
                for (int j = 0; j < g.Taille; j++)
                {
                    FrameworkElement elem = CreerCaseDeGrid(g, i, j);
                    FrontGrille.Children.Add(elem);
                }
            }
            
        }

        void tb_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static FrameworkElement CreerCaseDeGrid(Grille g, int i, int j)
        {
            FrameworkElement elementGraphique;
            char c = g.Tab[i, j].Valeur;
            if (c == '.')
            {
                Rectangle r = new Rectangle();
                r.Fill = new SolidColorBrush(Colors.White);
                elementGraphique = r;
            }
            else
            {
                Button b = new Button();
                b.Content = c;
                elementGraphique = b;
            }
            Grid.SetRow(elementGraphique, i);
            Grid.SetColumn(elementGraphique, j);
            return elementGraphique;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
