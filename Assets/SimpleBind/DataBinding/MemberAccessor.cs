// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using UnityEngine;
using System.Reflection;
using System;
using System.Linq;
using System.ComponentModel;
using UnityEngine.UI;

namespace SimpleBind.DataBinding
{
    public class MemberAccessor : IDisposable
    {
        private static readonly object InvalidTypeConversion = new object();

        public MemberAccessor(object instance, string[] path)
        {
            Path = path;
            ChangeInstance(instance);
        }

        private string[] Path { get; set; }
        private object Instance { get; set; }
        private MemberAccessor Next { get; set; }
        private PropertyInfo Property { get; set; }
        private FieldInfo Field { get; set; }

        public string MemberName
        {
            get
            {
                return Path.FirstOrDefault();
            }
        }

        public Type MemberType
        {
            get
            {
                if (Field == null)
                    return (Property == null) ? null : Property.PropertyType;
                else
                    return Field.FieldType;
            }
        }

        public void ChangeInstance(object instance)
        {
            if (Instance is INotifyPropertyChanged)
                ((INotifyPropertyChanged)Instance).PropertyChanged -= Object_PropertyChanged;

            Instance = instance;

            if (Instance == null)
            {
                Field = null;
                Property = null;

                if (Next != null)
                    Next.ChangeInstance(null);
            }
            else
            {
                if (!string.IsNullOrEmpty(MemberName))
                {
                    Field = Instance.GetType().GetField(MemberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (Field == null)
                        Property = Instance.GetType().GetProperty(MemberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (Field == null && Property == null)
                        Debug.LogWarning(string.Format("Unable to find member named '{0}' on '{1}'.", MemberName, Instance.GetType()));
                }

                if (Path.Length > 1)
                {
                    if (Next != null)
                        Next.Dispose();

                    Next = new MemberAccessor(GetLocal(), Path.Skip(1).ToArray());
                }

                if (Instance is INotifyPropertyChanged)
                    ((INotifyPropertyChanged)Instance).PropertyChanged += Object_PropertyChanged;
            }
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Next != null && string.Compare(e.PropertyName, MemberName, true) == 0)
                Next.ChangeInstance(GetLocal());
        }

        private object GetLocal()
        {
            object value = null;

            if (Instance != null)
            {
                if (Field == null)
                {
                    if (Property != null)
                    {
                        if (Property.CanRead)
                            value = Property.GetValue(Instance, null);
                        else
                            Debug.LogWarning(string.Format("Property '{0}' does not have read access.", Property.Name));
                    }

                }
                else
                    value = Field.GetValue(Instance);
            }

            return value;
        }

        public object Get()
        {
            if (Next != null)
                return Next.Get();
            else
                return GetLocal();
        }

        private void SetLocal(object value)
        {
            if (Instance != null)
            {
                object convertedValue = EnsureTypeConversion(value);

                if (convertedValue == null && Instance is InputField && Path[0].Equals("text"))
                {
                    convertedValue = string.Empty;
                }

                if (object.Equals(convertedValue, InvalidTypeConversion))
                {

                    if (value != null)
                    {
                        Debug.Log(string.Format("Unable to convert '{0}' to '{1}'.", value.GetType(), MemberType));
                    }
                }
                else
                {
                    if (Property != null)
                    {
                        if (Property.CanWrite)
                            Property.SetValue(Instance, convertedValue, null);
                        else
                            Debug.LogWarning(string.Format("Property '{0}' does not have write access.", Property.Name));
                    }
                    else if (Field != null)
                    {
                        Field.SetValue(Instance, convertedValue);
                    }
                }
            }
        }

        private object EnsureTypeConversion(object value)
        {
            if (MemberType != null)
            {
                if (value == null)
                {
                    if (MemberType.IsValueType)
                        return InvalidTypeConversion;
                }
                else if (!MemberType.IsAssignableFrom(value.GetType()))
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(value);

                    if (converter.CanConvertTo(MemberType))
                        return converter.ConvertTo(value, MemberType);
                    else
                        return InvalidTypeConversion;
                }
            }

            return value;
        }

        public void Set(object value)
        {
            if (Next != null)
                Next.Set(value);
            else
                SetLocal(value);
        }

        public void Dispose()
        {
            if (Instance is INotifyPropertyChanged)
                ((INotifyPropertyChanged)Instance).PropertyChanged -= Object_PropertyChanged;

            if (Next != null)
            {
                Next.Dispose();
                Next = null;
            }

            Instance = null;
            Property = null;
            Field = null;
        }
    }
}
