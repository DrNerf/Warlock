// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using UnityEngine;

namespace SimpleBind.DataBinding
{
	public class BindingBehaviour : MonoBehaviour
	{
		[SerializeField]
		private Binding _binding = null;

		public Binding Binding
		{
			get { return _binding; }
			private set { _binding = value; }
		}

		private void Start()
		{
			Binding.IsActive = true;
		}

		private void LateUpdate()
		{
			Binding.Update();
		}

		private void OnDestroy()
		{
			Binding.Dispose();
		}

		private void Reset()
		{
			if (Binding != null) {
				Binding.Dispose ();
			}

			Binding = new Binding();

			Binding.SourceEndpoints.Clear();
			Binding.TargetEndpoints.Clear();

			AddSource();
			AddTarget();
		}

		[ContextMenu("Add Source")]
		private void AddSource()
		{
			if (Binding != null)
			{
				BindingEndpoint endpoint = new BindingEndpoint();

				Binding.SourceEndpoints.Add(endpoint);
				endpoint.Object.Value = this.gameObject;
			}
		}

		[ContextMenu("Add Target")]
		private void AddTarget()
		{
			if (Binding != null)
			{
				BindingEndpoint endpoint = new BindingEndpoint();

				Binding.TargetEndpoints.Add(endpoint);
				endpoint.Object.Value = this.gameObject;
			}
		}
	}
}
