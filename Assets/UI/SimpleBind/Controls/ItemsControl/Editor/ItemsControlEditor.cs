// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using SimpleBind.Controls;
using UnityEngine;
using UnityEditor;

namespace SimpleBind.Editors
{
    [CustomEditor(typeof (ItemsControl))]
    public class ItemsControlEditor : Editor
    {

        public const string Notset = "None";

        private readonly BindingEndpointDrawer _sourceEndpointDrawer = new BindingEndpointDrawer();

        private SerializedProperty SourceEndpoint { get; set; }
        private SerializedProperty ItemTemplate { get; set; }

        public void OnEnable()
        {
            // Debug.Log("OnEnable");
            SourceEndpoint = serializedObject.FindProperty("_itemsSourceEndpoint");
            ItemTemplate = serializedObject.FindProperty("ItemTemplate").FindPropertyRelative("_value");

            Debug.Log(SourceEndpoint);
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
            _sourceEndpointDrawer.Draw(SourceEndpoint, "Items Source", GetRowLabelWidth(), GetColumnLabelWidth());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Item Template"), GUILayout.Width(GetRowLabelWidth()));
            ItemTemplate.objectReferenceValue = EditorGUILayout.ObjectField(ItemTemplate.objectReferenceValue, typeof (GameObject), true, GUILayout.Width(GetColumnLabelWidth()));
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        public static float GetRowLabelWidth()
        {
            return 80;
        }

        public static float GetColumnLabelWidth()
        {
            const int rightMargin = 30;
            const int miniButtonGroupWidth = 45;
            const float columnNumber = 3.0f;

            return ((EditorGUIUtility.currentViewWidth - miniButtonGroupWidth - GetRowLabelWidth() - rightMargin)/columnNumber);
        }
    }
}
