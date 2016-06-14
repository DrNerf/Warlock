// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using SimpleBind.DataBinding;
using UnityEditor;
using UnityEngine;

namespace SimpleBind.Editors
{
    public class DataTemplateDrawer
    {
        private GUIContent[] DatatTypes { get; set; }

        private SerializedProperty ObjectProperty { get; set; }
        private SerializedProperty DataTypeProperty { get; set; }
        private SerializedProperty KeyProperty { get; set; }

        private GameObject GameObject
        {
            get
            {
                return ObjectProperty.objectReferenceValue as GameObject;
            }
        }

        public void Draw(IBindingBehaviourEditor editor, SerializedProperty dataTemplate, int index, bool hasMultiple)
        {
            GUIContent guiContent = null;

            ObjectProperty = dataTemplate.FindPropertyRelative("Template").FindPropertyRelative("_value");
            DataTypeProperty = dataTemplate.FindPropertyRelative("DataType");
            KeyProperty = dataTemplate.FindPropertyRelative("Key");

            guiContent = new GUIContent(hasMultiple ? "Templates" : "Template");

            EditorGUILayout.LabelField((index == 0) ? guiContent : GUIContent.none, GUILayout.Width(editor.GetRowLabelWidth()));

            // Hack to get two columns
            float columnWidth = editor.GetColumnLabelWidth() * 3 / 2;

            DrawObjectField(columnWidth);
            DrawDataTypesField(columnWidth);

            if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20)))
                editor.DuplicateBindingEndpoint(false, index);

            if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                editor.RemoveBindingEndpoint(false, index, hasMultiple);
        }

        private void DrawObjectField(float columnLabelWidth)
        {
            ObjectProperty.objectReferenceValue = EditorGUILayout.ObjectField(ObjectProperty.objectReferenceValue, typeof(GameObject), true, GUILayout.Width(columnLabelWidth));
        }

        private void DrawDataTypesField(float columnWidth)
        {
            DataTypeProperty.stringValue = EditorGUILayout.TextField(DataTypeProperty.stringValue, GUILayout.Width(columnWidth));
        }

        private void DrawKeyField(float columnWidth)
        {
            KeyProperty.stringValue = GUILayout.TextField(KeyProperty.stringValue, GUILayout.Width(columnWidth));
        }

        private GUIContent[] GetDataTypes()
        {
            List<GUIContent> list = new List<GUIContent>();


            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()))
            {
                list.Add(new GUIContent(type.Name));
            }

            list = list.OrderBy(c => c.text).ToList();

            list.Insert(0, new GUIContent(BindingEndpoint.Notset));

            return list.ToArray();
        }
    }
}
