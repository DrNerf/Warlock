// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using UnityEngine;
using System.Linq;
using UnityEditor;

namespace SimpleBind.Editors
{
    [InitializeOnLoad]
    public class HierarchyVisuals
    {

        public static Texture2D[] BookmarkIcons = new Texture2D[10];

        public static Texture2D PrefabEditorIcon;
        public static Texture2D PrefabIcon;
        public static Texture2D PrefabMidIcon;

        public static string SimpleBindPathRoot;

        static HierarchyVisuals()
        {
            SimpleBindPathRoot = SimpleBindEditorMenus.FindSimpleBindRootPath();

            LoadIcons();

            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        private static void LoadIcons()
        {
            BookmarkIcons[1] = AssetDatabase.LoadAssetAtPath(SimpleBindPathRoot + "/PrefabEditor/Images/bookmark1.png", typeof (Texture2D)) as Texture2D;
            BookmarkIcons[2] = AssetDatabase.LoadAssetAtPath(SimpleBindPathRoot + "/PrefabEditor/Images/bookmark2.png", typeof (Texture2D)) as Texture2D;
            BookmarkIcons[3] = AssetDatabase.LoadAssetAtPath(SimpleBindPathRoot + "/PrefabEditor/Images/bookmark3.png", typeof (Texture2D)) as Texture2D;
            BookmarkIcons[4] = AssetDatabase.LoadAssetAtPath(SimpleBindPathRoot + "/PrefabEditor/Images/bookmark4.png", typeof (Texture2D)) as Texture2D;
            BookmarkIcons[5] = AssetDatabase.LoadAssetAtPath(SimpleBindPathRoot + "/PrefabEditor/Images/bookmark5.png", typeof (Texture2D)) as Texture2D;
            BookmarkIcons[6] = AssetDatabase.LoadAssetAtPath(SimpleBindPathRoot + "/PrefabEditor/Images/bookmark6.png", typeof (Texture2D)) as Texture2D;
            BookmarkIcons[7] = AssetDatabase.LoadAssetAtPath(SimpleBindPathRoot + "/PrefabEditor/Images/bookmark7.png", typeof (Texture2D)) as Texture2D;
            BookmarkIcons[8] = AssetDatabase.LoadAssetAtPath(SimpleBindPathRoot + "/PrefabEditor/Images/bookmark8.png", typeof (Texture2D)) as Texture2D;
            BookmarkIcons[9] = AssetDatabase.LoadAssetAtPath(SimpleBindPathRoot + "/PrefabEditor/Images/bookmark9.png", typeof (Texture2D)) as Texture2D;

            PrefabEditorIcon = AssetDatabase.LoadAssetAtPath(SimpleBindPathRoot + "/PrefabEditor/Images/ic_widgets_white_24dp.png", typeof (Texture2D)) as Texture2D;
            PrefabIcon = AssetDatabase.LoadAssetAtPath(SimpleBindPathRoot + "/PrefabEditor/Images/ic_view_compact_white_24dp.png", typeof (Texture2D)) as Texture2D;
            PrefabMidIcon = AssetDatabase.LoadAssetAtPath(SimpleBindPathRoot + "/PrefabEditor/Images/ic_arrow_downward_white_24dp.png", typeof (Texture2D)) as Texture2D;
        }

        private static void HierarchyWindowItemOnGUI(int instanceid, Rect selectionRect)
        {
            if (SimplePrefabEditor.PrefabEditorsInstanceId == instanceid)
            {
                DrawIcon(PrefabEditorIcon, selectionRect, Color.magenta);
                return;
            }

            if (SimplePrefabEditor.Editors.Any(t => t.PrefabObjectId == instanceid))
            {
                DrawIcon(PrefabIcon, selectionRect, Color.red);
                return;
            }

            if (SimplePrefabEditor.Editors.Any(t => t.MidId1 == instanceid || t.MidId2 == instanceid))
            {
                DrawIcon(PrefabMidIcon, selectionRect, Color.blue);
                return;
            }

            var bookmark = BookmarkEditor.Bookmark.FirstOrDefault(t => t != null && t.InstanceId == instanceid);
            if (bookmark != null)
            {
                DrawIcon(BookmarkIcons[bookmark.BookmarkId], selectionRect, Color.yellow);
                return;
            }
        }

        private static void DrawIcon(Texture2D texture, Rect selectionRect, Color color)
        {
            var oldColor = GUI.color;
            GUI.color = color;

            var r = new Rect(selectionRect);
            r.x = r.width - 20 + r.x;
            r.width = 18;
            GUI.Label(r, texture);

            GUI.color = oldColor;
        }
    }
}
