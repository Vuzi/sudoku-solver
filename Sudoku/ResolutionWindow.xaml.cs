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
    public partial class ResolutionWindow : Window
    {
        public ResolutionWindow()
        {
            InitializeComponent();
            DataContext = App.ViewModelSudoku;
        }

        private void Button_Add_Grid(object sender, RoutedEventArgs e)
        {
            SudokuGenerationFormWindow generationWindow = new SudokuGenerationFormWindow();
            generationWindow.Show();
        }

        private void Button_Remove_Grid(object sender, RoutedEventArgs e)
        {
            App.ViewModelSudoku.DeleteGrid();
        }

        private void Button_Load_File(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog() { Filter = "Sudoku Files (*.sud, *.sudoku)|*.sud;*.sudoku|Text Files (*.txt)|*.txt|All Files (*.*)|*.*" };
            var result = openFileDialog.ShowDialog();

            App.ViewModelSudoku.ChargerFichier(openFileDialog.FileName);

        }

        private void Button_Resolve_Grid(object sender, RoutedEventArgs e)
        {
            App.ViewModelSudoku.ResolveGrid();
            Reload_Selection();
        }

        private void ListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Reload_Selection();
        }

        void Reload_Selection()
        {
            FrontGrid.Children.Clear();
            FrontGrid.RowDefinitions.Clear();
            FrontGrid.ColumnDefinitions.Clear();

            Sudoku sudoku = App.ViewModelSudoku.SelectedSudoku;

            if (sudoku == null)
            {
                buttonRemoveGrid.Visibility = Visibility.Hidden;
                buttonResolveGrid.Visibility = Visibility.Hidden;
                return;
            }

            sudoku.Validate();
            for (int i = 0; i < sudoku.size; i++)
            {
                FrontGrid.RowDefinitions.Add(new RowDefinition());
                FrontGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < sudoku.size; i++)
            {
                for (int j = 0; j < sudoku.size; j++)
                {
                    FrameworkElement elem = CreateGridCases(sudoku, i, j);
                    FrontGrid.Children.Add(elem);
                }
            }

            buttonRemoveGrid.Visibility = Visibility.Visible;
            buttonResolveGrid.Visibility = Visibility.Visible;
        }

        private static FrameworkElement CreateGridCases(Sudoku sudoku, int x, int y)
        {
            FrameworkElement elementGraphique;
            char c = sudoku.sudoku[x, y];
            if (c == '.')
            {
                Rectangle rectangle = new Rectangle();
                rectangle.Fill = new SolidColorBrush(Color.FromRgb(109, 149, 202));
                elementGraphique = rectangle;
            }
            else
            {
                Button button = new Button();
                button.Content = c;
                elementGraphique = button;
            }
            Grid.SetRow(elementGraphique, x);
            Grid.SetColumn(elementGraphique, y);
            return elementGraphique;
        }

    }
}
