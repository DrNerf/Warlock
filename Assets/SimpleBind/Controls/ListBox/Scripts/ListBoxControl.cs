// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SimpleBind.Common;
using SimpleBind.DataAware;
using SimpleBind.DataBinding;
using UnityEngine.UI;

namespace SimpleBind.Controls
{
    public class ListBoxControl : ItemsControl
    {
        private float setVerticalScroll = -1f;

        [SerializeField] 
        public ObservableGameObject ItemContainer;

        [SerializeField] 
        public bool AllowMultiSelect = false;

        public object SelectedItem
        {
            get { return m_selectedItem != null ? m_selectedItem.GetData() : null; }
        }

        public IEnumerable<object> SelectedItems
        {
            get { return m_selectedItems.Select(t => t.GetData()); }
        }

        private ScrollRect m_scrollRect;
        private RectTransform m_contentPanel;
        private ISelectable m_selectedItem;
        private readonly List<ISelectable> m_selectedItems = new List<ISelectable>();

        // Use this for initialization
        protected override void Start()
        { 
            m_scrollRect = GetComponent<ScrollRect>();
            m_contentPanel = transform.FindChild("ContentPanel").GetComponent<RectTransform>();

            base.Start();
        }
            
        private void Update()
        {
            if (setVerticalScroll >= 0)
            {
                m_scrollRect.verticalScrollbar.value = setVerticalScroll;
                setVerticalScroll = -1;
            }
        }


        protected override void RebuildChildren()
        {
            // Destroy Children
            foreach (Transform child in m_contentPanel.transform)
            {
                RemoveChildItem(child.gameObject);
            }

            ItemToGameObjectMap.Clear();

            if (ItemsSourceAsList == null)
            {
                return;
            }

            // Create Children
            var index = 0;
            foreach (var item in ItemsSourceAsList)
            {
                CreateChildItem(item, index++);
            }

            setVerticalScroll = 1;
        }

        protected override void RemoveChildItem(GameObject child)
        {
            var selectable = child.GetComponent(typeof(ISelectable)) as ISelectable;
            if (selectable != null)
            {
                selectable.IsSelectedChanged -= ListBoxPresenter_IsSelectedChanged;
            }

            Destroy(child.gameObject);
        }

        protected override void CreateChildItem(object item, int insertIndex)
        {
            // Create Item Template
            var go = Instantiate(ItemTemplate.Value) as GameObject;

            // Create Item Container
            GameObject container;

            if (ItemTemplate != null && ItemContainer.Value != null)
            {
                container = Instantiate(ItemContainer.Value) as GameObject;
                container.transform.SetParent(m_contentPanel, false);
                container.transform.SetSiblingIndex(insertIndex);

                var selectable = container.GetComponent(typeof(ISelectable)) as ISelectable;
                if (selectable != null)
                {
                    selectable.IsSelectedChanged += ListBoxPresenter_IsSelectedChanged;
                    selectable.SetData(item);
                }

                // Adjust Item Container Layout to match Item Template Layout if required
                var layout = go.GetComponent<LayoutElement>();
                if (layout != null)
                {
                    var containerLayout = container.GetComponent<LayoutElement>();
                    containerLayout.minHeight = layout.minHeight;
                }

                ItemToGameObjectMap.Add(item, container);

                // Assign Item Template to Item Container
                go.transform.SetParent(container.transform, false);

            }
            else
            {
                container = m_contentPanel.gameObject;
                ItemToGameObjectMap.Add(item, go);

                // Assign Item Template to Item Container
                go.transform.SetParent(container.transform, false);
                go.transform.SetSiblingIndex(insertIndex);
            }

            
            var context = go.GetComponent<BindingContext>();
            if (context != null)
            {
                context.Value = item;

            }
            else
            {
                var dataAware = go.GetComponent(typeof(IDataAware)) as IDataAware;
                if (dataAware != null)
                {
                    dataAware.SetData(item);
                }
            }
        }

        private void ListBoxPresenter_IsSelectedChanged(object sender, System.EventArgs e)
        {
            var selectable = sender as ISelectable;
            if (selectable != null)
            {
                if (selectable.IsSelected)
                {
                    if (AllowMultiSelect)
                    {
                        if (m_selectedItem == null)
                        {
                            m_selectedItem = selectable;
                        }

                        m_selectedItems.Add(selectable);
                    }
                    else
                    {
                        if (m_selectedItem != null)
                        {
                            m_selectedItem.IsSelected = false;
                        }

                        m_selectedItem = selectable;
                        m_selectedItems.Clear();
                        m_selectedItems.Add(selectable);
                    }

                }
                else
                {
                    if (m_selectedItem == selectable)
                    {
                        m_selectedItem = null;
                    }

                    m_selectedItems.Remove(selectable);
                }
            }
        }
    }
}
