// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using SimpleBind.DataAware;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleBind.Controls
{
    public class ListBoxItemPresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectable
    {
        public Color Normal;
        public Color MouseOver;
        public Color Selected;

        public event EventHandler IsSelectedChanged;

        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                if (m_isSelected != value)
                {
                    m_isSelected = value;
                    EnsureImage();
                    if (m_isSelected)
                    {
                        m_image.color = m_isMouseOver ? (Selected + MouseOver) / 2 : Selected;
                    }
                    else
                    {
                        m_image.color = m_isMouseOver ? MouseOver : Normal;
                    }

                    OnSelectedChanged();
                }
            }
        }

        private Image m_image;
        private bool m_isSelected;

        private object m_selectableData;

        private bool m_isMouseOver;

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_isMouseOver = true;
            EnsureImage();
            m_image.color = IsSelected ? (Selected + MouseOver) /2 : MouseOver;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_isMouseOver = false;
            EnsureImage();
            m_image.color = IsSelected ? Selected : Normal;
        }

        private void EnsureImage()
        {
            if (m_image == null)
            {
                m_image = GetComponent<Image>();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            IsSelected = !IsSelected;
        }

        private void OnSelectedChanged()
        {
            var handler = IsSelectedChanged;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        public void SetData(object data)
        {
            m_selectableData = data;
        }

        public object GetData()
        {
            return m_selectableData;
        }
    }
}
