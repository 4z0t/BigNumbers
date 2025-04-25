using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigNumbers
{
    public readonly struct BigDecimalFloat
    {
        public const uint DefaultPrecision = 10;


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

        public BigDecimalFloat(BigDecimalInt value, uint precision) :this(value, precision, true)
        {
        }

        private BigDecimalFloat(BigDecimalInt value, uint precision, bool makeShift)
        {
            _value = makeShift ? value.CellsLeftShift(precision) : value;
            _precision = precision;
        }

        public static BigDecimalFloat Parse(string s)
        {
            if (!s.Contains('.'))
            {
                return new BigDecimalFloat(new BigDecimalInt(s), DefaultPrecision, true);
            }



            return new BigDecimalFloat();
        }

        public readonly bool IsZero => _value.IsZero;


        public override string ToString()
        {
            ReadOnlySpan<uint> value = _value.Value;

            StringBuilder builder = new();
            if(_value.Sign == Sign.Negative)
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

                return builder.ToString();
            }

            builder.Append(value[value.Length - 1]);
            for (int i = value.Length - 2; i > _precision; i--)
                builder.Append(value[i].ToString().PadLeft(9, '0'));

            builder.Append('.');

            int zeros = 0;
            bool afterPoint = false;
            for (int i = (int)_precision - 1; i >= 0; i--)
            {
                if(value[i] == 0)
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
            }

            return builder.ToString();
        }


        private readonly BigDecimalInt _value;
        private readonly uint _precision;
    }
}
