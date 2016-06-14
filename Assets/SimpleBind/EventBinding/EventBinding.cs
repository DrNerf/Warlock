// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleBind.EventBinding
{
    [Serializable]
    public class EventBinding : IDisposable
    {
        [SerializeField]
        private EventBindingEndpoint _sourceEndpoint = new EventBindingEndpoint();

        [SerializeField]
        private CallbackBindingEndpoint _targetEndpoint = new CallbackBindingEndpoint();

        [SerializeField]
        private List<CallbackParameter> _callbackParameters = new List<CallbackParameter>();

        private bool _isActive;

        public EventBindingEndpoint SourceEndpoint
        {
            get
            {
                return _sourceEndpoint;
            }
        }

        public CallbackBindingEndpoint TargetEndpoint
        {
            get
            {
                return _targetEndpoint;
            }
        }

        public List<CallbackParameter> CallbackParameters
        {
            get {  return _callbackParameters; }
        } 

        public EventBinding()
        {
            _sourceEndpoint.OnEventFired = OnEventFired;
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                _sourceEndpoint.EnsureInitialized();
                _targetEndpoint.EnsureInitialized();
            }
        }

        public void Update()
        {
            _sourceEndpoint.EnsureInitialized();
            _targetEndpoint.EnsureInitialized();
        }

        private void OnEventFired()
        {
            var parameters = BuildParameters();
            _targetEndpoint.Invoke(parameters);
        }

        private object[] BuildParameters()
        {
            object[] parameters = new object[_callbackParameters.Count];

            for (int i = 0; i < _callbackParameters.Count; i++)
            {
                parameters[i] = _callbackParameters[i].GetValue();
            }

            return parameters;
        }

        public void Dispose()
        {
            _sourceEndpoint.Dispose();
            _targetEndpoint.Dispose();
        }
    }
}