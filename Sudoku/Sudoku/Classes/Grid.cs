using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    class Grid
    {
        private int alphabetLength;
		private int nbCase;
        private string alphabet;
        public string Name { set; get; }
        public string Date { set; get; }
        private Case[,] grid;
        public static char EMPTY = '.';

        public Grid(char[][] orginalGrid, string alphabet){
            alphabetLength = alphabet.Length;
			nbCase = (int)Math.Pow (alphabetLength, 2);
            this.alphabet = alphabet;
            initGrid(orginalGrid);
        }

        private void initGrid(char[][] orginalGrid){
            grid = new Case[alphabetLength, alphabetLength];

            for (int i = 0; i < alphabetLength; i++)
            {
                for (int j = 0; j < alphabetLength; j++)
                {
                    if (orginalGrid[i][j] == EMPTY)
                    {
						grid[i, j] = new MutableCase(j, i, alphabetLength);
                    }
                    else
                    {
						grid[i, j] = new Case(orginalGrid[i][j], j, i, alphabetLength);
                    }
                }
            }
        }

		public IEnumerable<char> getPossibleCharacters(int position) {
			int row = position / alphabetLength, column = position % alphabetLength;
			foreach(char c in alphabet) {
				if(valueAccepted(c,row,column)) {
					yield return c;
				}
			}
		}

		bool valueAccepted(char c, int row, int column) {
			return valueNotInLine(c,column,row) 
				&& valueNotInColumn(c,column,row) 
				&& valueNotInBlock(c,column,row);
		}

		public Tuple<List<MutableCase>,int> initConstraints() {
			int bestIndex = -1;
			var mutables = new List<MutableCase>();
			for(int i = 0; i < nbCase; ++i) {
				if(this[i].isMutable()) {
					var mcase = (MutableCase)this[i];
					mutables.Add(mcase);
					var candidates = getPossibleCharacters(i);
					mcase.initCandidates(candidates);
					if(bestIndex != -1) {
						var best = (MutableCase)this[bestIndex];
						if(candidates.Count() < best.getCandidatesCount()) {
							bestIndex = i;
						} 
					} 
					else bestIndex = i;
				}
			}
			return new Tuple<List<MutableCase>, int>(mutables,bestIndex);
		}

		void rollbackLineConstraints( int y, char value ) {
			for(int x = 0; x < alphabetLength; ++x) {
				if(grid[x,y].isMutable())
					((MutableCase)grid[x,y]).addCandidate(value);
			}
		}

		void rollbackColumnConstraints( int x, char value ) {
			for(int y = 0; y < alphabetLength; ++y) {
				if(grid[x,y].isMutable())
					((MutableCase)grid[x,y]).addCandidate(value);
			}
		}

		void rollbackBlockConstraints( int x, int y, char value ) {
			int sqrt = (int)Math.Sqrt(alphabetLength);
			x = x / sqrt;
			y = y / sqrt;
			for (int i = y * sqrt; i < y * sqrt + sqrt; i++)
				for (int j = x * sqrt; j < x * sqrt + sqrt; j++)
					if(grid[x,y].isMutable())
						((MutableCase)grid[x,y]).addCandidate(value);
		}

		void rollbackLastConstraintsUpdate( int position ) {
			var c = this[position];
			int x = c.X;
			int y = c.Y;
			char value = c.getValue();
			rollbackLineConstraints(y,value);
			rollbackColumnConstraints(x,value);
			rollbackBlockConstraints(x,y,value);
		}

		void updateLineConstraints( int y, char value ) {
			for(int x = 0; x < alphabetLength; ++x) {
				if(grid[x,y].isMutable())
					((MutableCase)grid[x,y]).removeCandidate(value);
			}
		}

		void updateColumnConstraints( int x, char value ) {
			for(int y = 0; y < alphabetLength; ++y) {
				if(grid[x,y].isMutable())
					((MutableCase)grid[x,y]).removeCandidate(value);
			}
		}

		void updateBlockConstraints( int x, int y, char value ) {
			int sqrt = (int)Math.Sqrt(alphabetLength);
			x = x / sqrt;
			y = y / sqrt;
			for (int i = y * sqrt; i < y * sqrt + sqrt; i++)
				for (int j = x * sqrt; j < x * sqrt + sqrt; j++)
					if(grid[x,y].isMutable())
						((MutableCase)grid[x,y]).removeCandidate(value);
		}

		void updateConstraints( int position ) {
			int x = this[position].X;
			int y = this[position].Y;
			char value = this[position].getValue();
			updateLineConstraints(y,value);
			updateColumnConstraints(x,value);
			updateBlockConstraints(x,y,value);
		}

		int getNextBest( List<MutableCase> mutables , int oldBestIndex) {
			MutableCase best = oldBestIndex == 0 ? mutables[1]: mutables[0];
			int i = oldBestIndex == 0 ? 1 : 0, nbMutables = mutables.Count();
			while(best.getCandidatesCount() == 0 && i < nbMutables) {
				best = mutables[i++];
			}
			if(i < nbMutables) {
				if(best.getCandidatesCount() != 1) {
					for(; i < nbMutables; ++i) {
						if(i != oldBestIndex) {
							var e = mutables[i];
							int count = e.getCandidatesCount();
							if(count != 0 && count < best.getCandidatesCount())
								best = e;
							if(best.getCandidatesCount() == 1) {
								break;
							}
						}
					}
				}
				int r = best.getFlatIndex();
				return r;
			}
			return -1;
		}

		int updateConstaintsAndGetNextBestIndex( int position , List<MutableCase> mutables) {
			updateConstraints(position);
			((MutableCase)this[position]).setEmpty();
			return getNextBest(mutables, position);
		}

		void resetConstraintsFor( MutableCase mcase ) {
			mcase.initCandidates(getPossibleCharacters(mcase.getFlatIndex()));
		}

		public void resolve() {
			printGrid();

			var startSettings = initConstraints();

			int bestIndex = startSettings.Item2, nextBestIndex;
			var mutables = startSettings.Item1;

			if(mutables.Count == 0) return;

			var changes = new Stack<int>();
					
			while (changes.Count != mutables.Count) {
				var mcase = (MutableCase)this[bestIndex];
				while(changes.Count != mutables.Count && mcase.getCandidatesCount() != 0) {
					mcase.setRandomCandidate();
					mcase.setCurrentToTrue();
					printGrid();
					nextBestIndex = updateConstaintsAndGetNextBestIndex(bestIndex, mutables);
					if(nextBestIndex < 0) {
						rollbackLastConstraintsUpdate(bestIndex);
						printGrid();
					}
					else {
						changes.Push(bestIndex);
						bestIndex = nextBestIndex;
						mcase = (MutableCase)this[bestIndex];
					}
				}
				if(changes.Count != mutables.Count && mcase.getCandidatesCount() == 0) {
					resetConstraintsFor(mcase);
					bestIndex = changes.Pop();
					mcase = (MutableCase)this[bestIndex];
					mcase.setCurrentToTrue();
					rollbackLastConstraintsUpdate(bestIndex);
				}
				else {
					if(changes.Count == 0) {
						throw new Exception("No solutions");
					}
				}
				printGrid();
			}
		}

		public Case this[int index] {
			get {
				if(index < 0 || index >= (int)Math.Pow(alphabetLength, 2))
					throw new IndexOutOfRangeException();
				return grid[ index / alphabetLength , index % alphabetLength ];
			}
			set {
				if(index < 0 || index >= (int)Math.Pow(alphabetLength, 2))
					throw new IndexOutOfRangeException();
				grid[ index / alphabetLength , index % alphabetLength ] = value;
			}
		}

        public bool checkColumn(bool debug = false)
        {
            string col = "";
            for (int j = 0; j < alphabetLength; j++)
            {
                for (int i = 0; i < alphabetLength; i++)
                {
                    char val = grid[i, j].getValue();
                    if (col.Contains(val + "") || (alphabet != null && !alphabet.Contains(val + ""))) return false;
                    col += val;
                    if (debug)
                        Console.WriteLine(i + ", " + j + " : " + val);
                }
                col = "";
                if (debug)
                    Console.WriteLine("\n");
            }
            return true;
        }

        public bool checkLine(bool debug = false)
        {
            string col = "";
            for (int j = 0; j < alphabetLength; j++)
            {
                for (int i = 0; i < alphabetLength; i++)
                {
                    char val = grid[j, i].getValue();
                    if (col.Contains(val + "") || (alphabet != null && !alphabet.Contains(val + ""))) return false;
                    col += val;
                    if (debug)
                        Console.WriteLine(j + ", " + i + " : " + val);
                }
                col = "";
                if (debug)
                    Console.WriteLine("\n");
            }
            return true;
        }

        public bool checkBlock(bool debug = false)
        {
            int squareRoot = (int) Math.Sqrt(alphabetLength);
            string block = "";
            for (int i = 0; i < alphabetLength; i += squareRoot)
            {
                for (int j = 0; j < alphabetLength; j += squareRoot)
                {
                    for (int k = i; k < i + 3; k++)
                    {
                        for (int l = j; l < j + 3; l++)
                        {
                            char val = grid[k, l].getValue();
                            if (block.Contains(val + "") || (alphabet != null && !alphabet.Contains(val + ""))) return false;
                            block += val;
                            if (debug)
                                Console.WriteLine(k + ", " + l + " : " + val);
                        }
                    }
                    block = "";
                    if (debug)
                        Console.WriteLine("\n");
                }
            }
            return true;
        }

		public bool validate()
		{
			return checkColumn() && checkLine() && checkBlock();
		}

        private bool valueNotInLine(char value, int indR, int indC)
        {
            for (int i = 0; i < alphabetLength; i++)
            {
                if (grid[i, indR].getValue() == value)
                    return false;
            }
            return true;
        }

        private bool valueNotInColumn(char value, int indR, int indC)
        {
            for (int i = 0; i < alphabetLength; i++)
            {
                if (grid[indC, i].getValue() == value)
                    return false;
            }
            return true;
        }

        private bool valueNotInBlock(char value, int indR, int indC)
        {
            int n = (int)Math.Sqrt(alphabetLength);
            int divC, divR;
            divC = indC / n;
            divR = indR / n;
            for (int i = divC * n; i < divC * n + n; i++)
            {
                for (int j = divR * n; j < divR * n + n; j++)
                {
                    if (grid[i, j].getValue() == value)
                        return false;
                }
            }
            return true;
        }

        public void printGrid()
        {
            Console.WriteLine(Name);
            Console.WriteLine();
            for (int i = 0; i < alphabetLength; i++)
            {
                for (int j = 0; j < alphabetLength; j++)
                {
                    Console.Write(grid[i, j].getValue());
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
