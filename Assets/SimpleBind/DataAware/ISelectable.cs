// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_termsusing System;

using System;

namespace SimpleBind.DataAware
{
    public interface ISelectable : IDataAware
    {
        event EventHandler IsSelectedChanged;
        bool IsSelected { get; set; }
    }
}
