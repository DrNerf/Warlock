// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using UnityEngine;

namespace SimpleBind.DataBinding.Converters
{
    public class QuaternionToStringConverter : IValueConverter
    {
        public object ConvertToTarget(object value, System.Type sourceType, object parameter)
        {
            if (value != null)
            {
                var modelToConvert = (Quaternion)value;
                var rotation = new string[]{ modelToConvert.x.ToString(),
                                        modelToConvert.y.ToString(),
                                        modelToConvert.z.ToString(),
                                        modelToConvert.w.ToString()};
                var rotationString = string.Join(",", rotation);
                return rotationString;
            }

            return null;
        }

        public object ConvertToSource(object value, System.Type targetType, object parameter)
        {
            if (value != null)
            {
                var rotationValues = ((string)value).Split(',');
                return new Quaternion
                {
                    x = float.Parse(rotationValues[0]),
                    y = float.Parse(rotationValues[1]),
                    z = float.Parse(rotationValues[2]),
                    w = float.Parse(rotationValues[3])
                };
            }

            return null;
        }
    }
}