using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Sudoku
{
    class Case
    {
        protected char value;
		protected int alphabetLength;
		public int X { get; private set;}
		public int Y { get; private set;}

		public Case(char _value, int x, int y, int length){
            value = _value;
			X = x;
			Y = y;
			alphabetLength = length;
        }

        public char getValue()
        {
            return value;
        }

        public virtual bool isMutable()
        {
            return false;
        }

		public bool isNotMutable() {
			return !isMutable();
		}

		public int getFlatIndex() {
			return Y * alphabetLength + X;
		}
    }
}
