// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SimpleBind.Editors
{
    public static class BookmarkEditor
    {
        private static readonly Bookmark[] m_bookmarks = new Bookmark[10];

        public static Bookmark[] Bookmark
        {
            get { return m_bookmarks; }
        }

        static BookmarkEditor()
        {
            LoadBookmarks();
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 1 &1", false)]
        public static void Goto1()
        {
            GotoGameObject(m_bookmarks[1].GameObject);
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 1 &1", true)]
        public static bool Goto1Validate()
        {
            return m_bookmarks[1] != null;
        }

        [MenuItem("GameObject/Simple Bookmarks/Set 1 &#1", false)]
        public static void Set1()
        {
            Set(1);
        }

        private static void Set(int id) 
        {
            var go = Selection.activeObject as GameObject;
            if (go != null && !IsInProject(go))
            {
                if (m_bookmarks[id] != null && m_bookmarks[id].GameObject == go)
                {
                    m_bookmarks[id] = null;
                    EditorPrefs.DeleteKey("SimpleBookMark" + id);
                }
                else
                {
                    // If the GO is already bookmarked then remove the old bookmark before adding the new one
                    for (int i = 0; i < m_bookmarks.Length; i++)
                    {
                        if (m_bookmarks[i] != null && m_bookmarks[i].GameObject == go)
                        {
                            m_bookmarks[i] = null;
                        }
                    }

                    m_bookmarks[id] = new Bookmark
                    {
                        BookmarkId = id,
                        InstanceId = go.GetInstanceID(),
                        GameObject = go
                    };

                    EditorPrefs.SetInt("SimpleBookMark" + id, m_bookmarks[id].InstanceId);

                    GotoGameObject(go);
                }
            }
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 2 &2", false)]
        public static void Goto2()
        {
            GotoGameObject(m_bookmarks[2].GameObject);
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 2 &2", true)]
        public static bool Goto2Validate()
        {
            return m_bookmarks[2] != null;
        }

        [MenuItem("GameObject/Simple Bookmarks/Set 2 &#2", false)]
        public static void Set2()
        {
            Set(2);
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 3 &3", false)]
        public static void Goto3()
        {
            GotoGameObject(m_bookmarks[3].GameObject);
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 3 &3", true)]
        public static bool Goto3Validate()
        {
            return m_bookmarks[3] != null;
        }

        [MenuItem("GameObject/Simple Bookmarks/Set 3 &#3", false)]
        public static void Set3()
        {
            Set(3);
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 4 &4", false)]
        public static void Goto4()
        {
            GotoGameObject(m_bookmarks[4].GameObject);
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 4 &4", true)]
        public static bool Goto4Validate()
        {
            return m_bookmarks[4] != null;
        }

        [MenuItem("GameObject/Simple Bookmarks/Set 4 &#4", false)]
        public static void Set4()
        {
            Set(4);
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 5 &5", false)]
        public static void Goto5()
        {
            GotoGameObject(m_bookmarks[5].GameObject);
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 5 &5", true)]
        public static bool Goto5Validate()
        {
            return m_bookmarks[5] != null;
        }

        [MenuItem("GameObject/Simple Bookmarks/Set 5 &#5", false)]
        public static void Set5()
        {
            Set(5);
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 6 &6", false)]
        public static void Goto6()
        {
            GotoGameObject(m_bookmarks[6].GameObject);
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 6 &6", true)]
        public static bool Goto6Validate()
        {
            return m_bookmarks[6] != null;
        }

        [MenuItem("GameObject/Simple Bookmarks/Set 6 &#6", false)]
        public static void Set6()
        {
            Set(6);
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 7 &7", false)]
        public static void Goto7()
        {
            GotoGameObject(m_bookmarks[7].GameObject);
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 7 &7", true)]
        public static bool Goto7Validate()
        {
            return m_bookmarks[7] != null;
        }

        [MenuItem("GameObject/Simple Bookmarks/Set 7 &#7", false)]
        public static void Set7()
        {
            Set(7);
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 8 &8", false)]
        public static void Goto8()
        {
            GotoGameObject(m_bookmarks[8].GameObject);
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 8 &8", true)]
        public static bool Goto8Validate()
        {
            return m_bookmarks[8] != null;
        }

        [MenuItem("GameObject/Simple Bookmarks/Set 8 &#8", false)]
        public static void Set8()
        {
            Set(8);
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 9 &9", false)]
        public static void Goto9()
        {
            GotoGameObject(m_bookmarks[9].GameObject);
        }

        [MenuItem("GameObject/Simple Bookmarks/Goto 9 &9", true)]
        public static bool Goto9Validate()
        {
            return m_bookmarks[9] != null;
        }

        [MenuItem("GameObject/Simple Bookmarks/Set 9 &#9", false)]
        public static void Set9()
        {
            Set(9);
        }

        [MenuItem("GameObject/Simple Bookmarks/Clear All &#0", false)]
        public static void Goto0()
        {
            for (int i = 0; i < m_bookmarks.Length; i++)
            {
                m_bookmarks[i] = null;
                EditorPrefs.DeleteKey("SimpleBookMark" + i);
            }
        }

        [MenuItem("GameObject/Simple Bookmarks/Clear All &#0", true)]
        public static bool Goto0Validate()
        {
            return m_bookmarks.Any();
        }

        private static void GotoGameObject(GameObject go)
        {
            EditorGUIUtility.PingObject(go);
            Selection.activeGameObject = go;
            SceneView.lastActiveSceneView.FrameSelected();
        }

        private static bool IsInProject(GameObject prefab)
        {
            if (PrefabUtility.GetPrefabType(prefab) == PrefabType.None)
                return false;
            return true;
        }

        private static void LoadBookmarks()
        {
            for (int i = 0; i <= 9; i++)
            {
                var id = EditorPrefs.GetInt("SimpleBookMark" + i, -1);
                if (id != -1)
                {
                    var go = EditorUtility.InstanceIDToObject(id) as GameObject;
                    if (go != null)
                    {
                        m_bookmarks[i] = new Bookmark
                        {
                            BookmarkId = i,
                            InstanceId = id,
                            GameObject = go
                        };
                    }
                }
            }
        }
    }
}
