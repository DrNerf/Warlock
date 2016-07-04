// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleBind.Utilities
{
    public class DragRectTransform : MonoBehaviour, IDragHandler
    {
        private ScaleFinder m_scale;

        public RectTransform Target;

        void Start()
        {
            m_scale = new ScaleFinder(gameObject);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData != null && eventData.dragging)
            {
                var scale = m_scale.GetScale();
                var rt = Target ?? gameObject.GetComponent<RectTransform>();
                var x = Mathf.Clamp(rt.anchoredPosition.x + (eventData.delta.x/scale.x), 0, Screen.width - 50);
                var y = Mathf.Clamp(rt.anchoredPosition.y + (eventData.delta.y/scale.y), -(Screen.height - 50), 0);
                rt.anchoredPosition = new Vector2(x, y);
            }
        }
    }
}
