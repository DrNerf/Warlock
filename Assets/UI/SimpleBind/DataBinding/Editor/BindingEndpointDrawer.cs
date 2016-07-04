// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using SimpleBind.DataBinding;
using SimpleBind.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleBind.Editors
{
    [CustomPropertyDrawer(typeof(BindingEndpoint), true)]
    public class BindingEndpointDrawer : PropertyDrawer
    {
        private GUIContent[] Components { get; set; }
        private GUIContent[] ComponentMembers { get; set; }

        private SerializedProperty ObjectProperty { get; set; }
        private SerializedProperty ComponentNameProperty { get; set; }
        private SerializedProperty PathProperty { get; set; }
        private SerializedProperty ForcePropertyPath { get; set; }

        private GameObject GameObject
        {
            get { return ObjectProperty.objectReferenceValue as GameObject; }
        }

        public void Draw(SerializedProperty endpoint, string label, float rowLabelWidth, float columnLabelWidth)
        {
            ObjectProperty = endpoint.FindPropertyRelative("_object").FindPropertyRelative("_value");
            ComponentNameProperty = endpoint.FindPropertyRelative("_componentName").FindPropertyRelative("_value");
            PathProperty = endpoint.FindPropertyRelative("_path").FindPropertyRelative("_value");
            ForcePropertyPath = endpoint.FindPropertyRelative("_forcePropertyPath");

            EditorGUILayout.LabelField(new GUIContent(label), GUILayout.Width(rowLabelWidth));

            DrawObjectField(endpoint, columnLabelWidth);
            DrawComponentsField(endpoint, true, columnLabelWidth);
            DrawComponentMembersField(endpoint, columnLabelWidth);
        }


        public void Draw(IBindingBehaviourEditor editor, SerializedProperty endpoint, int index, bool isSource, bool hasMultiple)
        {
            GUIContent guiContent = null;

            ObjectProperty = endpoint.FindPropertyRelative("_object").FindPropertyRelative("_value");
            ComponentNameProperty = endpoint.FindPropertyRelative("_componentName").FindPropertyRelative("_value");
            PathProperty = endpoint.FindPropertyRelative("_path").FindPropertyRelative("_value");
            ForcePropertyPath = endpoint.FindPropertyRelative("_forcePropertyPath");

            if (isSource)
                guiContent = new GUIContent(hasMultiple ? "Sources" : "Source");
            else
                guiContent = new GUIContent(hasMultiple ? "Targets" : "Target");

            EditorGUILayout.LabelField((index == 0) ? guiContent : GUIContent.none, GUILayout.Width(editor.GetRowLabelWidth()));


            float columnWidth = editor.GetColumnLabelWidth();

            DrawObjectField(endpoint, columnWidth);
            DrawComponentsField(endpoint, isSource, columnWidth);
            DrawComponentMembersField(endpoint, columnWidth);

            if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20)))
                editor.DuplicateBindingEndpoint(isSource, index);

            GUI.enabled = hasMultiple;
            if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                editor.RemoveBindingEndpoint(isSource, index, hasMultiple);
            GUI.enabled = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ObjectProperty = property.FindPropertyRelative("_object").FindPropertyRelative("_value");
            ComponentNameProperty = property.FindPropertyRelative("_componentName").FindPropertyRelative("_value");
            PathProperty = property.FindPropertyRelative("_path").FindPropertyRelative("_value");
            ForcePropertyPath = property.FindPropertyRelative("_forcePropertyPath");

            var r = position;
            var w = position.width / 4f;
            var r0 = new Rect(r.xMin, r.yMin, w, r.height);
            var r1 = new Rect(r0.xMax, r.yMin, w, r.height);
            var r2 = new Rect(r1.xMax, r.yMin, w, r.height);
            var r3 = new Rect(r2.xMax, r.yMin, w, r.height);

            EditorGUI.LabelField(r0, label);

            DrawObjectField(r1);
            DrawComponentsField(true, r2);
            DrawComponentMembersField(r3);
        }

        private void DrawObjectField(Rect position)
        {
            EditorGUI.BeginChangeCheck();

            ObjectProperty.objectReferenceValue = EditorGUI.ObjectField(position, ObjectProperty.objectReferenceValue, typeof(GameObject), true);

            if (EditorGUI.EndChangeCheck() || Components == null)
            {
                Components = GetComponents(GameObject);
                ComponentMembers = GetComponentMembers(GameObject, ComponentNameProperty.stringValue);
            }
        }

        private void DrawObjectField(SerializedProperty endpoint, float columnLabelWidth)
        {
            EditorGUI.BeginChangeCheck();

            ObjectProperty.objectReferenceValue = EditorGUILayout.ObjectField(ObjectProperty.objectReferenceValue, typeof(GameObject), true, GUILayout.Width(columnLabelWidth));

            if (EditorGUI.EndChangeCheck() || Components == null)
            {
                Components = GetComponents(GameObject);
                ComponentMembers = GetComponentMembers(GameObject, ComponentNameProperty.stringValue);
            }
        }

        private void DrawComponentsField(bool isSource, Rect position)
        {
            if (Components != null)
            {
                SetDefaultComponentName(isSource);

                int selection = Array.FindIndex(Components, c => c.text == ComponentNameProperty.stringValue);

                EditorGUI.BeginChangeCheck();

                selection = EditorGUI.Popup(position, Math.Max(selection, 0), Components, EditorStyles.popup);

                if (selection <= 0)
                    ComponentNameProperty.stringValue = BindingEndpoint.Notset;
                else
                    ComponentNameProperty.stringValue = Components[selection].text;

                if (EditorGUI.EndChangeCheck() || ComponentMembers == null)
                {
                    ComponentMembers = GetComponentMembers(GameObject, ComponentNameProperty.stringValue);
                }
            }
        }

        private void DrawComponentsField(SerializedProperty endpoint, bool isSource, float columnLabelWidth)
        {
            if (Components != null)
            {
                SetDefaultComponentName(isSource);

                int selection = Array.FindIndex(Components, c => c.text == ComponentNameProperty.stringValue);

                EditorGUI.BeginChangeCheck();

                selection = EditorGUILayout.Popup(Math.Max(selection, 0), Components, EditorStyles.popup, GUILayout.Width(columnLabelWidth));

                if (selection < 0)
                    ComponentNameProperty.stringValue = null;
                else
                    ComponentNameProperty.stringValue = Components[selection].text;

                if (EditorGUI.EndChangeCheck() || ComponentMembers == null)
                {
                    ComponentMembers = GetComponentMembers(GameObject, ComponentNameProperty.stringValue);
                }
            }
        }

        private void DrawComponentMembersField(Rect position)
        {
            if (ComponentMembers != null)
            {
                SetDefaultMemberName();

                var r0 = new Rect(position.xMin, position.yMin, 10, position.height);
                var r1 = new Rect(r0.xMax, position.yMin, position.width - 10, position.height);

                ForcePropertyPath.boolValue = EditorGUI.Toggle(r0, ForcePropertyPath.boolValue, EditorStyles.miniButton);

                if (ComponentNameProperty.stringValue == typeof(BindingContext).Name)
                {
                    EditorGUI.BeginChangeCheck();

                    PathProperty.stringValue = EditorGUI.TextField(r1, PathProperty.stringValue);

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (string.IsNullOrEmpty(PathProperty.stringValue))
                            PathProperty.stringValue = "Value";
                        else if (Regex.IsMatch(PathProperty.stringValue, @"\.$", RegexOptions.IgnoreCase))
                            PathProperty.stringValue = PathProperty.stringValue.TrimEnd(".".ToCharArray());
                        else if (!Regex.IsMatch(PathProperty.stringValue, @"^(value$)|(value\.[a-z]+)", RegexOptions.IgnoreCase))
                            PathProperty.stringValue = "Value." + PathProperty.stringValue;
                    }
                }
                else if (ForcePropertyPath.boolValue)
                {
                    EditorGUI.BeginChangeCheck();

                    PathProperty.stringValue = EditorGUI.TextField(r1, PathProperty.stringValue);

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (Regex.IsMatch(PathProperty.stringValue, @"\.$", RegexOptions.IgnoreCase))
                            PathProperty.stringValue = PathProperty.stringValue.TrimEnd(".".ToCharArray());
                    }
                }
                else
                {
                    int selection = Array.FindIndex(ComponentMembers, c => c.text == PathProperty.stringValue);

                    selection = EditorGUI.Popup(r1, Math.Max(selection, 0), ComponentMembers, EditorStyles.popup);

                    if (selection <= 0)
                        PathProperty.stringValue = null;
                    else
                        PathProperty.stringValue = ComponentMembers[selection].text;
                }
            }
        }

        private void DrawComponentMembersField(SerializedProperty endpoint, float columnLabelWidth)
        {
            if (ComponentMembers != null)
            {
                SetDefaultMemberName();

                ForcePropertyPath.boolValue = EditorGUILayout.Toggle(ForcePropertyPath.boolValue, EditorStyles.miniButton, GUILayout.Width(10));

                if (ComponentNameProperty.stringValue == typeof(BindingContext).Name)
                {
                    EditorGUI.BeginChangeCheck();

                    PathProperty.stringValue = EditorGUILayout.TextField(PathProperty.stringValue, GUILayout.Width(columnLabelWidth - 10));

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (string.IsNullOrEmpty(PathProperty.stringValue))
                            PathProperty.stringValue = "Value";
                        else if (Regex.IsMatch(PathProperty.stringValue, @"\.$", RegexOptions.IgnoreCase))
                            PathProperty.stringValue = PathProperty.stringValue.TrimEnd(".".ToCharArray());
                        else if (!Regex.IsMatch(PathProperty.stringValue, @"^(value$)|(value\.[a-z]+)", RegexOptions.IgnoreCase))
                            PathProperty.stringValue = "Value." + PathProperty.stringValue;
                    }
                }
                else if (ForcePropertyPath.boolValue)
                {
                    EditorGUI.BeginChangeCheck();

                    PathProperty.stringValue = EditorGUILayout.TextField(PathProperty.stringValue, GUILayout.Width(columnLabelWidth - 10));

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (Regex.IsMatch(PathProperty.stringValue, @"\.$", RegexOptions.IgnoreCase))
                            PathProperty.stringValue = PathProperty.stringValue.TrimEnd(".".ToCharArray());
                    }
                }
                else
                {
                    int selection = Array.FindIndex(ComponentMembers, c => c.text == PathProperty.stringValue);

                    selection = EditorGUILayout.Popup(Math.Max(selection, 0), ComponentMembers, EditorStyles.popup, GUILayout.Width(columnLabelWidth - 10));

                    if (selection <= 0)
                        PathProperty.stringValue = null;
                    else
                        PathProperty.stringValue = ComponentMembers[selection].text;
                }
            }
        }

        private void SetDefaultComponentName(bool isSource)
        {
            bool isSet = false;

            if (GameObject != null && string.IsNullOrEmpty(ComponentNameProperty.stringValue) && ComponentNameProperty.stringValue != BindingEndpoint.Notset)
            {
                // Reset the Components list so the new value will be found when the control is drawn
                Components = GetComponents(GameObject);

                // Order of preference is top to bottom.  The top component is the most preferred.
                if (isSource)
                    isSet = isSet || SetDefaultComponentName(GameObject, typeof(BindingContext));

                isSet = isSet || SetDefaultComponentName(GameObject, typeof(InputField));
                isSet = isSet || SetDefaultComponentName(GameObject, typeof(Text));
                isSet = isSet || SetDefaultComponentName(GameObject, typeof(Image));
            }
        }

        private bool SetDefaultComponentName(GameObject gameObject, Type componentType)
        {
            bool isSet = false;
            Component component = gameObject.GetComponent(componentType);

            if (component != null)
            {
                ComponentNameProperty.stringValue = componentType.Name;

                isSet = true;
            }

            return isSet;
        }

        private void SetDefaultMemberName()
        {
            if (!string.IsNullOrEmpty(ComponentNameProperty.stringValue) && string.IsNullOrEmpty(PathProperty.stringValue))
            {
                // Reset the ComponentMembers list so the new value will be found when the control is drawn
                ComponentMembers = GetComponentMembers(GameObject, ComponentNameProperty.stringValue);

                if (ComponentNameProperty.stringValue == typeof(BindingContext).Name)
                    PathProperty.stringValue = Utility.GetPropertyName<BindingContext>(x => x.Value);
                else if (ComponentNameProperty.stringValue == typeof(InputField).Name)
                    PathProperty.stringValue = Utility.GetPropertyName<InputField>(x => x.text);
                else if (ComponentNameProperty.stringValue == typeof(Text).Name)
                    PathProperty.stringValue = Utility.GetPropertyName<Text>(x => x.text);
                else if (ComponentNameProperty.stringValue == typeof(Image).Name)
                    PathProperty.stringValue = Utility.GetPropertyName<Image>(x => x.color);
            }
        }

        private GUIContent[] GetComponents(GameObject gameObject)
        {
            List<GUIContent> list = new List<GUIContent>();


            if (gameObject != null)
            {
                foreach (var item in gameObject.GetComponents<Component>())
                {
                    list.Add(new GUIContent(item.GetType().Name));
                }
            }

            list = list.OrderBy(c => c.text).ToList();

            list.Insert(0, new GUIContent(BindingEndpoint.Notset));

            return list.ToArray();
        }

        private GUIContent[] GetComponentMembers(GameObject gameObject, string componentName)
        {
            List<GUIContent> list = new List<GUIContent>();

            if (gameObject != null)
            {
                if (!string.IsNullOrEmpty(componentName) && componentName != BindingEndpoint.Notset)
                {
                    Component component = gameObject.GetComponent(componentName);

                    if (component != null)
                    {
                        foreach (FieldInfo item in component.GetType().GetFields())
                        {
                            list.Add(new GUIContent(item.Name));
                        }

                        foreach (PropertyInfo item in component.GetType().GetProperties())
                        {
                            list.Add(new GUIContent(item.Name));
                        }
                    }
                }
                else
                {
                    foreach (FieldInfo item in gameObject.GetType().GetFields())
                    {
                        list.Add(new GUIContent(item.Name));
                    }

                    foreach (PropertyInfo item in gameObject.GetType().GetProperties())
                    {
                        list.Add(new GUIContent(item.Name));
                    }
                }
            }

            list = list.OrderBy(c => c.text).ToList();

            list.Insert(0, new GUIContent(BindingEndpoint.Notset));

            return list.ToArray();
        }
    }
}
