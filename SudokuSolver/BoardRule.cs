using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    public class BoardRule
    {
        // How the function should work:
        // Parameter 1: Full game board
        // Parameter 2: Cell analyzed
        // Returns: Values the cell (parameter 2) CANNOT have
        private List<Func<Board, Cell, IEnumerable<int>>> ValuesToNotRepeat;

        public BoardRule()
        {
            ValuesToNotRepeat = new List<Func<Board, Cell, IEnumerable<int>>>();
            KnightSteps = KnightStep.GetSteps().ToList();
        }

        public BoardRule ActivateUniqueInsideRow() { ValuesToNotRepeat.Add(ValuesInTheRowOf); return this; }
        public BoardRule ActivateUniqueInsideColumn() { ValuesToNotRepeat.Add(ValuesInTheColumnOf); return this; }
        public BoardRule ActivateUniqueInside9x9Block() { ValuesToNotRepeat.Add(ValuesInThe9x9BlockOf); return this; }
        public BoardRule ActivateUniqueOrthogonallyAdjacent() { ValuesToNotRepeat.Add(ValuesOrthogonallyAdjacent); return this; }
        public BoardRule ActivateNonConsecutiveOrthogonallyAdjacent() { ValuesToNotRepeat.Add(ConsecutiveValuesForOrthogonallyAdjacentCells); return this; }
        public BoardRule ActivateUniqueOnKnightStep() { ValuesToNotRepeat.Add(ValuesWithinKnightStep); return this; }

        public class BlockBoundary
        {
            public int UpperRow;
            public int LowerRow;
            public int UpperColumn;
            public int LowerColumn;
        }

        private BlockBoundary Get9x9BlockOf(Cell cell)
        {
            BlockBoundary boundary = new BlockBoundary();
            
            int aux = 0;

            if (cell.Row < 3) aux = 3;
            else if (cell.Row < 6) aux = 6;
            else if (cell.Row < 9) aux = 9;

            boundary.LowerRow = aux - 3;
            boundary.UpperRow = aux - 1;

            if (cell.Column < 3) aux = 3;
            else if (cell.Column < 6) aux = 6;
            else if (cell.Column < 9) aux = 9;

            boundary.LowerColumn = aux - 3;
            boundary.UpperColumn = aux - 1;

            return boundary;
        }

        private IEnumerable<int> ValuesInThe9x9BlockOf(Board board, Cell cell)
        {
            var blockBoundary = Get9x9BlockOf(cell);

            for (var row = blockBoundary.LowerRow; row <= blockBoundary.UpperRow; row++)
            {
                var cells = board.CellsByRow[row];

                foreach (var cellOfRow in cells)
                    if (cellOfRow.Column >= blockBoundary.LowerColumn && cellOfRow.Column <= blockBoundary.UpperColumn)
                        if(cellOfRow.CurrentNumber.HasValue)
                            yield return cellOfRow.CurrentNumber.Value; 
            }
        }

        public class KnightStep
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;

            public static IEnumerable<KnightStep> GetSteps()
            {
                yield return new KnightStep() { Left = 1, Top = 2, Right = 0, Bottom = 0 };
                yield return new KnightStep() { Left = 2, Top = 1, Right = 0, Bottom = 0 };

                yield return new KnightStep() { Left = 1, Top = 0, Right = 0, Bottom = 2 };
                yield return new KnightStep() { Left = 2, Top = 0, Right = 0, Bottom = 1 };

                yield return new KnightStep() { Left = 0, Top = 2, Right = 1, Bottom = 0 };
                yield return new KnightStep() { Left = 0, Top = 1, Right = 2, Bottom = 0 };

                yield return new KnightStep() { Left = 0, Top = 0, Right = 1, Bottom = 2 };
                yield return new KnightStep() { Left = 0, Top = 0, Right = 2, Bottom = 1 };
            }
        }

        private IEnumerable<KnightStep> KnightSteps;
        public IEnumerable<int> ValuesWithinKnightStep(Board board, Cell cell)
        {
            foreach(var step in KnightSteps)
            {
                int row = cell.Row - step.Left + step.Right;
                int column = cell.Column - step.Top + step.Bottom;
            
                if(row >= 0 && row <= 8 && column >= 0 && column <= 8)
                {
                    var cellsOfRow = board.CellsByRow[row];
                    var cellOfRow = cellsOfRow.First(x => x.Column == column);

                    if(cellOfRow.CurrentNumber.HasValue)
                        yield return cellOfRow.CurrentNumber.Value;
                }
            }
        }

        private IEnumerable<Cell> ListOrthogonallyAdjacentCells(Board board, Cell cell)
        {
            var cellRow = board.CellsByRow[cell.Row];
            var cellColumn = board.CellsByColumn[cell.Column];

            if (cell.Column > 0)
            {
                var columnIndex = cell.Column - 1;
                yield return cellRow.First(x => x.Column == columnIndex);
            }

            if (cell.Column < 8)
            {
                var columnIndex = cell.Column + 1;
                yield return cellRow.First(x => x.Column == columnIndex);
            }

            if (cell.Row > 0)
            {
                var rowIndex = cell.Row - 1;
                yield return cellColumn.First(x => x.Row == rowIndex);
            }

            if (cell.Row < 8)
            {
                var rowIndex = cell.Row + 1;
                yield return cellColumn.First(x => x.Row == rowIndex);
            }
        }

        private IEnumerable<int> ConsecutiveValuesForOrthogonallyAdjacentCells(Board board, Cell cell)
        {
            foreach (var adjacentCell in ListOrthogonallyAdjacentCells(board, cell))
            {
                if (adjacentCell.CurrentNumber.HasValue)
                {
                    yield return adjacentCell.CurrentNumber.Value - 1;
                    yield return adjacentCell.CurrentNumber.Value + 1;
                }
            }
        }

        private IEnumerable<int> ValuesOrthogonallyAdjacent(Board board, Cell cell)
        {
            foreach (var adjacentCell in ListOrthogonallyAdjacentCells(board, cell))
                if (adjacentCell.CurrentNumber.HasValue)
                    yield return adjacentCell.CurrentNumber.Value;
        }

        private IEnumerable<int> ValuesInTheRowOf(Board board, Cell cell)
        {
            return board.CellsByRow[cell.Row].Where(x => x.CurrentNumber.HasValue).Select(x => x.CurrentNumber.Value);
        }

        private IEnumerable<int> ValuesInTheColumnOf(Board board, Cell cell)
        {
            return board.CellsByColumn[cell.Column].Where(x => x.CurrentNumber.HasValue).Select(x => x.CurrentNumber.Value);
        }
        
        public int[] GetPossibleNumbers(Board board, Cell cell)
        {
            PossibleNumbers possibleNumbers = new PossibleNumbers();

            var unavailableValues = ValuesToNotRepeat.SelectMany(x => x(board, cell));

            foreach(var unavailableValue in unavailableValues)
                possibleNumbers.MakeUnavailable(unavailableValue);
                
            return possibleNumbers.GetPossibleValues().ToArray();
        }

        private class PossibleNumbers
        {
            public bool Number1 = true;
            public bool Number2 = true;
            public bool Number3 = true;
            public bool Number4 = true;
            public bool Number5 = true;
            public bool Number6 = true;
            public bool Number7 = true;
            public bool Number8 = true;
            public bool Number9 = true;
            
            public void MakeUnavailable(int value)
            {
                switch(value)
                {
                    case 1: Number1 = false; break;
                    case 2: Number2 = false; break;
                    case 3: Number3 = false; break;
                    case 4: Number4 = false; break;
                    case 5: Number5 = false; break;
                    case 6: Number6 = false; break;
                    case 7: Number7 = false; break;
                    case 8: Number8 = false; break;
                    case 9: Number9 = false; break;
                }
            }

            public IEnumerable<int> GetPossibleValues()
            {
                if (Number1) yield return 1;
                if (Number2) yield return 2;
                if (Number3) yield return 3;
                if (Number4) yield return 4;
                if (Number5) yield return 5;
                if (Number6) yield return 6;
                if (Number7) yield return 7;
                if (Number8) yield return 8;
                if (Number9) yield return 9;
            }
        }
    }
}