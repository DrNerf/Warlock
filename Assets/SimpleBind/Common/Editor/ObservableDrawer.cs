// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using SimpleBind.Common;
using UnityEngine;
using UnityEditor;

namespace SimpleBind.Editors
{
    [CustomPropertyDrawer(typeof (ObservableInt))]
    [CustomPropertyDrawer(typeof (ObservableFloat))]
    [CustomPropertyDrawer(typeof (ObservableString))]
    [CustomPropertyDrawer(typeof (ObservableBool))]
    [CustomPropertyDrawer(typeof (ObservableGameObject))]
    public class ObservableDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty valueProperty = property.FindPropertyRelative("_value");

            EditorGUI.PropertyField(position, valueProperty, label);

            EditorGUI.EndProperty();
        }
    }
}