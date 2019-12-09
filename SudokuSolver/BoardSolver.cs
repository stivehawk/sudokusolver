using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    public class BoardSolver
    {
        public Board Solve(Board board, BoardRule rules, Action<Board> onStep = null)
        {
            return SolveAll(board, rules, onStep).FirstOrDefault();
        }

        public IEnumerable<Board> SolveAll(Board board, BoardRule rules, Action<Board> onStep = null)
        {
            return GenerateCompleteBoards(board, rules, onStep);
        }

        private IEnumerable<Board> GenerateCompleteBoards(Board board, BoardRule rules, Action<Board> onStep = null)
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
                    onStep?.Invoke(combination);

                    var nextCompleteBoards = GenerateCompleteBoards(combination, rules, onStep);
                    
                    foreach (var nextCompleteBoard in nextCompleteBoards)
                        yield return nextCompleteBoard;
                }
            }
        }
    }
}
