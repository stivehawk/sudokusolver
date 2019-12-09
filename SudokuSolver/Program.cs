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
                
                .ActivateUniqueInside3x3Blocks();
                //.ActivateCustomAreasForUniqueValues(areas =>
                //{
                //    // Default 9 3x3 areas. No need to activate this + UniqueInside3x3Blocks
                //    areas.RegisterAreasByParsingBoard
                //    (
                //        @"AAABBBCCC",
                //        @"AAABBBCCC",
                //        @"AAABBBCCC",
                //        @"DDDEEEFFF",
                //        @"DDDEEEFFF",
                //        @"DDDEEEFFF",
                //        @"GGGHHHIII",
                //        @"GGGHHHIII",
                //        @"GGGHHHIII"
                //    );
                //
                //    // Custom Aad Van De Wetering sudoku - Example of: https://www.youtube.com/watch?v=f5GWiAIZXGI
                //    // areas.RegisterAreasByParsingBoard
                //    // (
                //    //     @"AAABBBCZC",
                //    //     @"ZAABBBCCC",
                //    //     @"AAAABZCCC",
                //    //     @"DDZDBBCFF",
                //    //     @"DDDDZFFFF",
                //    //     @"DDGHHFZFF",
                //    //     @"GGGZHIIII",
                //    //     @"GGGHHHIIZ",
                //    //     @"GZGHHHIII"
                //    // );
                //
                //    /*
                //    areas.CreateUniqueValuesAreaBuilder(0, 0)
                //        .AddCoordinate(1, 1)
                //        .AddCoordinate(2, 2)
                //        .AddCoordinate(3, 3)
                //        .AddCoordinate(4, 4)
                //        .AddCoordinate(5, 5)
                //        .AddCoordinate(6, 6)
                //        .AddCoordinate(7, 7)
                //        .AddCoordinate(8, 8)
                //        .Build();
                //
                //    areas.CreateUniqueValuesAreaBuilder(0, 8)
                //        .AddCoordinate(1, 7)
                //        .AddCoordinate(2, 6)
                //        .AddCoordinate(3, 5)
                //        .AddCoordinate(4, 4)
                //        .AddCoordinate(5, 3)
                //        .AddCoordinate(6, 2)
                //        .AddCoordinate(7, 1)
                //        .AddCoordinate(8, 0)
                //        .Build();
                //        */
                //});

                //.ActivateUniqueOrthogonallyAdjacent()

                // NonConsecutiveOrthogonallyAdjacent + UniqueOnKnightStep - Example of: https://www.youtube.com/watch?v=QNzltTzv0fc
                //.ActivateNonConsecutiveOrthogonallyAdjacent()
                //.ActivateUniqueOnKnightStep();
            
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
            var results = solver.SolveAll(board, rule, null);
            var count = 0;

            foreach (var result in results)
            {
                count++;
                DisplayBoard(result);

                Console.ReadLine();
            }

            Console.Write("End of solutions");

            Console.ReadLine();
        }

        private static void DisplayBoard(Board board)
        {
            Console.Write(board.ToString());
            // Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("------------------------------");
            Console.WriteLine();
        }
    }
}