// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using SimpleBind.Common;
using UnityEngine;

namespace SimpleBind.DataBinding
{
    public class BindingContext : Inheritable<object>
    {
        [SerializeField]
        private BindingEndpoint _valueSourceEndpoint;

        private void Update()
        {
            if (_valueSourceEndpoint.Refresh())
            {
                _valueSourceEndpoint.Commit();
                Value = _valueSourceEndpoint.Value;
            }
            else if (IsInheriting && !string.IsNullOrEmpty(_valueSourceEndpoint.Path.Value))
            {
                Value = null;
            }
        }

        private void Reset()
        {
            Debug.Log("Reset");

            if (_valueSourceEndpoint != null)
            {
                _valueSourceEndpoint.OnValueChanged -= ValueSourceChanged;
                _valueSourceEndpoint = null;
            }

            _valueSourceEndpoint = new BindingEndpoint();
            _valueSourceEndpoint.OnValueChanged += ValueSourceChanged;

        }

        private void ValueSourceChanged(ValueChangedEventArgs<object> obj)
        {
            Value = _valueSourceEndpoint.Value;
        }
    }
}
