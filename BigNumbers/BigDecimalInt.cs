using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BigNumbers
{
    public readonly struct BigDecimalInt : IEquatable<BigDecimalInt>, IComparable<BigDecimalInt>//, INumber<BigDecimalInt>
    {
        public static uint Pow10(int n)
        {
            uint result = 1;
            for (int i = 0; i < n; i++)
                result *= 10;
            return result;
        }

        private static int NumLen(uint n)
        {
            int length = 0;
            while (n != 0)
            {
                n /= 10;
                length++;
            }
            return length;
        }

        public const uint Base = 1_000_000_000;

        public static readonly BigDecimalInt Zero = new BigDecimalInt([0], Sign.Positive);
        public static readonly BigDecimalInt One = new BigDecimalInt([1], Sign.Positive);

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

        public BigDecimalInt(long value)
        {
            long q, r1, r2, r3;
            (q, r1) = Math.DivRem(Math.Abs(value), Base);
            (q, r2) = Math.DivRem(q, Base);
            (q, r3) = Math.DivRem(q, Base);

            _contrainer = new BigIntContrainer<uint>([(uint)r1, (uint)r2, (uint)r3], Utitility.SignOf(value));
        }

        public BigDecimalInt(BigDecimalInt other)
        {
            _contrainer = new BigIntContrainer<uint>(other._contrainer);
        }

        public BigDecimalInt(string value) : this(Parse(value))
        {
        }

        private BigDecimalInt(uint[] value, Sign sign)
        {
            _contrainer = new BigIntContrainer<uint>(value, sign);
        }

        private BigDecimalInt(ReadOnlySpan<uint> value, Sign sign)
        {
            _contrainer = new BigIntContrainer<uint>(value, sign);
        }

        public readonly bool IsZero => _contrainer.IsZero;
        public readonly ReadOnlySpan<uint> Value => _contrainer.Value;
        public readonly Sign Sign => _contrainer.Sign;
        public readonly int Length => _contrainer.Length;

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
                return Zero;
            }

            BigDecimalInt large = c > 0 ? this : other;
            BigDecimalInt small = c > 0 ? other : this;
            uint[] newValue = large._contrainer.Value.ToArray();

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
            if (_contrainer.IsZero || other._contrainer.IsZero)
                return Zero;

            int len1 = _contrainer.Length;
            int len2 = other._contrainer.Length;
            uint[] newValue = new uint[len1 + len2];

            for (int i = 0; i < len1; i++)
            {
                for (int j = 0; j < len2; j++)
                {
                    if (_contrainer[i] != 0 && other._contrainer[j] != 0)
                    {
                        ulong mult = (ulong)_contrainer[i] * (ulong)other._contrainer[j];
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
        public static bool operator >(BigDecimalInt left, BigDecimalInt right) => left.CompareTo(right) == 1;
        public static bool operator >=(BigDecimalInt left, BigDecimalInt right) => left.CompareTo(right) >= 0;
        public static bool operator <(BigDecimalInt left, BigDecimalInt right) => left.CompareTo(right) == -1;
        public static bool operator <=(BigDecimalInt left, BigDecimalInt right) => left.CompareTo(right) <= 0;

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

        public static BigDecimalInt operator -(BigDecimalInt value)
            => new BigDecimalInt(value._contrainer.Value, Utitility.InvertSign(value._contrainer.Sign));

        public (BigDecimalInt Quotiont, BigDecimalInt Reminder) DivRem(BigDecimalInt other)
        {
            if (other._contrainer.IsZero)
                throw new DivideByZeroException();

            BigDecimalInt numerator = Abs(this);

            BigDecimalInt result = Zero;

            while (true)
            {
                BigDecimalInt divider = Abs(other);
                if (divider > numerator)
                    break;

                BigDecimalInt step = One;
                int dist = numerator.Distance(divider);
                while (numerator >= divider.DecimalLeftShift(dist))
                {
                    dist++;
                }

                divider = divider.DecimalLeftShift(dist - 1);
                step = step.DecimalLeftShift(dist - 1);

                while (numerator >= divider)
                {
                    numerator -= divider;
                    result += step;
                }
            }

            return (_contrainer.Sign == other._contrainer.Sign ? result : -result, numerator);
        }

        public static BigDecimalInt operator /(BigDecimalInt left, BigDecimalInt right)
        {
            var (q, r) = left.DivRem(right);
            return q;
        }
        public static BigDecimalInt operator %(BigDecimalInt left, BigDecimalInt right)
        {
            var (q, r) = left.DivRem(right);
            return r;
        }

        public readonly BigDecimalInt DecimalLeftShift(int times)
        {
            var (q, r) = Math.DivRem(times, 9);
            uint ls = Pow10(9 - r);
            uint rs = Pow10(r);

            uint[] newValue = new uint[_contrainer.Length + q + 1];

            for (int i = 0; i < _contrainer.Length; i++)
            {
                newValue[i + q] += (_contrainer[i] % ls) * rs;
                newValue[i + q + 1] += _contrainer[i] / ls;
            }

            return new BigDecimalInt(newValue, _contrainer.Sign);
        }

        public readonly BigDecimalInt CellsRightShift(int cells)
        {
            if (cells == 0)
                return this;

            if (_contrainer.Length <= cells)
                return Zero;

            uint[] newValue = new uint[_contrainer.Length - cells];

            _contrainer.Value.Slice(cells).CopyTo(newValue);

            return new BigDecimalInt(newValue, _contrainer.Sign);
        }

        public readonly BigDecimalInt CellsLeftShift(int cells)
        {
            if (cells == 0)
                return this;

            uint[] newValue = new uint[_contrainer.Length + cells];

            _contrainer.Value.CopyTo(newValue.AsSpan().Slice(cells));

            return new BigDecimalInt(newValue, _contrainer.Sign);
        }

        public readonly int Distance(BigDecimalInt other)
            => (_contrainer.Length - other._contrainer.Length) * 9 + NumLen(_contrainer[_contrainer.Length - 1]) - NumLen(other._contrainer[other._contrainer.Length - 1]);


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

        public static bool TryParse(string s, out BigDecimalInt result)
        {
            result = Zero;
            return false;
        }

        public static BigDecimalInt Parse(string s)
        {
            BigDecimalInt v = s.StartsWith('-')
                ? new BigDecimalInt(ParseStringToValueArray(s.Substring(1)), Sign.Negative)
                : new BigDecimalInt(ParseStringToValueArray(s), Sign.Positive);

            return v.IsZero ? Zero : v;
        }

        private static uint[] ParseStringToValueArray(string s)
        {
            Regex regex = new Regex(@"^\d+$");
            if (!regex.IsMatch(s))
                throw new Exception();

            int length = s.Length;
            List<uint> chunks = new List<uint>(length / 9 + 1);

            for (int i = length; i > 0; i -= 9)
            {
                int start = Math.Max(0, i - 9);
                int count = i - start;
                chunks.Add(uint.Parse(s.Substring(start, count)));
            }

            return chunks.ToArray();
        }

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

        public override readonly bool Equals(object obj)
        {
            return obj is BigDecimalInt i && Equals(i);
        }

        public static BigDecimalInt Abs(BigDecimalInt value)
            => value._contrainer.Sign == Sign.Positive
            ? value
            : new BigDecimalInt(value._contrainer.Value, Sign.Positive);


        private readonly BigIntContrainer<uint> _contrainer;
    }
}
