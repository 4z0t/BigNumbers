using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigNumbers
{
    public sealed class BigDecimalInt
    {
        public const int Base = 1_000_000_000;

        public BigDecimalInt()
        {
            _contrainer = new BigIntContrainer<int>();
        }

        public BigDecimalInt(int value)
        {
            Sign s = Utitility.SignOf(value);
            DivResult r = new(Math.Abs(value), Base);
            if(r.quotient == 0)
            {
                _contrainer = new BigIntContrainer<int>(1, s);
                _contrainer.Value[0] = r.reminder;
            }
            else
            {
                _contrainer = new BigIntContrainer<int>(2, s)
                {
                    Length = 2
                };

                _contrainer.Value[0] = r.reminder;
                _contrainer.Value[1] = r.quotient;
            }
        }


        private readonly struct DivResult
        {
            public readonly int reminder;
            public readonly int quotient;

            public DivResult(int dividend, int divisor)
            {
                quotient = Math.DivRem(dividend, divisor, out reminder);
            }
        }

        private BigIntContrainer<int> _contrainer;
    }
}
