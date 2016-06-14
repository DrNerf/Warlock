// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleBind.Utilities
{
    public class ResizeRectTransform : MonoBehaviour, IDragHandler
    {
        private ScaleFinder m_scaler;
        public RectTransform Target;

        public float MinHeight = 100;
        public float MinWidth = 100;

        void Start()
        {
            m_scaler = new ScaleFinder(gameObject);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData != null && eventData.dragging)
            {
                var scale = m_scaler.GetScale();

                var rt = Target ?? gameObject.GetComponent<RectTransform>();
                var newWidth = Mathf.Max(MinWidth, rt.sizeDelta.x + (eventData.delta.x/scale.x));
                var newHeight = Mathf.Max(MinHeight, rt.sizeDelta.y - (eventData.delta.y/scale.y));
                rt.SetSize(new Vector2(newWidth, newHeight));
            }
        }
    }
}