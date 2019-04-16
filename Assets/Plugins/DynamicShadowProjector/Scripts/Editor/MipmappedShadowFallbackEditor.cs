using UnityEngine;
using UnityEditor;

namespace DynamicShadowProjector.Editor {
	[CustomEditor(typeof(MipmappedShadowFallback))]
	public class MipmappedShadowFallbackEditor : EditorBase {
		private bool m_testFallback = false;
		private int      m_mipLevel;
		private int      m_blurLevel;
		private float    m_blurSize;
		private ShadowTextureRenderer.TextureMultiSample m_multiSampling;
		private ShadowTextureRenderer.TextureSuperSample m_superSampling;
		private int      m_textureWidth;
		private int      m_textureHeight;
		private Material m_projectorMaterial;
		private Shader   m_projectorShader;
		private Projector m_projector;
		const string s_defaultFallbackShader = "DynamicShadowProjector/Projector/Shadow With Linear Falloff";
		void OnEnable()
		{
			MipmappedShadowFallback fallback = target as MipmappedShadowFallback;
			m_projector = fallback.GetComponent<Projector>();
			bool modified = false;
			if (fallback.m_tex2DlodCheckShader == null) {
				fallback.m_tex2DlodCheckShader = Shader.Find("Hidden/DynamicShadowProjector/Caps/tex2Dlod");
				modified = true;
			}
			if (fallback.m_glslCheckShader == null) {
				fallback.m_glslCheckShader = Shader.Find("Hidden/DynamicShadowProjector/Caps/GLSL");
				modified = true;
			}
			if (fallback.m_fallbackShaderOrMaterial == null) {
				fallback.m_fallbackShaderOrMaterial = Shader.Find(s_defaultFallbackShader);
				modified = true;
			}
			if (modified) {
				EditorUtility.SetDirty(fallback);
				serializedObject.Update();
			}
		}
		void TestFallback()
		{
			if (!m_testFallback) {
				MipmappedShadowFallback fallback = target as MipmappedShadowFallback;
				ShadowTextureRenderer shadowRenderer = fallback.GetComponent<ShadowTextureRenderer>();
				m_mipLevel = shadowRenderer.mipLevel;
				m_blurLevel = shadowRenderer.blurLevel;
				m_blurSize = shadowRenderer.blurSize;
				m_multiSampling = shadowRenderer.multiSampling;
				m_superSampling = shadowRenderer.superSampling;
				m_textureWidth = shadowRenderer.textureWidth;
				m_textureHeight = shadowRenderer.textureHeight;
				m_projectorShader = m_projector.material.shader;
				m_projectorMaterial = m_projector.material;
				m_testFallback = true;
				fallback.ApplyFallback(m_projector);
				shadowRenderer.hideFlags |= HideFlags.NotEditable;
			}
		}
		void RestoreShadowTextureRenderer()
		{
			if (m_testFallback) {
				m_testFallback = false;
				MipmappedShadowFallback fallback = target as MipmappedShadowFallback;
				ShadowTextureRenderer shadowRenderer = fallback.GetComponent<ShadowTextureRenderer>();
				shadowRenderer.hideFlags &= ~HideFlags.NotEditable;
				shadowRenderer.mipLevel = m_mipLevel;
				shadowRenderer.blurLevel = m_blurLevel;
				shadowRenderer.blurSize = m_blurSize;
				shadowRenderer.multiSampling = m_multiSampling;
				shadowRenderer.superSampling = m_superSampling;
				shadowRenderer.textureWidth = m_textureWidth;
				shadowRenderer.textureHeight = m_textureHeight;
				m_projector.material = m_projectorMaterial;
				m_projector.material.shader = m_projectorShader;
				m_testFallback = false;
			}
		}
		void OnDisable()
		{
			RestoreShadowTextureRenderer();
		}
		public override void OnInspectorGUI ()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Online Document");
			if (GUILayout.Button("<color=blue>http://nyahoon.com/products/dynamic-shadow-projector/mipmapped-shadow-fallback-component</color>", richTextStyle)) {
				Application.OpenURL("http://nyahoon.com/products/dynamic-shadow-projector/mipmapped-shadow-fallback-component");
			}
			EditorGUILayout.EndHorizontal();
			bool isGUIEnabled = GUI.enabled;
			SerializedProperty prop = serializedObject.FindProperty("m_fallbackShaderOrMaterial");
			Object newObject = EditorGUILayout.ObjectField("Fallback Shader/Material", prop.objectReferenceValue, typeof(Object), false);
			if (newObject != prop.objectReferenceValue) {
				if (newObject == null) {
					prop.objectReferenceValue = Shader.Find(s_defaultFallbackShader);
				}
				else if (newObject is Shader || newObject is Material) {
					prop.objectReferenceValue = newObject;
				}
			}

			prop = serializedObject.FindProperty("m_blurLevel");
			EditorGUILayout.IntPopup(prop, s_blurLevelDisplayOption, s_blurLevelOption);
			++EditorGUI.indentLevel;
			GUI.enabled = isGUIEnabled && 0 < prop.intValue;
			EditorGUILayout.Slider(serializedObject.FindProperty("m_blurSize"), 1.0f, 4.0f);
			GUI.enabled = isGUIEnabled;
			--EditorGUI.indentLevel;

			prop = serializedObject.FindProperty("m_modifyTextureSize");
			EditorGUILayout.PropertyField(prop);
			++EditorGUI.indentLevel;
			GUI.enabled = isGUIEnabled && prop.boolValue;
			EditorGUILayout.IntPopup(serializedObject.FindProperty("m_textureWidth"), s_textureSizeDisplayOption, s_textureSizeOption);
			EditorGUILayout.IntPopup(serializedObject.FindProperty("m_textureHeight"), s_textureSizeDisplayOption, s_textureSizeOption);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_multiSampling"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_superSampling"));
			GUI.enabled = isGUIEnabled;
			--EditorGUI.indentLevel;

			EditorGUILayout.Separator();
			if (m_projector.material != null) {
				bool test = GUILayout.Toggle(m_testFallback, "Test Fallback");
				if (test && !m_testFallback) {
					TestFallback();
				}
				else if (m_testFallback && !test) {
					RestoreShadowTextureRenderer();
				}
			}
			if (serializedObject.ApplyModifiedProperties()) {
				if (m_testFallback) {
					MipmappedShadowFallback fallback = target as MipmappedShadowFallback;
					fallback.ApplyFallback(m_projector);
				}
			}
		}
	}
}
