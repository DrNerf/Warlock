// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using System.Collections;
using UnityEngine;

namespace SimpleBind.DataAware
{
    public abstract class DataSource : MonoBehaviour, IDataSource
    {
        public event EventHandler<DataSourceChangedEventArgs> DataSourceChanged;

        public abstract IList GetItems(); 

        protected void OnDataSourceChanged()
        {
            var handler = DataSourceChanged;
            if (handler != null)
            {
                handler(this, new DataSourceChangedEventArgs());
            }
        }
    }
}
