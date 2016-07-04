// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using SimpleBind.DataAware;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleBind.Controls
{
    public class TextItemPresenter : MonoBehaviour, IDataAware
    {
        private string m_data;
	
        public void SetData(object data)
        {
            m_data = data as string;

            GetComponent<Text>().text = m_data;
        }

        public object GetData()
        {
            return m_data;
        }
    }
}
