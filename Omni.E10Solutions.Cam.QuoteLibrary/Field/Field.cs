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

        protected T _value;
        protected Type _typeOfT;
        protected string _textValue;
        protected bool _isCorrupted = false;
        protected Exception _exception = null;

        public Field(int position, string name, string value)
        {
            Position = position;
            Name = name;
            _textValue = value;
            _typeOfT = typeof(T);
            _value = Convert(value);
        }

        internal Field(int position, string name, string textValue, T value, Exception ex)
        {
            Position = position;
            Name = name;
            _textValue = textValue;
            _value = value;
            _exception = ex;
            if (_exception != null) _isCorrupted = true;
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

    public class CantBeLessThan0Field : Field<decimal?>
    {
        public CantBeLessThan0Field(int position, string name, string value)
            : base(position, name, value)
        {
            if (!_isCorrupted && _value.HasValue && _value.Value < 0)
            {
                _isCorrupted = true;
                _exception = new FieldCannotBeLessThanZeroException(position, name, value);
            }
        }
    }
}
