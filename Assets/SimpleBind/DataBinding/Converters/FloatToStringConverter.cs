// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System.Diagnostics;

namespace SimpleBind.DataBinding.Converters
{
    public class FloatToStringConverter : IValueConverter
    {
        public object ConvertToTarget(object value, System.Type targetType, object parameter)
        {
            if (value != null)
            {
                return value.ToString();
            }

            return string.Empty;
        }

        public object ConvertToSource(object value, System.Type sourceType, object parameter)
        {
            if (value != null)
            {
                float result = 0.0f;
                if (!float.TryParse(value.ToString(), out result))
                {
                    Debug.WriteLine("Unable to parse float value from: " + value);
                }
                return result;
            }

            return 0.0f;
        }
    }
}
