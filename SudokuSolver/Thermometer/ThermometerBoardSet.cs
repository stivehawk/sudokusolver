using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver.Thermometer
{
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

                if (themoDataForCellChecked != null)
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

                            if (themoDataForCellChecked.SequenceIndex < thermoData.SequenceIndex)
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
    }
}
