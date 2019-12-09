using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver.Thermometer
{
    public class Thermometer
    {
        public List<CoordinateData> CoordinateDatas;
        private Dictionary<int, Dictionary<int, CoordinateData>> ImpossibleValuesForRow;

        public Thermometer(List<Coordinate> thermometerCoordinates)
        {
            this.CoordinateDatas = new List<CoordinateData>();
            this.ImpossibleValuesForRow = new Dictionary<int, Dictionary<int, CoordinateData>>();

            var length = thermometerCoordinates.Count;

            int minPossibleValue = 1;
            int maxPossibleValue = 9 - length + 1;

            for (var i = 0; i < length; i++)
            {
                var coordinate = thermometerCoordinates[i];
                var possibleValues = CreateRange(minPossibleValue + i, maxPossibleValue + i);
                var impossibleValues = ImpossibleValues(possibleValues);

                RegisterImpossibleValuesForCoordinate(coordinate.Row, coordinate.Column, i, impossibleValues);
            }
        }

        private int[] ImpossibleValues(int[] possibleValues)
        {
            ValuesPool numbers = new ValuesPool();

            foreach (var value in possibleValues)
                numbers.MakeUnavailable(value);

            return numbers.GetAvailableValues().ToArray();
        }

        private void RegisterImpossibleValuesForCoordinate(int row, int column, int sequenceIndex, int[] impossibleValues)
        {
            if (!ImpossibleValuesForRow.TryGetValue(row, out Dictionary<int, CoordinateData> impossibleValuesForColumn))
            {
                impossibleValuesForColumn = new Dictionary<int, CoordinateData>();
                ImpossibleValuesForRow.Add(row, impossibleValuesForColumn);
            }

            var coordinateData = new CoordinateData(row, column, sequenceIndex, impossibleValues);
            impossibleValuesForColumn.Add(column, coordinateData);

            CoordinateDatas.Add(coordinateData);
        }

        private int[] CreateRange(int min, int max)
        {
            int length = max - min;
            int[] values = new int[length + 1];

            for (var i = 0; i <= length; i++)
                values[i] = min + i;

            return values;
        }

        public CoordinateData GetDataForCell(Cell cell)
        {
            if (ImpossibleValuesForRow.TryGetValue(cell.Row, out Dictionary<int, CoordinateData> impossibleValuesForColumn))
                if (impossibleValuesForColumn.TryGetValue(cell.Column, out CoordinateData coordinateData))
                    return coordinateData;

            return null;
        }
        
        public override string ToString()
        {
            List<string> c = new List<string>();

            for(var row = 0; row < 9; row++)
            {
                for(var column = 0; column < 9; column++)
                {
                    var coordinateData = CoordinateDatas.FirstOrDefault(x => x.Row == row && x.Column == column);

                    if (coordinateData == null)
                        c.Add(" "); 
                    else
                        c.Add($"{coordinateData.SequenceIndex}");
                }

                c.Add(Environment.NewLine);
            }

            return string.Join(string.Empty, c);
        }

        public class CoordinateData
        {
            public int Row;
            public int Column;
            public int SequenceIndex;
            public int[] ImpossibleValues;
            
            public CoordinateData(int row, int column, int sequenceIndex, int[] impossibleValues)
            {
                this.Row = row;
                this.Column = column;

                this.SequenceIndex = sequenceIndex;
                this.ImpossibleValues = impossibleValues;
            }
        }
    }
}
