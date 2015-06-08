using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication {

    class Program {

        static void Main(string[] args) {
            
            // DEBUG
            /*
            char[,] grid = new char[9, 9]
            // 5.78 s
            {
                {'0','0','0','0','0','0','0','0','0'},
                {'0','0','0','0','0','3','0','8','5'},
                {'0','0','1','0','2','0','0','0','0'},
                {'0','0','0','5','0','7','0','0','0'},
                {'0','0','4','0','0','0','1','0','0'},
                {'0','9','0','0','0','0','0','0','0'},
                {'5','0','0','0','0','0','0','7','3'},
                {'0','0','2','0','1','0','0','0','0'},
                {'0','0','0','0','4','0','0','0','9'}
            };
            // 1.1 ms
            {
                {'9','0','0','1','0','0','0','0','5'},
                {'0','0','5','0','9','0','2','0','1'},
                {'8','0','0','0','4','0','0','0','0'},
                {'0','0','0','0','8','0','0','0','0'},
                {'0','0','0','7','0','0','0','0','0'},
                {'0','0','0','0','2','6','0','0','9'},
                {'2','0','0','3','0','0','0','0','6'},
                {'0','0','0','2','0','0','9','0','0'},
                {'0','0','1','9','0','4','5','7','0'}
            };

            Sudoku sudoku = new Sudoku("Test", new DateTime(), "123456789", grid, '0');
            sudoku.DebugDisplaySudoku();

            Console.ReadLine();

            Stopwatch watch = Stopwatch.StartNew();
            var solved = sudoku.Solve();
            watch.Stop();

            if (solved) {
                Console.WriteLine(String.Format("Sudoku solved in {0:0.0000}ms", watch.Elapsed.TotalMilliseconds));
                Console.WriteLine("Is it valid ? " + (sudoku.Validate() == null));
                Console.ReadLine();
                sudoku.DebugDisplaySudoku();
            } else {
                Console.WriteLine("Sudoku not solved");
            }
            
            Console.ReadLine();

            return;
            // DEBUG
            */

            // Handle options
            Stopwatch watch = Stopwatch.StartNew();
            Options.ParseArgs(args);
            watch.Stop();

            if (Options.verbose)
                Console.WriteLine(String.Format("Options parsed in {0:0.0000}ms", watch.Elapsed.TotalMilliseconds));

            List<Sudoku> sudokus = new List<Sudoku>();

            try {
                // Read files
                for (int i = 0; i < Options.files.Count; i++) {
                    watch.Restart();
                    List<Sudoku> tmp = Sudoku.InitFromFile(Options.files.ElementAt(i));
                    watch.Stop();

                    if(Options.verbose)
                        Console.WriteLine(String.Format("File '{0}' loaded ({1} sudoku(s)) in {2:0.0000}ms",
                            Options.files.ElementAt(i), tmp.Count, watch.Elapsed.TotalMilliseconds));

                    sudokus.AddRange(tmp);
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                System.Environment.Exit(1);
            }

            // Validate/Solve loaded sudokus
            long ticks = 0;

            if(Options.solve) {
                foreach (Sudoku sudoku in sudokus) {

                    if (Options.show)
                        sudoku.DisplaySudoku();

                    watch.Restart();
                    var valid = sudoku.Solve();
                    watch.Stop();

                    ticks += watch.ElapsedTicks;

                    if (valid) {
                        if (sudoku.Validate() != null)
                            Console.WriteLine(sudoku.name + " can't be solved : error while solving it");
                        else {
                            Console.WriteLine(sudoku.name + " is solved");

                            if (Options.show)
                                sudoku.DisplaySudoku();
                        }
                    } else {
                        Console.WriteLine(sudoku.name + " can't be solved");
                    }
                }

                if (Options.verbose)
                    Console.WriteLine(String.Format("Resolution of {0} sudoku(s) in {1:0.0000}ms", sudokus.Count, (double)ticks/Stopwatch.Frequency * 1000));
            } else {
                foreach (Sudoku sudoku in sudokus) {

                    if (Options.show)
                        sudoku.DisplaySudoku();

                    watch.Restart();
                    var valid = sudoku.Validate();
                    watch.Stop();

                    ticks += watch.ElapsedTicks;
                        
                    if (valid == null) {
                        Console.WriteLine(sudoku.name + " is valid");
                    } else {
                        Console.WriteLine(sudoku.name + " is not valid : " + valid.error + " at " + valid.nb);
                    }

                }

                if (Options.verbose)
                    Console.WriteLine(String.Format("Validation of {0} sudoku(s) in {1:0.0000}ms", sudokus.Count, (double)ticks/Stopwatch.Frequency * 1000));
            }
            Console.ReadLine();
        }
    }

}
