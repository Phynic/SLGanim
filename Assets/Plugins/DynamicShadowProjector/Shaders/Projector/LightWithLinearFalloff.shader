Shader "DynamicShadowProjector/Projector/Light With Linear Falloff" {
	Properties {
		_LightTex ("Cookie", 2D) = "gray" {}
		_ClipScale ("Near Clip Sharpness", Float) = 100
		_Alpha ("Light Intensity", Range (0, 1)) = 1.0
		_Offset ("Offset", Range (-1, -10)) = -1.0
	}
	Subshader {
		Tags {"Queue"="Transparent-1"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend DstColor One
			Offset -1, [_Offset]

			CGPROGRAM
			#pragma vertex DSPProjectorVertLightLinearFalloff
			#pragma fragment DSPProjectorFragLight
			#pragma multi_compile_fog
			#include "DSPProjector.cginc"
			ENDCG
		}
	}
}
