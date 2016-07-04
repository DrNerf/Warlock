// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System.Collections.Generic;
using SimpleBind.Controls;
using UnityEditor;
using UnityEngine;

namespace SimpleBind.Editors
{
    [CustomEditor(typeof(ContentControl))]
    public class ContentControlEditor : Editor, IBindingBehaviourEditor
    {
        private readonly BindingEndpointDrawer _contentEndpointDrawer = new BindingEndpointDrawer();

        private SerializedProperty ContentEndpoint { get; set; }
        private SerializedProperty DataTemplates { get; set; }

        private List<DataTemplateDrawer> _dataTemplateDrawers = null;

        private List<DataTemplateDrawer> DataTemplateDrawers
        {
            get
            {
                if (_dataTemplateDrawers == null)
                    _dataTemplateDrawers = new List<DataTemplateDrawer>();

                return _dataTemplateDrawers;
            }
        }

        public void OnEnable()
        {
            Debug.Log("OnEnable");
            ContentEndpoint = serializedObject.FindProperty("_contentEndpoint");
            DataTemplates = serializedObject.FindProperty("_templates");

            Debug.Log(ContentEndpoint);
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
            _contentEndpointDrawer.Draw(ContentEndpoint, "Content", GetRowLabelWidth(), GetColumnLabelWidth());
            EditorGUILayout.EndHorizontal();


            if (DataTemplates.arraySize > 0)
            {

                // Hack to get two columns
                float headerWidth = GetColumnLabelWidth() * 3 / 2;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(GetRowLabelWidth()));
                EditorGUILayout.LabelField(new GUIContent("Prefab"), GUILayout.Width(headerWidth));
                EditorGUILayout.LabelField(new GUIContent("Content Class Name"), GUILayout.Width(headerWidth));
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < DataTemplates.arraySize; i++)
                {
                    SerializedProperty dataTemplate = DataTemplates.GetArrayElementAtIndex(i);

                    EditorGUILayout.BeginHorizontal();
                    GetBindingEndpointDrawer(i, DataTemplateDrawers).Draw(this, dataTemplate, i, DataTemplates.arraySize > 1);
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Template"), GUILayout.Width(GetRowLabelWidth()));
                if (GUILayout.Button("Add Template", EditorStyles.miniButton, GUILayout.Width(100)))
                {
                    var templates = ((ContentControl)target).Templates;
                    templates.Add(new DataTemplate());
                }

                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }
        private DataTemplateDrawer GetBindingEndpointDrawer(int index, List<DataTemplateDrawer> bindingEndpointDrawers)
        {
            DataTemplateDrawer bindingEndpointDrawer = null;

            if (index >= 0)
            {
                if (index < bindingEndpointDrawers.Count)
                    bindingEndpointDrawer = bindingEndpointDrawers[index];
                else if (index == bindingEndpointDrawers.Count)
                {
                    bindingEndpointDrawer = new DataTemplateDrawer();
                    bindingEndpointDrawers.Add(bindingEndpointDrawer);
                }
            }

            return bindingEndpointDrawer;
        }

        public void DuplicateBindingEndpoint(bool isSource, int index)
        {
            var templates = ((ContentControl)target).Templates;

            var template = new DataTemplate();

            templates.Add(template);
        }

        public void RemoveBindingEndpoint(bool isSource, int index, bool hasMultiple)
        {
            var templates = ((ContentControl)target).Templates;

            templates.RemoveAt(index);
            _dataTemplateDrawers.RemoveAt(index);
        }


        public float GetRowLabelWidth()
        {
            return 80;
        }

        public float GetColumnLabelWidth()
        {
            const int rightMargin = 30;
            const int miniButtonGroupWidth = 45;
            const float columnNumber = 3.0f;

            return ((EditorGUIUtility.currentViewWidth - miniButtonGroupWidth - GetRowLabelWidth() - rightMargin) / columnNumber);
        }

    }
}
