using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BigNumbers
{
    public struct BigDecimalInt : IEquatable<BigDecimalInt>, IComparable<BigDecimalInt>//, INumber<BigDecimalInt>
    {
        public const uint Base = 1_000_000_000;

        public static readonly BigDecimalInt Zero = new BigDecimalInt([0], Sign.Positive);

        public BigDecimalInt()
        {
            _contrainer = Zero._contrainer;
        }

        public BigDecimalInt(int value)
        {
            DivResult r = new((uint)Math.Abs(value), Base);

            _contrainer = new BigIntContrainer<uint>([r.reminder, r.quotient], Utitility.SignOf(value));
        }

        public BigDecimalInt(uint value)
        {
            DivResult r = new(value, Base);

            _contrainer = new BigIntContrainer<uint>([r.reminder, r.quotient], Sign.Positive);
        }

        public BigDecimalInt(BigDecimalInt other)
        {
            _contrainer = new BigIntContrainer<uint>(other._contrainer);
        }

        private BigDecimalInt(uint[] value, Sign sign)
        {
            _contrainer = new BigIntContrainer<uint>(value, sign);
        }

        private readonly BigDecimalInt AbsAdd(BigDecimalInt other)
        {
            int len1 = _contrainer.Length;
            int len2 = other._contrainer.Length;

            uint[] newValue = new uint[Math.Max(len1, len2) + 1];
            BigDecimalInt large = len1 > len2 ? this : other;
            BigDecimalInt small = len1 > len2 ? other : this;

            for (int i = 0; i < large._contrainer.Length; i++)
            {
                uint n1 = large._contrainer[i];

                if (i < small._contrainer.Length)
                {
                    uint n2 = small._contrainer[i];
                    uint s = n1 + n2;

                    if (s == 0)
                        continue;

                    DivResult d = new(s, Base);

                    newValue[i] += d.reminder;
                    newValue[i + 1] += d.quotient;

                    d = new DivResult(newValue[i], Base);

                    newValue[i] = d.reminder;
                    newValue[i + 1] += d.quotient;
                }
                else
                {
                    if (n1 == 0)
                        continue;


                    DivResult d = new(newValue[i] + n1, Base);

                    newValue[i] = d.reminder;
                    newValue[i + 1] += d.quotient;
                }
            }


            return new BigDecimalInt(newValue, _contrainer.Sign);
        }

        private readonly BigDecimalInt AbsSub(BigDecimalInt other)
        {
            int c = AbsCompare(this, other);
            if (c == 0)
            {
                return new BigDecimalInt();
            }

            BigDecimalInt large = c > 0 ? this : other;
            BigDecimalInt small = c > 0 ? other : this;
            uint[] newValue = large._contrainer.CopyData();

            for (int i = 0; i < small._contrainer.Length; i++)
                if (newValue[i] >= small._contrainer[i])
                {
                    newValue[i] -= small._contrainer[i];
                }
                else
                {
                    int j = i + 1;
                    while (newValue[j] == 0)
                    {
                        newValue[j] = Base - 1;
                        j++;
                        if (j == small._contrainer.Length)
                            break;
                    }
                    newValue[j] -= 1;
                   newValue[i] += Base - small._contrainer[i];
                }

            return new BigDecimalInt(newValue, c > 0 ? _contrainer.Sign : Utitility.InvertSign(_contrainer.Sign));
        }

        private readonly BigDecimalInt AbsMult(BigDecimalInt other, Sign sign)
        {
            int len1 = _contrainer.Length;
            int len2 = other._contrainer.Length;
            uint[] newValue = new uint[len1 * len2];

            for (int i = 0; i < len1; i++)
            {
                for (int j = 0; j < len2; j++)
                {
                    if (_contrainer[i] != 0 && other._contrainer[i] != 0)
                    {
                        ulong mult = (ulong)_contrainer[i] * (ulong)other._contrainer[i];
                        var (q, r) = Math.DivRem(mult, Base);

                        newValue[i + j] += (uint)r;
                        newValue[i + j + 1] += (uint)q + (uint)r / Base;
                        newValue[i + j] %= Base;
                    }
                }
            }
            return new BigDecimalInt(newValue, sign);
        }


        public static bool operator ==(BigDecimalInt left, BigDecimalInt right) => left.Equals(right);
        public static bool operator !=(BigDecimalInt left, BigDecimalInt right) => !(left == right);
        public static BigDecimalInt operator +(BigDecimalInt left, BigDecimalInt right)
        {
            if (left._contrainer.Sign == right._contrainer.Sign)
            {
                return left.AbsAdd(right);
            }
            else
            {
                return left.AbsSub(right);
            }
        }

        public static BigDecimalInt operator -(BigDecimalInt left, BigDecimalInt right)
        {
            if (left._contrainer.Sign == right._contrainer.Sign)
            {
                return left.AbsSub(right);
            }
            else
            {
                return left.AbsAdd(right);
            }
        }

        public static BigDecimalInt operator *(BigDecimalInt left, BigDecimalInt right)
            => left.AbsMult(right, Utitility.MultSigns(left._contrainer.Sign, right._contrainer.Sign));

        public override readonly string ToString()
        {
            StringBuilder builder = new(_contrainer.Length + 1);
            if (_contrainer.Sign == Sign.Negative)
                builder.Append('-');

            builder.Append(_contrainer[_contrainer.Length - 1]);
            for (int i = _contrainer.Length - 2; i >= 0; i--)
            {
                builder.Append(_contrainer[i].ToString().PadLeft(9, '0'));
            }

            return builder.ToString();
        }

        private static int AbsCompare(BigDecimalInt left, BigDecimalInt right)
            => BigIntContrainer<uint>.AbsCompare(left._contrainer, right._contrainer);

        public readonly bool Equals(BigDecimalInt other) => CompareTo(other) == 0;

        public readonly int CompareTo(BigDecimalInt other)
        {
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

        public override readonly bool Equals(object obj)
        {
            return obj is BigDecimalInt i && Equals(i);
        }
    }
}
