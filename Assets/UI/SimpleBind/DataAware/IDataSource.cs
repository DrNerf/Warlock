// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using System.Collections;

namespace SimpleBind.DataAware
{
    public interface IDataSource
    {
        event EventHandler<DataSourceChangedEventArgs> DataSourceChanged;

        IList GetItems();
    }
}