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
        Negarive,
    }

    public static class Utitility
    {
        public static Sign SignOf(int value) => value >= 0 ? Sign.Positive : Sign.Negarive;

    }


    internal class BigIntContrainer<T> where T : struct, INumber<T>
    {
        public BigIntContrainer():this(1)
        {
        }

        public BigIntContrainer(int capacity, Sign sign = Sign.Positive)
        {
            _sign = sign;
            Length = 1;
            _num = new T[capacity];
            _num[0] = default;
        }

        public BigIntContrainer(BigIntContrainer<T> contrainer)
        {
            _sign = contrainer.Sign;
            Length = contrainer.Length;
            _num = new T[contrainer.Length];
            Array.Copy(contrainer.Value, _num, contrainer.Length);
        }

        public int Length
        {
            get => _numLen;
            set => _numLen = value;
        }

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

        public bool IsZero => Value[Length - 1] == default;

        private void Extend(int newCapacity)
        {
            if (newCapacity <= Capacity)
                throw new ArgumentException(nameof(newCapacity)+" must be larger than current one");

            T[] num = new T[newCapacity];
            Array.Copy(_num, num, Length);
            _num = num;
        }




        private T[] _num;
        private int _numLen;
        private Sign _sign;
    }
}
