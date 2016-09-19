using System;
using System.ComponentModel;

namespace Uniso.InStat.Conv
{
    public class BoolConverter : TypeConverter
    {
        bool[] _keys = null;
        string[] _values = null;

        public BoolConverter()
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (_values == null)
                GetValues(context);

            int i;
            if (value is string)
            {
                var str = ((string)value).ToLower();
                for (i = 0; i < _values.Length; ++i)
                {
                    if (_values[i].ToLower() == str)
                        return _keys[i];
                }

                throw new ArgumentException();
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (_values == null)
                GetValues(context);

            int i;
            if ((destinationType == typeof(string)) && (_values.Length > 0))
            {
                if (value.GetType() == _keys[0].GetType())
                {
                    for (i = 0; i < _keys.Length; ++i)
                    {
                        if (_keys[i].Equals(value))
                            return _values[i];
                    }
                }
                else
                    throw new Exception();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (_values == null)
                GetValues(context);

            return new StandardValuesCollection(_keys);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public void GetValues(ITypeDescriptorContext context)
        {
            _values = new string[] { "Нет", "Да" };
            _keys = new bool[] { false, true };
        }
    }
}