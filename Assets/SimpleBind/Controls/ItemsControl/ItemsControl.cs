// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using SimpleBind.Common;
using SimpleBind.DataBinding;

namespace SimpleBind.Controls
{
    public class ItemsControl : MonoBehaviour
    {
        protected object ItemsSource;
        protected IEnumerable ItemsSourceAsList;

        protected INotifyCollectionChanged ItemsSourceAsNotifyCollectionChanged;

        [SerializeField]
        private BindingEndpoint _itemsSourceEndpoint;

        [SerializeField]
        protected ObservableGameObject ItemTemplate;

        private Thread _mainThread;
        private bool _rebuildOnUpdate;
        private readonly Queue<NotifyCollectionChangedEventArgs> _notifyCollectionChangedQueue = new Queue<NotifyCollectionChangedEventArgs>();

        protected readonly Dictionary<object, GameObject> ItemToGameObjectMap = new Dictionary<object, GameObject>();

        // Use this for initialization
        protected virtual void Start()
        {
            _mainThread = Thread.CurrentThread;
            ItemsSourceChanged(null);
        }

        protected virtual void LateUpdate()
        {
            if (_itemsSourceEndpoint.Refresh())
            {
                _itemsSourceEndpoint.Commit();
                //ClearExistingItemsSource();
                SetNewItemsSource(_itemsSourceEndpoint.Value);
                RebuildChildren();
            }
            else if (_rebuildOnUpdate)
            {
                _rebuildOnUpdate = false;
                RebuildChildren();
            }
            else if (_notifyCollectionChangedQueue.Count > 0)
            {
                ApplyCollectionChangeQueue();
            }
        }

        private void Reset()
        {
            Debug.Log("Reset");

            if (_itemsSourceEndpoint != null)
            {
                _itemsSourceEndpoint.OnValueChanged -= ItemsSourceChanged;
                _itemsSourceEndpoint = null;
            }

            _itemsSourceEndpoint = new BindingEndpoint();
            _itemsSourceEndpoint.OnValueChanged += ItemsSourceChanged;

        }

        private void ItemsSourceChanged(ValueChangedEventArgs<object> o)
        {
            Debug.Log("ItemsSourceChanged");

            _itemsSourceEndpoint.Refresh();

            if (SetNewItemsSource(_itemsSourceEndpoint.Value))
            {
                RebuildChildren();
            }
        }

        protected virtual void ClearExistingItemsSource()
        {
            if (ItemsSource != null)
            {
                ItemsSource = null;
                ItemsSourceAsList = null;
            }
        }

        protected virtual bool SetNewItemsSource(object value)
        {
            if (ItemsSourceAsNotifyCollectionChanged != null)
            {
                ItemsSourceAsNotifyCollectionChanged.CollectionChanged -= ItemsSourceAsNotifyCollectionChanged_CollectionChanged;
            }

            var isEnumerable = value != null && value.GetType().DeclaringType == typeof(System.Linq.Enumerable);

            if (isEnumerable)
            {
                if (ItemsSource != null && ItemsSource.GetType().DeclaringType == typeof(System.Linq.Enumerable))
                {
                    // setting and IEnumerable as source will continuosly re-evaluate so do not upate a second time.
                    // need to determine solution for this
                    return false;
                }
            }

            ItemsSourceAsNotifyCollectionChanged = value as INotifyCollectionChanged;

            if (ItemsSourceAsNotifyCollectionChanged != null)
            {
                ItemsSourceAsNotifyCollectionChanged.CollectionChanged += ItemsSourceAsNotifyCollectionChanged_CollectionChanged;
            }

            ItemsSource = value;
            ItemsSourceAsList = value as IList;

            return true;
        }

        private void ItemsSourceAsNotifyCollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _notifyCollectionChangedQueue.Enqueue(e);

            // TODO: SoftSource: criddle - Make this actually smart
            if (_mainThread.Equals(Thread.CurrentThread))
            {
                ApplyCollectionChangeQueue();
            }
        }

        private void ApplyCollectionChangeQueue()
        {
            while (_notifyCollectionChangedQueue.Count > 0)
            {
                var changes = _notifyCollectionChangedQueue.Dequeue();
                ApplyNotifyCollectionChange(changes);
            }
        }

        private void ApplyNotifyCollectionChange(NotifyCollectionChangedEventArgs changes)
        {
            if (changes.Action == NotifyCollectionChangedAction.Reset)
            {
                RebuildChildren();
            }
            else
            {
                if (changes.OldItems != null)
                {
                    foreach (var oldItem in changes.OldItems)
                    {
                        GameObject oldGo;
                        if (ItemToGameObjectMap.TryGetValue(oldItem, out oldGo))
                        {
                            RemoveChildItem(oldGo);
                            ItemToGameObjectMap.Remove(oldItem);
                        }
                    }
                }

                if (changes.NewItems != null)
                {
                    var insertIndex = changes.NewStartingIndex;
                    foreach (var newItem in changes.NewItems)
                    {
                        CreateChildItem(newItem, insertIndex++);
                    }
                }
            }
        }

        protected virtual void RebuildChildren()
        {
            // Destroy Children
            foreach (Transform child in transform)
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
        }

        protected virtual void CreateChildItem(object item, int insertIndex)
        {
            if (ItemTemplate.Value == null)
            {
                return;
            }

            // Create Item Template
            var go = Instantiate(ItemTemplate.Value) as GameObject;
            go.transform.SetParent(transform, false);
            go.transform.SetSiblingIndex(insertIndex);

            // Assign Item Template to Item Container
            var bindingContext = go.GetComponent<BindingContext>();
            if (bindingContext != null)
            {
                bindingContext.Value = item;
            }

            ItemToGameObjectMap.Add(item, go);
        }

        protected virtual void RemoveChildItem(GameObject child)
        {
            Destroy(child);
        }
    }
}
