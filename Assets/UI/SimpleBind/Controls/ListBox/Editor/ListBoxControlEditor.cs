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
    [CustomEditor(typeof (ListBoxControl))]
    public class ListBoxControlEditor : Editor
    {

        public const string NOTSET = "None";

        private readonly BindingEndpointDrawer _sourceEndpointDrawer = new BindingEndpointDrawer();

        private SerializedProperty SourceEndpoint { get; set; }
        private SerializedProperty ItemTemplate { get; set; }
        private SerializedProperty ItemContainer { get; set; }
        private SerializedProperty AllowMultiSelect { get; set; }

        public void OnEnable()
        {
            Debug.Log("OnEnable");
            SourceEndpoint = serializedObject.FindProperty("_itemsSourceEndpoint");
            ItemTemplate = serializedObject.FindProperty("ItemTemplate").FindPropertyRelative("_value");
            ItemContainer = serializedObject.FindProperty("ItemContainer").FindPropertyRelative("_value");
            AllowMultiSelect = serializedObject.FindProperty("AllowMultiSelect");

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
            ItemTemplate.objectReferenceValue = EditorGUILayout.ObjectField(ItemTemplate.objectReferenceValue, typeof (GameObject), true, GUILayout.Width(GetColumnLabelWidth()*2));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Item Container"), GUILayout.Width(GetRowLabelWidth()));
            ItemContainer.objectReferenceValue = EditorGUILayout.ObjectField(ItemContainer.objectReferenceValue, typeof (GameObject), true, GUILayout.Width(GetColumnLabelWidth()*2));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Allow Multi Select"), GUILayout.Width(GetRowLabelWidth() + 20));
            AllowMultiSelect.boolValue = EditorGUILayout.Toggle(AllowMultiSelect.boolValue, GUILayout.Width(GetColumnLabelWidth()*2));
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        public static float GetRowLabelWidth()
        {
            return 90;
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
