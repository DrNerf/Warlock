﻿// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;

namespace SimpleBind.DataBinding.Converters
{
    public class PrependStringConverter : IValueConverter
    {
        public object ConvertToTarget(object value, Type targetType, object parameter)
        {
            return (value == null) ? "" : (parameter == null ? "" : parameter.ToString()) + value;
        }
        public object ConvertToSource(object value, Type sourceType, object parameter)
        {
            return null;
        }
    }
}