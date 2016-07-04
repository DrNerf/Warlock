// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SimpleBind.Utilities;
using UnityEditor;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SimpleBind.Editors
{
    [InitializeOnLoad]
    public class SimplePrefabEditor
    {
        private const int m_editorSpaceingY = 100;

        private static int m_currentEditorIndex = 0;
        private static List<PrefabEditor> m_editors = new List<PrefabEditor>();

        private static float m_currentEditorY = -50000;
        private const string PrefabEditorName = "Prefab Editors";

        public static int PrefabEditorsInstanceId { get; set; }

        public static List<PrefabEditor> Editors
        {
            get { return m_editors; }
        }

        static SimplePrefabEditor()
        {
            var prefabEditor = GameObject.Find(PrefabEditorName);
            if (prefabEditor != null)
            {
                PrefabEditorsInstanceId = prefabEditor.GetInstanceID();
            }

            LoadEditorPrefs();

            Selection.selectionChanged += SelectionChanged;
        }

        private static void SelectionChanged()
        {
            for (int i = 0; i < m_editors.Count; i++)
            {
                var editor = m_editors[i];
                if (editor != null && IsGameObjectInEditor(editor, Selection.activeGameObject))
                {
                    m_currentEditorIndex = i;
                    // Debug.Log("Editor Index Changed: " + i);
                    return;
                }
            }

            m_currentEditorIndex = -1;
        }

        private static bool IsGameObjectInEditor(PrefabEditor editor, GameObject activeGameObject)
        {
            if (activeGameObject == null)
            {
                return false;
            }

            if (activeGameObject == editor.RootObject)
            {
                return true;
            }

            return activeGameObject.transform.parent != null && IsGameObjectInEditor(editor, activeGameObject.transform.parent.gameObject);

        }

        public static PrefabEditor GetCurrentEditor()
        {
            if (m_editors.Count > 0 && m_currentEditorIndex >= 0 && m_currentEditorIndex < m_editors.Count)
            {
                return m_editors[m_currentEditorIndex];
            }

            return null;
        }

#if UNITY_EDITOR_OSX
        [MenuItem("Assets/Simple Prefab Editor/Current Editor Options &#e", false, 10005)]
#else
        [MenuItem("Assets/Simple Prefab Editor/Current Editor Options %#e", false, 10005)]
#endif
        public static void ShowCurrentEditorOptions()
        {
            if (GetCurrentEditor() != null && SceneView.lastActiveSceneView != null)
            {
                var sceneViewPosision = SceneView.lastActiveSceneView.position;
                PrefabEditorPopup window = ScriptableObject.CreateInstance<PrefabEditorPopup>();
                window.position = new Rect(sceneViewPosision.x + (sceneViewPosision.width/2) - 125, sceneViewPosision.y + 30, 250, 160);
                window.ShowPopup();
            }
        }

#if UNITY_EDITOR_OSX
        [MenuItem("Assets/Simple Prefab Editor/Next Editor &q", false, 10005)]
#else
        [MenuItem("Assets/Simple Prefab Editor/Next Editor %q", false, 10005)]
#endif
        public static void NextEditor(MenuCommand menuCommand)
        {
            // Debug.Log("Next Prefab");
            if (m_editors.Count == 0)
            {
                return;
            }

            m_currentEditorIndex++;
            if (m_currentEditorIndex >= m_editors.Count)
            {
                m_currentEditorIndex = 0;
            }

            var editor = m_editors[m_currentEditorIndex];

            // This removes invalid editors that were not removed correctly
            if (editor.PrefabObject == null)
            {
                m_editors.RemoveAt(m_currentEditorIndex);
                NextEditor(menuCommand);
            }
            else
            {
                GotoGameObject(editor.PrefabObject);
            }
        }

#if UNITY_EDITOR_OSX
        [MenuItem("Assets/Simple Prefab Editor/Previous Editor &w", false, 10006)]
#else
        [MenuItem("Assets/Simple Prefab Editor/Previous Editor %w", false, 10006)]
#endif
        public static void PreviousEditor(MenuCommand menuCommand)
        {
            // Debug.Log("Previous Prefab");
            if (m_editors.Count == 0)
            {
                return;
            }

            m_currentEditorIndex--;
            if (m_currentEditorIndex < 0)
            {
                m_currentEditorIndex = m_editors.Count - 1;
            }

            var editor = m_editors[m_currentEditorIndex];
            // This removes invalid editors that were not removed correctly
            if (editor.PrefabObject == null)
            {
                m_editors.RemoveAt(m_currentEditorIndex);
                PreviousEditor(menuCommand);
            }
            else
            {
                GotoGameObject(editor.PrefabObject);
            }
        }

#if UNITY_EDITOR_OSX
        [MenuItem("Assets/Simple Prefab Editor/Edit Prefab &e", false, 10000)]
#else
        [MenuItem("Assets/Simple Prefab Editor/Edit Prefab %e", false, 10000)]
#endif
        public static void EditView(MenuCommand menuCommand)
        {
            var prefab = Selection.activeObject as GameObject;

            if (prefab == null || !IsInProject(prefab))
            {
                return;
            }

            var name = "Editor: " + prefab.name;
            var existing = GameObject.Find(name);

            if (existing != null)
            {
                var existingPrefab = existing.transform.GetChild(0).GetChild(0);
                GotoGameObject(existingPrefab.gameObject);
                return;
            }

            var prefabEditor = GameObject.Find(PrefabEditorName);

            if (prefabEditor == null)
            {
                prefabEditor = new GameObject(PrefabEditorName);
                prefabEditor.transform.position = new Vector3(50000, 0, 0);
                prefabEditor.AddComponent<DisableOnStart>();
                PrefabEditorsInstanceId = prefabEditor.GetInstanceID();
            }

            var cameraGo = new GameObject(name);
            var camera = cameraGo.AddComponent<Camera>();
            camera.targetDisplay = 7;
            cameraGo.transform.position = new Vector3(0, m_currentEditorY += m_editorSpaceingY, -10);

            cameraGo.transform.SetParent(prefabEditor.transform, false);

            if (prefab.GetComponent<RectTransform>() != null)
            {
                camera.orthographic = true;

                var canvas = new GameObject("Editor Canvas");
                canvas.AddComponent<RectTransform>();
                var c = canvas.AddComponent<Canvas>();
                c.renderMode = RenderMode.ScreenSpaceCamera;
                c.worldCamera = camera;
                var cs = canvas.AddComponent<CanvasScaler>();
                cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                cs.referenceResolution = new Vector2(1280, 800);
                GameObjectUtility.SetParentAndAlign(canvas, cameraGo);
                var go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                go.transform.SetParent(canvas.transform, false);
                var findAndFrame = go.AddComponent<SingleActionOnUpdate>();
                findAndFrame.OnUpdateAction = () =>
                {
                    EditorApplication.delayCall += () =>
                    {
                        GotoGameObject(go);
                        Object.DestroyImmediate(findAndFrame);
                    };
                };

                var editor = new PrefabEditor
                {
                    RootObject = cameraGo,
                    RootObjectInstanceId = cameraGo.GetInstanceID(),
                    PrefabObject = go,
                    PrefabObjectId = go.GetInstanceID(),
                    MidId1 = cameraGo.GetInstanceID(),
                    MidId2 = canvas.GetInstanceID()
                };

                m_editors.Add(editor);
                // Debug.Log("Editor Added: " + m_editors.Count);
                m_currentEditorIndex = m_editors.Count - 1;
                var onDestroyAction = cameraGo.AddComponent<SingleActionOnDestroy>();
                onDestroyAction.OnDestroyAction = () => EditorDestroyed(editor);
            }
            else
            {
                var go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                go.transform.SetParent(cameraGo.transform, false);
                var findAndFrame = go.AddComponent<SingleActionOnUpdate>();
                findAndFrame.OnUpdateAction = () =>
                {
                    EditorApplication.delayCall += () =>
                    {
                        GotoGameObject(go);
                        Object.DestroyImmediate(findAndFrame);
                    };
                };

                var editor = new PrefabEditor
                {
                    RootObject = cameraGo,
                    RootObjectInstanceId = cameraGo.GetInstanceID(),
                    PrefabObject = go,
                    PrefabObjectId = go.GetInstanceID(),
                    MidId1 = cameraGo.GetInstanceID()
                };

                m_editors.Add(editor);
                // Debug.Log("Editor Added: " + m_editors.Count);
                m_currentEditorIndex = m_editors.Count - 1;
                var onDestroyAction = cameraGo.AddComponent<SingleActionOnDestroy>();
                onDestroyAction.OnDestroyAction = () => EditorDestroyed(editor);
            }

            SaveEditorPrefs();

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(prefab, "Edit View " + prefab.name);
        }

        private static void SaveEditorPrefs()
        {
            var editorPrefs = string.Join("|", m_editors.Select(t => t.ToEditorPrefs()).ToArray());
            // Debug.Log("Save Editor Prefs: " + editorPrefs);
            EditorPrefs.SetString("SimplePrefabEditor", editorPrefs);
        }

        private static void LoadEditorPrefs()
        {
            var editorPrefs = EditorPrefs.GetString("SimplePrefabEditor");

            // Debug.Log("Load Editor Prefs: " + editorPrefs);

            var splits = editorPrefs.Split('|');

            foreach (var split in splits)
            {
                var editor = PrefabEditor.FromEditorPrefs(split);
                if (editor != null && editor.RootObject != null)
                {
                    if (m_currentEditorY + m_editorSpaceingY <= editor.RootObject.transform.position.y)
                    {
                        m_currentEditorY = Mathf.CeilToInt(editor.RootObject.transform.position.y + m_editorSpaceingY);
                    }
                    m_editors.Add(editor);
                    // Debug.Log("Editor Added (Load): " + m_editors.Count);
                }
            }
        }

        private static bool IsInProject(GameObject prefab)
        {
            if (PrefabUtility.GetPrefabType(prefab) == PrefabType.None)
                return false;
            else
                return true;
        }

        private static void EditorDestroyed(PrefabEditor editor)
        {
            var index = m_editors.IndexOf(editor);
            if (index >= 0)
            {
                m_editors.RemoveAt(index);
                // Debug.Log("Editor Remove: " + m_editors.Count);
                if (index == m_currentEditorIndex)
                {
                    NextEditor(null);
                }
            }

            SaveEditorPrefs();

            var prefabEditor = GameObject.Find(PrefabEditorName);

            if (prefabEditor != null && m_editors.Count == 0)
            {
                // Must delay destorying the parent object until the child is destroyed.
                // Add a SingleActionOnUpdate to the PrefabEditor and use it to call Destroy on next frame
                EditorApplication.delayCall += () =>
                {
                    Object.DestroyImmediate(prefabEditor);
                };
            }
        }

        [MenuItem("Assets/Edit View", true)]
        private static bool EditViewValidation()
        {
            // This returns true when the selected object is a Texture2D (the menu item will be disabled otherwise).
            return Selection.activeObject.GetType() == typeof (GameObject);
        }

        private static void GotoGameObject(GameObject go)
        {
            EditorGUIUtility.PingObject(go);
            Selection.activeGameObject = go;
            SceneView.lastActiveSceneView.FrameSelected();
        }
    }
}
