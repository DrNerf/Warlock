// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using SimpleBind.Common;
using UnityEngine;

namespace SimpleBind.EventBinding
{
    [Serializable]
    public class CallbackBindingEndpoint : Observable<object>
    {
        [SerializeField]
        private ObservableGameObject _object;
        [SerializeField]
        private ObservableString _componentName;
        [SerializeField]
        private ObservableString _path;
        [SerializeField] 
        private ObservableInt _parameterCount; 


        private CallbackAccessor _callbackAccessor = null;

        public CallbackBindingEndpoint()
        {
            Object = new ObservableGameObject();
            ComponentName = new ObservableString();
            Path = new ObservableString();
            ParameterCount = new ObservableInt();

            Object.OnValueChanged += Object_OnValueChanged;
            ComponentName.OnValueChanged += ComponentName_OnValueChanged;
            Path.OnValueChanged += Path_OnValueChanged;
            ParameterCount.OnValueChanged += ParameterCount_OnValueChanged;

        }

        public ObservableGameObject Object
        {
            get { return _object; }
            set { _object = value; }

        }
        public ObservableString ComponentName
        {
            get { return _componentName; }
            set { _componentName = value; }

        }
        public ObservableString Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public ObservableInt ParameterCount
        {
            get {  return _parameterCount; }
            set { _parameterCount = value; }
        }

        public void Invoke(object[] parameters)
        {
            EnsureCallbackAccessor();
            if (_callbackAccessor != null)
            {
                _callbackAccessor.Invoke(parameters);
            }
            else
            {
                Debug.Log("_callbackAccessor is null");
            }
        }

        public Action GetCallBack()
        {
            if (_callbackAccessor != null)
            {
                return _callbackAccessor.Get();
            }

            return () => { };
        }

        public void EnsureInitialized()
        {
            EnsureCallbackAccessor();
        }

        private void EnsureCallbackAccessor()
        {
            if (_callbackAccessor == null)
            {
                Component component = null;

                if (Object != null && !string.IsNullOrEmpty(ComponentName.Value))
                {
                    component = Object.Value.GetComponent(ComponentName.Value);

                    if (component == null)
                        Debug.LogWarning(string.Format("Binding: ComponentName '{0}' not found on '{1}'.", ComponentName.Value, Object.Value));
                }

                if (!string.IsNullOrEmpty(Path.Value))
                {
                    string[] path = Path.Value.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    _callbackAccessor = new CallbackAccessor(component, path, ParameterCount.Value);
                }
            }
        }


        private void Path_OnValueChanged(ValueChangedEventArgs<string> obj)
        {
            ClearMemberAccessor();
        }

        private void ComponentName_OnValueChanged(ValueChangedEventArgs<string> obj)
        {
            ClearMemberAccessor();
        }

        private void Object_OnValueChanged(ValueChangedEventArgs<GameObject> obj)
        {
            ClearMemberAccessor();
        }

        private void ParameterCount_OnValueChanged(ValueChangedEventArgs<int> obj)
        {
            ClearMemberAccessor();
        }

        private void ClearMemberAccessor()
        {
            if (_callbackAccessor != null)
            {
                _callbackAccessor.Dispose();
                _callbackAccessor = null;
            }
        }

        public bool Refresh()
        {
            EnsureCallbackAccessor();
            return _callbackAccessor != null;
        }

        public override void Dispose()
        {
            base.Dispose();

            Object.OnValueChanged -= Object_OnValueChanged;
            ComponentName.OnValueChanged -= ComponentName_OnValueChanged;
            Path.OnValueChanged -= Path_OnValueChanged;

            if (_callbackAccessor != null)
                _callbackAccessor.Dispose();

            Object = null;
            ComponentName = null;
            Path = null;
        }
    }
}
