using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigNumbers
{
    public readonly struct BigDecimalFloat : IEquatable<BigDecimalFloat>, IComparable<BigDecimalFloat>
    {
        public const int DefaultPrecision = 10;


        public BigDecimalFloat()
        {
            _value = new BigDecimalInt(0);
            _precision = DefaultPrecision;
        }

        public BigDecimalFloat(BigDecimalFloat other)
        {
            _value = other._value;
            _precision = other._precision;
        }

        public BigDecimalFloat(BigDecimalInt value) : this(value, DefaultPrecision)
        {
        }

        public BigDecimalFloat(string value) : this(Parse(value))
        {
        }

        public BigDecimalFloat(BigDecimalInt integerPart, BigDecimalInt fractionalPart)
            : this(integerPart.CellsLeftShift(fractionalPart.Length) + fractionalPart, fractionalPart.Length, false)
        {
        }

        public BigDecimalFloat(BigDecimalInt value, int precision) : this(value, precision, true)
        {
        }

        private BigDecimalFloat(BigDecimalInt value, int precision, bool makeShift)
        {
            _value = makeShift ? value.CellsLeftShift(precision) : value;
            _precision = precision;
        }

        public static BigDecimalFloat BigIntAsFloat(BigDecimalInt bigInt, int precision = DefaultPrecision)
            => new BigDecimalFloat(bigInt, precision, false);

        public static BigDecimalFloat operator +(BigDecimalFloat left, BigDecimalFloat right)
        {
            int leftP = left._precision;
            int rightP = right._precision;

            switch (leftP.CompareTo(right))
            {
                case 0:
                    return BigIntAsFloat(left._value + right._value, leftP);
                case > 0:
                    return BigIntAsFloat(left._value.CellsRightShift(leftP - rightP) + right._value, rightP);
                case < 0:
                    return BigIntAsFloat(left._value + right._value.CellsRightShift(rightP - leftP), leftP);
            }
        }
        public static BigDecimalFloat operator -(BigDecimalFloat left, BigDecimalFloat right)
        {
            int leftP = left._precision;
            int rightP = right._precision;

            switch (leftP.CompareTo(right))
            {
                case 0:
                    return BigIntAsFloat(left._value - right._value, leftP);
                case > 0:
                    return BigIntAsFloat(left._value.CellsRightShift(leftP - rightP) - right._value, rightP);
                case < 0:
                    return BigIntAsFloat(left._value - right._value.CellsRightShift(rightP - leftP), leftP);
            }
        }

        public static BigDecimalFloat operator *(BigDecimalFloat left, BigDecimalFloat right)
        {
            int leftP = left._precision;
            int rightP = right._precision;
            int p = leftP > rightP ? leftP : rightP;

            return BigIntAsFloat((left._value * right._value).CellsRightShift(p), p);
        }

        public static BigDecimalFloat operator *(BigDecimalFloat left, BigDecimalInt right)
            => BigIntAsFloat(left._value * right, left._precision);

        public static BigDecimalFloat operator /(BigDecimalFloat left, BigDecimalFloat right)
            => BigIntAsFloat(left._value.CellsLeftShift(right._precision) / right._value, left._precision);

        public static BigDecimalFloat operator /(BigDecimalFloat left, BigDecimalInt right)
            => BigIntAsFloat(left._value / right, left._precision);


        public static bool operator ==(BigDecimalFloat left, BigDecimalFloat right) => left.Equals(right);
        public static bool operator !=(BigDecimalFloat left, BigDecimalFloat right) => !(left == right);

        public static BigDecimalFloat Parse(string s)
        {
            if (!s.Contains('.'))
            {
                return new BigDecimalFloat(new BigDecimalInt(s), DefaultPrecision, true);
            }

            string[] ss = s.Split('.', 2);
            int l2 = ss[1].Length;
            if (l2 % 9 != 0)
            {
                ss[1] = ss[1] + new string('0', 9 - l2 % 9);
            }
            return new BigDecimalFloat(new BigDecimalInt(ss[0]), new BigDecimalInt(ss[1]));
        }

        public readonly bool IsZero => _value.IsZero;


        public readonly override string ToString()
        {
            ReadOnlySpan<uint> value = _value.Value;

            StringBuilder builder = new();
            if (_value.Sign == Sign.Negative)
            {
                builder.Append('-');
            }

            if (value.Length <= _precision)
            {
                if (IsZero)
                    return "0.0";

                builder.Append("0.");
                builder.Append(new string('0', 9 * ((int)_precision - value.Length)));

                for (int i = value.Length - 1; i >= 0; i--)
                    builder.Append(value[i].ToString().PadLeft(9, '0'));

                return builder.ToString().TrimEnd('0');
            }

            builder.Append(value[value.Length - 1]);
            for (int i = value.Length - 2; i > _precision; i--)
                builder.Append(value[i].ToString().PadLeft(9, '0'));

            builder.Append('.');

            int zeros = 0;
            bool afterPoint = false;
            for (int i = (int)_precision - 1; i >= 0; i--)
            {
                if (value[i] == 0)
                {
                    zeros++;
                    continue;
                }

                builder.Append(new string('0', 9 * zeros));
                zeros = 0;
                builder.Append(value[i].ToString().PadLeft(9, '0'));
                afterPoint = true;
            }

            if (!afterPoint)
            {
                builder.Append('0');
                return builder.ToString();
            }

            return builder.ToString().TrimEnd('0');
        }

        public bool Equals(BigDecimalFloat other) => CompareTo(other) == 0;

        public int CompareTo(BigDecimalFloat other)
        {
            int c = _precision.CompareTo(other._precision);
            switch (c)
            {
                case 0:
                    return _value.CompareTo(other._value);
                case > 0:
                    return _value.CompareTo(other._value.CellsLeftShift(_precision - other._precision));
                case < 0:
                    return _value.CellsLeftShift(other._precision - _precision).CompareTo(other._value);
            }
        }

        private readonly BigDecimalInt _value;
        private readonly int _precision;
    }
}
