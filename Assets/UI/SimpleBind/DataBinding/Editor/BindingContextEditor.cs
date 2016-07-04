// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using UnityEngine;
using UnityEditor;
using SimpleBind.DataBinding;

namespace SimpleBind.Editors
{
    [CustomEditor(typeof (BindingContext))]
    public class BindingContextEditor : Editor
    {
        private readonly BindingEndpointDrawer _sourceEndpointDrawer = new BindingEndpointDrawer();

        private BindingContext Context { get; set; }

        private SerializedProperty SourceEndpoint { get; set; }

        public void OnEnable()
        {
            Context = target as BindingContext;
            SourceEndpoint = serializedObject.FindProperty("_valueSourceEndpoint");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            float columnWidth = GetColumnLabelWidth();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(GetRowLabelWidth()));
            EditorGUILayout.LabelField(new GUIContent("Object"), GUILayout.Width(columnWidth));
            EditorGUILayout.LabelField(new GUIContent("Component"), GUILayout.Width(columnWidth));
            EditorGUILayout.LabelField(new GUIContent("Path"), GUILayout.Width(columnWidth));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _sourceEndpointDrawer.Draw(SourceEndpoint, "Value Source", GetRowLabelWidth(), GetColumnLabelWidth());
            EditorGUILayout.EndHorizontal();

            if (Context.Parent == null)
                EditorGUILayout.LabelField(new GUIContent("Parent"), new GUIContent("Null"));
            else
                EditorGUILayout.ObjectField(new GUIContent("Parent"), Context.Parent.gameObject, typeof (GameObject), true);

            EditorGUILayout.LabelField(new GUIContent("Is Inheriting"), new GUIContent((Context.IsInheriting) ? "Yes" : "No"));
            EditorGUILayout.LabelField(new GUIContent("Value"), new GUIContent((Context.Value == null) ? "Null" : Context.Value.ToString()));

            serializedObject.ApplyModifiedProperties();
        }

        public static float GetRowLabelWidth()
        {
            return 80;
        }

        public static float GetColumnLabelWidth()
        {
            int rightMargin = 30;
            int miniButtonGroupWidth = 45;
            float columnNumber = 3.0f;

            return ((EditorGUIUtility.currentViewWidth - miniButtonGroupWidth - GetRowLabelWidth() - rightMargin)/columnNumber);
        }

    }
}
