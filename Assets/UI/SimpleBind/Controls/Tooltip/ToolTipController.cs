// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using SimpleBind.DataBinding;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleBind.Controls
{
    public class ToolTipController : MonoBehaviour
    {
        public GameObject DefaultTemplate;

        private bool m_isToolTipEntered;
        private bool m_isToolTipContainerEntered;
        private bool m_isPendingToolTipEntered;

        private ToolTip m_toolTip;
        private ToolTip m_pendingToolTip;
        private PointerEventData m_pendingEventData;

        private GameObject m_currentVisual;

        // Use this for initialization
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }

        public void ToolTipEnter(ToolTip toolTip, PointerEventData eventData)
        {
            Debug.Log("ToolTip Entered");

            if (m_toolTip == toolTip)
            {
                m_isToolTipEntered = true;
                return;
            }

            m_isPendingToolTipEntered = true;
            m_pendingToolTip = toolTip;
            m_pendingEventData = eventData;

            Invoke("OpenToolTipDelayed", toolTip.Delay);
        }

        private void OpenToolTipDelayed()
        {
            if (m_isPendingToolTipEntered)
            {
                OpenPendingToolTip();
            }
        }

        private void OpenPendingToolTip()
        {
            if (m_pendingToolTip != null)
            {
                CloseToolTip();

                m_toolTip = m_pendingToolTip;

                m_currentVisual = (m_pendingToolTip.Template != null ? Instantiate(m_pendingToolTip.Template) : Instantiate(DefaultTemplate)) as GameObject;
                var da = m_currentVisual.GetComponent<BindingContext>();
                if (da != null)
                {
                    da.Value = m_pendingToolTip.GetData();
                }

                m_currentVisual.AddComponent<ToolTipContainer>().Controller = this;

                m_currentVisual.transform.SetParent(transform, false);
                var rectTransform = GetComponent<RectTransform>();
                Vector2 localPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, m_pendingEventData.position, m_pendingEventData.pressEventCamera, out localPosition);
                m_currentVisual.GetComponent<RectTransform>().localPosition = localPosition;
                m_currentVisual.transform.SetAsLastSibling();
                m_currentVisual.SetActive(true);
            }

            m_isPendingToolTipEntered = false;
            m_pendingToolTip = null;
            m_pendingEventData = null;
        }

        public void ToolTipExit(ToolTip toolTip)
        {
            Debug.Log("ToolTip Exited");
            m_isToolTipEntered = false;

            if (toolTip == m_pendingToolTip)
            {
                m_isPendingToolTipEntered = false;
                m_pendingToolTip = null;
                m_pendingEventData = null;
            }

            Invoke("ToolTipExitDelayed", 0.05f);
        }

        private void ToolTipExitDelayed()
        {
            if (m_isToolTipContainerEntered || m_isToolTipEntered)
            {
                return;
            }

            CloseToolTip();
        }

        private void CloseToolTip()
        {
            m_toolTip = null;

            if (m_currentVisual != null)
            {
                m_currentVisual.SetActive(false);
                Destroy(m_currentVisual);
                m_currentVisual = null;
            }
        }

        public void ToolTipContainerEnter()
        {
            Debug.Log("Container Entered");
            m_isToolTipContainerEntered = true;
        }

        public void ToolTipContainerExit(PointerEventData eventData)
        {
            Debug.Log("Container Exit");
            m_isToolTipContainerEntered = false;

            Invoke("ToolTipExitDelayed", 0.05f);
        }
    }
}
