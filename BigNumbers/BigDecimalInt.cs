using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BigNumbers
{
    public sealed class BigDecimalInt : IEquatable<BigDecimalInt>, IComparable<BigDecimalInt>//, INumber<BigDecimalInt>
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

        private static int AbsCompare(BigDecimalInt left, BigDecimalInt right)
            => BigIntContrainer<uint>.AbsCompare(left._contrainer, right._contrainer);

        public bool Equals(BigDecimalInt? other) => CompareTo(other) == 0;

        public int CompareTo(BigDecimalInt? other)
        {
            ArgumentNullException.ThrowIfNull(other);

            Sign r = other._contrainer.Sign;
            Sign l = this._contrainer.Sign;

            if (l == Sign.Positive && r == Sign.Negative)
                return 1;

            if (l == Sign.Negative && r == Sign.Positive)
                return -1;

            int c = AbsCompare(this, other);

            if (r == Sign.Positive)
                return c;

            return -c;
        }

        private BigIntContrainer<uint> _contrainer;
    }
}
