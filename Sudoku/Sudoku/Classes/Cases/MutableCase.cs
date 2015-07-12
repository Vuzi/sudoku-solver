using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    class MutableCase : Case
    {
		private static Random rnd = new Random();

		private Dictionary<char,bool> candidates;

		public MutableCase(int x, int y, int nb) : base(Grid.EMPTY, x, y, nb) {
			candidates = new Dictionary<char,bool>();
        }

        public void setValue(char _value) {
            value = _value;
        }

        public override bool isMutable() {
            return true;
        }

		public void initCandidates(IEnumerable<char> characters) {
			candidates.Clear();
			foreach(char c in characters) {
				candidates[c] = false;
			}
		}

		public int getCandidatesInitialCount() {
			return candidates.Count;
		}

		public int getCandidatesCount() {
			return candidates.Count(key => !key.Value);
		}

		public void removeCandidate( char value ) {
			if(candidates.ContainsKey(value))
				candidates[value] = true;
		}

		public void addCandidate( char value ) {
			if(candidates.ContainsKey(value))
				candidates[value] = false;
		}

		public void setRandomCandidate() {
			var potentialCharacters = candidates.Where(tuple => !tuple.Value).Select(p => p.Key).ToArray();
			value = potentialCharacters[rnd.Next(potentialCharacters.Count())];
		}

		public void setCurrentToTrue() {
			candidates[value] = true;
		}

        public void setTrue(char c) {
			candidates[c] = true;
        }

		public bool isFalse(char c) {
			return !candidates[c];
		}

		public bool isEmpty() {
			return value == Grid.EMPTY;
		}

		public void setEmpty() {
			value = Grid.EMPTY;
		}
    }
}
