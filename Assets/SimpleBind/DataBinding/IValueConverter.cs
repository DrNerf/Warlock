// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;

namespace SimpleBind.DataBinding
{
	public interface IValueConverter
	{
		object ConvertToTarget(object value, Type targetType, object parameter);
        object ConvertToSource(object value, Type sourceType, object parameter);

    }
}