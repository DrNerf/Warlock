// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SimpleBind.Common;
using SimpleBind.DataBinding;

namespace SimpleBind.Controls
{
    public class ContentControl : MonoBehaviour
    {

        [SerializeField]
        private BindingEndpoint _contentEndpoint = new BindingEndpoint();

        [SerializeField]
        private List<DataTemplate> _templates = new List<DataTemplate>();

        private Dictionary<Type, GameObject> _cachedContent = new Dictionary<Type, GameObject>();

        public List<DataTemplate> Templates
        {
            get { return _templates; }
        }

        private void FixedUpdate()
        {
            if (_contentEndpoint.Refresh())
            {
                _contentEndpoint.Commit();
                UpdateContent();
            }
        }

        private void UpdateContent()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

            var content = _contentEndpoint.Value;
            if (content != null)
            {
                GameObject cachedGameObject;

                if (_cachedContent.TryGetValue(content.GetType(), out cachedGameObject))
                {
                    var bindingContext = cachedGameObject.GetComponent<BindingContext>();
                    if (bindingContext != null)
                    {
                        bindingContext.Value = content;
                    }

                    cachedGameObject.SetActive(true);
                }
                else
                {

                    var contentType = content.GetType().Name;
                    var template = _templates.FirstOrDefault(t => t.DataType == contentType);
                    if (template != null)
                    {
                        var go = Instantiate(template.Template.Value) as GameObject;
                        go.transform.SetParent(transform, false);

                        // Assign Item Template to Item Container
                        var bindingContext = go.GetComponent<BindingContext>();
                        if (bindingContext != null)
                        {
                            bindingContext.Value = content;
                        }

                        _cachedContent.Add(content.GetType(), go);
                    }
                }
            }
        }
    }

    [Serializable]
    public class DataTemplate
    {
        [SerializeField]
        public ObservableGameObject Template;

        [SerializeField]
        public string DataType;

        [SerializeField]
        public string Key;
    }
}
