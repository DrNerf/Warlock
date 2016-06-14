// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System.ComponentModel;
using UnityEngine;

namespace SimpleBind.DataBinding
{
    public class Inheritable<T> : MonoBehaviour, INotifyPropertyChanged
    {
        private static string ValuePropertyName = "Value";

        public event PropertyChangedEventHandler PropertyChanged;

        private T _value;

        // This is used to track the actual location in the hiearchy and when changed re-evaluate the Inheritable Parent
        private Transform _currentTransformParent;

        private Binding Binding { get; set; }
        public Inheritable<T> Parent { get; private set; }

        public bool IsInheriting
        {
            get
            {
                return Parent != null && (Binding != null && Binding.IsActive);
            }
            set
            {
                if (value)
                    ActivateBinding();
                else
                    DeactivateBinding();
            }
        }

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (Binding != null)
                {
                    if (!Binding.SourceEndpoints[0].HasChanged)
                        DeactivateBinding();
                }

                if (!Equals(_value, value))
                {
                    _value = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private void Start()
        {
            _currentTransformParent = gameObject.transform.parent;
            if (_value == null)
            {
                ActivateBinding();
            }
            else
            {
                Parent = FindFirstParent();
            }
        }

        private void Update()
        {
            // If the hiearchy parent has changed then create a new binding
            if (_currentTransformParent != gameObject.transform.parent)
            {
                _currentTransformParent = gameObject.transform.parent;
                DeactivateBinding();
                Value = default(T);
                ActivateBinding();
            }
            else if (Binding != null)
            {
                Binding.Update();
            }
        }

        private void OnDestroy()
        {
            DeactivateBinding();

            Parent = null;
            Value = default(T);
        }

        private void ActivateBinding()
        {
            Parent = FindFirstParent();

            if (Parent != null && Binding == null)
            {
                Binding = new Binding();

                BindingEndpoint source = new BindingEndpoint();
                source.Object.Value = Parent.gameObject;
                source.ComponentName.Value = this.GetType().Name;
                source.Path.Value = ValuePropertyName;

                Binding.SourceEndpoints.Add(source);

                BindingEndpoint target = new BindingEndpoint();
                target.Object.Value = this.gameObject;
                target.ComponentName.Value = this.GetType().Name;
                target.Path.Value = ValuePropertyName;

                Binding.TargetEndpoints.Add(target);

                Binding.IsActive = true;
            }
        }

        private void DeactivateBinding()
        {
            if (Binding != null)
            {
                Binding.Dispose();
                Binding = null;
            }
        }

        private Inheritable<T> FindFirstParent()
        {
            return gameObject != null && gameObject.transform.parent != null ? gameObject.transform.parent.GetComponentInParent(typeof(Inheritable<T>)) as Inheritable<T> : default(Inheritable<T>);
        }

        private void NotifyPropertyChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(ValuePropertyName));
        }
    }
}
