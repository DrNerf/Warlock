// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using UnityEngine;

namespace SimpleBind.EventBinding
{
    [Serializable] 
	public class EventBindingBehaviour : MonoBehaviour
	{
		[SerializeField]
		private EventBinding _eventBinding = null;

        public EventBinding EventBinding
		{
            get { return _eventBinding; }
            private set { _eventBinding = value; }
		}

		private void Start()
		{
            EventBinding.IsActive = true;
		}

        private void LateUpdate()
        {
            EventBinding.Update();
        }

		private void OnDestroy()
		{
            EventBinding.Dispose();
		}

		private void Reset()
		{
			if (EventBinding != null) {
                EventBinding.Dispose();
			}

            EventBinding = new EventBinding();
		}
	}
}
