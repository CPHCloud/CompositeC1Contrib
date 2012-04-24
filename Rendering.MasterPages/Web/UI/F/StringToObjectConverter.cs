﻿using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace CompositeC1Contrib.Web.UI.F
{
    public class StringToObjectConverter : TypeConverter
    {
        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return true;
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
            if (destinationType == typeof(object))
            {
                return true;
            }

            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value.ToString();
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(object))
            {
                return (object)value;
            }

            if (destinationType == typeof(InstanceDescriptor))
            {
                return new InstanceDescriptor(typeof(ParamObjectConverter).GetConstructor(new Type[] { value.GetType() }), new object[] { value });
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
