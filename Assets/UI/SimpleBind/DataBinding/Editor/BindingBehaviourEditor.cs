// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using SimpleBind.DataBinding;

namespace SimpleBind.Editors
{
    [CustomEditor(typeof (BindingBehaviour))]
    public class BindingBehaviourEditor : Editor, IBindingBehaviourEditor
    {
        //public const string Notset = "None";

        private List<BindingEndpointDrawer> _sourceEndpointDrawers = null;
        private List<BindingEndpointDrawer> _targetEndpointDrawers = null;

        private List<BindingEndpointDrawer> SourceEndpointDrawers
        {
            get
            {
                if (_sourceEndpointDrawers == null)
                    _sourceEndpointDrawers = new List<BindingEndpointDrawer>();

                return _sourceEndpointDrawers;
            }
        }

        private List<BindingEndpointDrawer> TargetEndpointDrawers
        {
            get
            {
                if (_targetEndpointDrawers == null)
                    _targetEndpointDrawers = new List<BindingEndpointDrawer>();

                return _targetEndpointDrawers;
            }
        }

        private GUIContent[] ValueConverters { get; set; }
        private GUIContent[] MultiValueConverters { get; set; }

        private SerializedProperty SourceEndpoints { get; set; }
        private SerializedProperty TargetEndpoints { get; set; }
        private SerializedProperty Direction { get; set; }
        private SerializedProperty ConverterName { get; set; }
        private SerializedProperty ConverterParameter { get; set; }

        public void OnEnable()
        {
            //Debug.Log("OnEnable");	
            ValueConverters = GetConverters<IValueConverter>();
            MultiValueConverters = GetConverters<IMultiValueConverter>();

            SerializedProperty binding = serializedObject.FindProperty("_binding");

            SourceEndpoints = binding.FindPropertyRelative("_sourceEndpoints");
            TargetEndpoints = binding.FindPropertyRelative("_targetEndpoints");
            Direction = binding.FindPropertyRelative("_direction");
            ConverterName = binding.FindPropertyRelative("_converterName").FindPropertyRelative("_value");
            ConverterParameter = binding.FindPropertyRelative("_converterParameter").FindPropertyRelative("_value");
        }

        public void OnDisable()
        {
            //Debug.Log("OnDisable Editor");
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

            for (int i = 0; i < SourceEndpoints.arraySize; i++)
            {
                SerializedProperty endpoint = SourceEndpoints.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();
                GetBindingEndpointDrawer(i, SourceEndpointDrawers).Draw(this, endpoint, i, true, SourceEndpoints.arraySize > 1);
                EditorGUILayout.EndHorizontal();
            }

            if (SourceEndpoints.arraySize > 1 || TargetEndpoints.arraySize > 1)
                EditorGUILayout.Space();

            for (int i = 0; i < TargetEndpoints.arraySize; i++)
            {
                SerializedProperty endpoint = TargetEndpoints.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();
                GetBindingEndpointDrawer(i, TargetEndpointDrawers).Draw(this, endpoint, i, false, TargetEndpoints.arraySize > 1);
                EditorGUILayout.EndHorizontal();
            }

            if (SourceEndpoints.arraySize > 1 || TargetEndpoints.arraySize > 1)
                EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Direction"), GUILayout.Width(GetRowLabelWidth()));
            DrawDirectionField();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Converter", GUILayout.Width(GetRowLabelWidth()));
            DrawConvertersField();
            DrawConverterParameterField(columnWidth);
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDirectionField()
        {
            BindingDirections selected = GetCurrentDirection(Direction);

            if (selected == BindingDirections.Both)
            {
                if (SourceEndpointDrawers.Count > 1)
                    selected = BindingDirections.SourceToTarget;
                else if (TargetEndpointDrawers.Count > 1)
                    selected = BindingDirections.TargetToSource;
            }

            Direction.enumValueIndex = (int) (BindingDirections) EditorGUILayout.EnumPopup(selected);
        }

        private BindingDirections GetCurrentDirection(SerializedProperty directionProperty)
        {
            return (BindingDirections) System.Enum.GetValues(typeof (BindingDirections)).GetValue(directionProperty.enumValueIndex);
        }

        private void DrawConverterParameterField(float columnWidth)
        {
            ConverterParameter.stringValue = EditorGUILayout.TextField(ConverterParameter.stringValue, GUILayout.Width(columnWidth));
        }

        private void DrawConvertersField()
        {
            GUIContent[] converters = null;
            BindingDirections direction = GetCurrentDirection(Direction);

            if (SourceEndpointDrawers.Count > 1 && direction == BindingDirections.SourceToTarget)
                converters = MultiValueConverters;
            else if (TargetEndpointDrawers.Count > 1 && direction == BindingDirections.TargetToSource)
                converters = MultiValueConverters;
            else
                converters = ValueConverters;

            if (converters != null)
            {
                int selection = System.Array.FindIndex(converters, c => c.text == GetDisplayTypeName(ConverterName.stringValue));

                selection = EditorGUILayout.Popup(System.Math.Max(selection, 0), converters, EditorStyles.popup);

                if (selection <= 0)
                    ConverterName.stringValue = null;
                else
                    ConverterName.stringValue = GetFullTypeName(converters[selection].text);
            }
        }

        private string GetDisplayTypeName(string fullTypeName)
        {
            string result = null;

            // See GetFullTypeName(). Format: "class.namespace"
            Match match = Regex.Match(fullTypeName, @"^(?'namespace'.+(?=\.))\.(?'class'.+)$", RegexOptions.Singleline);

            if (match.Success)
                result = string.Format("{0} ({1})", match.Groups["class"], match.Groups["namespace"]);
            else
                result = fullTypeName;

            return result;
        }

        private string GetFullTypeName(string displayTypeName)
        {
            string result = null;

            // See GetDisplayTypeName(). Format: "class (namespace)"
            Match match = Regex.Match(displayTypeName, @"^(?'class'[^\s]*)\s\((?'namespace'[^\)]*)\)$", RegexOptions.Singleline);

            if (match.Success)
                result = string.Format("{0}.{1}", match.Groups["namespace"], match.Groups["class"]);
            else
                result = displayTypeName;

            return result;
        }

        private BindingEndpointDrawer GetBindingEndpointDrawer(int index, List<BindingEndpointDrawer> bindingEndpointDrawers)
        {
            BindingEndpointDrawer bindingEndpointDrawer = null;

            if (index >= 0)
            {
                if (index < bindingEndpointDrawers.Count)
                    bindingEndpointDrawer = bindingEndpointDrawers[index];
                else if (index == bindingEndpointDrawers.Count)
                {
                    bindingEndpointDrawer = new BindingEndpointDrawer();
                    bindingEndpointDrawers.Add(bindingEndpointDrawer);
                }
            }

            return bindingEndpointDrawer;
        }

        private GUIContent[] GetConverters<T>()
        {
            Assembly assembly = Assembly.GetAssembly(typeof (T));

            List<GUIContent> list = assembly.GetTypes()
                .Where(t => t.GetInterface(typeof (T).Name) != null)
                .Select(t => new GUIContent(GetDisplayTypeName(t.FullName)))
                .OrderBy(c => c.text)
                .ToList();

            list.Insert(0, new GUIContent(BindingEndpoint.Notset));

            return list.ToArray();
        }

        public void DuplicateBindingEndpoint(bool isSource, int index)
        {
            List<BindingEndpoint> endpoints = null;
            BindingEndpoint newEndpoint = null;
            Binding binding = ((BindingBehaviour) target).Binding;

            if (isSource)
                endpoints = binding.SourceEndpoints;
            else
                endpoints = binding.TargetEndpoints;

            newEndpoint = new BindingEndpoint();
            newEndpoint.Object.Value = endpoints[index].Object.Value;
            newEndpoint.ComponentName.Value = endpoints[index].ComponentName.Value;

            endpoints.Add(newEndpoint);
        }

        public void RemoveBindingEndpoint(bool isSource, int index, bool hasMultiple)
        {
            Binding binding = ((BindingBehaviour) target).Binding;

            if (hasMultiple)
            {
                List<BindingEndpoint> endpoints = null;
                List<BindingEndpointDrawer> drawers = null;

                if (isSource)
                {
                    endpoints = binding.SourceEndpoints;
                    drawers = SourceEndpointDrawers;
                }
                else
                {
                    endpoints = binding.TargetEndpoints;
                    drawers = TargetEndpointDrawers;
                }

                endpoints[index].Dispose();
                endpoints.RemoveAt(index);
                drawers.RemoveAt(index);
            }
        }

        public float GetRowLabelWidth()
        {
            return 70;
        }

        public float GetColumnLabelWidth()
        {
            int rightMargin = 30;
            int miniButtonGroupWidth = 45;
            float columnNumber = 3.0f;

            return ((EditorGUIUtility.currentViewWidth - miniButtonGroupWidth - GetRowLabelWidth() - rightMargin)/columnNumber);
        }
    }
}
