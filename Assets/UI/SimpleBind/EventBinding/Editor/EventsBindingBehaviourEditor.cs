// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using SimpleBind.DataBinding;
using SimpleBind.EventBinding;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SimpleBind.Editors
{
    [CustomEditor(typeof (EventBindingBehaviour))]
    public class EventsBindingBehaviourEditor : Editor, IBindingBehaviourEditor
    {
        public const string NOTSET = "None";

        private List<CallbackParameterDrawer> _parameterEndpointDrawers = null;

        private List<CallbackParameterDrawer> ParameterEndpointDrawers
        {
            get
            {
                if (_parameterEndpointDrawers == null)
                    _parameterEndpointDrawers = new List<CallbackParameterDrawer>();

                return _parameterEndpointDrawers;
            }
        }

        private SerializedProperty SourceEndpoint { get; set; }
        private SerializedProperty TargetEndpoint { get; set; }

        private SerializedProperty CallbackParameters { get; set; }

        public void OnEnable()
        {
            //Debug.Log("OnEnable");	
            SerializedProperty binding = serializedObject.FindProperty("_eventBinding");

            SourceEndpoint = binding.FindPropertyRelative("_sourceEndpoint");
            TargetEndpoint = binding.FindPropertyRelative("_targetEndpoint");
            CallbackParameters = binding.FindPropertyRelative("_callbackParameters");
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

            EditorGUILayout.BeginHorizontal();
            var sourceDrawer = new EventBindingEndpointDrawer();
            sourceDrawer.Draw(SourceEndpoint, "Event", GetRowLabelWidth(), GetColumnLabelWidth());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            var targetDrawer = new CallbackBindingEndpointDrawer();
            targetDrawer.Draw(TargetEndpoint, "Callback", GetRowLabelWidth(), GetColumnLabelWidth());
            EditorGUILayout.EndHorizontal();

            if (CallbackParameters.arraySize > 0)
            {
                for (int i = 0; i < CallbackParameters.arraySize; i++)
                {
                    SerializedProperty endpoint = CallbackParameters.GetArrayElementAtIndex(i);

                    EditorGUILayout.BeginHorizontal();
                    GetBindingEndpointDrawer(i, ParameterEndpointDrawers).Draw(this, endpoint, i, CallbackParameters.arraySize > 1);
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Params"), GUILayout.Width(GetRowLabelWidth()));
                if (GUILayout.Button("Add Parameters", EditorStyles.miniButton, GUILayout.Width(100)))
                {
                    var binding = ((EventBindingBehaviour) target).EventBinding;
                    binding.CallbackParameters.Add(new CallbackParameter());
                    binding.TargetEndpoint.ParameterCount.Value = binding.CallbackParameters.Count;
                }

                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private CallbackParameterDrawer GetBindingEndpointDrawer(int index, List<CallbackParameterDrawer> bindingEndpointDrawers)
        {
            CallbackParameterDrawer bindingEndpointDrawer = null;

            if (index >= 0)
            {
                if (index < bindingEndpointDrawers.Count)
                    bindingEndpointDrawer = bindingEndpointDrawers[index];
                else if (index == bindingEndpointDrawers.Count)
                {
                    bindingEndpointDrawer = new CallbackParameterDrawer();
                    bindingEndpointDrawers.Add(bindingEndpointDrawer);
                }
            }

            return bindingEndpointDrawer;
        }

        public void DuplicateBindingEndpoint(bool isSource, int index)
        {
            var binding = ((EventBindingBehaviour) target).EventBinding;
            var parameters = binding.CallbackParameters;

            var parameter = new CallbackParameter
            {
                IsStatic = parameters[index].IsStatic,
                StaticValue = parameters[index].StaticValue,
            };

            if (parameters[index].Binding != null)
            {
                parameter.Binding = new BindingEndpoint
                {
                    Object = {Value = parameters[index].Binding.Object.Value},
                    ComponentName = {Value = parameters[index].Binding.ComponentName.Value}
                };
            }

            parameters.Add(parameter);

            binding.TargetEndpoint.ParameterCount.Value = parameters.Count;
        }

        public void RemoveBindingEndpoint(bool isSource, int index, bool hasMultiple)
        {
            var binding = ((EventBindingBehaviour) target).EventBinding;

            var parameters = binding.CallbackParameters;

            parameters[index].Dispose();
            parameters.RemoveAt(index);
            _parameterEndpointDrawers.RemoveAt(index);
            binding.TargetEndpoint.ParameterCount.Value = parameters.Count;
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
