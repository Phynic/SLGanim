//
// ShadowTextureRendererEditor.cs
//
// Dynamic Shadow Projector
//
// Copyright 2015 NYAHOON GAMES PTE. LTD. All Rights Reserved.
//

using UnityEngine;
using UnityEditor;

namespace DynamicShadowProjector.Editor {
	[CustomEditor(typeof(ShadowTextureRenderer))]
	public class ShadowTextureRendererEditor : EditorBase {
		static bool s_showAdvancedOptions = false;
		void OnEnable()
		{
			ShadowTextureRenderer shadowRenderer = target as ShadowTextureRenderer;
			bool modified = false;
			if (shadowRenderer.blurShader == null) {
				shadowRenderer.blurShader = FindMaterial("DynamicShadowProjector/Blit/Blur");
				ClearMaterialProperties(shadowRenderer.blurShader);
				modified = true;
			}
			if (shadowRenderer.downsampleShader == null) {
				shadowRenderer.downsampleShader = FindMaterial("DynamicShadowProjector/Blit/Downsample");
				ClearMaterialProperties(shadowRenderer.downsampleShader);
				modified = true;
			}
			if (shadowRenderer.copyMipmapShader == null) {
				shadowRenderer.copyMipmapShader = FindMaterial("DynamicShadowProjector/Blit/CopyMipmap");
				ClearMaterialProperties(shadowRenderer.copyMipmapShader);
				modified = true;
			}
			if (shadowRenderer.eraseShadowShader == null) {
				shadowRenderer.eraseShadowShader = FindMaterial("DynamicShadowProjector/Blit/EraseShadow");
				ClearMaterialProperties(shadowRenderer.eraseShadowShader);
				modified = true;
			}
			Projector projector = shadowRenderer.GetComponent<Projector>();
			if (projector.material != null) {
				RemoveUnusedMaterialProperties(projector.material);
			}
			if (modified) {
				EditorUtility.SetDirty(shadowRenderer);
				serializedObject.Update();
			}
		}
		private SerializedObject m_cameraSerializedObject = null;
		private Camera m_camera;
		public override void OnInspectorGUI ()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Online Document");
			if (GUILayout.Button("<color=blue>http://nyahoon.com/products/dynamic-shadow-projector/shadow-texture-renderer-component</color>", richTextStyle)) {
				Application.OpenURL("http://nyahoon.com/products/dynamic-shadow-projector/shadow-texture-renderer-component");
			}
			EditorGUILayout.EndHorizontal();
			bool isGUIEnabled = GUI.enabled;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_shadowColor"));
			EditorGUILayout.IntPopup(serializedObject.FindProperty("m_textureWidth"), s_textureSizeDisplayOption, s_textureSizeOption);
			EditorGUILayout.IntPopup(serializedObject.FindProperty("m_textureHeight"), s_textureSizeDisplayOption, s_textureSizeOption);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_multiSampling"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_superSampling"));
			SerializedProperty prop = serializedObject.FindProperty("m_blurLevel");
			EditorGUILayout.IntPopup(prop, s_blurLevelDisplayOption, s_blurLevelOption);
			++EditorGUI.indentLevel;
			GUI.enabled = isGUIEnabled && 0 < prop.intValue;
			ShadowTextureRenderer shadowRenderer = target as ShadowTextureRenderer;
			EditorGUILayout.Slider(serializedObject.FindProperty("m_blurSize"), 1.0f, shadowRenderer.blurFilter == ShadowTextureRenderer.BlurFilter.Uniform ? 6.0f : 4.0f);
			GUI.enabled = isGUIEnabled;
			--EditorGUI.indentLevel;
			prop = serializedObject.FindProperty("m_mipLevel");
			EditorGUILayout.PropertyField(prop);
			++EditorGUI.indentLevel;
			GUI.enabled = isGUIEnabled && 0 < prop.intValue;
			SerializedProperty fastBlur = serializedObject.FindProperty("m_singlePassMipmapBlur");
			EditorGUILayout.PropertyField(fastBlur);
			float maxBlurSize;
			if (fastBlur.boolValue) {
				maxBlurSize = 1.0f;
			}
			else {
				maxBlurSize = shadowRenderer.blurFilter == ShadowTextureRenderer.BlurFilter.Uniform ? 3.0f : 2.0f;
			}
			EditorGUILayout.Slider(serializedObject.FindProperty("m_mipmapBlurSize"), 0.0f, maxBlurSize);
			if (shadowRenderer.GetComponent<MipmappedShadowFallback>() == null) {
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Add Fallback Component")) {
					Undo.AddComponent<MipmappedShadowFallback>(shadowRenderer.gameObject);
				}
				EditorGUILayout.EndHorizontal();
				GUI.enabled = isGUIEnabled;
				--EditorGUI.indentLevel;
			}
			else if (prop.intValue == 0) {
				GUI.enabled = isGUIEnabled;
				--EditorGUI.indentLevel;
				EditorGUILayout.BeginHorizontal();
				GUILayout.TextArea("<color=red>Still has Mipmap Fallback Component!</color>", richTextStyle);
				if (GUILayout.Button("Remove the Component")) {
					Undo.DestroyObjectImmediate(shadowRenderer.GetComponent<MipmappedShadowFallback>());
				}
				EditorGUILayout.EndHorizontal();
			}
			else {
				GUI.enabled = isGUIEnabled;
				--EditorGUI.indentLevel;
			}
			prop = serializedObject.FindProperty("m_testViewClip");
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Don't update while out of view");
			prop.boolValue = EditorGUILayout.Toggle(prop.boolValue);
			EditorGUILayout.EndHorizontal();
			++EditorGUI.indentLevel;
			GUI.enabled = isGUIEnabled && prop.boolValue;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_camerasForViewClipTest"), true);
			GUI.enabled = isGUIEnabled;
			--EditorGUI.indentLevel;

			s_showAdvancedOptions = GUILayout.Toggle(s_showAdvancedOptions, "Show Advanced Options");
			if (s_showAdvancedOptions) {
				++EditorGUI.indentLevel;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_blurFilter"));
				prop = serializedObject.FindProperty("m_mipmapFalloff");
				EditorGUILayout.PropertyField(prop);
				if (prop.intValue == (int)ShadowTextureRenderer.MipmapFalloff.Custom) {
					prop = serializedObject.FindProperty("m_customMipmapFalloff");
					if (shadowRenderer.customMipmapFalloff != null && shadowRenderer.customMipmapFalloff.Length != prop.arraySize) {
						serializedObject.Update();
						prop = serializedObject.FindProperty("m_customMipmapFalloff");
					}
					EditorGUILayout.PropertyField(prop, true);
				}
				float near = EditorGUILayout.FloatField("Near Clip Plane", shadowRenderer.cameraNearClipPlane);
				if (m_cameraSerializedObject == null) {
					m_camera = shadowRenderer.GetComponent<Camera>();
					m_cameraSerializedObject = new SerializedObject(shadowRenderer.GetComponent<Camera>());
				}
				if (near != shadowRenderer.cameraNearClipPlane) {
					Undo.RecordObject(m_camera, "Inspector");
					shadowRenderer.cameraNearClipPlane = near;
				}
				bool bShowCamera = (m_camera.hideFlags & HideFlags.HideInInspector) == 0;
				bool newValue = EditorGUILayout.Toggle("Show Camera in Inspector", bShowCamera);
				if (bShowCamera != newValue) {
					if (newValue) {
						m_camera.hideFlags &= ~HideFlags.HideInInspector;
					}
					else {
						m_camera.hideFlags |= HideFlags.HideInInspector;
					}
				}
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_blurShader"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_downsampleShader"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_copyMipmapShader"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_preferredTextureFormats"), true);
				--EditorGUI.indentLevel;
			}
			serializedObject.ApplyModifiedProperties();
		}
		private Material   m_blitMaterial;
		private Vector3    m_lastPosition;
		private Quaternion m_lastRotation;
		void OnSceneGUI()
		{
			ShadowTextureRenderer shadowRenderer = target as ShadowTextureRenderer;
			if (shadowRenderer.shadowTexture == null || shadowRenderer.GetComponent<Projector>().material == null) {
				return;
			}

			if (m_lastPosition != shadowRenderer.transform.position || m_lastRotation != shadowRenderer.transform.rotation) {
				// This function call will have the following error on Unity 5.6.0:
				// Rendering camera 'XXXX', but calling code does not set it up as current camera (current camera: 'Main Camera') UnityEngine.Camera:Render()
				// But it seems like there are no way to avoid this error message.
				shadowRenderer.ForceRenderTexture();
				m_lastPosition = shadowRenderer.transform.position;
				m_lastRotation = shadowRenderer.transform.rotation;
			}

			if (m_blitMaterial == null) {
				m_blitMaterial = FindMaterial("DynamicShadowProjector/Blit/Blit");
			}
			int mipLevel = shadowRenderer.mipLevel + 1;
			int w = shadowRenderer.textureWidth;
			int h = shadowRenderer.textureHeight;
			for (int i = 1; i < mipLevel; ++i) {
				if ((w >> i) <= 4 || (h >> i) <=4) {
					mipLevel = i;
					break;
				}
			}
			int displayWidth = 128;
			float mipBias = Mathf.Log(displayWidth/w)/Mathf.Log(2.0f);
			int displayHeight = h*displayWidth/w;
			int marginSize = 4;
			int windowWidth = mipLevel * (displayWidth + marginSize) + marginSize;
			int windowHeight = displayHeight + 2*GUI.skin.window.border.bottom + marginSize;
			int windowPosX = Screen.width - windowWidth - 10;
			int windowPosY = Screen.height - windowHeight - GUI.skin.window.border.top - 10;
			GUI.WindowFunction func = id => {
				if (Event.current.type.Equals(EventType.Repaint)) {
					int x = marginSize;
					int y = GUI.skin.window.border.top;
					for (int i = 0; i < mipLevel; ++i) {
						m_blitMaterial.SetFloat("_MipLevel", i);
						m_blitMaterial.SetFloat("_MipBias", mipBias + i);
						Graphics.DrawTexture(new Rect(x, y, displayWidth, displayHeight), shadowRenderer.shadowTexture, m_blitMaterial);
						x += displayWidth + marginSize;
					}
				}
			};
			GUI.Window(0, new Rect(windowPosX, windowPosY, windowWidth, windowHeight), func, "Shadow Texture");
		}
	}
}
