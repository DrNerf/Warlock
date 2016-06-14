// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBind.DataBinding.Converters
{
    public class MultiDoubleToStringConverter : IMultiValueConverter
    {
        public object ConvertToTarget(IEnumerable<object> value, Type targetType, object parameter)
        {
            if (value != null && value.Count() > 1)
            {
                var x = value.ElementAt(0);
                var y = value.ElementAt(1);
                var sum = (double)x * (double)y;

                for (var i = 2; i < value.Count(); i++)
                {
                    sum = sum * (double)value.ElementAt(i);
                }

                return sum;
            }

            return null;
        }

        public object ConvertToSource(IEnumerable<object> value, Type sourceType, object parameter)
        {
            return null;
        }
    }
}