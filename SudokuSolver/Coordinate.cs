using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver
{
    public class Coordinate
    {
        public int Row;
        public int Column;

        public Coordinate(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }
    }
}
