// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using UnityEditor;
using UnityEngine;

namespace SimpleBind.Editors
{
    public class PrefabEditorPopup : EditorWindow
    {
        private bool setFocus = true;
        private bool isClosing = false;

        public void OnInspectorUpdate()
        {
            if (SimplePrefabEditor.GetCurrentEditor() == null)
            {
                isClosing = true;
                this.Close();
            }

            // This will only get called 10 times per second.
            if (!isClosing)
            {
                Repaint();
            }
        }

        void OnGUI()
        {
            if (setFocus)
            {
                this.Focus();
                setFocus = false;
            }

            var editor = SimplePrefabEditor.GetCurrentEditor();

            if ((Event.current.isKey && Event.current.keyCode == KeyCode.Escape) || editor == null)
            {
                isClosing = true;
                this.Close();
                return;
            }

            EditorGUILayout.LabelField("Editing: " + editor.PrefabObject.name, EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("Apply Changes (A)") || (Event.current.isKey && Event.current.keyCode == KeyCode.A))
            {
                var parentPrefab = PrefabUtility.GetPrefabParent(editor.PrefabObject);
                PrefabUtility.ReplacePrefab(editor.PrefabObject, parentPrefab, ReplacePrefabOptions.ConnectToPrefab);
                CheckActionAndDestoryState(editor);
                isClosing = true;
                this.Close();
            }
            GUILayout.Space(2);

            if (GUILayout.Button("Revert Prefab State (R)") || (Event.current.isKey && Event.current.keyCode == KeyCode.R))
            {
                PrefabUtility.RevertPrefabInstance(editor.PrefabObject);
                CheckActionAndDestoryState(editor);
                isClosing = true;
                this.Close();
            }
            GUILayout.Space(2);

            GUI.enabled = true;

            if (GUILayout.Button("Deleted Editor (D)") || (Event.current.isKey && Event.current.keyCode == KeyCode.D))
            {
                // TODO: SoftSource: criddle: Add Warning for changes are in place
                DestroyImmediate(editor.RootObject);
                CheckActionAndDestoryState(editor);
                isClosing = true;
                this.Close();
            }
            GUILayout.Space(2);

            if (GUILayout.Button("Close (Esc)"))
            {
                isClosing = true;
                this.Close();
            }
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Ctrl + Action - Perform Action and Delete Editor", EditorStyles.wordWrappedLabel);
        }

        private void CheckActionAndDestoryState(PrefabEditor editor)
        {
            if (Event.current.control)
            {
                DestroyImmediate(editor.RootObject);
            }
        }
    }
}