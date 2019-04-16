//
// DrawTargetObjectEditor.cs
//
// Dynamic Shadow Projector
//
// Copyright 2015 NYAHOON GAMES PTE. LTD. All Rights Reserved.
//

using UnityEngine;
using UnityEditor;

namespace DynamicShadowProjector.Editor {
	[CustomEditor(typeof(DrawTargetObject))]
	public class DrawTargetObjectEditor : EditorBase {
		void OnEnable()
		{
			DrawTargetObject component = target as DrawTargetObject;
			if (component.shadowShader == null && (component.replacementShaders == null || component.replacementShaders.Length == 0)) {
				component.shadowShader = FindMaterial("DynamicShadowProjector/Shadow/Opaque");
				component.replacementShaders = new DrawTargetObject.ReplaceShader[2];
				component.replacementShaders[0].renderType = "Transparent";
				component.replacementShaders[0].shader = Shader.Find("DynamicShadowProjector/Shadow/Transparent");
				component.replacementShaders[1].renderType = "TransparentCutout";
				component.replacementShaders[1].shader = component.replacementShaders[0].shader;
				serializedObject.Update();
				EditorUtility.SetDirty(component);
			}
		}
		static bool s_showAdvancedOptions = false;
		public override void OnInspectorGUI ()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Online Document");
			if (GUILayout.Button("<color=blue>http://nyahoon.com/products/dynamic-shadow-projector/draw-target-object-component</color>", richTextStyle)) {
				Application.OpenURL("http://nyahoon.com/products/dynamic-shadow-projector/draw-target-object-component");
			}
			EditorGUILayout.EndHorizontal();
			bool isGUIEnabled = GUI.enabled;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_target"));
			SerializedProperty prop = serializedObject.FindProperty("m_renderChildren");
			EditorGUILayout.PropertyField(prop);
			++EditorGUI.indentLevel;
			GUI.enabled = isGUIEnabled && prop.boolValue;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_layerMask"));
			GUI.enabled = isGUIEnabled;
			--EditorGUI.indentLevel;
			prop = serializedObject.FindProperty("m_textureAlignment");
			EditorGUILayout.PropertyField(prop);
			bool bUpdate = prop.intValue != (int)DrawTargetObject.TextureAlignment.None;
			prop = serializedObject.FindProperty("m_followTarget");
			EditorGUILayout.PropertyField(prop);
			bUpdate = bUpdate || prop.boolValue;
			++EditorGUI.indentLevel;
			GUI.enabled = isGUIEnabled && bUpdate;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_updateFunction"));
			GUI.enabled = isGUIEnabled;
			--EditorGUI.indentLevel;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_targetDirection"));

			s_showAdvancedOptions = GUILayout.Toggle(s_showAdvancedOptions, "Show Advanced Options");
			if (s_showAdvancedOptions) {
				++EditorGUI.indentLevel;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_shadowShader"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_replacementShaders"), true);
				--EditorGUI.indentLevel;
			}
			serializedObject.ApplyModifiedProperties();
		}

	}
}
