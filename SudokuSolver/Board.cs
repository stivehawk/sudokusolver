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

        public Board() : this(CreateCellsForBoard().ToArray())
        {
           
        }

        public Board(Cell[] cell)
        {
            this.Cells = cell;

            CellsByColumn = Cells.GroupBy(x => x.Column).ToDictionary(x => x.Key, x => x.ToList());
            CellsByRow = Cells.GroupBy(x => x.Row).ToDictionary(x => x.Key, x => x.ToList());
        }

        public static IEnumerable<Cell> CreateCellsForBoard()
        {
            for(var row = 0; row < 9; row++)
            {
                for(var column = 0; column < 9; column++)
                {
                    Cell cell = new Cell()
                    {
                        Row = row,
                        Column = column
                    };

                    yield return cell;
                }
            }
        }

        public Board CreateChildBoard(Cell changedCell, int newValue)
        {
            List<Cell> cellsForNewBoard = new List<Cell>(Cells.Length);

            foreach(var cell in Cells)
            {
                if(cell.Row == changedCell.Row && cell.Column == changedCell.Column)
                {
                    var duplicate = changedCell.Duplicate();
                    duplicate.CurrentNumber = newValue;
                    cellsForNewBoard.Add(duplicate);
                }
                else
                {
                    cellsForNewBoard.Add(cell);
                }
            }

            var board = new Board(cellsForNewBoard.ToArray());
            return board;
        }

        public IEnumerable<Board> NextBestBoards(BoardRule rules)
        {
            var cellToChange = Cells
                .Where(x => !x.CurrentNumber.HasValue)
                .Select(x => new
                {
                    Cell = x,
                    PossibleNumbers = rules.GetPossibleNumbers(this, x)
                })
                .OrderBy(x => x.PossibleNumbers.Length)
                .FirstOrDefault();

            //if(cellToChange.Cell.Row > 0)
            //{
            //    var breaker = 0;
            //}

            foreach(var possibleValue in cellToChange.PossibleNumbers)
            {
                var newBoard = CreateChildBoard(cellToChange.Cell, possibleValue);
                yield return newBoard;
            }
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
