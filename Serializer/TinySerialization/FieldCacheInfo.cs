﻿using System;
using System.Reflection;

namespace Expanse.TinySerialization
{
    public class FieldCacheInfo
    {
        public SupportedFieldType type;
        public FieldInfo fieldInfo;
        public object getter;
        public object setter;

        public FieldCacheInfo(FieldInfo fieldInfo)
        {
            this.fieldInfo = fieldInfo;

            Type fieldType = fieldInfo.FieldType;

            if (fieldType == typeof(int))
                type = SupportedFieldType.INT;
            else if (fieldType == typeof(bool))
                type = SupportedFieldType.BOOL;
            else if (fieldType == typeof(float))
                type = SupportedFieldType.FLOAT;
            else if (fieldType == typeof(double))
                type = SupportedFieldType.DOUBLE;
            else if (fieldType == typeof(char))
                type = SupportedFieldType.CHAR;
            else if (fieldType == typeof(string))
                type = SupportedFieldType.STRING;
            else if (fieldType == typeof(DateTime))
                type = SupportedFieldType.DATE_TIME;
            else if (fieldType == typeof(short))
                type = SupportedFieldType.SHORT;
            else if (fieldType == typeof(long))
                type = SupportedFieldType.LONG;
            else if (fieldType == typeof(uint))
                type = SupportedFieldType.UINT;
            else if (fieldType == typeof(ushort))
                type = SupportedFieldType.USHORT;
            else if (fieldType == typeof(ulong))
                type = SupportedFieldType.ULONG;
            else if (fieldType == typeof(byte))
                type = SupportedFieldType.BYTE;
            else if (fieldType == typeof(sbyte))
                type = SupportedFieldType.SBYTE;
            else if (fieldType == typeof(decimal))
                type = SupportedFieldType.DECIMAL;
            else
                type = SupportedFieldType.NONE;
        }

        public TReturn GetValue<TSource, TReturn>(TSource source)
        {
            Func<TSource, TReturn> getter = this.getter as Func<TSource, TReturn>;

            if (getter == null)
            {
                getter = EmitUtil.GenerateGetter<TSource, TReturn>(fieldInfo);
                this.getter = getter;
            }

            return getter(source);
        }

        public void SetValue<TSource, TValue>(TSource source, TValue value)
        {
            Action<TSource, TValue> setter = this.setter as Action<TSource, TValue>;

            if (setter == null)
            {
                setter = EmitUtil.GenerateSetter<TSource, TValue>(fieldInfo);
                this.setter = setter;
            }

            setter(source, value);
        }
    }
}
