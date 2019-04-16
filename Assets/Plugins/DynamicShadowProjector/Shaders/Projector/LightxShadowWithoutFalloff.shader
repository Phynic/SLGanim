Shader "DynamicShadowProjector/Projector/Light x Shadow Without Falloff" {
	Properties {
		_LightTex ("Light Cookie", 2D) = "gray" {}
		_ShadowTex ("Shadow Cookie", 2D) = "gray" {}
		_ClipScale ("Near Clip Sharpness", Float) = 100
		_Alpha ("Light Intensity", Range (0, 1)) = 1.0
		_Offset ("Offset", Range (-1, -10)) = -1.0
	}
	Subshader {
		Tags {"Queue"="Transparent-1"}
		Pass {
			Name "PASS"
			ZWrite Off
			ColorMask RGB
			Blend DstColor One
			Offset -1, [_Offset]

			CGPROGRAM
			#pragma vertex DSPProjectorVertLightNoFalloff
			#pragma fragment DSPProjectorFragLightWithShadow
			#pragma multi_compile_fog
			#include "DSPProjector.cginc"
			ENDCG
		}
	}
}
