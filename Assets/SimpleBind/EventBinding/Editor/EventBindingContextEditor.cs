// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using UnityEngine;
using UnityEditor;
using SimpleBind.EventBinding;

namespace SimpleBind.Editors
{
    [CustomEditor(typeof (EventBindingContext))]
    public class EventBindingContextEditor : Editor
    {
        private EventBindingContext Context { get; set; }

        public void OnEnable()
        {
            Context = target as EventBindingContext;
        }

        public override void OnInspectorGUI()
        {
            if (Context.Parent == null)
                EditorGUILayout.LabelField(new GUIContent("Parent"), new GUIContent("Null"));
            else
                EditorGUILayout.ObjectField(new GUIContent("Parent"), Context.Parent.gameObject, typeof (GameObject), true);

            EditorGUILayout.LabelField(new GUIContent("Is Inheriting"), new GUIContent((Context.IsInheriting) ? "Yes" : "No"));
            EditorGUILayout.LabelField(new GUIContent("Value"), new GUIContent((Context.Value == null) ? "Null" : Context.Value.ToString()));
        }
    }
}
