// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using System.ComponentModel;
using SimpleBind.DataBinding;
using UnityEngine;

namespace SimpleBind.Common
{
    public class Observable<T> : ISerializationCallbackReceiver, INotifyPropertyChanged, IDisposable
    {
        private static readonly string ValuePropertyName = "Value";

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<ValueChangedEventArgs<T>> OnValueChanged = null;

        public Observable() { }
        public Observable(T defaultValue)
        {
            _value = defaultValue;
        }

        [SerializeField]
        private T _value;

        private T PreviousValue { get; set; }
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!Equals(_value, value))
                    OnChange(value);
            }
        }
        public bool HasChanged
        {
            get
            {
                return !Equals(PreviousValue, Value);
            }
        }

        private void OnChange(T value)
        {
            _value = value;

            if (OnValueChanged != null)
            {
                ValueChangedEventArgs<T> args = new ValueChangedEventArgs<T>(PreviousValue, _value);

                OnValueChanged(args);

                if (args.Handled)
                    Commit();
            }


            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(ValuePropertyName));

        }

        public bool Commit()
        {
            bool changeValue = HasChanged;

            PreviousValue = Value;

            return changeValue;
        }

        public virtual void Dispose()
        {
            Value = PreviousValue = default(T);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            OnChange(_value);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Do nothing
        }
    }

    public class ValueChangedEventArgs<T> : EventArgs
    {
        public ValueChangedEventArgs(T previous, T current)
        {
            PreviousValue = previous;
            CurrentValue = current;
        }

        public T PreviousValue { get; private set; }
        public T CurrentValue { get; private set; }
        public bool Handled { get; set; }
    }


    [Serializable]
    public class ObservableGameObject : Observable<GameObject>
    {
        public ObservableGameObject() : base() { }
        public ObservableGameObject(GameObject defaultValue) : base(defaultValue) { }
    }

    [Serializable]
    public class ObservableInt : Observable<int>
    {
        public ObservableInt() : base() { }
        public ObservableInt(int defaultValue) : base(defaultValue) { }
    }

    [Serializable]
    public class ObservableFloat : Observable<float>
    {
        public ObservableFloat() : base() { }
        public ObservableFloat(float defaultValue) : base(defaultValue) { }
    }

    [Serializable]
    public class ObservableString : Observable<string>
    {
        public ObservableString() : base() { }
        public ObservableString(string defaultValue) : base(defaultValue) { }
    }

    [Serializable]
    public class ObservableBool : Observable<bool>
    {
        public ObservableBool() : base() { }
        public ObservableBool(bool defaultValue) : base(defaultValue) { }
    }
}
