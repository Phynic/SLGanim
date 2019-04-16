//
// ShadowTextureRenderer.cs
//
// Dynamic Shadow Projector
//
// Copyright 2015 NYAHOON GAMES PTE. LTD. All Rights Reserved.
//

using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace DynamicShadowProjector {
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Projector))]
	public class ShadowTextureRenderer : MonoBehaviour {
		public enum TextureMultiSample {
			x1 = 1,
			x2 = 2,
			x4 = 4,
			x8 = 8,
		}
		public enum TextureSuperSample {
			x1 = 1,
			x4 = 2,
			x16 = 4,
		}
		public enum MipmapFalloff {
			None = 0,
			Linear,
			Custom,
		}
		public enum BlurFilter {
			Uniform = 0,
			Gaussian,
		}
		// Serialize Fields
		[SerializeField]
		private TextureMultiSample m_multiSampling = TextureMultiSample.x4;
		[SerializeField]
		private TextureSuperSample m_superSampling = TextureSuperSample.x1;
		[SerializeField]
		private MipmapFalloff m_mipmapFalloff = MipmapFalloff.Linear;
		[SerializeField]
		private BlurFilter m_blurFilter = BlurFilter.Uniform;
		[SerializeField]
		private bool m_testViewClip = true;
		[SerializeField]
		private int m_textureWidth = 64;
		[SerializeField]
		private int m_textureHeight = 64;
		[SerializeField]
		private int m_mipLevel = 0;
		[SerializeField]
		private int m_blurLevel = 1;
		[SerializeField]
		private float m_blurSize = 3;
		[SerializeField]
		private float m_mipmapBlurSize = 0;
		[SerializeField]
		private bool m_singlePassMipmapBlur = false;
		[SerializeField]
		private Color m_shadowColor = new Color(0,0,0,1);
		[SerializeField]
		private Material m_blurShader;
		[SerializeField]
		private Material m_downsampleShader;
		[SerializeField]
		private Material m_copyMipmapShader;
		[SerializeField]
		private Material m_eraseShadowShader;
		[SerializeField]
		private float[] m_customMipmapFalloff;
		[SerializeField]
		private RenderTextureFormat[] m_preferredTextureFormats;
		[SerializeField]
		private Camera[] m_camerasForViewClipTest;

		// public properties
		public TextureMultiSample multiSampling
		{
			get { return m_multiSampling; }
			set {
				if (m_multiSampling != value) {
					m_multiSampling = value;
					SetTexturePropertyDirty();
				}
			}
		}
		public TextureSuperSample superSampling
		{
			get { return m_superSampling; }
			set {
				if (m_superSampling != value) {
					bool b = useIntermediateTexture;
					m_superSampling = value;
					if (b != useIntermediateTexture && m_multiSampling != TextureMultiSample.x1) {
						SetTexturePropertyDirty();
					}
				}
			}
		}
		public int textureWidth
		{
			get { return m_textureWidth; }
			set {
				if (m_textureWidth != value) {
					m_textureWidth = value;
					SetTexturePropertyDirty();
				}
			}
		}
		public int textureHeight
		{
			get { return m_textureHeight; }
			set {
				if (m_textureHeight != value) {
					m_textureHeight = value;
					SetTexturePropertyDirty();
				}
			}
		}
		public int mipLevel
		{
			get { return m_mipLevel; }
			set {
				if (m_mipLevel != value) {
					if (m_mipLevel == 0 || value == 0) {
						SetTexturePropertyDirty();
					}
					m_mipLevel = value;
				}
			}
		}
		public int blurLevel
		{
			get { return m_blurLevel; }
			set
			{
				if (m_blurLevel != value) {
					bool b = useIntermediateTexture;
					m_blurLevel = value;
					if (b != useIntermediateTexture && m_multiSampling != TextureMultiSample.x1) {
						SetTexturePropertyDirty();
					}
				}
			}
		}
		public float blurSize
		{
			get { return m_blurSize; }
			set { m_blurSize = value; }
		}
		public BlurFilter blurFilter
		{
			get { return m_blurFilter; }
			set { m_blurFilter = value; }
		}
		public float mipmapBlurSize
		{
			get { return m_mipmapBlurSize; }
			set
			{
				m_mipmapBlurSize = value;
			}
		}
		public bool singlePassMipmapBlur {
			get { return m_singlePassMipmapBlur; }
			set { m_singlePassMipmapBlur = value; }
		}
		public MipmapFalloff mipmapFalloff
		{
			get { return m_mipmapFalloff; }
			set
			{
				m_mipmapFalloff = value;
			}
		}
		public float[] customMipmapFalloff
		{
			get { return m_customMipmapFalloff; }
			set
			{
				m_customMipmapFalloff = value;
			}
		}
		public Color shadowColor
		{
			get { return m_shadowColor; }
			set {
				if (m_shadowColor != value) {
					bool b = useIntermediateTexture;
					m_shadowColor = value;
					if (b != useIntermediateTexture && m_multiSampling != TextureMultiSample.x1) {
						SetTexturePropertyDirty();
					}
				}
			}
		}
		public Material blurShader
		{
			get { return m_blurShader; }
			set
			{
				m_blurShader = value;
			}
		}
		public Material downsampleShader
		{
			get { return m_downsampleShader; }
			set { m_downsampleShader = value; }
		}
		public Material copyMipmapShader
		{
			get { return m_copyMipmapShader; }
			set
			{
				m_copyMipmapShader = value;
			}
		}
		public Material eraseShadowShader
		{
			get { return m_eraseShadowShader; }
			set { m_eraseShadowShader = value; }
		}
		public RenderTexture shadowTexture
		{
			get { return m_shadowTexture; }
		}
		public bool testViewClip
		{
			get { return m_testViewClip; }
			set { m_testViewClip = value; }
		}
		public Camera[] camerasForViewClipTest
		{
			get { return m_camerasForViewClipTest; }
			set { m_camerasForViewClipTest = value; }
		}
		public float cameraNearClipPlane
		{
			get {
				if (m_camera == null) {
					Initialize();
				}
				return m_camera.nearClipPlane;
			}
			set {
				if (m_camera == null) {
					Initialize();
				}
				m_camera.nearClipPlane = value;
			}
		}
		public LayerMask cameraCullingMask
		{
			get {
				if (m_camera == null) {
					Initialize();
				}
				return m_camera.cullingMask;
			}
			set {
				if (m_camera == null) {
					Initialize();
				}
				m_camera.cullingMask = value;
			}
		}
		public void SetReplacementShader(Shader shader, string replacementTag)
		{
			if (m_camera == null) {
				Initialize();
			}
			if (shader != null) {
				m_camera.SetReplacementShader(shader, replacementTag);
			}
			else {
				m_camera.ResetReplacementShader();
			}
		}

		private static int s_falloffParamID;
		private static int s_blurOffsetHParamID;
		private static int s_blurOffsetVParamID;
		private static int s_blurWeightHParamID;
		private static int s_blurWeightVParamID;
		private static int s_downSampleBlurOffset0ParamID;
		private static int s_downSampleBlurOffset1ParamID;
		private static int s_downSampleBlurOffset2ParamID;
		private static int s_downSampleBlurOffset3ParamID;
		private static int s_downSampleBlurWeightParamID;
		private Projector m_projector;
		private Material m_projectorMaterial;
		private CommandBuffer m_commandBuffer;
		private RenderTexture m_shadowTexture;
		[SerializeField][HideInInspector]
		private Camera m_camera;
		private bool m_isTexturePropertyChanged;
		private bool m_isVisible = false;
		private bool m_shadowTextureValid = false;

		public bool isProjectorVisible
		{
			get { return m_isVisible; }
		}

		// Call SetCommandBufferDirty or UpdateCommandBuffer when child objects are added/deleted/disabled/enabled.
		public void SetTexturePropertyDirty()
		{
			m_isTexturePropertyChanged = true;
		}
		public void CreateRenderTexture()
		{
			if (m_textureWidth <= 0 || m_textureHeight <= 0 || m_projector == null) {
				return;
			}
			// choose a texture format
			RenderTextureFormat textureFormat = RenderTextureFormat.ARGB32;
			if (m_preferredTextureFormats != null && 0 < m_preferredTextureFormats.Length) {
				foreach (RenderTextureFormat format in m_preferredTextureFormats) {
					if (SystemInfo.SupportsRenderTextureFormat(textureFormat)) {
						textureFormat = format;
					}
				}
			}
			// create texture
			if (m_shadowTexture != null) {
				if (m_camera != null) {
					m_camera.targetTexture = null;
				}
				DestroyImmediate(m_shadowTexture);
			}
			m_shadowTexture = new RenderTexture(m_textureWidth, m_textureHeight, 0, textureFormat, RenderTextureReadWrite.Linear);
			if (useIntermediateTexture) {
				m_shadowTexture.antiAliasing = 1;
			}
			else {
				m_shadowTexture.antiAliasing = (int)m_multiSampling;
			}
			if (0 < m_mipLevel) {
				m_shadowTexture.useMipMap = true;
#if UNITY_5_5_OR_NEWER
				m_shadowTexture.autoGenerateMips = false;
#else
				m_shadowTexture.generateMips = false;
#endif
				m_shadowTexture.mipMapBias = 0.0f;
				m_shadowTexture.filterMode = FilterMode.Trilinear;
			}
			else {
				m_shadowTexture.useMipMap = false;
				m_shadowTexture.filterMode = FilterMode.Bilinear;
			}
			m_shadowTexture.wrapMode = TextureWrapMode.Clamp;
			m_shadowTexture.Create();
			m_shadowTextureValid = false;
			if (m_projector.material != null) {
				m_projector.material.SetTexture("_ShadowTex", m_shadowTexture);
				m_projector.material.SetFloat("_DSPMipLevel", m_mipLevel);
			}
			if (m_camera != null) {
				m_camera.targetTexture = m_shadowTexture;
			}
			m_isTexturePropertyChanged = false;
		}
		public void AddCommandBuffer(CommandBuffer commandBuffer)
		{
			m_camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, commandBuffer); // just in case
			m_camera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, commandBuffer);
		}

		public void RemoveCommandBuffer(CommandBuffer commandBuffer)
		{
			m_camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, commandBuffer);
		}

		static void InitializeShaderPropertyIDs()
		{
			s_falloffParamID = Shader.PropertyToID("_Falloff");
			s_blurOffsetHParamID = Shader.PropertyToID("_OffsetH");
			s_blurOffsetVParamID = Shader.PropertyToID("_OffsetV");
			s_blurWeightHParamID = Shader.PropertyToID("_WeightH");
			s_blurWeightVParamID = Shader.PropertyToID("_WeightV");
			s_downSampleBlurOffset0ParamID = Shader.PropertyToID("_Offset0");
			s_downSampleBlurOffset1ParamID = Shader.PropertyToID("_Offset1");
			s_downSampleBlurOffset2ParamID = Shader.PropertyToID("_Offset2");
			s_downSampleBlurOffset3ParamID = Shader.PropertyToID("_Offset3");
			s_downSampleBlurWeightParamID = Shader.PropertyToID("_Weight");
		}

		bool useIntermediateTexture
		{
			get {
				return m_superSampling != TextureSuperSample.x1 || 0 < m_blurLevel || HasShadowColor() || (0 < m_mipLevel && m_multiSampling != TextureMultiSample.x1);
			}
		}
		bool Initialize()
		{
			m_isVisible = false;
			if (IsInitialized()) { 
				return true;
			}
			m_isTexturePropertyChanged = true;
			InitializeShaderPropertyIDs();
			m_projector = GetComponent<Projector>();
			CloneProjectorMaterialIfShared();
			if (m_camera == null) {
				m_camera = gameObject.GetComponent<Camera>();
				if (m_camera == null) {
					m_camera = gameObject.AddComponent<Camera>();
				}
				m_camera.hideFlags = HideFlags.HideInInspector;
			}
			else {
				m_camera.RemoveAllCommandBuffers();
			}
			m_camera.depth = -100;
			m_camera.cullingMask = 0;
			m_camera.clearFlags = CameraClearFlags.Nothing;
			m_camera.backgroundColor = new Color(1,1,1,0);
			m_camera.useOcclusionCulling = false;
			m_camera.renderingPath = RenderingPath.Forward;
			m_camera.nearClipPlane = 0.01f;
#if UNITY_5_6_OR_NEWER
			m_camera.forceIntoRenderTexture = true;
#endif
			m_camera.enabled = true;
			CreateRenderTexture();
			return true;
		}

		bool IsInitialized()
		{
			return m_projector != null && m_camera != null;
		}

		void Awake()
		{
			Initialize();
		}

		void OnEnable()
		{
			if (m_camera != null) {
				m_camera.enabled = true;
			}
		}

		void OnDisable()
		{
			if (m_camera != null) {
				m_camera.enabled = false;
			}
		}

		void Start()
		{
#if UNITY_EDITOR
			if (m_testViewClip && Application.isPlaying)
#else
			if (m_testViewClip)
#endif
			{
				if (m_camerasForViewClipTest == null || m_camerasForViewClipTest.Length == 0) {
					if (Camera.main != null) {
						m_camerasForViewClipTest = new Camera[1] {Camera.main};
					}
				}
			}
		}

		void OnValidate()
		{
			CreateRenderTexture();
			InitializeShaderPropertyIDs();
			// check custom mipmap falloff
			if (m_mipmapFalloff == MipmapFalloff.Custom && 0 < m_mipLevel) {
				if (m_customMipmapFalloff == null || m_customMipmapFalloff.Length == 0) {
					m_customMipmapFalloff = new float[m_mipLevel];
					for (int i = 0; i < m_mipLevel; ++i) {
						m_customMipmapFalloff[i] = ((float)(m_mipLevel - i))/(float)(m_mipLevel + 1);
					}
				}
				else if (m_mipLevel != m_customMipmapFalloff.Length) {
					float[] customFalloff = new float[m_mipLevel];
					for (int i = 0; i < m_mipLevel; ++i) {
						float oldIndex = ((float)(m_customMipmapFalloff.Length + 1)*(i + 1))/(float)(m_mipLevel + 1);
						int j = Mathf.FloorToInt(oldIndex);
						float w = oldIndex - j;
						float v0 = (j == 0 ? 1.0f : m_customMipmapFalloff[j - 1]);
						float v1 = (j < m_customMipmapFalloff.Length) ? m_customMipmapFalloff[j] : 0.0f;
						customFalloff[i] = Mathf.Lerp(v0, v1, w);
					}
					m_customMipmapFalloff = customFalloff;
				}
			}
		}
		private static HashSet<Material> s_sharedMaterials;
		const HideFlags CLONED_MATERIAL_HIDE_FLAGS = HideFlags.HideAndDontSave;
		void CloneProjectorMaterialIfShared()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) {
				return;
			}
#endif
			if (m_projector.material == null || (m_projector.material.hideFlags == CLONED_MATERIAL_HIDE_FLAGS && m_projector.material == m_projectorMaterial)) {
				return;
			}
			if (m_projectorMaterial != null && m_projectorMaterial.hideFlags == CLONED_MATERIAL_HIDE_FLAGS) {
				DestroyImmediate(m_projectorMaterial);
			}
			if (s_sharedMaterials == null) {
				s_sharedMaterials = new HashSet<Material>();
			}
			if (s_sharedMaterials.Contains(m_projector.material)) {
				m_projector.material = new Material(m_projector.material);
				m_projector.material.hideFlags = CLONED_MATERIAL_HIDE_FLAGS;
			}
			else {
				s_sharedMaterials.Add(m_projector.material);
			}
			m_projectorMaterial = m_projector.material;
		}

		void OnDestroy()
		{
			if (m_projectorMaterial != null) {
				if (s_sharedMaterials != null && s_sharedMaterials.Contains(m_projectorMaterial)) {
					s_sharedMaterials.Remove(m_projectorMaterial);
				}
				if (m_projectorMaterial.hideFlags == CLONED_MATERIAL_HIDE_FLAGS) {
					if (m_projector.material == m_projectorMaterial) {
						m_projector.material = null;
					}
					DestroyImmediate(m_projectorMaterial);
				}
			}
			if (m_shadowTexture != null) {
				if (m_camera != null) {
					m_camera.targetTexture = null;
				}
				DestroyImmediate(m_shadowTexture);
				m_shadowTexture = null;
			}
			if (m_camera != null) {
				m_camera.RemoveAllCommandBuffers();
			}
			m_isVisible = false;
		}

		bool IsReadyToExecute()
		{
			if (m_textureWidth <= 0 || m_textureHeight <= 0 || m_eraseShadowShader == null) {
				return false;
			}
			if (0 < m_mipLevel || m_superSampling != TextureSuperSample.x1) {
				if (m_downsampleShader == null) {
					return false;
				}
			}
			if (0 < m_blurLevel || (0.0f < m_mipmapBlurSize && 0 < m_mipLevel)) {
				if (m_blurShader == null) {
					return false;
				}
			}
			if (0 < m_mipLevel && (m_copyMipmapShader == null || m_downsampleShader == null)) {
				return false;
			}
			return true;
		}

		void SetVisible(bool isVisible)
		{
			m_isVisible = isVisible;
			SendMessage("OnVisibilityChanged", isVisible);
		}

#if UNITY_EDITOR
		private bool m_isRenderingFromUpdate = false;
		private Vector3 m_lastPosition;
		private Quaternion m_lastRotation;
		public void ForceRenderTexture()
		{
			m_isRenderingFromUpdate = true;
			// it is necessary to set camera parameters before Render, because render events will not be invoked if the volume of the view frustum is zero,
			// and there is no chance to fix the camera parameters in OnPreCull function.
			m_camera.orthographic = m_projector.orthographic;
			m_camera.orthographicSize = m_projector.orthographicSize;
			m_camera.fieldOfView = m_projector.fieldOfView;
			m_camera.aspect = m_projector.aspectRatio;
			m_camera.farClipPlane = m_projector.farClipPlane;
			// In edit mode, Unity 2017 has a bug that will happen when render target is changed during OnPreRender. To avoid it, change render target here.
			if (useIntermediateTexture) {
				int width = m_textureWidth * (int)m_superSampling;
				int height = m_textureHeight * (int)m_superSampling;
				m_camera.targetTexture = RenderTexture.GetTemporary(width, height, 0, m_shadowTexture.format, RenderTextureReadWrite.Linear, (int)m_multiSampling);
				m_camera.targetTexture.filterMode = FilterMode.Bilinear;
			}
			m_camera.Render();
			m_isRenderingFromUpdate = false;
		}
#endif
		void Update()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) {
				bool positionChanged = true;
				DrawTargetObject drawTargetObject = GetComponent<DrawTargetObject>();
				if (drawTargetObject != null) {
					if (drawTargetObject.target != null) {
						Vector3 pos = transform.InverseTransformPoint(drawTargetObject.target.position);
						Quaternion rot = Quaternion.Inverse(transform.rotation) * drawTargetObject.target.rotation;
						positionChanged = (pos != m_lastPosition) || (rot != m_lastRotation);
						m_lastPosition = pos;
						m_lastRotation = rot;
					}
					else {
						positionChanged = false;
					}
				}
				if (!m_shadowTextureValid
					|| positionChanged
					|| m_camera.orthographic != m_projector.orthographic
					|| m_camera.orthographicSize != m_projector.orthographicSize
					|| m_camera.fieldOfView != m_projector.fieldOfView
					|| m_camera.aspect != m_projector.aspectRatio
					|| m_camera.farClipPlane != m_projector.farClipPlane
				) {
					ForceRenderTexture();
				}
			}
#endif
			if (m_camera != null && !m_camera.enabled) {
				m_camera.enabled = true;
			}
		}

		void OnPreCull()
		{
			if (m_projector.material != m_projectorMaterial) {
				// projector material changed.
				CloneProjectorMaterialIfShared();
				m_projector.material.SetTexture("_ShadowTex", m_shadowTexture);
				m_projector.material.SetFloat("_DSPMipLevel", m_mipLevel);
			}
#if UNITY_EDITOR
			if (!(Application.isPlaying || m_isRenderingFromUpdate)) {
				m_camera.enabled = false;
				return;
			}
			if (!IsReadyToExecute()) {
				m_camera.enabled = false;
				return;
			}
			if (!IsInitialized() && !Initialize()) {
				m_camera.enabled = false;
				return;
			}
			if (m_projector.material == null) {
				if (m_isVisible) {
					SetVisible(false);
				}
				m_camera.enabled = false;
				return;
			}
			m_projector.material.SetTexture("_ShadowTex", m_shadowTexture);
			m_projector.material.SetFloat("_DSPMipLevel", m_mipLevel);
#endif
			if (m_isTexturePropertyChanged) {
				CreateRenderTexture();
			}
			m_camera.orthographic = m_projector.orthographic;
			m_camera.orthographicSize = m_projector.orthographicSize;
			m_camera.fieldOfView = m_projector.fieldOfView;
			m_camera.aspect = m_projector.aspectRatio;
			m_camera.farClipPlane = m_projector.farClipPlane;
			// view clip test
			bool isVisible = true;
			if (!m_projector.enabled) {
				isVisible = false;
			}
#if UNITY_EDITOR
			else if (m_testViewClip && Application.isPlaying)
#else
			else if (m_testViewClip)
#endif
			{
				if (m_camerasForViewClipTest == null || m_camerasForViewClipTest.Length == 0) {
					if (Camera.main != null) {
						m_camerasForViewClipTest = new Camera[1] {Camera.main};
					}
				}
				if (m_camerasForViewClipTest != null && 0 < m_camerasForViewClipTest.Length) {
					Vector3 v0 = m_camera.ViewportToWorldPoint(new Vector3(0,0,m_camera.nearClipPlane));
					Vector3 v1 = m_camera.ViewportToWorldPoint(new Vector3(1,0,m_camera.nearClipPlane));
					Vector3 v2 = m_camera.ViewportToWorldPoint(new Vector3(0,1,m_camera.nearClipPlane));
					Vector3 v3 = m_camera.ViewportToWorldPoint(new Vector3(1,1,m_camera.nearClipPlane));
					Vector3 v4 = m_camera.ViewportToWorldPoint(new Vector3(0,0,m_camera.farClipPlane));
					Vector3 v5 = m_camera.ViewportToWorldPoint(new Vector3(1,0,m_camera.farClipPlane));
					Vector3 v6 = m_camera.ViewportToWorldPoint(new Vector3(0,1,m_camera.farClipPlane));
					Vector3 v7 = m_camera.ViewportToWorldPoint(new Vector3(1,1,m_camera.farClipPlane));
					isVisible = false;
					for (int i = 0; i < m_camerasForViewClipTest.Length; ++i) {
						Camera cam = m_camerasForViewClipTest[i];
						Vector3 min = cam.WorldToViewportPoint(v0);
						if (min.z < 0.0f) { min.x = -min.x; min.y = -min.y; }
						Vector3 max = min;
						Vector3 v = cam.WorldToViewportPoint(v1);
						if (v.z < 0.0f) { v.x = -v.x; v.y = -v.y; }
						min.x = Mathf.Min(min.x, v.x); min.y = Mathf.Min(min.y, v.y); min.z = Mathf.Min(min.z, v.z);
						max.x = Mathf.Max(max.x, v.x); max.y = Mathf.Max(max.y, v.y); max.z = Mathf.Max(max.z, v.z);
						v = cam.WorldToViewportPoint(v2);
						if (v.z < 0.0f) { v.x = -v.x; v.y = -v.y; }
						min.x = Mathf.Min(min.x, v.x); min.y = Mathf.Min(min.y, v.y); min.z = Mathf.Min(min.z, v.z);
						max.x = Mathf.Max(max.x, v.x); max.y = Mathf.Max(max.y, v.y); max.z = Mathf.Max(max.z, v.z);
						v = cam.WorldToViewportPoint(v3);
						if (v.z < 0.0f) { v.x = -v.x; v.y = -v.y; }
						min.x = Mathf.Min(min.x, v.x); min.y = Mathf.Min(min.y, v.y); min.z = Mathf.Min(min.z, v.z);
						max.x = Mathf.Max(max.x, v.x); max.y = Mathf.Max(max.y, v.y); max.z = Mathf.Max(max.z, v.z);
						v = cam.WorldToViewportPoint(v4);
						if (v.z < 0.0f) { v.x = -v.x; v.y = -v.y; }
						min.x = Mathf.Min(min.x, v.x); min.y = Mathf.Min(min.y, v.y); min.z = Mathf.Min(min.z, v.z);
						max.x = Mathf.Max(max.x, v.x); max.y = Mathf.Max(max.y, v.y); max.z = Mathf.Max(max.z, v.z);
						v = cam.WorldToViewportPoint(v5);
						if (v.z < 0.0f) { v.x = -v.x; v.y = -v.y; }
						min.x = Mathf.Min(min.x, v.x); min.y = Mathf.Min(min.y, v.y); min.z = Mathf.Min(min.z, v.z);
						max.x = Mathf.Max(max.x, v.x); max.y = Mathf.Max(max.y, v.y); max.z = Mathf.Max(max.z, v.z);
						v = cam.WorldToViewportPoint(v6);
						if (v.z < 0.0f) { v.x = -v.x; v.y = -v.y; }
						min.x = Mathf.Min(min.x, v.x); min.y = Mathf.Min(min.y, v.y); min.z = Mathf.Min(min.z, v.z);
						max.x = Mathf.Max(max.x, v.x); max.y = Mathf.Max(max.y, v.y); max.z = Mathf.Max(max.z, v.z);
						v = cam.WorldToViewportPoint(v7);
						if (v.z < 0.0f) { v.x = -v.x; v.y = -v.y; }
						min.x = Mathf.Min(min.x, v.x); min.y = Mathf.Min(min.y, v.y); min.z = Mathf.Min(min.z, v.z);
						max.x = Mathf.Max(max.x, v.x); max.y = Mathf.Max(max.y, v.y); max.z = Mathf.Max(max.z, v.z);
						if (0.0f < max.x && min.x < 1.0f && 0.0f < max.y && min.y < 1.0f && cam.nearClipPlane < max.z && min.z < cam.farClipPlane) {
							isVisible = true;
							break;
						}
					}
				}
			}
			if (isVisible != m_isVisible) {
				SetVisible(isVisible);
			}
			if (!isVisible) {
				if (m_camera != null) {
					m_camera.enabled = false;
				}
				if (m_shadowTexture != null && !m_shadowTextureValid) {
					RenderTexture currentRT = RenderTexture.active;
					RenderTexture.active = m_shadowTexture;
					GL.Clear(false, true, new Color(1,1,1,0));
					m_shadowTextureValid = true;
					RenderTexture.active = currentRT;
				}
			}
		}

		bool HasShadowColor()
		{
			return m_shadowColor.a != 1 || (m_shadowColor.r + shadowColor.g + shadowColor.b) != 0;
		}

		void OnPreRender()
		{
#if UNITY_EDITOR
			if (!(Application.isPlaying || m_isRenderingFromUpdate)) {
				return;
			}
#endif
			if (!m_isVisible) {
				return;
			}
			m_shadowTexture.DiscardContents();
			if (useIntermediateTexture) {
#if UNITY_EDITOR
				if (!m_isRenderingFromUpdate)
#endif
				{
					int width = m_textureWidth * (int)m_superSampling;
					int height = m_textureHeight * (int)m_superSampling;
					m_camera.targetTexture = RenderTexture.GetTemporary(width, height, 0, m_shadowTexture.format, RenderTextureReadWrite.Linear, (int)m_multiSampling);
					m_camera.targetTexture.filterMode = FilterMode.Bilinear;
				}
			}
			else {
				m_camera.targetTexture = m_shadowTexture;
			}
			m_camera.clearFlags = CameraClearFlags.SolidColor;
		}
		private const int MAX_BLUR_TAP_SIZE = 7;
		private static float[] s_blurWeights = new float[MAX_BLUR_TAP_SIZE];
		struct BlurParam {
			public int tap;
			public Vector4 offset;
			public Vector4 weight;
		};
		static BlurParam GetBlurParam(float blurSize, BlurFilter filter)
		{
			BlurParam param = new BlurParam();

			if (blurSize < 0.1f) {
				param.tap = 3;
				param.offset.x = 0.0f;
				param.offset.y = 0.0f;
				param.offset.z = 0.0f;
				param.offset.w = 0.0f;
				param.weight.x = 1.0f;
				param.weight.y = 0.0f;
				param.weight.z = 0.0f;
				param.weight.w = 0.0f;
				return param;
			}
			// calculate weights
			if (filter == BlurFilter.Gaussian) {
				// gaussian filter
				float a = 1.0f/(2.0f*blurSize*blurSize);
				float totalWeight = 1.0f;
				s_blurWeights[0] = 1.0f;
				for (int i = 1; i < s_blurWeights.Length; ++i) {
					s_blurWeights[i] = Mathf.Exp(-i*i*a);
					totalWeight += 2.0f*s_blurWeights[i];
				}
				float w = 1.0f/totalWeight;
				for (int i = 0; i < s_blurWeights.Length; ++i) {
					s_blurWeights[i] *= w;
				}
			}
			else {
				// uniform filter
				float a = 0.5f/(0.5f + blurSize);
				for (int i = 0; i < s_blurWeights.Length; ++i) {
					if (i <= blurSize) {
						s_blurWeights[i] = a;
					}
					else if (i - 1 < blurSize) {
						s_blurWeights[i] = a * (blurSize - (i - 1));
					}
					else {
						s_blurWeights[i] = 0.0f;
					}
				}
			}
			param.offset.x = 1.0f + s_blurWeights[2]/(s_blurWeights[1] + s_blurWeights[2]);
			param.offset.y = 3.0f + s_blurWeights[4]/(s_blurWeights[3] + s_blurWeights[4]); 
			param.offset.z = 5.0f + s_blurWeights[6]/(s_blurWeights[5] + s_blurWeights[6]);
			param.offset.w = 0.0f;

			if (s_blurWeights[3] < 0.02f) {
				param.tap = 3;
				float a = 0.5f/(0.5f*s_blurWeights[0] + s_blurWeights[1] + s_blurWeights[2]);
				param.weight.x = Mathf.Round(255*a*s_blurWeights[0])/255.0f;
				param.weight.y = 0.5f - 0.5f*param.weight.x;
				param.weight.z = 0.0f;
				param.weight.w = 0.0f;
			}
			else if (s_blurWeights[5] < 0.02f) {
				param.tap = 5;
				float a = 0.5f/(0.5f*s_blurWeights[0] + s_blurWeights[1] + s_blurWeights[2] + s_blurWeights[3] + s_blurWeights[4]);
				param.weight.x = Mathf.Round(255*a*s_blurWeights[0])/255.0f;
				param.weight.y = Mathf.Round(255*a*(s_blurWeights[1] + s_blurWeights[2]))/255.0f;
				param.weight.z = 0.5f - (0.5f*param.weight.x + param.weight.y);
				param.weight.w = 0.0f;
			}
			else {
				param.tap = 7;
				param.weight.x = Mathf.Round(255*s_blurWeights[0])/255.0f;
				param.weight.y = Mathf.Round(255*(s_blurWeights[1] + s_blurWeights[2]))/255.0f;
				param.weight.z = Mathf.Round(255*(s_blurWeights[3] + s_blurWeights[4]))/255.0f;
				param.weight.w = 0.5f - (0.5f*param.weight.x + param.weight.y + param.weight.z);
			}
			return param;
		}
		static BlurParam GetDownsampleBlurParam(float blurSize, BlurFilter filter)
		{
			BlurParam param = new BlurParam();
			param.tap = 4;
			if (blurSize < 0.1f) {
				param.offset.x = 0.0f;
				param.offset.y = 0.0f;
				param.offset.z = 0.0f;
				param.offset.w = 0.0f;
				param.weight.x = 1.0f;
				param.weight.y = 0.0f;
				param.weight.z = 0.0f;
				param.weight.w = 0.0f;
				return param;
			}
			// calculate weights
			if (filter == BlurFilter.Gaussian) {
				// gaussian filter
				float a = 1.0f/(2.0f*blurSize*blurSize);
				float totalWeight = 0.0f;
				for (int i = 0; i < param.tap; ++i) {
					float x = i + 0.5f;
					s_blurWeights[i] = Mathf.Exp(-x*x*a);
					totalWeight += 2.0f*s_blurWeights[i];
				}
				float w = 1.0f/totalWeight;
				for (int i = 0; i < param.tap; ++i) {
					s_blurWeights[i] *= w;
				}
			}
			else {
				// uniform filter
				float a = 0.5f/blurSize;
				for (int i = 0; i < param.tap; ++i) {
					if (i + 1 <= blurSize) {
						s_blurWeights[i] = a;
					}
					else if (i < blurSize) {
						s_blurWeights[i] = a * (blurSize - i);
					}
					else {
						s_blurWeights[i] = 0.0f;
					}
				}
			}
			param.offset.x = 0.5f + s_blurWeights[1]/(s_blurWeights[0] + s_blurWeights[1]);
			param.offset.y = 2.5f + s_blurWeights[3]/(s_blurWeights[2] + s_blurWeights[3]); 
			param.offset.z = 0.0f;
			param.offset.w = 0.0f;
			
			param.weight.x = s_blurWeights[0] + s_blurWeights[1];
			param.weight.y = s_blurWeights[2] + s_blurWeights[3];
			param.weight.z = 0.0f;
			param.weight.w = 0.0f;

			return param;
		}
		void OnPostRender()
		{
#if UNITY_EDITOR
			if (!(Application.isPlaying || m_isRenderingFromUpdate)) {
				return;
			}
#endif
			m_camera.clearFlags = CameraClearFlags.Nothing;
			if (!m_isVisible) {
				return;
			}
			RenderTexture srcRT = m_camera.targetTexture;
#if UNITY_5_6
			// workaround for Unity 5.6
			// Unity 5.6 has a bug whereby a temporary render texture does not work if m_camera.targetTexture != null. This bug is fixed in Unity 2017.
			// However, this workaround might conflict with VR support. If you have a problem with VR SDK, please let us know via e-mail (support@nyahoon.com).
			if (srcRT != m_shadowTexture) {
				m_camera.targetTexture = null;
			}
#else
			m_camera.targetTexture = m_shadowTexture;
#endif
			if (m_superSampling != TextureSuperSample.x1 || HasShadowColor()) {
				m_downsampleShader.color = m_shadowColor;
				// downsample
				RenderTexture dstRT;
				if (0 < m_blurLevel) {
					dstRT = RenderTexture.GetTemporary(m_textureWidth, m_textureHeight, 0, m_shadowTexture.format, RenderTextureReadWrite.Linear);
					dstRT.filterMode = FilterMode.Bilinear;
				}
				else {
					dstRT = m_shadowTexture;
				}
				Graphics.SetRenderTarget(dstRT);
				int pass = m_superSampling == TextureSuperSample.x16 ? 0 : 2;
				Graphics.Blit(srcRT, dstRT, m_downsampleShader, HasShadowColor() ? pass + 1 : pass);
				RenderTexture.ReleaseTemporary(srcRT);
				srcRT = dstRT;
			}
			if (0 < m_blurLevel) {
				// adjust blur size according to texel aspect
				float texelAspect = (m_projector.aspectRatio * m_textureHeight)/(float)m_textureWidth;
				float blurSizeH = m_blurSize;
				float blurSizeV = m_blurSize;
				if (texelAspect < 1.0f) {
					blurSizeV *= texelAspect;
				}
				else {
					blurSizeH /= texelAspect;
				}
				// blur parameters
				BlurParam blurH = GetBlurParam(blurSizeH, m_blurFilter);
				BlurParam blurV = GetBlurParam(blurSizeV, m_blurFilter);
				blurH.tap = (blurH.tap - 3); // index of pass
				blurV.tap = (blurV.tap - 3) + 1; // index of pass
				m_blurShader.SetVector(s_blurOffsetHParamID, blurH.offset);
				m_blurShader.SetVector(s_blurOffsetVParamID, blurV.offset);
				m_blurShader.SetVector(s_blurWeightHParamID, blurH.weight);
				m_blurShader.SetVector(s_blurWeightVParamID, blurV.weight);

				RenderTexture dstRT = RenderTexture.GetTemporary(m_textureWidth, m_textureHeight, 0, m_shadowTexture.format, RenderTextureReadWrite.Linear);
				dstRT.filterMode = FilterMode.Bilinear;
				srcRT.wrapMode = TextureWrapMode.Clamp;
				dstRT.wrapMode = TextureWrapMode.Clamp;
				Graphics.Blit(srcRT, dstRT, m_blurShader, blurH.tap);
				if (1 < srcRT.antiAliasing) {
					RenderTexture.ReleaseTemporary(srcRT);
					srcRT = RenderTexture.GetTemporary(m_textureWidth, m_textureHeight, 0, m_shadowTexture.format, RenderTextureReadWrite.Linear);
				}
				else {
					srcRT.DiscardContents();
				}
				for (int i = 1; i < m_blurLevel - 1; ++i) {
					Graphics.Blit(dstRT, srcRT, m_blurShader, blurV.tap);
					dstRT.DiscardContents();
					Graphics.Blit(srcRT, dstRT, m_blurShader, blurH.tap);
					srcRT.DiscardContents();
				}
				RenderTexture.ReleaseTemporary(srcRT);
				srcRT = m_shadowTexture;
				Graphics.Blit(dstRT, srcRT, m_blurShader, blurV.tap);
				RenderTexture.ReleaseTemporary(dstRT);
			}
			Graphics.SetRenderTarget(m_shadowTexture);
			if (srcRT != m_shadowTexture) {
				Graphics.Blit(srcRT, m_downsampleShader, 2);
				if (m_mipLevel == 0) {
					RenderTexture.ReleaseTemporary(srcRT);
				}
			}
			EraseShadowOnBoarder(m_textureWidth, m_textureHeight);
			if (0 < m_mipLevel) {
				// setup blur parameters
				BlurParam blurH = new BlurParam(), blurV = new BlurParam();
				if (0.1f < m_mipmapBlurSize) {
					// adjust blur size according to texel aspect
					float texelAspect = (m_projector.aspectRatio * m_textureHeight)/(float)m_textureWidth;
					float blurSizeH = m_mipmapBlurSize;
					float blurSizeV = m_mipmapBlurSize;
					if (texelAspect < 1.0f) {
						blurSizeV *= texelAspect;
					}
					else {
						blurSizeH /= texelAspect;
					}
					// blur parameters
					if (m_singlePassMipmapBlur) {
						blurH = GetDownsampleBlurParam(2.0f + 2.0f*blurSizeH, m_blurFilter);
						blurV = GetDownsampleBlurParam(2.0f + 2.0f*blurSizeV, m_blurFilter);
						Vector4 weight = new Vector4(blurH.weight.x * blurV.weight.x, blurH.weight.x * blurV.weight.y, blurH.weight.y * blurV.weight.x, blurH.weight.y * blurV.weight.y);
						float a = 0.25f/(weight.x + weight.y + weight.z + weight.w);
						weight.x = Mathf.Round(255*a*weight.x)/255.0f;
						weight.y = Mathf.Round(255*a*weight.y)/255.0f;
						weight.z = Mathf.Round(255*a*weight.z)/255.0f;
						weight.w = 0.25f - weight.x - weight.y - weight.z;
						m_downsampleShader.SetVector(s_downSampleBlurWeightParamID, weight);
					}
					else {
						blurH = GetBlurParam(blurSizeH, m_blurFilter);
						blurV = GetBlurParam(blurSizeV, m_blurFilter);
						blurH.tap = (blurH.tap - 3); // index of pass
						blurV.tap = (blurV.tap - 3) + 1; // index of pass
						m_blurShader.SetVector(s_blurOffsetHParamID, blurH.offset);
						m_blurShader.SetVector(s_blurOffsetVParamID, blurV.offset);
						m_blurShader.SetVector(s_blurWeightHParamID, blurH.weight);
						m_blurShader.SetVector(s_blurWeightVParamID, blurV.weight);
					}
				}
				int w = m_textureWidth >> 1;
				int h = m_textureHeight >> 1;
				RenderTexture tempRT = RenderTexture.GetTemporary(w, h, 0, m_shadowTexture.format, RenderTextureReadWrite.Linear);
				tempRT.filterMode = FilterMode.Bilinear;
				bool downSampleWithBlur = m_singlePassMipmapBlur && 0.1f < m_mipmapBlurSize;
				if (downSampleWithBlur) {
					SetDownsampleBlurOffsetParams(blurH, blurV, w, h);
				}
				if (srcRT == m_shadowTexture) {
					if (downSampleWithBlur) {
						Graphics.Blit(srcRT, tempRT, m_downsampleShader, 5);
					}
					else {
						Graphics.Blit(srcRT, tempRT, m_copyMipmapShader, 1);
					}
				}
				else {
					Graphics.Blit(srcRT, tempRT, m_downsampleShader, downSampleWithBlur ? 4 : 0);
					RenderTexture.ReleaseTemporary(srcRT);
				}
				srcRT = tempRT;
				int i = 0;
				float falloff = 1.0f;
				for ( ; ; ) {
					if (0.1f < m_mipmapBlurSize && !m_singlePassMipmapBlur) {
						tempRT = RenderTexture.GetTemporary(w, h, 0, m_shadowTexture.format, RenderTextureReadWrite.Linear);
						tempRT.filterMode = FilterMode.Bilinear;
						tempRT.wrapMode = TextureWrapMode.Clamp;
						srcRT.wrapMode = TextureWrapMode.Clamp;
						Graphics.Blit(srcRT, tempRT, m_blurShader, blurH.tap);
						srcRT.DiscardContents();
						Graphics.Blit(tempRT, srcRT, m_blurShader, blurV.tap);
						RenderTexture.ReleaseTemporary(tempRT);
					}
					if (m_mipmapFalloff == MipmapFalloff.Linear) {
						falloff = ((float)(m_mipLevel - i))/(m_mipLevel + 1.0f);
					}
					else if (m_mipmapFalloff == MipmapFalloff.Custom && m_customMipmapFalloff != null && 0 < m_customMipmapFalloff.Length) {
						falloff = m_customMipmapFalloff[Mathf.Min(i, m_customMipmapFalloff.Length-1)];
					}
					m_copyMipmapShader.SetFloat(s_falloffParamID, falloff);
					m_copyMipmapShader.SetFloat(s_falloffParamID, falloff);
					m_shadowTexture.DiscardContents(); // To avoid Tiled GPU perf warning. It just tells GPU not to copy back the rendered image to a tile buffer. It won't destroy the rendered image.
					++i;
					Graphics.SetRenderTarget(m_shadowTexture, i);
					Graphics.Blit(srcRT, m_copyMipmapShader, 0);
					EraseShadowOnBoarder(w, h);
					w = Mathf.Max(1, w >> 1);
					h = Mathf.Max(1, h >> 1);
					if (i == m_mipLevel || w <= 4 || h <= 4) {
						RenderTexture.ReleaseTemporary(srcRT);
						break;
					}
					tempRT = RenderTexture.GetTemporary(w, h, 0, m_shadowTexture.format, RenderTextureReadWrite.Linear);
					tempRT.filterMode = FilterMode.Bilinear;
					if (downSampleWithBlur) {
						SetDownsampleBlurOffsetParams(blurH, blurV, w, h);
						Graphics.Blit(srcRT, tempRT, m_downsampleShader, 4);
					}
					else {
						Graphics.Blit(srcRT, tempRT, m_downsampleShader, 0);
					}
					RenderTexture.ReleaseTemporary(srcRT);
					srcRT = tempRT;
				}
				while (1 <= w || 1 <= h) {
					++i;
					Graphics.SetRenderTarget(m_shadowTexture, i);
					GL.Clear(false, true, new Color(1,1,1,0));
					w = w >> 1;
					h = h >> 1;
				}
			}
			m_shadowTextureValid = true;
		}
		void EraseShadowOnBoarder(int w, int h)
		{
			float x = 1.0f - 1.0f/w;
			float y = 1.0f - 1.0f/h;
			m_eraseShadowShader.SetPass(0);
			GL.Begin(GL.LINES);
			GL.Vertex3(-x,-y,0);
			GL.Vertex3( x,-y,0);
			GL.Vertex3( x,-y,0);
			GL.Vertex3( x, y,0);
			GL.Vertex3( x, y,0);
			GL.Vertex3(-x, y,0);
			GL.Vertex3(-x, y,0);
			GL.Vertex3(-x,-y,0);
			GL.End();
		}
		void SetDownsampleBlurOffsetParams(BlurParam blurH, BlurParam blurV, int w, int h)
		{
			float invW = 0.5f/w;
			float invH = 0.5f/h;
			float offsetX0 = invW * blurH.offset.x;
			float offsetX1 = invW * blurH.offset.y;
			float offsetY0 = invH * blurV.offset.x;
			float offsetY1 = invH * blurV.offset.y;
			m_downsampleShader.SetVector(s_downSampleBlurOffset0ParamID, new Vector4(offsetX0, offsetY0, -offsetX0, -offsetY0));
			m_downsampleShader.SetVector(s_downSampleBlurOffset1ParamID, new Vector4(offsetX0, offsetY1, -offsetX0, -offsetY1));
			m_downsampleShader.SetVector(s_downSampleBlurOffset2ParamID, new Vector4(offsetX1, offsetY0, -offsetX1, -offsetY0));
			m_downsampleShader.SetVector(s_downSampleBlurOffset3ParamID, new Vector4(offsetX1, offsetY1, -offsetX1, -offsetY1));
		}
	}
}
