// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "DynamicShadowProjector/Blit/CopyMipmap" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	half4 _MainTex_TexelSize;
	fixed _Falloff;

	struct v2f_blit
	{
		float4 pos : SV_POSITION;
		half2  uv0 : TEXCOORD0;
	};
	struct v2f_downsample
	{
		float4 pos : SV_POSITION;
		half2  uv0 : TEXCOORD0;
		half2  uv1 : TEXCOORD1;
		half2  uv2 : TEXCOORD2;
		half2  uv3 : TEXCOORD3;
	};

	v2f_blit vert_blit(appdata_img v)
	{
		v2f_blit o;
		o.pos = UnityObjectToClipPos(v.vertex);
    	o.uv0 = v.texcoord.xy;
		return o;
	}

	fixed4 frag_blit_with_falloff(v2f_blit i) : COLOR
	{
		fixed4 color = tex2D(_MainTex, i.uv0);
		return lerp(fixed4(1,1,1,0), color, _Falloff);
	}

	v2f_downsample vert_downsample(appdata_img v)
	{
		v2f_downsample o;
		o.pos = UnityObjectToClipPos(v.vertex);
    	o.uv0 = v.texcoord.xy + _MainTex_TexelSize.xy;
		o.uv1 = v.texcoord.xy - _MainTex_TexelSize.xy;
		o.uv2 = v.texcoord.xy + _MainTex_TexelSize.xy * half2(1,-1);
		o.uv3 = v.texcoord.xy + _MainTex_TexelSize.xy * half2(-1,1);
		return o;
	}

	fixed4 frag_downsample_mip0(v2f_downsample i) : COLOR
	{
		half4 uv;
		uv.zw = 0;
		uv.xy = i.uv0;
		fixed4 color = 0.25*tex2Dlod(_MainTex, uv);
		uv.xy = i.uv1;
		color += 0.25*tex2Dlod(_MainTex, uv);
		uv.xy = i.uv2;
		color += 0.25*tex2Dlod(_MainTex, uv);
		uv.xy = i.uv3;
		color += 0.25*tex2Dlod(_MainTex, uv);
		return color;
	}
	ENDCG

	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
			CGPROGRAM
			#pragma vertex vert_blit
			#pragma fragment frag_blit_with_falloff
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
			CGPROGRAM
			#pragma vertex vert_downsample
			#pragma fragment frag_downsample_mip0
			#pragma target 3.0
			ENDCG
		}
	}
}
