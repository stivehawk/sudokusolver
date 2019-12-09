using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver
{
    public class ValuesPool
    {
        public bool Number1 = true;
        public bool Number2 = true;
        public bool Number3 = true;
        public bool Number4 = true;
        public bool Number5 = true;
        public bool Number6 = true;
        public bool Number7 = true;
        public bool Number8 = true;
        public bool Number9 = true;

        public ValuesPool MakeUnavailable(int value)
        {
            switch (value)
            {
                case 1: if (Number1) Number1 = false; break;
                case 2: if (Number2) Number2 = false; break;
                case 3: if (Number3) Number3 = false; break;
                case 4: if (Number4) Number4 = false; break;
                case 5: if (Number5) Number5 = false; break;
                case 6: if (Number6) Number6 = false; break;
                case 7: if (Number7) Number7 = false; break;
                case 8: if (Number8) Number8 = false; break;
                case 9: if (Number9) Number9 = false; break;
            }

            return this;
        }

        public IEnumerable<int> GetAvailableValues()
        {
            if (Number1) yield return 1;
            if (Number2) yield return 2;
            if (Number3) yield return 3;
            if (Number4) yield return 4;
            if (Number5) yield return 5;
            if (Number6) yield return 6;
            if (Number7) yield return 7;
            if (Number8) yield return 8;
            if (Number9) yield return 9;
        }

        public ValuesPool MakeUnavailableIfBiggerThan(int value)
        {
            while (value++ <= 9)
                MakeUnavailable(value);

            return this;
        }

        public ValuesPool MakeUnavailableIfSmallerThan(int value)
        {
            while (value-- >= 1)
                MakeUnavailable(value);

            return this;
        }

        public ValuesPool CreateReverse()
        {
            return new ValuesPool()
            {
                Number1 = !Number1,
                Number2 = !Number2,
                Number3 = !Number3,
                Number4 = !Number4,
                Number5 = !Number5,
                Number6 = !Number6,
                Number7 = !Number7,
                Number8 = !Number8,
                Number9 = !Number9
            };
        }
    }
}
