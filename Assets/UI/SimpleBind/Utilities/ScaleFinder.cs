// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using UnityEngine;
using UnityEngine.UI;

namespace SimpleBind.Utilities
{
    public class ScaleFinder
    {
        private readonly Vector3 m_noScale = new Vector3(1, 1, 1);

        private GameObject m_target;
        private CanvasScaler m_canvasScaler;
        private RectTransform m_canvasTransform;


        public ScaleFinder(GameObject target)
        {
            m_target = target;
        }

        public Vector3 GetScale()
        {
            if (m_canvasTransform == null)
            {
                m_canvasScaler = m_target.GetComponentInParent(typeof (CanvasScaler)) as CanvasScaler;
                if (m_canvasScaler != null)
                {
                    m_canvasTransform = m_canvasScaler.transform as RectTransform;
                }
            }

            return m_canvasTransform != null ? m_canvasTransform.localScale : m_noScale;
        }

    }
}