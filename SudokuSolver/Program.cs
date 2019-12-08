using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                .ActivateUniqueOrthogonallyAdjacent()

                // NonConsecutiveOrthogonallyAdjacent + UniqueOnKnightStep - Example of: https://www.youtube.com/watch?v=QNzltTzv0fc
                .ActivateNonConsecutiveOrthogonallyAdjacent()
                .ActivateUniqueOnKnightStep();
            
                // Thermometers - Example of: https://www.youtube.com/watch?v=KTth49YrQVU
                //.ActivateThermometers(set =>
                //{
                //    set.CreateThermometerBuilder(3, 3)
                //        .AddSequence(2, 3)
                //        .AddSequence(1, 3)
                //        .AddSequence(0, 3)
                //        .AddSequence(0, 2)
                //        .AddSequence(0, 1)
                //        .Build();
                //
                //    set.CreateThermometerBuilder(3, 3)
                //        .AddSequence(3, 2)
                //        .AddSequence(3, 1)
                //        .AddSequence(3, 0)
                //        .AddSequence(2, 0)
                //        .AddSequence(1, 0)
                //        .AddSequence(0, 0)
                //        .Build();
                //
                //    set.CreateThermometerBuilder(0, 5)
                //        .AddSequence(1, 5)
                //        .Build();
                //
                //    set.CreateThermometerBuilder(0, 6)
                //        .AddSequence(0, 7)
                //        .AddSequence(1, 7)
                //        .AddSequence(2, 7)
                //        .AddSequence(2, 6)
                //        .AddSequence(2, 5)
                //        .Build();
                //
                //    set.CreateThermometerBuilder(4, 4)
                //        .AddSequence(4, 5)
                //        .AddSequence(4, 6)
                //        .AddSequence(4, 7)
                //        .AddSequence(4, 8)
                //        .Build();
                //
                //    set.CreateThermometerBuilder(4, 4)
                //        .AddSequence(5, 4)
                //        .AddSequence(6, 4)
                //        .AddSequence(7, 4)
                //        .Build();
                //
                //    set.CreateThermometerBuilder(6, 2)
                //        .AddSequence(5, 2)
                //        .Build();
                //
                //    set.CreateThermometerBuilder(7, 1)
                //        .AddSequence(7, 2)
                //        .Build();
                //
                //    set.CreateThermometerBuilder(7, 0)
                //        .AddSequence(6, 0)
                //        .AddSequence(5, 0)
                //        .AddSequence(5, 1)
                //        .Build();
                //
                //    set.CreateThermometerBuilder(7, 8)
                //        .AddSequence(6, 8)
                //        .AddSequence(5, 8)
                //        .Build();
                //
                //    set.CreateThermometerBuilder(7, 8)
                //        .AddSequence(8, 8)
                //        .AddSequence(8, 7)
                //        .AddSequence(8, 6)
                //        .AddSequence(8, 5)
                //        .AddSequence(8, 4)
                //        .Build();
                //});

            /*
            Console.Write(rule.Thermometers);
            Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("------------------------------");
            Console.WriteLine();
            */

            BoardSolver solver = new BoardSolver();
            var results = solver.SolveAll(board, rule);
            var count = 0;

            foreach (var result in results)
            {
                count++;
                Console.Write(result.ToString());
                // Console.ReadLine();

                Console.WriteLine();
                Console.WriteLine("------------------------------");
                Console.WriteLine();
            }

            Console.Write("End of solutions");

            Console.ReadLine();
        }
    }
}