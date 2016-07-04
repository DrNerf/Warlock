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
	public class Binding : IDisposable
	{
		[SerializeField]
		private List<BindingEndpoint> _sourceEndpoints = new List<BindingEndpoint>();

		[SerializeField]
		private List<BindingEndpoint> _targetEndpoints = new List<BindingEndpoint>();

		[SerializeField]
		private ObservableString _converterName = new ObservableString();

		[SerializeField]
		private ObservableString _converterParameter = new ObservableString();

		[SerializeField]
		private BindingDirections _direction;

		private object _converter = null;

	    private bool _forceInitialUpdate = true;
	    private bool _isActive;

		public Binding()
		{
			ConverterName.OnValueChanged += ConverterName_OnValueChanged;
			ConverterParameter.OnValueChanged += ConverterParameter_OnValueChanged;
		}

		public BindingDirections BindingDirection
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
			}
		}

		public object Converter
		{
			get
			{
				if (_converter == null)
					_converter = InstanciateConverter(ConverterName.Value);

				return _converter;
			}
			set
			{
				_converter = value;
			}
		}

		public List<BindingEndpoint> SourceEndpoints
		{
			get
			{
				return _sourceEndpoints;
			}
		}

		public List<BindingEndpoint> TargetEndpoints
		{
			get
			{
				return _targetEndpoints;
			}
		}

		public ObservableString ConverterName
		{
			get { return _converterName; }
		}

		public ObservableString ConverterParameter
		{
			get { return _converterParameter; }
		}

	    public bool IsActive
	    {
	        get { return _isActive; }
	        set
	        {
	            _isActive = value;
	            _forceInitialUpdate = true;
	        }
	    }

	    private object InstanciateConverter(string converterTypeName)
		{
			object converter = null;

			if (!string.IsNullOrEmpty(converterTypeName))
			{
				Type converterType = Type.GetType(converterTypeName);

				if (converterType == null)
					Debug.Log(String.Format("Unable to create a converter of type '{0}'.", converterTypeName));
				else
					converter = Activator.CreateInstance(converterType);
			}

			return converter;
		}

		public bool Update()
		{
			return Update(_forceInitialUpdate);
		}
		public bool Update(bool forceUpdate)
		{
			bool sourceHasChanged = false;
			bool targetHasChanged = false;


			if (IsActive)
			{
				if (BindingDirection == BindingDirections.SourceToTarget || BindingDirection == BindingDirections.Both)
					sourceHasChanged = UpdateBindingEndpoints(SourceEndpoints, TargetEndpoints, forceUpdate, UpdateDirection.ToTarget);

				if (BindingDirection == BindingDirections.TargetToSource || BindingDirection == BindingDirections.Both)
					targetHasChanged = UpdateBindingEndpoints(TargetEndpoints, SourceEndpoints, forceUpdate, UpdateDirection.ToSource);

			    _forceInitialUpdate = false;
			}

			return sourceHasChanged || targetHasChanged;
		}

		private bool UpdateBindingEndpoints(List<BindingEndpoint> readEndpoints, List<BindingEndpoint> writeEndpoints, bool forceUpdate, UpdateDirection direction)
		{
			bool hasChanged = false;


			hasChanged = readEndpoints.Select(readEndpoint => readEndpoint.Refresh()).Any(result => result);

			if (hasChanged || forceUpdate)
			{
				writeEndpoints.ForEach(writeEndpoint => writeEndpoint.Update(readEndpoints, Converter, ConverterParameter.Value, direction, forceUpdate));
				readEndpoints.ForEach(readEndpoint => readEndpoint.Commit());
			}

			return hasChanged;
		}

		private void ConverterName_OnValueChanged(ValueChangedEventArgs<string> obj)
		{
			Converter = null;
			Update(true);
			obj.Handled = true;

		}

		private void ConverterParameter_OnValueChanged(ValueChangedEventArgs<string> obj)
		{
			Update(true);
			obj.Handled = true;

		}

		public void Dispose()
		{
			ConverterName.OnValueChanged -= ConverterName_OnValueChanged;
			ConverterParameter.OnValueChanged -= ConverterParameter_OnValueChanged;

			foreach (BindingEndpoint sourceEndpoint in SourceEndpoints)
			{
				sourceEndpoint.Dispose();
			}

			foreach (BindingEndpoint targetEndpoint in TargetEndpoints)
			{
				targetEndpoint.Dispose();
			}
		}
	}


	public enum BindingDirections
	{
		SourceToTarget,
		TargetToSource,
		Both
	}
}
