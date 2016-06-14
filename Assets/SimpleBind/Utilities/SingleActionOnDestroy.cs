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
    public class SingleActionOnDestroy : MonoBehaviour
    {
        public Action OnDestroyAction;

        // used to avoid re-entry and multiple calls
        private bool m_actionCalled;

        void OnDestroy()
        {
            if (OnDestroyAction != null && !m_actionCalled)
            {
                m_actionCalled = true;
                OnDestroyAction();
            }
        }
    }
}
