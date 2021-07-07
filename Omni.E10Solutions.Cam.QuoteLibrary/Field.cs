using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class Field<T> : IField
    {
        public int Position { get; protected set; }
        public string Name { get; protected set; }

        T _value;
        Type _typeOfT;
        string _textValue;
        bool _isCorrupted = false;
        Exception _exception = null;

        public Field(int position, string name, string value)
        {
            Position = position;
            Name = name;
            _textValue = value;
            _typeOfT = typeof(T);

            try
            {
                _value = (T)Convert.ChangeType(value, _typeOfT);
                _isCorrupted = true;
            }
            catch
            {
                _value = default(T);
                _isCorrupted = false;
                _exception = new CannotConvertToTypeException(Position, Name, _textValue, _typeOfT);
            }
        }

        public T ToValue()
        {
            return _value;
        }

        public bool IsCorrupted()
        {
            return _isCorrupted;
        }

        public Exception GetCorruptionException()
        {
            return _exception;
        }

        public override string ToString()
        {
            return _textValue;
        }
    }

    public interface IField
    {
        int Position { get; }
        string Name { get; }
        string ToString();
        bool IsCorrupted();
        Exception GetCorruptionException();
    }
}
