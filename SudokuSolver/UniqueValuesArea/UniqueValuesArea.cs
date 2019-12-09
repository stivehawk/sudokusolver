using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver.UniqueValuesArea
{
    public class UniqueValuesArea
    {
        public List<Coordinate> Coordinates;
        private Dictionary<int, Dictionary<int, Coordinate>> CoordiantesForRow;

        public UniqueValuesArea(List<Coordinate> coordinates)
        {
            this.Coordinates = coordinates;
            this.CoordiantesForRow = new Dictionary<int, Dictionary<int, Coordinate>>();

            foreach (var coordinate in coordinates)
            {
                if (!CoordiantesForRow.TryGetValue(coordinate.Row, out var coordinatesForColumn))
                {
                    coordinatesForColumn = new Dictionary<int, Coordinate>();
                    CoordiantesForRow.Add(coordinate.Row, coordinatesForColumn);
                }

                coordinatesForColumn.Add(coordinate.Column, coordinate);
            }
        }

        public bool ContainsCell(Cell cell)
        {
            if (CoordiantesForRow.TryGetValue(cell.Row, out var coordinatesForColumn))
                return coordinatesForColumn.ContainsKey(cell.Column);

            return false;
        }
    }
}