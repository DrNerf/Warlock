// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using SimpleBind.Common;
using UnityEngine;

namespace SimpleBind.DataBinding
{
    [Serializable]
    public class BindingEndpoint : Observable<object>
    {
        public const string Notset = "None";

        [SerializeField]
        private ObservableGameObject _object;
        [SerializeField]
        private ObservableString _componentName;
        [SerializeField]
        private ObservableString _path;
        [SerializeField]
        private bool _forcePropertyPath;


        private MemberAccessor _memberAccessor = null;

        public BindingEndpoint()
        {
            Object = new ObservableGameObject();
            ComponentName = new ObservableString();
            Path = new ObservableString();

            Object.OnValueChanged += Object_OnValueChanged;
            ComponentName.OnValueChanged += ComponentName_OnValueChanged;
            Path.OnValueChanged += Path_OnValueChanged;
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

        private MemberAccessor MemberAccessor
        {
            get
            {
                if (_memberAccessor == null)
                {
                    if (Object != null)
                    {
                        if (!string.IsNullOrEmpty(ComponentName.Value) && ComponentName.Value != Notset)
                        {
                            _memberAccessor = BuildComponentMemberAccessor();
                        }
                        else
                        {
                            _memberAccessor = BuildObjectMemberAccessor();
                        }
                    }
                }

                return _memberAccessor;
            }
        }

        private MemberAccessor BuildComponentMemberAccessor()
        {
            var component = Object.Value.GetComponent(ComponentName.Value);

            if (component != null)
            {
                if (!string.IsNullOrEmpty(Path.Value))
                {
                    string[] path = Path.Value.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    return new MemberAccessor(component, path);
                }
            }
            else
            {
                Debug.LogWarning(string.Format("Binding: ComponentName '{0}' not found on '{1}'.", ComponentName.Value, Object.Value));
            }

            return null;
        }

        private MemberAccessor BuildObjectMemberAccessor()
        {
            if (Object.Value != null && !string.IsNullOrEmpty(Path.Value))
            {
                string[] path = Path.Value.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                return new MemberAccessor(Object.Value, path);
            }

            return null;
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

        private void ClearMemberAccessor()
        {
            if (_memberAccessor != null)
            {
                _memberAccessor.Dispose();
                _memberAccessor = null;
            }
        }

        public bool Refresh()
        {
            var ma = MemberAccessor;
            if (ma != null)
            {
                Value = ma.Get();
            }
            else
            {
                if (Object.Value != null && !string.IsNullOrEmpty(ComponentName.Value) && ComponentName.Value != Notset)
                {
                    Value = Object.Value.GetComponent(ComponentName.Value);
                }
            }

            return HasChanged;
        }

        public void Update(object value)
        {
            if (MemberAccessor != null)
            {
                MemberAccessor.Set(value);
            }
        }

        public void Update(IList<BindingEndpoint> bindingEndpoints, object converter, object converterParameter, UpdateDirection direction, bool forceUpdate)
        {
            bool hasChanged = bindingEndpoints.Any(endpoint => endpoint.HasChanged) || forceUpdate;

            if (hasChanged && MemberAccessor != null)
            {
                object value = null;

                if (converter == null)
                {
                    if (bindingEndpoints.Count() > 1)
                        Debug.LogError("No IMultiValueConverter provided.");
                    else
                        value = bindingEndpoints.ElementAt(0).Value;
                }
                else
                {
                    if (bindingEndpoints.Count() > 1)
                    {
                        IMultiValueConverter multiValueConverter = converter as IMultiValueConverter;

                        if (multiValueConverter != null)
                        {
                            value = direction == UpdateDirection.ToTarget
                                ? multiValueConverter.ConvertToTarget(bindingEndpoints.Select(e => e.Value), MemberAccessor.MemberType, converterParameter)
                                : multiValueConverter.ConvertToSource(bindingEndpoints.Select(e => e.Value), MemberAccessor.MemberType, converterParameter);
                        }
                    }
                    else
                    {
                        IValueConverter valueConverter = converter as IValueConverter;

                        if (valueConverter != null)
                            value = direction == UpdateDirection.ToTarget
                                ? valueConverter.ConvertToTarget(bindingEndpoints.ElementAt(0).Value, MemberAccessor.MemberType, converterParameter)
                                : valueConverter.ConvertToSource(bindingEndpoints.ElementAt(0).Value, MemberAccessor.MemberType, converterParameter);
                    }
                }

                MemberAccessor.Set(value);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            Object.OnValueChanged -= Object_OnValueChanged;
            ComponentName.OnValueChanged -= ComponentName_OnValueChanged;
            Path.OnValueChanged -= Path_OnValueChanged;

            if (MemberAccessor != null)
                MemberAccessor.Dispose();

            Object = null;
            ComponentName = null;
            Path = null;
        }
    }

    public enum UpdateDirection
    {
        ToTarget,
        ToSource
    }

}
