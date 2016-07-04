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
    public class EventBindingEndpoint: Observable<object>
	{
		[SerializeField]
		private ObservableGameObject _object;

		[SerializeField]
		private ObservableString _componentName;

		[SerializeField]
		private ObservableString _path;

		private EventAccessor _eventAccessor;

        public Action OnEventFired { get; set; }

        public EventBindingEndpoint()
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

        public void EnsureInitialized()
        {
            EnsureEventAccessor();
        }

        private void EnsureEventAccessor()
        {
            if (_eventAccessor == null)
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

                    _eventAccessor = new EventAccessor(component, path) {OnEventFired = EventAccessorFired};
                }
            }
        }

        private void EventAccessorFired()
        {
            if (OnEventFired != null)
            {
                OnEventFired();
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

		private void ClearMemberAccessor()
		{
			if (_eventAccessor != null)
			{
				_eventAccessor.Dispose();
				_eventAccessor = null;
			}
		}

		public bool Refresh()
		{
		    EnsureEventAccessor();

			return _eventAccessor != null;
		}

        public override void Dispose()
		{
			base.Dispose();

			Object.OnValueChanged -= Object_OnValueChanged;
			ComponentName.OnValueChanged -= ComponentName_OnValueChanged;
			Path.OnValueChanged -= Path_OnValueChanged;

		    if (_eventAccessor != null)
		    {
                _eventAccessor.Dispose();
		    }

		    Object = null;
			ComponentName = null;
			Path = null;
		}
	}
}
