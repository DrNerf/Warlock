// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using UnityEditor;
using UnityEngine;

namespace SimpleBind.Editors
{
    public class PrefabEditor
    {
        public GameObject RootObject;
        public int RootObjectInstanceId = -1;
        public GameObject PrefabObject;
        public int PrefabObjectId = -1;
        public int MidId1 = -1;
        public int MidId2 = -1;

        public string ToEditorPrefs()
        {
            return string.Format("{0}!{1}!{2}!{3}", RootObjectInstanceId, PrefabObjectId, MidId1, MidId2);
        }

        public static PrefabEditor FromEditorPrefs(string editorPrefs)
        {
            var splits = editorPrefs.Split('!');
            if (splits.Length != 4)
            {
                return null;
            }

            var result = new PrefabEditor();
            int.TryParse(splits[0], out result.RootObjectInstanceId);
            int.TryParse(splits[1], out result.PrefabObjectId);
            int.TryParse(splits[2], out result.MidId1);
            int.TryParse(splits[3], out result.MidId2);

            result.RootObject = EditorUtility.InstanceIDToObject(result.RootObjectInstanceId) as GameObject;
            result.PrefabObject = EditorUtility.InstanceIDToObject(result.PrefabObjectId) as GameObject;

            return result;
        }
    }
}