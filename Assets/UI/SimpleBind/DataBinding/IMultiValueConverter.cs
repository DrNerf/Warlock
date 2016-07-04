// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using System.Collections.Generic;

namespace SimpleBind.DataBinding
{
    public interface IMultiValueConverter
    {
        object ConvertToTarget(IEnumerable<object> value, Type targetType, object parameter);

        object ConvertToSource(IEnumerable<object> value, Type sourceType, object parameter);
    }
}