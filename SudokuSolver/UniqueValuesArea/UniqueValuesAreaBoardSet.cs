using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver.UniqueValuesArea
{
    public class UniqueValuesAreaBoardSet
    {
        private BoardRule Rules;
        public List<UniqueValuesArea> Areas;

        public UniqueValuesAreaBoardSet(BoardRule rules)
        {
            this.Rules = rules;
            this.Areas = new List<UniqueValuesArea>();
        }

        public UniqueValuesAreaBuilder CreateUniqueValuesAreaBuilder(int row, int column)
        {
            return new UniqueValuesAreaBuilder(this, row, column);
        }

        public void RegisterAreasByParsingBoard(params string[] rows)
        {
            Dictionary<char, UniqueValuesAreaBuilder> builders = new Dictionary<char, UniqueValuesAreaBuilder>();
            
            for(var rowIndex = 0; rowIndex < 9; rowIndex++)
            {
                for(var columnIndex = 0; columnIndex < 9; columnIndex++)
                {
                    var identifier = rows[rowIndex][columnIndex];

                    if(!builders.TryGetValue(identifier, out var builder))
                    {
                        builder = new UniqueValuesAreaBuilder(this, rowIndex, columnIndex);
                        builders.Add(identifier, builder);
                    }
                    else
                    {
                        builder.AddCoordinate(rowIndex, columnIndex);
                    }
                }
            }

            foreach(var builder in builders)
                builder.Value.Build();
        }

        public IEnumerable<int> GetImpossibleValuesForCell(Board board, Cell cellChecked)
        {
            return Areas
                .Where(area => area.ContainsCell(cellChecked))
                .SelectMany(area => area.Coordinates)
                .Select(coordinate => board.GetCell(coordinate.Row, coordinate.Column))
                .Where(cell => cell.CurrentNumber.HasValue)
                .Select(cell => cell.CurrentNumber.Value);
        }
    }
}
