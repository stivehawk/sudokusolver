using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sudoku Solver");
            Console.WriteLine("Paste your board (0 = empty)");

            // Read 9 lines of input
            List<string> rows = new List<string>();
            for (var i = 0; i < 9; i++)
                rows.Add(Console.ReadLine());

            // Convert lines into a board game
            var board = new Board();
            for (var row = 0; row < 9; row++)
            {
                for (var column = 0; column < 9; column++)
                {
                    char c = rows[row][column];

                    if (char.IsNumber(c))
                    {
                        int value = (int)char.GetNumericValue(c);

                        if(value > 0)
                            board.Cells.FirstOrDefault(x => x.Row == row && x.Column == column).CurrentNumber = (int)char.GetNumericValue(c);
                    }
                }
            }
            
            Console.WriteLine();
            Console.WriteLine("------------------------------");
            Console.WriteLine();

            // Set rules
            BoardRule rule = new BoardRule()
                .ActivateUniqueInsideColumn()
                .ActivateUniqueInsideRow()
                .ActivateUniqueInside9x9Block()
                //.ActivateUniqueOrthogonallyAdjacent()
                .ActivateNonConsecutiveOrthogonallyAdjacent()
                .ActivateUniqueOnKnightStep();
            
            BoardSolver solver = new BoardSolver();
            var results = solver.SolveAll(board, rule);

            foreach (var result in results)
            {
                Console.Write(result.ToString());
                Console.ReadLine();

                Console.WriteLine();
                Console.WriteLine("------------------------------");
                Console.WriteLine();
            }

            Console.Write("End of solutions");

            Console.ReadLine();
        }
    }
}