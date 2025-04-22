using System;
using System.Collections.Generic;
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
        public static Sign SignOf(int value) => value >= 0 ? Sign.Positive : Sign.Negative;

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
        public BigIntContrainer() : this(1)
        {
        }

        public BigIntContrainer(int capacity, Sign sign = Sign.Positive)
        {
            _sign = sign;
            _numLen = 1;
            _num = new T[capacity];
            _num[0] = default;
        }

        public BigIntContrainer(BigIntContrainer<T> contrainer)
        {
            _sign = contrainer.Sign;
            _numLen = contrainer.Length;
            _num = new T[contrainer.Length];
            Array.Copy(contrainer.Value, _num, contrainer.Length);
        }

        public int Length => _numLen;

        public int Capacity
        {
            get => _num.Length;
            set
            {
                if (_num.Length >= value)
                    return;

                Extend(value);
            }
        }
        public Sign Sign => _sign;
        public T[] Value => _num;

        public bool IsZero => Length == 1 && Value[0] == default;

        public int ResetLength()
        {
            for (int i = Capacity - 1; i >= 0; --i)
            {
                if (_num[i] == default)
                {
                    _numLen = i + 1;
                    break;
                }
            }
            return _numLen;
        }


        private void Extend(int newCapacity)
        {
            if (newCapacity <= Capacity)
                throw new ArgumentException(nameof(newCapacity) + " must be larger than current one");

            T[] num = new T[newCapacity];
            Array.Copy(_num, num, Length);
            _num = num;
        }

        public static int AbsCompare(BigIntContrainer<T> left, BigIntContrainer<T> right)
        {
            int l = left.Length.CompareTo(right.Length);

            if (l != 0)
                return l;

            for (int i = left.Length - 1; i >= 0; i--)
            {
                int c = left.Value[i].CompareTo(right.Value[i]);
                if (c != 0)
                {
                    return c;
                }
            }

            return 0;
        }




        private T[] _num;
        private int _numLen;
        private Sign _sign;
    }
}
