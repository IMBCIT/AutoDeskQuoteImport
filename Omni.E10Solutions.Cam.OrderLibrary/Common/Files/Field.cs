using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.OrderLibrary
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
            _value = Convert(value);
        }

        T Convert(string value)
        {
            var underlyingTypeOfT = Nullable.GetUnderlyingType(_typeOfT);
            var isUnderlyingTypeNullable = underlyingTypeOfT != null;

            // under this condition, getting the value is straightforward
            if (isUnderlyingTypeNullable && string.IsNullOrWhiteSpace(value))
            {
                var convertedValue = default(T); // null
                _isCorrupted = false;
                return convertedValue;
            }

            // otherwise, we have some work to do
            var conversionType = isUnderlyingTypeNullable ? underlyingTypeOfT : _typeOfT;

            try
            {
                // System.Convert.ChangeType throws an exception if the type is nullable.
                var convertedValue = (T)System.Convert.ChangeType(value, conversionType);
                _isCorrupted = false;
                return convertedValue;
            }
            catch
            {
                var convertedValue = default(T);
                _isCorrupted = true;
                _exception = new CannotConvertToTypeException(Position, Name, _textValue, _typeOfT);
                return convertedValue;
            }
        }

        public T GetValue()
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

        public string GetTextValue()
        {
            return _textValue;
        }
    }
}
