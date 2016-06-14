// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using UnityEngine;
using UnityEditor;

namespace SimpleBind.Editors
{
    public class SimpleBindEditorMenus
    {
        private static string SimpleBindFoler = "SimpleBind";

        public static string SimpleBindRootPath;

        static SimpleBindEditorMenus()
        {
            FindSimpleBindRootPath();
        }

        public static string FindSimpleBindRootPath()
        {
            SimpleBindRootPath = FindSimpleBindRootPath("Assets");
            return SimpleBindRootPath;
        }

        private static string FindSimpleBindRootPath(string path)
        {
            foreach (var subFolder in AssetDatabase.GetSubFolders(path))
            {
                if (subFolder.EndsWith(SimpleBindFoler))
                {
                    return subFolder;
                }
            }

            foreach (var subFolder in AssetDatabase.GetSubFolders(path))
            {
                var found = FindSimpleBindRootPath(subFolder);
                if (found != string.Empty)
                {
                    return found;
                }
            }

            return string.Empty;
        }


        [MenuItem("GameObject/UI/Simple Bind/Command Button")]
        public static void CreateCommandButton(MenuCommand menuCommand)
        {
            // Create a custom game object
            var prefab = AssetDatabase.LoadAssetAtPath(SimpleBindRootPath + "/Controls/Commands/CommandButton.prefab", typeof (GameObject)) as GameObject;
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "Command Button";
            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [MenuItem("GameObject/UI/Simple Bind/Items Control - Horizontal")]
        public static void CreateItemsControlHorizontal(MenuCommand menuCommand)
        {
            // Create a custom game object
            var prefab = AssetDatabase.LoadAssetAtPath(SimpleBindRootPath + "/Controls/ItemsControl/HorizontalItemsControl.prefab", typeof (GameObject)) as GameObject;
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "Items Control Horizontal";
            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [MenuItem("GameObject/UI/Simple Bind/Items Control - Vertical")]
        public static void CreateItemsControlVertical(MenuCommand menuCommand)
        {
            // Create a custom game object
            var prefab = AssetDatabase.LoadAssetAtPath(SimpleBindRootPath + "/Controls/ItemsControl/VerticalItemsControl.prefab", typeof (GameObject)) as GameObject;
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "Items Control Vertical";
            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [MenuItem("GameObject/UI/Simple Bind/Content Control")]
        public static void CreateContentControl(MenuCommand menuCommand)
        {
            // Create a custom game object
            var prefab = AssetDatabase.LoadAssetAtPath(SimpleBindRootPath + "/Controls/ContentControl/ContentControl.prefab", typeof (GameObject)) as GameObject;
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "Content Control";
            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [MenuItem("GameObject/UI/Simple Bind/List Box")]
        public static void CreateListBox(MenuCommand menuCommand)
        {
            // Create a custom game object
            var prefab = AssetDatabase.LoadAssetAtPath(SimpleBindRootPath + "/Controls/ListBox/ListBox.prefab", typeof (GameObject)) as GameObject;
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "List Box";
            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}
