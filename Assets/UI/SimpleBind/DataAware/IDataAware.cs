// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

namespace SimpleBind.DataAware
{
    public interface IDataAware
    {
        void SetData(object data);
        object GetData();
    }
}
