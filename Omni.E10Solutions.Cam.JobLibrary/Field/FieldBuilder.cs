using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class FieldBuilder<T>
    {
        int _position;
        string _name;
        string _value;
        List<FieldFailCondition<T>> _failConditions = new List<FieldFailCondition<T>>();

        public FieldBuilder<T> SetPosition(int position)
        {
            _position = position;
            return this;
        }

        public FieldBuilder<T> SetName(string name)
        {
            _name = name;
            return this;
        }

        public FieldBuilder<T> SetValue(string value)
        {
            _value = value;
            return this;
        }

        public FieldBuilder<T> AddFailCondition(Func<T, bool> hasFailedDelegate, Exception failException)
        {
            _failConditions.Add(new FieldFailCondition<T>(hasFailedDelegate, failException));
            return this;
        }

        public Field<T> BuildField()
        {
            Exception ex;
            var convertedValue = Convert(_value, out ex);
            if (ex != null)
                return new Field<T>(_position, _name, _value, convertedValue, ex);

            foreach (var condition in _failConditions)
            {
                if (condition.HasFailed(convertedValue))
                    ex = condition.FailException;
                if (ex != null)
                    return new Field<T>(_position, _name, _value, convertedValue, ex);
            }

            return new Field<T>(_position, _name, _value, convertedValue, ex);
        }

        T Convert(string value, out Exception ex)
        {
            var _typeOfT = typeof(T);
            var underlyingTypeOfT = Nullable.GetUnderlyingType(_typeOfT);
            var isUnderlyingTypeNullable = underlyingTypeOfT != null;

            // under this condition, getting the value is straightforward
            if (isUnderlyingTypeNullable && string.IsNullOrWhiteSpace(value))
            {
                var convertedValue = default(T); // null
                ex = null;
                return convertedValue;
            }

            // otherwise, we have some work to do
            var conversionType = isUnderlyingTypeNullable ? underlyingTypeOfT : _typeOfT;

            try
            {
                // System.Convert.ChangeType throws an exception if the type is nullable.
                var convertedValue = (T)System.Convert.ChangeType(value, conversionType);
                ex = null;
                return convertedValue;
            }
            catch
            {
                var convertedValue = default(T);
                ex = new CannotConvertToTypeException(_position, _name, _value, _typeOfT);
                return convertedValue;
            }
        }
    }
}
