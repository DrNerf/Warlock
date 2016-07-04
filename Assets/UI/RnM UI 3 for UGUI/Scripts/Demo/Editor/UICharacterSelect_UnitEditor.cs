using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

namespace UnityEditor.UI
{
	[CustomEditor(typeof(UICharacterSelect_Unit), true)]
	public class UICharacterSelect_UnitEditor : ToggleEditor {
		
		private SerializedProperty nameTextComponentProperty;
		private SerializedProperty nameColorsProperty;
		private SerializedProperty raceTextComponentProperty;
		private SerializedProperty raceColorsProperty;
		private SerializedProperty classTextComponentProperty;
		private SerializedProperty classColorsProperty;
		private SerializedProperty levelTextComponentProperty;
		private SerializedProperty levelLabelTextComponentProperty;
		private SerializedProperty levelColorsProperty;
		private SerializedProperty deleteButtonProperty;
		private SerializedProperty deleteButtonAlwaysVisibleProperty;
		private SerializedProperty deleteButtonFadeDurationProperty;
		private SerializedProperty m_Separator;
		
		protected override void OnEnable()
		{
			base.OnEnable();
			
			this.nameTextComponentProperty = this.serializedObject.FindProperty("m_NameTextComponent");
			this.nameColorsProperty = this.serializedObject.FindProperty("m_NameColors");
			this.raceTextComponentProperty = this.serializedObject.FindProperty("m_RaceTextComponent");
			this.raceColorsProperty = this.serializedObject.FindProperty("m_RaceColors");
			this.classTextComponentProperty = this.serializedObject.FindProperty("m_ClassTextComponent");
			this.classColorsProperty = this.serializedObject.FindProperty("m_ClassColors");
			this.levelTextComponentProperty = this.serializedObject.FindProperty("m_LevelTextComponent");
			this.levelLabelTextComponentProperty = this.serializedObject.FindProperty("m_LevelLabelTextComponent");
			this.levelColorsProperty = this.serializedObject.FindProperty("m_LevelColors");
			this.deleteButtonProperty = this.serializedObject.FindProperty("m_DeleteButton");
			this.deleteButtonAlwaysVisibleProperty = this.serializedObject.FindProperty("m_DeleteButtonAlwaysVisible");
			this.deleteButtonFadeDurationProperty = this.serializedObject.FindProperty("m_DeleteButtonFadeDuration");
			this.m_Separator = this.serializedObject.FindProperty("m_Separator");
		}
		
		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.LabelField("Name Layout Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUILayout.PropertyField(nameTextComponentProperty, new GUIContent("Name Text"));
			if (nameTextComponentProperty.objectReferenceValue != null)
			{
				EditorGUILayout.PropertyField(this.nameColorsProperty, true);
			}
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.LabelField("Race Layout Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUILayout.PropertyField(this.raceTextComponentProperty, new GUIContent("Race Text"));
			if (this.raceTextComponentProperty.objectReferenceValue != null)
			{
				EditorGUILayout.PropertyField(this.raceColorsProperty, true);
			}
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.LabelField("Class Layout Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUILayout.PropertyField(this.classTextComponentProperty, new GUIContent("Class Text"));
			if (this.classTextComponentProperty.objectReferenceValue != null)
			{
				EditorGUILayout.PropertyField(this.classColorsProperty, true);
			}
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.LabelField("Level Layout Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUILayout.PropertyField(this.levelTextComponentProperty, new GUIContent("Level Text"));
			EditorGUILayout.PropertyField(this.levelLabelTextComponentProperty, new GUIContent("Level Label Text"));
			if (this.levelTextComponentProperty.objectReferenceValue != null || this.levelLabelTextComponentProperty.objectReferenceValue != null)
			{
				EditorGUILayout.PropertyField(this.levelColorsProperty, true);
			}
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.LabelField("Separator Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUILayout.PropertyField(this.m_Separator, new GUIContent("Game Object"));
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.LabelField("Delete Button Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUILayout.PropertyField(this.deleteButtonProperty, new GUIContent("Button"));
			EditorGUILayout.PropertyField(this.deleteButtonAlwaysVisibleProperty, new GUIContent("Always Visible"));
			if (this.deleteButtonAlwaysVisibleProperty.boolValue != true)
				EditorGUILayout.PropertyField(this.deleteButtonFadeDurationProperty, new GUIContent("Fade Duration"));
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			
			this.serializedObject.ApplyModifiedProperties();
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.LabelField("Toggle Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			base.OnInspectorGUI();
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
		}
	}
}