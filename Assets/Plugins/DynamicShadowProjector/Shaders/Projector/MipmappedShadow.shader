// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "DynamicShadowProjector/Projector/Mipmapped Shadow" {
	Properties {
		_ShadowTex ("Cookie", 2D) = "gray" {}
		_ClipScale ("Near Clip Sharpness", Float) = 100
		_Alpha ("Shadow Darkness", Range (0, 1)) = 1.0
		_Ambient ("Ambient", Range (0, 1)) = 0.3
		_DSPMipLevel ("Max Mip Level", float) = 4.0
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
			#pragma vertex DSPProjectorVertMipmap
			#pragma fragment DSPProjectorFragMipmap
			#pragma multi_compile_fog
			#pragma target 3.0
			#include "UnityCG.cginc"
			#include "DSPProjector.cginc"

			uniform half _DSPMipLevel;

			DSP_V2F_PROJECTOR DSPProjectorVertMipmap(float4 vertex : POSITION, float3 normal : NORMAL)
			{
				DSP_V2F_PROJECTOR o;
				o.pos = UnityObjectToClipPos (vertex);
				o.uvShadow = mul (unity_Projector, vertex);
				float z = mul(unity_ProjectorClip, vertex).x;
				o.uvShadow.z = _DSPMipLevel * z;
				o.alpha.x = _ClipScale * z;
				o.alpha.y = DSPCalculateDiffuseShadowAlpha(vertex, normal);
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}

			fixed4 DSPGetMipmappedShadowTex(DSP_V2F_PROJECTOR i)
			{
				float3 uv;
				uv.xy = saturate(i.uvShadow.xy/i.uvShadow.w);
				uv.z = i.uvShadow.z;
				return tex2Dlod(_ShadowTex, uv.xyzz);
			}

			fixed4 DSPProjectorFragMipmap(DSP_V2F_PROJECTOR i) : SV_Target
			{
				fixed4 shadow = DSPGetMipmappedShadowTex(i);
				return DSPCalculateFinalShadowColor(shadow, i);
			}
			ENDCG
		}
	}
}
