// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SimpleBind.DataBinding;
using SimpleBind.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleBind.Editors
{
    public class CallbackBindingEndpointDrawer
    {
        private GUIContent[] Components { get; set; }
        private GUIContent[] ComponentMembers { get; set; }

        private SerializedProperty ObjectProperty { get; set; }
        private SerializedProperty ComponentNameProperty { get; set; }
        private SerializedProperty PathProperty { get; set; }

        private GameObject GameObject
        {
            get { return ObjectProperty.objectReferenceValue as GameObject; }
        }

        public void Draw(SerializedProperty endpoint, string label, float rowLabelWidth, float columnLabelWidth)
        {
            ObjectProperty = endpoint.FindPropertyRelative("_object").FindPropertyRelative("_value");
            ComponentNameProperty = endpoint.FindPropertyRelative("_componentName").FindPropertyRelative("_value");
            PathProperty = endpoint.FindPropertyRelative("_path").FindPropertyRelative("_value");

            EditorGUILayout.LabelField(new GUIContent(label), GUILayout.Width(rowLabelWidth));

            DrawObjectField(endpoint, columnLabelWidth);
            DrawComponentsField(endpoint, true, columnLabelWidth);
            DrawComponentMembersField(endpoint, columnLabelWidth);
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

        private void DrawComponentsField(SerializedProperty endpoint, bool isSource, float columnLabelWidth)
        {
            if (Components != null)
            {
                SetDefaultComponentName(isSource);

                int selection = Array.FindIndex(Components, c => c.text == ComponentNameProperty.stringValue);

                EditorGUI.BeginChangeCheck();

                selection = EditorGUILayout.Popup(Math.Max(selection, 0), Components, EditorStyles.popup, GUILayout.Width(columnLabelWidth));

                if (selection <= 0)
                    ComponentNameProperty.stringValue = null;
                else
                    ComponentNameProperty.stringValue = Components[selection].text;

                if (EditorGUI.EndChangeCheck() || ComponentMembers == null)
                {
                    ComponentMembers = GetComponentMembers(GameObject, ComponentNameProperty.stringValue);
                }
            }
        }

        private void DrawComponentMembersField(SerializedProperty endpoint, float columnLabelWidth)
        {
            if (ComponentMembers != null)
            {
                SetDefaultMemberName();

                if (ComponentNameProperty.stringValue == typeof(BindingContext).Name)
                {
                    EditorGUI.BeginChangeCheck();

                    PathProperty.stringValue = EditorGUILayout.TextField(PathProperty.stringValue, GUILayout.Width(columnLabelWidth));

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
                else
                {
                    int selection = Array.FindIndex(ComponentMembers, c => c.text == PathProperty.stringValue);

                    selection = EditorGUILayout.Popup(Math.Max(selection, 0), ComponentMembers, EditorStyles.popup, GUILayout.Width(columnLabelWidth));

                    PathProperty.stringValue = selection <= 0 ? null : ComponentMembers[selection].text;
                }
            }
        }

        private void SetDefaultComponentName(bool isSource)
        {
            bool isSet = false;

            if (GameObject != null && string.IsNullOrEmpty(ComponentNameProperty.stringValue))
            {
                // Reset the Components list so the new value will be found when the control is drawn
                Components = GetComponents(GameObject);

                // Order of preference is top to bottom.  The top component is the most preferred.
                if (isSource)
                    isSet = isSet || SetDefaultComponentName(GameObject, typeof(BindingContext));

                isSet = isSet || SetDefaultComponentName(GameObject, typeof(InputField));
                isSet = isSet || SetDefaultComponentName(GameObject, typeof(Text));
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
                Component component = gameObject.GetComponent(componentName);

                if (component != null)
                {
                    list.AddRange(component.GetType().GetMethods()
                        .Where(t => !t.Name.StartsWith("get_") && !t.Name.StartsWith("set_"))
                        .Select(item => new GUIContent(item.Name)));
                }
            }

            list = list.OrderBy(c => c.text).ToList();

            list.Insert(0, new GUIContent(BindingEndpoint.Notset));

            return list.ToArray();
        }
    }
}
