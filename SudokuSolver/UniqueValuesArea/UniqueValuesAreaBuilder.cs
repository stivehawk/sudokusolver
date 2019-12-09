using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver.UniqueValuesArea
{
    public class UniqueValuesAreaBuilder
    {
        private UniqueValuesAreaBoardSet AreaBoardSet;
        private List<Coordinate> Coordinates;

        public UniqueValuesAreaBuilder(UniqueValuesAreaBoardSet boardSet, int row, int column)
        {
            this.AreaBoardSet = boardSet;
            this.Coordinates = new List<Coordinate>();
            this.Coordinates.Add(new Coordinate(row, column));
        }

        public UniqueValuesAreaBuilder AddCoordinate(int row, int column)
        {
            this.Coordinates.Add(new Coordinate(row, column));
            return this;
        }

        public UniqueValuesArea Build()
        {
            var area = new UniqueValuesArea(Coordinates);
            AreaBoardSet.Areas.Add(area);
            return area;
        }
    }
}