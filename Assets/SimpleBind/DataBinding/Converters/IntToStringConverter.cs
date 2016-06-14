﻿// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

namespace SimpleBind.DataBinding.Converters
{
    public class IntToStringConverter : IValueConverter
    {
        public object ConvertToTarget(object value, System.Type targetType, object parameter)
        {
            if (value != null)
            {
                return ((int)value).ToString();
            }

            return null;
        }

        public object ConvertToSource(object value, System.Type sourceType, object parameter)
        {
            int result = 0;
            if (value != null)
            {
                int.TryParse(value.ToString(), out result);
            }

            return result;
        }
    }
}