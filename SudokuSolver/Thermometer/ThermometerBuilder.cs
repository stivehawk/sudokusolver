using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver.Thermometer
{
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
