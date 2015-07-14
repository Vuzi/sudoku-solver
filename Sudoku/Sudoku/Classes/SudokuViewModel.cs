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
            ApplicationName = "Sudoku Application";
            SudokuList = new ObservableCollection<Sudoku>();
        }

        public void AddGrid(int size, int difficulty)
        {
            String dictionary = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Sudoku generatedSudoku = new Sudoku("Generated sudoku " + (SudokuList.Count + 1), DateTime.Now, dictionary.Substring(0, size), difficulty);
            generatedSudoku.Validate();
            SudokuList.Add(generatedSudoku);
        }

        public void DeleteGrid()
        {
            if (SelectedSudoku == null)
                System.Windows.MessageBox.Show("No item selected", "Info");
            else
                SudokuList.Remove(SelectedSudoku);
        }

        public void ChargerFichier(String file)
        {
            if (file == null) return;

            try
            {
                List<Sudoku> sudokuList = Sudoku.InitFromFile(file);
                SudokuList.Clear();

                foreach (Sudoku sudoku in sudokuList)
                {
                    sudoku.Validate();
                    SudokuList.Add(sudoku);
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("The file is not valid", "Error");
            }
        }

        public void ResolveGrid()
        {
            if (SelectedSudoku == null)
                System.Windows.MessageBox.Show("No item selected", "Info");
            else
            {
                SelectedSudoku.Solve();
                SelectedSudoku.Validate();
            }
        }
    }
}
