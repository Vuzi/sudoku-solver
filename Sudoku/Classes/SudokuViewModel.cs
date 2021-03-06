﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SudokuSolver
{

    public class SudokuGridViewModel : INotifyPropertyChanged {
        public Sudoku sudoku { get; private set; }
        public bool Valid { get { return sudoku.valid; } }
        public String Name { get { return sudoku.name; } }
        public DateTime Date { get { return sudoku.date; } }
        public int Size { get { return sudoku.size;  } }
        public String Dictionary { get { return sudoku.dictionnary; } }
        public String SizeFormated { get { return sudoku.size + "x" + sudoku.size; } }
        public event PropertyChangedEventHandler PropertyChanged;
        private bool[,] editableValues;

        public SudokuGridViewModel(Sudoku _sudoku) {
            sudoku = _sudoku;

            editableValues = new bool[sudoku.size, sudoku.size];

            for (int i = 0; i < sudoku.size; i++) {
                for (int j = 0; j < sudoku.size; j++) {

                    if (sudoku.sudoku[i, j] == '.') {
                        editableValues[i, j] = true;
                    } else {
                        editableValues[i, j] = false;
                    }
                }
            }
        }

        public bool IsEditableAt(int x, int y) {
            if (Valid)
                return false;
            return editableValues[x, y];
        }

        public char GetValueAt(int x, int y) {
            return sudoku.sudoku[x, y];
        }

        public uint[,] GetValuesBin() {
            return sudoku.sudokuValues;
        }

        public void SetValueAt(int x, int y, char val) {
            sudoku.SetValueAt(x, y, val);
            Validate();
        }

        private void OnPropertyChanged(string prop) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
           }
        }

        public void Solve() {
            sudoku.Solve();
        }

        public void Validate() {
            sudoku.Validate();
            OnPropertyChanged("Valid");
        }

        internal void Reset() {
            for (int i = 0; i < sudoku.size; i++) {
                for (int j = 0; j < sudoku.size; j++) {
                    if (editableValues[i, j])
                        sudoku.SetValueAt(i, j, '.');
                }
            }


        }

        internal void SetValuesBin(uint[,] oldValues) {
            sudoku.sudokuValues = oldValues;
            sudoku.UpdateSudoku();
        }
    }

    public class SudokuViewModel
    {
        public string ApplicationName { get; set; }

        public ObservableCollection<SudokuGridViewModel> SudokuList { get; set; }
        public SudokuGridViewModel SelectedSudoku { get; set; }

        public SudokuViewModel()
        {
            ApplicationName = "SudokuSolver";
            SudokuList = new ObservableCollection<SudokuGridViewModel>();
        }
        
        public void AddGrid(int size, int difficulty)
        {
            String dictionary = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Sudoku generatedSudoku = new Sudoku("Sudoku " + (SudokuList.Count + 1), DateTime.Now, dictionary.Substring(0, size), difficulty);
            generatedSudoku.Validate();
            SudokuList.Add(new SudokuGridViewModel(generatedSudoku));
        }

        public void DeleteGrid()
        {
            if (SelectedSudoku == null)
                System.Windows.MessageBox.Show("Aucune grille selectionnée", "Information");
            else
                SudokuList.Remove(SelectedSudoku);
        }

        public void ChargerFichier(String file)
        {
            if (file == null || file == "") return;

            try
            {
                List<Sudoku> sudokuList = Sudoku.InitFromFile(file);

                foreach (Sudoku sudoku in sudokuList)
                {
                    sudoku.Validate();
                    SudokuList.Add(new SudokuGridViewModel(sudoku));
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Le fichier est invalide", "Erreur au chargement du fichier");
            }
        }

        public void ResolveGrid()
        {
            if (SelectedSudoku == null)
                System.Windows.MessageBox.Show("Aucune grille selectionnée", "Information");
            else {
                SelectedSudoku.Solve();
                SelectedSudoku.Validate();
            }
        }

        internal void ValidateGrid() {
            if (SelectedSudoku == null)
                System.Windows.MessageBox.Show("Aucune grille selectionnée", "Information");
            else {
                SelectedSudoku.Validate();
            }
        }

        internal void ExportAll(string file) {
            if (file == null || file == "") return;

            try {
                List<Sudoku> sudokuList = new List<Sudoku>();

                foreach (SudokuGridViewModel sudoku in SudokuList) {
                    sudokuList.Add(sudoku.sudoku);
                }

                Sudoku.WriteToFile(file, sudokuList);
                System.Windows.MessageBox.Show("Sudokus exportés avec succès dans le fichier '" + file + "'", "Export terminé");

            } catch (Exception) {
                System.Windows.MessageBox.Show("Le fichier est invalide", "Erreur au chargement du fichier");
            }
        }
    }
}
