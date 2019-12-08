using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver
{
    public class Cell
    {
        public int Row;
        public int Column;
        public int? CurrentNumber;

        public Cell Duplicate()
        {
            return new Cell()
            {
                Row = Row,
                Column = Column,
                CurrentNumber = CurrentNumber
            };
        }

        public override string ToString()
        {
            return $"{{{Row},{Column}}}={CurrentNumber?.ToString() ?? string.Empty}";
        }
    }
}
