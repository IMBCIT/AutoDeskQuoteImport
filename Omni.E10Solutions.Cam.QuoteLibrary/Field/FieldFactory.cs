using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public static class FieldFactory
    {
        public static Field<string> NewField(int position, string name, string value)
        {
            var fb = new FieldBuilder<string>();
            fb.SetName(name);
            fb.SetValue(value);
            fb.SetPosition(position);
            return fb.BuildField();
        }

        public static Field<string> NewMandatoryField(int position, string name, string value)
        {
            var fb = new FieldBuilder<string>();
            fb.SetName(name);
            fb.SetValue(value);
            fb.SetPosition(position);

            // add fail conditions
            Func<string, bool> hasFailedCondition = s => string.IsNullOrWhiteSpace(s);
            var ex = new FieldCannotBeBlankException(position, name, value);
            fb.AddFailCondition(hasFailedCondition, ex);

            var field = fb.BuildField();
            return field;
        }

        public static Field<decimal?> NewNullableDecimalField(int position, string name, string value)
        {
            return NewNullableDecimalField(position, name, value, decimal.MinValue, decimal.MaxValue);
        }

        public static Field<decimal?> NewNullableDecimalField(int position, string name, string value, decimal min)
        {
            return NewNullableDecimalField(position, name, value, min, decimal.MaxValue);
        }

        public static Field<decimal?> NewNullableDecimalField(int position, string name, string value, decimal min, decimal max)
        {
            var fb = new FieldBuilder<decimal?>();
            fb.SetName(name);
            fb.SetValue(value);
            fb.SetPosition(position);

            // add fail conditions
            if (min != decimal.MinValue)
            {
                if (min != 0)
                {
                    Func<decimal?, bool> hasFailedCondition = d => d.HasValue && d.Value < min;
                    var ex = new FieldCannotBeLessThanValueException(position, name, value, min);
                    fb.AddFailCondition(hasFailedCondition, ex);
                }

                if (min == 0)
                {
                    Func<decimal?, bool> hasFailedCondition = d => d.HasValue && d.Value < min;
                    var ex = new FieldCannotBeLessThanZeroException(position, name, value);
                    fb.AddFailCondition(hasFailedCondition, ex);
                }
            }

            if (max != decimal.MaxValue)
            {
                Func<decimal?, bool> hasFailedCondition = d => d.HasValue && d.Value > max;
                var ex = new FieldCannotBeGreaterThanValueException(position, name, value, max);
                fb.AddFailCondition(hasFailedCondition, ex);
            }

            var field = fb.BuildField();
            return field;
        }

        public static Field<decimal> NewDecimalField(int position, string name, string value)
        {
            return NewDecimalField(position, name, value, decimal.MinValue, decimal.MaxValue);
        }

        public static Field<decimal> NewDecimalField(int position, string name, string value, decimal min)
        {
            return NewDecimalField(position, name, value, min, decimal.MaxValue);
        }

        public static Field<decimal> NewDecimalField(int position, string name, string value, decimal min, decimal max)
        {
            var fb = new FieldBuilder<decimal>();
            fb.SetName(name);
            fb.SetValue(value);
            fb.SetPosition(position);

            // add fail conditions
            if (min != decimal.MinValue)
            {
                if (min != 0)
                {
                    Func<decimal, bool> hasFailedCondition = d => d < min;
                    var ex = new FieldCannotBeLessThanValueException(position, name, value, min);
                    fb.AddFailCondition(hasFailedCondition, ex);
                }

                if (min == 0)
                {
                    Func<decimal, bool> hasFailedCondition = d => d < min;
                    var ex = new FieldCannotBeLessThanZeroException(position, name, value);
                    fb.AddFailCondition(hasFailedCondition, ex);
                }
            }

            if (max != decimal.MaxValue)
            {
                Func<decimal, bool> hasFailedCondition = d => d > max;
                var ex = new FieldCannotBeGreaterThanValueException(position, name, value, max);
                fb.AddFailCondition(hasFailedCondition, ex);
            }

            var field = fb.BuildField();
            return field;
        }

        public static Field<int> NewIntField(int position, string name, string value)
        {
            var fb = new FieldBuilder<int>();
            fb.SetName(name);
            fb.SetValue(value);
            fb.SetPosition(position);
            return fb.BuildField();
        }
    }
}
