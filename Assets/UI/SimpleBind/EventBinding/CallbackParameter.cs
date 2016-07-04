// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using SimpleBind.DataBinding;
using UnityEngine;

namespace SimpleBind.EventBinding
{
    [Serializable]
    public class CallbackParameter : IDisposable
    {
        [SerializeField]
        private bool _isStatic;

        [SerializeField]
        private string _staticValue;

        [SerializeField]
        private BindingEndpoint _binding;

        public bool IsStatic
        {
            get { return _isStatic; }
            set { _isStatic = value; }
        }

        public string StaticValue
        {
            get { return _staticValue; }
            set { _staticValue = value; }
        }

        public BindingEndpoint Binding
        {
            get { return _binding; }
            set { _binding = value; }
        }

        public void Dispose()
        {
            if (Binding != null)
            {
                Binding.Dispose();
            }
        }

        public object GetValue()
        {
            if (IsStatic)
            {
                return StaticValue;
            }

            if (Binding != null)
            {
                Binding.Refresh();
                return Binding.Value;
            }

            return null;
        }
    }
}
