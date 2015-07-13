using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SudokuSolver
{
    public class SudokuViewModel
    {
        public string ApplicationName { get; set; }

        public ObservableCollection<Sudoku> SudokuList { get; set; }
        public Sudoku SelectedSudoku { get; set; }

        public SudokuViewModel()
        {
            ApplicationName = "Application de Sudokus";
            SudokuList = new ObservableCollection<Sudoku>();
        }

        public void AddGrid()
        {
            Sudoku generatedSudoku = new Sudoku("Generated sudoku " + SudokuList.Count + 1, DateTime.Now, "123456789");
            SudokuList.Add(generatedSudoku);
        }

        public void DeleteGrid()
        {
            if (SelectedSudoku == null)
                System.Windows.MessageBox.Show("Aucun item sélectionné", "Info");

            SudokuList.Remove(SelectedSudoku);
        }

        public void ChargerFichier(String file)
        {
            List<Sudoku> sudokuList = Sudoku.InitFromFile(file);
            SudokuList.Clear();

            foreach (Sudoku sudoku in sudokuList)
            {
                SudokuList.Add(sudoku);
                //sudoku.Solve();
                //sudoku.DisplaySudoku();
            }
        }
    }
}
