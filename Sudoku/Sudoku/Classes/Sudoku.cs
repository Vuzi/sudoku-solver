using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.CompilerServices; 

namespace SudokuSolver {

    enum SudokuValidationError {
        LINE, SQUARE, COLUMN
    };

    class SudokuValidation {

        public SudokuValidationError error { get; protected set; }
        public int nb { get; protected set; }
        public String message {
            get {
                switch (error) {
                    case SudokuValidationError.LINE:
                        return String.Format("Line {0} not valid", nb);
                    case SudokuValidationError.COLUMN:
                        return String.Format("Column {0} not valid", nb);
                    case SudokuValidationError.SQUARE:
                        return String.Format("Square {0} not valid", nb);
                    default:
                        return "Sudoku not valid";
                }
            }
        }

        public SudokuValidation(SudokuValidationError error, int nb) {
            this.error = error;
            this.nb = nb;
        }
    }

    /// <summary>
    /// Sudoku class
    /// </summary>
    class Sudoku {

        public String name { get; protected set; }
        public DateTime date { get; protected set; }
        public String dictionnary { get; protected set; }
        public int size { get; protected set; }
        public int squareSize { get; protected set; }
        public char[,] sudoku { get; protected set; }
        private uint[,] sudokuValues;
        private Dictionary<uint, char> correspondances;
        private char jocker;
        private uint controlSum;
        private uint maxValue;

        /// <summary>
        /// Sudoku constructor 
        /// </summary>
        /// <param name="name">The name of the sudoku</param>
        /// <param name="date">The date of the sudoku</param>
        /// <param name="dictionnary">The dictionnary (all the possible values)</param>
        /// <param name="sudoku">The sudoku itself</param>
        public Sudoku(string name, DateTime date, String dictionnary, char[,] sudoku, char jocker='.') {
            this.name = name;
            this.date = date;
            this.dictionnary = dictionnary;
            this.sudoku = sudoku;

            if (name == null || date == null || dictionnary == null || sudoku == null)
                throw new ArgumentNullException();

            // The sudoku size must be a perfect square
            if (Math.Sqrt(dictionnary.Length) % 1 != 0 || dictionnary.Length != sudoku.GetLength(0))
                throw new Exception(String.Format("Error : invalid sudoku size {0}", dictionnary.Length));
            
            this.size = dictionnary.Length;
            this.squareSize = (int) Math.Sqrt(dictionnary.Length);
            this.controlSum = (uint) ~(0xFFFFFFFF << this.size);
            this.maxValue = (uint)Math.Pow(2, this.size);

            // Init corresponding values
            uint value = 0x1;
            int size = dictionnary.Length;

            this.jocker = jocker;
            this.correspondances = new Dictionary<uint,char>();
            for (int i = 0; i < size; i++)
                correspondances.Add(value << i, dictionnary[i]);

            // Init sudoku values + check values
            this.sudokuValues = new uint[dictionnary.Length, dictionnary.Length];

            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    if (sudoku[i, j] == jocker) {
                        sudokuValues[i, j] = 0x0;
                        continue;
                    }

                    int index = dictionnary.IndexOf(sudoku[i, j]);

                    if (index < 0)
                        throw new Exception("Error : invalid value at " + i + ":" + j + " in the sudoku");

                    sudokuValues[i, j] = (value << index);
                }
            }
        }

        private void UpdateSudoku() {
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    sudoku[i, j] = correspondances[sudokuValues[i, j]];
                }
            }
        }

        /// <summary>
        /// Valite the current sudoku
        /// </summary>
        /// <returns>True if the sudoku is valid, false otherwise</returns>
        public SudokuValidation Validate() {
            for (int i = 0; i < size; i++) {
                uint line = 0;
                uint col = 0;
                uint square = 0;

                for (int j = 0; j < size; j++) {
                    line ^= sudokuValues[i, j];
                    col ^= sudokuValues[j, i];
                    square ^= sudokuValues[(i / squareSize) * squareSize + j / squareSize, i * squareSize % size + j % squareSize];
                }

                if (line != controlSum)
                    return new SudokuValidation(SudokuValidationError.LINE, i);
                else if (col != controlSum)
                    return new SudokuValidation(SudokuValidationError.COLUMN, i);
                else if (square != controlSum)
                    return new SudokuValidation(SudokuValidationError.SQUARE, i);
            }

            return null;
        }

        /// <summary>
        /// Return the possible values at the specified position
        /// </summary>
        /// <param name="x">The x position</param>
        /// <param name="y">The y position</param>
        /// <returns>An array containing the possible values</returns>
        public char[] GetPossibleValuesAt(int x, int y) {

            if (sudokuValues[x, y] != 0)
                return new char[0];

            List<char> possible = new List<char>();
            uint value = GetPossibleValuesAtInternal(x, y);
            
            foreach(KeyValuePair<uint, char> entry in correspondances) {
                if((value & entry.Key) == entry.Key) // If the flag is up, add the entry
                    possible.Add(entry.Value);
            }

            return possible.ToArray();
        }

        /// <summary>
        /// Internal value for evaluating the possible values at a given position
        /// </summary>
        /// <param name="x">The x position</param>
        /// <param name="y">The y position</param>
        /// <returns>An unsigned integer flagged with each value possible. This should be used along with
        /// the correspondance dictionnary</returns>
        private uint GetPossibleValuesAtInternal(int x, int y) {
            uint value = 0x0;
            int square = ((x / squareSize) * squareSize) + (y / squareSize);
                        
            for (int i = 0; i < size; i++) {
                value |= sudokuValues[i, y];
                value |= sudokuValues[x, i];
                value |= sudokuValues[(square / squareSize) * squareSize + i / squareSize, square * squareSize % size + i % squareSize];
            }

            return value ^ controlSum;
        }

        /// <summary>
        /// Solve the sudoku. This methods will try to solve the sudoku using trivial
        /// values, and switch to hypothesis tests when no more solutions could be found.
        /// If still no solution are found, the sudoku is likely to be invalid.
        /// </summary>
        /// <returns>True if the sudoku can be (and is) solved, false otherwise</returns>
        public bool Solve() {
            // Init possible values for lines, columns and squares
            this.lines = new uint[size];
            this.cols = new uint[size];
            this.squares = new uint[size];

            for (int i = 0; i < size; i++) {
                lines[i] = 0x0;
                cols[i] = 0x0;
                squares[i] = 0x0;

                for (int j = 0; j < size; j++) {
                    lines[i] |= sudokuValues[i, j];
                    cols[i] |= sudokuValues[j, i];
                    squares[i] |= sudokuValues[(i / squareSize) * squareSize + j / squareSize, i * squareSize % size + j % squareSize];
                }

                lines[i] ^= controlSum;
                cols[i] ^= controlSum;
                squares[i] ^= controlSum;
            }

            // Try the speed method
            if (SolveSpeed()) {
                UpdateSudoku();
                return true;
            }

            // Try with hypothesis
            if (SolveInternal(0, 0)) {
                UpdateSudoku();
                return true;
            } else
                return false;
        }

        /// <summary>
        /// Test trivial resolutions for the sudoku
        /// </summary>
        /// <returns>True if the sudoku is solved, false otherwise</returns>
        private bool SolveSpeed() {
            uint lastUnsolved = 0;
            uint unsolved = uint.MaxValue;

            // Check until we can't solve any more values
            do {
                lastUnsolved = unsolved;
                unsolved = 0;

                // For each empty value
                for (int x = 0; x < size; x++) {
                    for (int y = 0; y < size; y++) {
                        if (sudokuValues[x, y] == 0) {
                            uint value = lines[x] & cols[y] & squares[((x / squareSize) * squareSize) + (y / squareSize)];

                            // Solved
                            if ((value & (value - 1)) == 0) {
                                // Update value
                                sudokuValues[x, y] = value;

                                // Update possible values
                                lines[x] ^= value;
                                cols[y] ^= value;
                                squares[((x / squareSize) * squareSize) + (y / squareSize)] ^= value;
                            } else
                                unsolved++;
                        }
                    }
                }
            } while (unsolved > 0 && unsolved < lastUnsolved);

            return (unsolved == 0);
        }

        /// <summary>
        /// Solve recursively the sudoku
        /// </summary>
        /// <param name="x">The x position to solve from</param>
        /// <param name="y">The y position to solve from</param>
        /// <returns>True if the sudoku can be solved from the specified position, false otherwise</returns>
        private bool SolveInternal(int x, int y) {
            
            if (x >= size) {
                x = 0;
                y++;

                if (y >= size)
                    return true; // Sudoku completed
            }

            if (sudokuValues[x, y] != 0x0) // Value already present
                return SolveInternal(x + 1, y);

            int squarePos = ((x / squareSize) * squareSize) + (y / squareSize);
            uint value = lines[x] & cols[y] & squares[squarePos];

            // Test if no possible value
            if (value == 0x0)
                return false;

            // Test recursively with each possible value
            for (uint i = 0x1; i <= maxValue; i <<= 1) {
                if ((value & i) == i) {
                    // Update value
                    sudokuValues[x, y] = i;

                    // Update possible values
                    lines[x] ^= i;
                    cols[y] ^= i;
                    squares[squarePos] ^= i;

                    if (SolveInternal(x + 1, y)) {
                        return true; // Solution working, quit
                    } else {
                        // Revert
                        lines[x] |= i;
                        cols[y] |= i;
                        squares[squarePos] |= i;
                    }
                }
            }

            // No result found, revert
            sudokuValues[x, y] = 0x0;
            return false;
        }

        /// <summary>
        /// Display the sudoku in the console
        /// </summary>
        public void DisplaySudoku() {
            Console.WriteLine("Name : " + name);
            Console.WriteLine("Date : " + date);
            Console.WriteLine("Dictionnary : " + dictionnary);

            Console.WriteLine("Sudoku : ");

            for (int i = 0; i < dictionnary.Length; i++) {
                Console.Write("|");
                for (int j = 0; j < dictionnary.Length; j++) {
                    Console.Write(sudoku[i, j] + "|");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Display the sudoku in the console
        /// </summary>
        public void DebugDisplaySudoku() {
            DisplaySudoku();
            Console.WriteLine("Sudoku values : ");

            for (int i = 0; i < dictionnary.Length; i++) {
                Console.Write("|");
                for (int j = 0; j < dictionnary.Length; j++) {
                    Console.Write(String.Format(" {0,4} |", sudokuValues[i, j]));
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public override string ToString() {
            return "Sudoku{ name:" + name + ", date:" + date + ", dictionnary" + dictionnary + ", sudoku:" + sudoku + " }";
        }

        /// <summary>
        /// Init the sudoku from a file
        /// </summary>
        /// <param name="path">The file path</param>
        /// <returns>The array of created sudoku</returns>
        public static List<Sudoku> InitFromFile(String path) {
            List<Sudoku> loadedSudokus = new List<Sudoku>();

            StreamReader sr = new StreamReader(path);
            String s;

            while ((s = sr.ReadLine()) != null) {
                // Comment, ignore
                if (!s.StartsWith("-") && !s.StartsWith("//")) {
                    String name = s;
                    String date = sr.ReadLine();
                    String dictionnary = sr.ReadLine();

                    // Test if enough line
                    if (name == null || date == null || dictionnary == null) {
                        sr.Close();
                        throw new Exception("Error : malformed file");
                    }

                    // Load values
                    char[,] values = new char[dictionnary.Length, dictionnary.Length];
                    int i = 0;

                    for (int size = dictionnary.Length; i < size; i++) {
                        String line = sr.ReadLine();

                        if (line == null || line.Length != size) {
                            sr.Close();
                            throw new Exception("Error : malformed file (line " + 1 + " too short/long)");
                        }

                        int j = 0;
                        foreach (char c in line) {
                            values[i, j++] = c;
                        }

                        if (j < size) {
                            sr.Close();
                            throw new Exception("Error : malformed file (not enough columns)");
                        }
                    }

                    if (i < dictionnary.Length) {
                        sr.Close();
                        throw new Exception("Error : malformed file (not enough lines)");
                    }

                    // Create Sudoku
                    try {
                        loadedSudokus.Add(new Sudoku(name, DateTime.Parse(date), dictionnary, values));
                    } catch (Exception e) {
                        sr.Close();
                        throw e;
                    }
                }
            }

            sr.Close();

            return loadedSudokus;
        }

        public uint[] lines { get; set; }
        public uint[] cols { get; set; }
        public uint[] squares { get; set; }
    }
}
