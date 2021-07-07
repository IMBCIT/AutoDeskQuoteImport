using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class Field<T> : IField
    {
        public int Position { get; protected set; }
        public string Name { get; protected set; }

        protected T _value;
        protected string _textValue;
        protected bool _isCorrupted = false;
        protected Exception _exception = null;

        internal Field(int position, string name, string textValue, T value, Exception ex)
        {
            Position = position;
            Name = name;
            _textValue = textValue;
            _value = value;
            _exception = ex;
            if (_exception != null) _isCorrupted = true;
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
