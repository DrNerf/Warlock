// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using UnityEngine;

namespace SimpleBind.Utilities
{
    [ExecuteInEditMode]
    public class SingleActionOnUpdate : MonoBehaviour
    {
        public Action OnUpdateAction;

        // Use this for initialization
        void Update()
        {
            if (OnUpdateAction != null)
            {
                OnUpdateAction();
            }
        }
    }
}
