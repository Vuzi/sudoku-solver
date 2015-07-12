using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sudoku
{
    class SudokuSpliter
    {
        internal Grid[] split(string path)
        {
            String[] content;
            try
            {
                content = File.ReadAllLines(path);
            }
            catch
            {
                Console.WriteLine("An error occured when reading file " + path);
                return new Grid[0];
            }

            if (!content[0].StartsWith("//-"))
            {
                Console.WriteLine("File is invalid. It must begin by '------'");
                return new Grid[0];
            }

            List<List<String>> splitedGrid = new List<List<String>>();
            List<String> sudoku = null;
            int i = 0;

            do
            {
                if (content[i].StartsWith("//-"))
                {
                    sudoku = new List<String>();
                    splitedGrid.Add(sudoku);
                }
                else
                {
                    sudoku.Add(content[i]);
                }
                i++;
            } while (i < content.Length);

            return gridsFrom(splitedGrid);
        }

        private Grid[] gridsFrom(List<List<string>> splitedGrid)
        {
            List<Grid> grids = new List<Grid>();
            foreach (List<string> sudoku in splitedGrid)
            {
                int i, j = 0;
                char[][] grid = null;
                string name = null, date = null, alphabet = null;
                for (i = 0; i > -1 && i < sudoku.Count; i++)
                {
                    switch (i)
                    {
                        case 0: name = sudoku[i]; break;
                        case 1: date = sudoku[i]; break;
                        case 2:
                            alphabet = sudoku[i];
                            grid = new char[alphabet.Length][];
                            break;
                        default:
                            Char[] line = sudoku[i].ToCharArray();
                            if (line.Length != alphabet.Length || j >= alphabet.Length)
                            {
                                Console.WriteLine("File is invalid. The sudoku " + name + " is not a square. We ignore it.");
                                i = -2;
                            }
                            else
                            {
                                int k = 0;
                                for (; k < line.Length; k++)
                                {
                                    if (!alphabet.Contains(line[k]) && line[k] != Grid.EMPTY)
                                    {
                                        break;
                                    }
                                }

                                if (k < line.Length)
                                {
                                    Console.WriteLine("File is invalid. Unknown symbol " + line[k] + " in sudoku " + name + ".");
                                    i = -2;
                                }
                                else
                                {
                                    grid[j++] = line;
                                }
                            }
                        break;
                    }
                }
                if (i != -1)
                {
                    Grid g = new Grid(grid, alphabet);
                    g.Name = name;
                    g.Date = date;
                    grids.Add(g);
                }
            }
            return grids.ToArray();
        }
    }
}
