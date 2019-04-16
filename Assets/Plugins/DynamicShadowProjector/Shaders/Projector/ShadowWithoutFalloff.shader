Shader "DynamicShadowProjector/Projector/Shadow Without Falloff" {
	Properties {
		_ShadowTex ("Cookie", 2D) = "gray" {}
		_ClipScale ("Near Clip Sharpness", Float) = 100
		_Alpha ("Shadow Darkness", Range (0, 1)) = 1.0
		_Ambient ("Ambient", Range (0, 1)) = 0.3
		_Offset ("Offset", Range (-1, -10)) = -1.0
	}
	Subshader {
		Tags {"Queue"="Transparent-1"}
		Pass {
			Name "PASS"
			ZWrite Off
			ColorMask RGB
			Blend DstColor Zero
			Offset -1, [_Offset]

			CGPROGRAM
			#pragma vertex DSPProjectorVertNoFalloff
			#pragma fragment DSPProjectorFrag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			#include "DSPProjector.cginc"
			ENDCG
		}
	}
}
