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

namespace SimpleBind.EventBinding
{
    public class CallbackAccessor : IDisposable
    {
        private int ParameterCount { get; set; }
        private string[] Path { get; set; }
        private object Instance { get; set; }
        private CallbackAccessor Next { get; set; }
        private MethodInfo Method { get; set; }

        // If this CallbackAccessor is not the actual callback but a partent object then the Property or Field objects will hold the intermediate objects in the path
        private PropertyInfo Property { get; set; }
        private FieldInfo Field { get; set; }

        public CallbackAccessor(object instance, string[] path, int parameterCount)
        {
            ParameterCount = parameterCount;
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

        public void ChangeInstance(object instance)
        {
            if (Instance is INotifyPropertyChanged)
                ((INotifyPropertyChanged)Instance).PropertyChanged -= Object_PropertyChanged;

            Instance = instance;

            if (Instance == null)
            {
                Method = null;

                if (Next != null)
                    Next.ChangeInstance(null);
            }
            else
            {
                if (!string.IsNullOrEmpty(MemberName))
                {
                    if (Path.Length > 1)
                    {
                        Field = Instance.GetType().GetField(MemberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                        if (Field == null)
                            Property = Instance.GetType().GetProperty(MemberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                        if (Field == null && Property == null)
                            Debug.LogWarning(string.Format("Unable to find member named '{0}' on '{1}'.", MemberName, Instance.GetType()));

                        if (Next != null)
                            Next.Dispose();

                        Next = new CallbackAccessor(GetLocalFieldOrProperty(), Path.Skip(1).ToArray(), ParameterCount);
                    }
                    else
                    {
                        Method = Instance.GetType().GetMethods(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                            .Where(t => t.Name.Equals(MemberName) && t.GetParameters().Length <= ParameterCount)
                            .OrderByDescending(t => t.GetParameters().Length)
                            .FirstOrDefault();
                        if (Method == null)
                        {
                            Debug.LogWarning(string.Format("Unable to find member named '{0}' on '{1}'.", MemberName, Instance.GetType()));
                        }
                    }

                    if (Instance is INotifyPropertyChanged)
                        ((INotifyPropertyChanged)Instance).PropertyChanged += Object_PropertyChanged;
                }
            }
        }

        private object GetLocalFieldOrProperty()
        {
            object value = null;

            if (Instance != null)
            {
                if (Field == null)
                {
                    if (Property != null)
                    {
                        if (Property.CanRead)
                            value = Property.GetValue(Instance, null);
                        else
                            Debug.LogWarning(string.Format("Property '{0}' does not have read access.", Property.Name));
                    }

                }
                else
                    value = Field.GetValue(Instance);
            }

            return value;
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Next != null && string.Compare(e.PropertyName, MemberName, true) == 0)
            {
                Next.ChangeInstance(GetLocalFieldOrProperty());
            }
        }

        private Action GetLocal()
        {
            Action value = null;

            if (Instance != null)
            {
                if (Method != null)
                {

                    value = () => Method.Invoke(Instance, null);
                }
            }

            return value;
        }

        public void Invoke(object[] parameters)
        {
            if (Next != null)
            {
                Next.Invoke(parameters);
            }
            else
            {
                if (Instance != null)
                {
                    if (Method != null)
                    {
                        try
                        {
                            Method.Invoke(Instance, parameters);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogException(ex);
                        }
                    }
                    else
                    {
                        Debug.Log("Method is null in Invoke");
                    }
                }
                else
                {
                    Debug.Log("Instance is null in Invoke");
                }
            }
        }

        public Action Get()
        {
            return Next != null ? Next.Get() : GetLocal();
        }

        public void Dispose()
        {
            if (Next != null)
            {
                Next.Dispose();
                Next = null;
            }

            if (Instance is INotifyPropertyChanged)
                ((INotifyPropertyChanged)Instance).PropertyChanged -= Object_PropertyChanged;

            Instance = null;
            Method = null;
            Property = null;
            Field = null;
        }
    }
}
