// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace SimpleBind.EventBinding
{
    public class EventAccessor : IDisposable
    {
        private string[] Path { get; set; }
        private object Instance { get; set; }
        private EventAccessor Next { get; set; }
        private PropertyInfo Property { get; set; }

        private UnityEvent UnityEvent { get; set; }
        private UnityEvent<int> UnityEventInt { get; set; }

        private UnityEvent<string> UnityEventString { get; set; }

        public Action OnEventFired { get; set; }

        public EventAccessor(object instance, string[] path)
		{
			Path = path;
			ChangeInstance(instance);
		}

        public string MemberName
        {
            get
            {
                return Path.FirstOrDefault();
            }
        }

        public Type MemberType
        {
            get
            {
                return (Property == null) ? null : Property.PropertyType;
            }
        }

        public void ChangeInstance(object instance)
        {
            Instance = instance;

            if (Instance == null)
            {
                Property = null;

                if (Next != null)
                    Next.ChangeInstance(null);

                if (UnityEvent != null)
                {
                    UnityEvent.RemoveListener(UnityEventListener);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(MemberName))
                {
                    Property = Instance.GetType().GetProperty(MemberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (Property == null)
                        Debug.LogWarning(string.Format("Unable to find member named '{0}' on '{1}'.", MemberName, Instance.GetType()));
                }

                if (Path.Length > 1)
                {
                    if (Next != null)
                        Next.Dispose();

                    Next = new EventAccessor(GetLocal(), Path.Skip(1).ToArray());
                }
                else
                {
                    var localValue = GetLocal();
                    UnityEvent = localValue as UnityEvent;
                    if (UnityEvent != null)
                    {
                        UnityEvent.AddListener(UnityEventListener);
                    }
                    else
                    {
                        UnityEventInt = localValue as UnityEvent<int>;
                        if (UnityEventInt != null)
                        {
                            UnityEventInt.AddListener(UnityEventIntListener);
                        }
                        else
                        {
                            UnityEventString = localValue as UnityEvent<string>;
                            if (UnityEventString != null)
                            {
                                UnityEventString.AddListener(UnityEventStringListener);
                            }
                            else
                            {
                                Debug.Log("Unable to bind to Event: " + localValue.GetType());
                            }
                        }
                    }
                }
            }
        }

        private void UnityEventListener()
        {
            Debug.Log("Unity Event Fired");
            if (OnEventFired != null)
            {
                OnEventFired();
            }
        }

        private void UnityEventIntListener(int value)
        {
            Debug.Log("Unity Event Int Fired");
            if (OnEventFired != null)
            {
                OnEventFired();
            }
        }

        private void UnityEventStringListener(string value)
        {
            Debug.Log("Unity Event String Fired");
            if (OnEventFired != null)
            {
                OnEventFired();
            }
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Next != null && string.Compare(e.PropertyName, MemberName, true) == 0)
                Next.ChangeInstance(GetLocal());
        }

        private object GetLocal()
        {
            object value = null;

            if (Instance != null)
            {
                if (Property != null)
                {
                    if (Property.CanRead)
                        value = Property.GetValue(Instance, null);
                    else
                        Debug.LogWarning(string.Format("Property '{0}' does not have read access.", Property.Name));
                }
            }

            return value;
        }

        public void Dispose()
        {
            if (Next != null)
            {
                Next.Dispose();
                Next = null;
            }

            if (UnityEvent != null)
            {
                UnityEvent.RemoveListener(UnityEventListener);
            }

            Instance = null;
            Property = null;
            OnEventFired = null;
        }
    }
}
