Shader "Hidden/DynamicShadowProjector/Shadow/Replacement" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	CGINCLUDE
	#include "DSPShadow.cginc"
	ENDCG

	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
			Blend One OneMinusSrcAlpha
			CGPROGRAM
			#pragma multi_compile _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma vertex DSPShadowVertStandard
			#pragma fragment DSPShadowFragStandard
			ENDCG
		}
	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
			Blend One OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex DSPShadowVertTrans
			#pragma fragment DSPShadowFragTrans
			ENDCG
		}
	} 
	SubShader {
		Tags { "RenderType"="TransparentCutout" }
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
			Blend One OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex DSPShadowVertTrans
			#pragma fragment DSPShadowFragTrans
			ENDCG
		}
	} 
}
