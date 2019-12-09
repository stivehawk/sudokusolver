using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    public class Board
    {
        public Cell[] Cells;

        public Dictionary<int, List<Cell>> CellsByRow;
        public Dictionary<int, List<Cell>> CellsByColumn;


        public Board()
        {
            this.CellsByRow = new Dictionary<int, List<Cell>>();
            this.CellsByColumn = new Dictionary<int, List<Cell>>();
            this.Cells = new Cell[81];
            
            for (var row = 0; row < 9; row++)
            {
                for (var column = 0; column < 9; column++)
                {
                    // Add cell into cells array
                    Cell cell = new Cell()
                    {
                        Row = row,
                        Column = column
                    };

                    Cells[row * 9 + column] = cell;

                    // Add cell into row list
                    if(!CellsByRow.TryGetValue(row, out List<Cell> rowList))
                    {
                        rowList = new List<Cell>(9);
                        CellsByRow.Add(row, rowList);
                    }

                    rowList.Add(cell);

                    // Add cell into column list
                    if (!CellsByColumn.TryGetValue(column, out List<Cell> columnList))
                    {
                        columnList = new List<Cell>(9);
                        CellsByColumn.Add(column, columnList);
                    }

                    columnList.Add(cell);
                }
            }
        }
        
        public Board(Board board, Cell newCell)
        {
            this.CellsByRow = new Dictionary<int, List<Cell>>();
            this.CellsByColumn = new Dictionary<int, List<Cell>>();
            this.Cells = new Cell[81];

            for (var row = 0; row < 9; row++)
            {
                if(row != newCell.Row)
                {
                    CellsByRow.Add(row, board.CellsByRow[row]);
                }
                else
                {
                    var cells = board
                        .CellsByRow[row]
                        .Select(x =>
                        {
                            if (x.Column == newCell.Column) return newCell;
                            return x;
                        })
                        .ToList();

                    CellsByRow.Add(row, cells);
                }
            }

            for (var column = 0; column < 9; column++)
            {
                if (column != newCell.Column)
                {
                    CellsByColumn.Add(column, board.CellsByColumn[column]);
                }
                else
                {
                    var cells = board
                        .CellsByColumn[column]
                        .Select(x =>
                        {
                            if (x.Row == newCell.Row) return newCell;
                            return x;
                        })
                        .ToList();

                    CellsByColumn.Add(column, cells);
                }
            }

            for(var i = 0; i < Cells.Length; i++)
            {
                if(board.Cells[i].Column == newCell.Column && board.Cells[i].Row == newCell.Row)
                {
                    Cells[i] = newCell;
                }
                else
                {
                    Cells[i] = board.Cells[i];
                }
            }
        }

        public Cell GetCell(int row, int column)
        {
            return CellsByRow[row][column];
        }

        public Board CreateChildBoard(Cell changedCell, int newValue)
        {
            var duplicate = changedCell.Duplicate();
            duplicate.CurrentNumber = newValue;

            var board = new Board(this, duplicate);
            return board;
        }

        //private static Random Random = new Random();

        private CellToChange SelectNextCellToChange(BoardRule rules)
        {
            CellToChange cellWithLessPossibilities = null;

            foreach (var cell in Cells)
            {
                if(!cell.CurrentNumber.HasValue)
                {
                    var possibleNumbers = rules.GetPossibleNumbers(this, cell);

                    if(cellWithLessPossibilities == null || cellWithLessPossibilities.PossibleNumbers.Length > possibleNumbers.Length)
                    {
                        cellWithLessPossibilities = new CellToChange()
                        {
                            Cell = cell,
                            PossibleNumbers = possibleNumbers
                        };
                    }

                    // Shortcut to not process cells when there's a best one already
                    // Has the drawback of not identifying cells with 0 possibilities if there's a cell with 1 before
                    if (cellWithLessPossibilities.PossibleNumbers.Length <= 1)
                        return cellWithLessPossibilities;
                }
            }

            return cellWithLessPossibilities;
        }

        public IEnumerable<Board> NextBestBoards(BoardRule rules)
        {
            var cellToChange = SelectNextCellToChange(rules);
            
            foreach (var possibleValue in cellToChange.PossibleNumbers)
            {
                var newBoard = CreateChildBoard(cellToChange.Cell, possibleValue);
                yield return newBoard;
            }
        }

        private class CellToChange
        {
            public Cell Cell;
            public int[] PossibleNumbers;
        }

        public bool IsComplete()
        {
            return Cells.All(x => x.CurrentNumber.HasValue);
        }

        public override string ToString()
        {
            return string.Join
            (
                Environment.NewLine,
                CellsByRow
                    .OrderBy(row => row.Key)
                    .Select(row => string.Join(string.Empty, row.Value.OrderBy(cell => cell.Column).Select(cell => cell.CurrentNumber?.ToString() ?? " ")))
            );
        }
    }
}
