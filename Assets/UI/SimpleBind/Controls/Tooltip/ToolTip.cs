// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using SimpleBind.DataAware;
using SimpleBind.DataBinding;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleBind.Controls
{
    public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDataAware
    {
        public GameObject Template;
        public string Text;
        public bool UseBindingContext = false;
        public float Delay = 0.1f;

        private ToolTipController m_controller;
        private object m_toolTipData;

        // Use this for initialization
        void Start ()
        {
            m_controller = GetComponentInParent<ToolTipController>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_controller.ToolTipEnter(this, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_controller.ToolTipExit(this);
        }

        public void SetData(object data)
        {
            m_toolTipData = data;
        }

        public object GetData()
        {
            if (UseBindingContext)
            {
                var context = gameObject.GetComponentInParent<BindingContext>();
                if (context != null)
                {
                    return context.Value;
                }
            }

            return m_toolTipData ?? Text;
        }
    }
}
