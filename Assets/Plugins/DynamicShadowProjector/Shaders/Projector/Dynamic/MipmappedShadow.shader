Shader "DynamicShadowProjector/Projector/Dynamic/Mipmapped Shadow" {
	Properties {
		_ClipScale ("Near Clip Sharpness", Float) = 100
		_Alpha ("Shadow Darkness", Range (0, 1)) = 1.0
		_Ambient ("Ambient", Range (0, 1)) = 0.3
		_Offset ("Offset", Range (-1, -10)) = -1.0
	}
	Subshader {
		Tags {"Queue"="Transparent-1"}
		UsePass "DynamicShadowProjector/Projector/Mipmapped Shadow/PASS"
	}
}
