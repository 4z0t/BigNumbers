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

        public BigDecimalInt(BigDecimalInt other)
        {
            _contrainer = new BigIntContrainer<uint>(other._contrainer);
        }

        private BigDecimalInt(int capacity, Sign sign)
        {
            _contrainer = new BigIntContrainer<uint>(capacity, sign);
        }

        private BigDecimalInt AbsAdd(BigDecimalInt other, Sign sign)
        {
            int len1 = _contrainer.Length;
            int len2 = other._contrainer.Length;

            BigDecimalInt result = new BigDecimalInt(Math.Max(len1, len2) + 1, sign);
            BigDecimalInt large = len1 > len2 ? this : other;
            BigDecimalInt small = len1 > len2 ? other : this;

            for (int i = 0; i < large._contrainer.Length; i++)
            {
                uint n1 = large._contrainer.Value[i];

                if (i < small._contrainer.Length)
                {
                    uint n2 = small._contrainer.Value[i];
                    uint s = n1 + n2;

                    if (s == 0)
                        continue;

                    DivResult d = new(s, Base);

                    result._contrainer.Value[i] += d.reminder;
                    result._contrainer.Value[i + 1] += d.quotient;

                    d = new DivResult(result._contrainer.Value[i], Base);

                    result._contrainer.Value[i] = d.reminder;
                    result._contrainer.Value[i + 1] += d.quotient;
                }
                else
                {
                    if (n1 == 0)
                        continue;


                    DivResult d = new(result._contrainer.Value[i] + n1, Base);

                    result._contrainer.Value[i] = d.reminder;
                    result._contrainer.Value[i + 1] += d.quotient;
                }
            }


            result._contrainer.ResetLength();
            return result;
        }

        private BigDecimalInt AbsSub(BigDecimalInt other, Sign sign)
        {
            int len1 = _contrainer.Length;
            int len2 = other._contrainer.Length;

            int c = AbsCompare(this, other);
            if (c == 0)
            {
                return new BigDecimalInt();
            }

            BigDecimalInt large = c > 0 ? this : other;
            BigDecimalInt small = c > 0 ? other : this;
            BigDecimalInt result = new(large);

            for (int i = 0; i < small._contrainer.Length; i++)
                if (result._contrainer.Value[i] >= small._contrainer.Value[i])
                {
                    result._contrainer.Value[i] -= small._contrainer.Value[i];
                }
                else
                {
                    int j = i + 1;
                    while (result._contrainer.Value[j] == 0)
                    {
                        result._contrainer.Value[j] = Base - 1;
                        j++;
                        if (j == small._contrainer.Length)
                            break;
                    }
                    result._contrainer.Value[j] -= 1;
                    result._contrainer.Value[i] += Base - small._contrainer.Value[i];
                }

            result._contrainer.ResetLength();
            return result;
        }

        private static int AbsCompare(BigDecimalInt left, BigDecimalInt right)
            => BigIntContrainer<uint>.AbsCompare(left._contrainer, right._contrainer);

        public bool Equals(BigDecimalInt? other) => CompareTo(other) == 0;

        public int CompareTo(BigDecimalInt? other)
        {
            ArgumentNullException.ThrowIfNull(other);

            Sign r = other._contrainer.Sign;
            Sign l = _contrainer.Sign;

            if (l == Sign.Positive && r == Sign.Negative)
                return 1;

            if (l == Sign.Negative && r == Sign.Positive)
                return -1;

            int c = AbsCompare(this, other);

            return r == Sign.Positive ? c : -c;
        }

        private BigIntContrainer<uint> _contrainer;
    }
}
