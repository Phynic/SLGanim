Shader "DynamicShadowProjector/Shadow/Opaque" {
	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
		
			CGPROGRAM
			#pragma vertex DSPShadowVertOpaque
			#pragma fragment DSPShadowFragOpaque
			#include "DSPShadow.cginc"
			ENDCG
		}
	}
}
