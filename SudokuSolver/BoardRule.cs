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
        //public ThermometerBoardSet Thermometers;

        public BoardRule()
        {
            this.ValuesToNotRepeat = new List<Func<Board, Cell, IEnumerable<int>>>();
            this.KnightSteps = KnightStep.GetSteps().ToList();
        }

        public BoardRule ActivateUniqueInsideRow() { ValuesToNotRepeat.Add(ValuesInTheRowOf); return this; }
        public BoardRule ActivateUniqueInsideColumn() { ValuesToNotRepeat.Add(ValuesInTheColumnOf); return this; }
        public BoardRule ActivateUniqueInside9x9Block() { ValuesToNotRepeat.Add(ValuesInThe9x9BlockOf); return this; }
        public BoardRule ActivateUniqueOrthogonallyAdjacent() { ValuesToNotRepeat.Add(ValuesOrthogonallyAdjacent); return this; }
        public BoardRule ActivateNonConsecutiveOrthogonallyAdjacent() { ValuesToNotRepeat.Add(ConsecutiveValuesForOrthogonallyAdjacentCells); return this; }
        public BoardRule ActivateUniqueOnKnightStep() { ValuesToNotRepeat.Add(ValuesWithinKnightStep); return this; }
        public BoardRule ActivateThermometers(Action<ThermometerBoardSet> action)
        {
            var thermometerSet = new ThermometerBoardSet(this);
            action(thermometerSet);

            ValuesToNotRepeat.Add((board, cell) => thermometerSet.GetImpossibleValuesForCell(board, cell));
            //Thermometers = thermometerSet;

            return this;
        }

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
            ValuesPool possibleNumbers = new ValuesPool();

            var unavailableValues = ValuesToNotRepeat.SelectMany(x => x(board, cell));

            foreach(var unavailableValue in unavailableValues)
                possibleNumbers.MakeUnavailable(unavailableValue);
                
            return possibleNumbers.GetAvailableValues().ToArray();
        }

        public class ThermometerBoardSet
        {
            private BoardRule Rules;
            public List<Thermometer> Thermometers;

            public ThermometerBoardSet(BoardRule rules)
            {
                this.Rules = rules;
                this.Thermometers = new List<Thermometer>();
            }

            public ThermometerBuilder CreateThermometerBuilder(int row, int column)
            {
                return new ThermometerBuilder(this, row, column);
            }

            public IEnumerable<int> GetImpossibleValuesForCell(Board board, Cell cellChecked)
            {
                foreach (var thermometer in Thermometers)
                {
                    var themoDataForCellChecked = thermometer.GetDataForCell(cellChecked);

                    if(themoDataForCellChecked != null)
                    {
                        #region Sequencial thermometer (unusual)
                        // This code was created for sequencial thermometers (1, 2, 3...).
                        // Thermometers usually are not sequencial, they just need to get bigger (2, 4, 8...)

                        /*
                        // Check if any cell inside the thermometer is already set - this should define the entire sequence of the thermometer
                        bool sequenceIsFixed = false;
                        foreach (var thermoData in thermometer.CoordinateDatas)
                        {
                            var cellFromThermometer = board.CellsByRow[thermoData.Row].First(boardCell => boardCell.Column == thermoData.Column);

                            if(cellFromThermometer.CurrentNumber.HasValue)
                            {
                                // If any cell inside the thermometer already has a value defined, all the numbers that don't satisfy the sequence are impossible
                                int onlyValidNumber;

                                int difference = thermoData.SequenceIndex - themoDataForCellChecked.SequenceIndex;
                                if (difference < 0) difference = -difference;

                                if (thermoData.SequenceIndex > themoDataForCellChecked.SequenceIndex)
                                    onlyValidNumber = cellFromThermometer.CurrentNumber.Value - difference;
                                else
                                    onlyValidNumber = cellFromThermometer.CurrentNumber.Value + difference;

                                foreach (var impossibleNumber in new AvailableValues().MakeUnavailable(onlyValidNumber).GetAvailableValues())
                                    yield return impossibleNumber;

                                sequenceIsFixed = true;
                                break;
                            }
                        }

                        if (!sequenceIsFixed)
                        {
                            foreach (var value in themoDataForCellChecked.ImpossibleValues)
                            {
                                yield return value;
                            }
                        }
                        */
                        #endregion

                        foreach (var thermoData in thermometer.CoordinateDatas)
                        {
                            var cellFromThermometer = board.GetCell(cellChecked.Row, cellChecked.Column);

                            if (cellFromThermometer.CurrentNumber.HasValue)
                            {
                                var validNumbers = new ValuesPool();
                                
                                if(themoDataForCellChecked.SequenceIndex < thermoData.SequenceIndex)
                                {
                                    validNumbers.MakeUnavailableIfBiggerThan(cellFromThermometer.CurrentNumber.Value);
                                }
                                else
                                {
                                    validNumbers.MakeUnavailableIfSmallerThan(cellFromThermometer.CurrentNumber.Value);
                                }

                                validNumbers.MakeUnavailable(cellFromThermometer.CurrentNumber.Value);

                                // Method return invalid numbers
                                foreach (var impossibleValues in validNumbers.CreateReverse().GetAvailableValues())
                                    yield return impossibleValues;

                                break;
                            }
                        }

                        foreach (var value in themoDataForCellChecked.ImpossibleValues)
                        {
                            yield return value;
                        }
                    }
                }
            }

            public override string ToString()
            {
                List<string> c = new List<string>();

                for (var row = 0; row < 9; row++)
                {
                    for (var column = 0; column < 9; column++)
                    {
                        var coordinateData = Thermometers.SelectMany(x => x.CoordinateDatas).FirstOrDefault(x => x.Row == row && x.Column == column);

                        if (coordinateData == null)
                            c.Add(" ");
                        else
                            c.Add($"{coordinateData.SequenceIndex}");
                    }

                    c.Add(Environment.NewLine);
                }

                return string.Join(string.Empty, c);
            }


            public class ThermometerBuilder
            {
                private ThermometerBoardSet ThermometerBoardSet;
                private List<Coordinate> Coordinates;

                public ThermometerBuilder(ThermometerBoardSet thermometerBoardSet, int row, int column)
                {
                    this.ThermometerBoardSet = thermometerBoardSet;
                    this.Coordinates = new List<Coordinate>();
                    this.Coordinates.Add(new Coordinate(row, column));
                }

                public ThermometerBuilder AddSequence(int row, int column)
                {
                    this.Coordinates.Add(new Coordinate(row, column));
                    return this;
                }

                public Thermometer Build()
                {
                    var thermometer = new Thermometer(Coordinates);
                    ThermometerBoardSet.Thermometers.Add(thermometer);
                    return thermometer;
                }
            }
        }
    }
}