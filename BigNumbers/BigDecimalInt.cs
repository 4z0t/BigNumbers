using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigNumbers
{
    public sealed class BigDecimalInt
    {
        public const uint Base = 1_000_000_000;

        public BigDecimalInt()
        {
            _contrainer = new BigIntContrainer<uint>();
        }

        public BigDecimalInt(int value)
        {
            DivResult r = new((uint)Math.Abs(value), Base);

            _contrainer = new BigIntContrainer<uint>(2, Utitility.SignOf(value));

            _contrainer.Value[0] = r.reminder;
            _contrainer.Value[1] = r.quotient;

            _contrainer.ResetLength();
        }

        public BigDecimalInt(uint value)
        {
            DivResult r = new(value, Base);

            _contrainer = new BigIntContrainer<uint>(2);

            _contrainer.Value[0] = r.reminder;
            _contrainer.Value[1] = r.quotient;

            _contrainer.ResetLength();
        }

        private BigIntContrainer<uint> _contrainer;
    }
}
