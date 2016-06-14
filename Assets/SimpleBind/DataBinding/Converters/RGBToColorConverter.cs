// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using UnityEngine;
using System.Collections.Generic;

namespace SimpleBind.DataBinding.Converters
{
    public class RGBToColorConverter : IMultiValueConverter
    {
        public object ConvertToTarget(IEnumerable<object> value, Type targetType, object parameter)
        {
            var result = new Color() {a = 255};
            int i = 0;
            foreach (var color in value)
            {
                switch (i)
                {
                    case 0:
                        result.r = (float) (color ?? 0.0f);
                        break;
                    case 1:
                        result.g = (float) (color ?? 0.0f);
                        break;
                    case 2:
                        result.b = (float) (color ?? 0.0f);
                        break;
                }
                i++;
            }

            return result;
        }

        public object ConvertToSource(IEnumerable<object> value, Type sourceType, object parameter)
        {
            return 0.0f;
        }
    }
}
