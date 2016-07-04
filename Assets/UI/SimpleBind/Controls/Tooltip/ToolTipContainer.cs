// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleBind.Controls
{
    public class ToolTipContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public ToolTipController Controller;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Controller.ToolTipContainerEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Controller.ToolTipContainerExit(eventData);
        }
    }
}
