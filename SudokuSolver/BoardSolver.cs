using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    public class BoardSolver
    {
        public Board Solve(Board board, BoardRule rules)
        {
            return SolveAll(board, rules).FirstOrDefault();
        }

        public IEnumerable<Board> SolveAll(Board board, BoardRule rules)
        {
            return GenerateCompleteBoards(board, rules);
        }

        private IEnumerable<Board> GenerateCompleteBoards(Board board, BoardRule rules)
        {
            if (board.IsComplete())
            {
                yield return board;
            }
            else
            {
                var combinations = board.NextBestBoards(rules);

                foreach (var combination in combinations)
                {
                    var nextCompleteBoards = GenerateCompleteBoards(combination, rules);
                    
                    foreach (var nextCompleteBoard in nextCompleteBoards)
                        yield return nextCompleteBoard;
                }
            }
        }
    }
}
