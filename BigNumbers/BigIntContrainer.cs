﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BigNumbers
{
    public enum Sign
    {
        None,
        Positive,
        Negative,
    }

    public static class Utitility
    {
        public static Sign SignOf<T>(T value) where T : INumber<T> => T.IsPositive(value) ? Sign.Positive : Sign.Negative;
        public static Sign InvertSign(Sign sign) => sign == Sign.Positive ? Sign.Negative : Sign.Positive;
        public static Sign MultSigns(Sign left, Sign right) => left == right ? Sign.Positive : Sign.Negative;

    }

    internal readonly struct DivResult
    {
        public readonly uint reminder;
        public readonly uint quotient;

        public DivResult(uint dividend, uint divisor)
        {
            (quotient, reminder) = Math.DivRem(dividend, divisor);
        }
    }

    internal class BigIntContrainer<T> where T : struct, INumber<T>
    {
        public BigIntContrainer(T[] value, Sign sign)
        {
            _sign = sign;
            _numLen = 1;
            for (int i = value.Length - 1; i >= 0; --i)
            {
                if (value[i] != default)
                {
                    _numLen = i + 1;
                    break;
                }
            }
            _num = value;
        }

        public BigIntContrainer(ReadOnlySpan<T> value, Sign sign)
        {
            _sign = sign;
            _numLen = 1;
            for (int i = value.Length - 1; i >= 0; --i)
            {
                if (value[i] != default)
                {
                    _numLen = i + 1;
                    break;
                }
            }
            _num = value.Slice(0, _numLen).ToArray();
        }

        public BigIntContrainer(BigIntContrainer<T> contrainer)
        {
            _sign = contrainer.Sign;
            _numLen = contrainer.Length;
            _num = contrainer._num;
        }

        public int Length => _numLen;
        public int Capacity => _num.Length;
        public Sign Sign => _sign;
        public T this[int index] => _num[index];

        public bool IsZero => Length == 1 && _num[0] == default;

        public static int AbsCompare(BigIntContrainer<T> left, BigIntContrainer<T> right)
        {
            int l = left.Length.CompareTo(right.Length);

            if (l != 0)
                return l;

            for (int i = left.Length - 1; i >= 0; i--)
            {
                int c = left._num[i].CompareTo(right._num[i]);
                if (c != 0)
                {
                    return c;
                }
            }

            return 0;
        }

        public ReadOnlySpan<T> Value => new ReadOnlySpan<T>(_num).Slice(0, _numLen);

        private readonly T[] _num;
        private readonly int _numLen;
        private readonly Sign _sign;
    }
}
