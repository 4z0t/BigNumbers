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

    internal class BigIntContrainer<T> where T : struct
    {
        public BigIntContrainer():this(1)
        {
        }

        public BigIntContrainer(int capacity)
        {
            _numLen = 1;
            _num = new T[capacity];
            _num[0] = default;
            _sign = Sign.Positive;
        }

        public BigIntContrainer(BigIntContrainer<T> contrainer)
        {
            _numLen = contrainer.Length;
            _num = new T[contrainer.Length];
            Array.Copy(contrainer.NumArray, _num, contrainer.Length);
            _sign = contrainer.Sign;
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
        public T[] NumArray => _num;

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
