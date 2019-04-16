// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#if !defined(DSP_SHADOW_CGINC_INCLUDED)
#define DSP_SHADOW_CGINC_INCLUDED
#include "UnityCG.cginc"

float4 DSPShadowVertOpaque(float4 vertex : POSITION) : SV_POSITION
{
	return UnityObjectToClipPos(vertex);
}

fixed4 DSPShadowFragOpaque() : COLOR
{
	return fixed4(0,0,0,1);
}

struct DSP_V2F_SHADOW_TRANS {
	float4 pos : SV_POSITION;
	float2 uv  : TEXCOORD0;
};
float4 _MainTex_ST;
DSP_V2F_SHADOW_TRANS DSPShadowVertTrans(float4 vertex : POSITION, float4 texcoord : TEXCOORD0)
{
	DSP_V2F_SHADOW_TRANS o;
	o.pos = UnityObjectToClipPos(vertex);
	o.uv = TRANSFORM_TEX(texcoord, _MainTex);
	return o;
}

sampler2D _MainTex;
fixed4 _Color;
fixed4 DSPShadowFragTrans(DSP_V2F_SHADOW_TRANS i) : COLOR
{
	fixed a = tex2D(_MainTex, i.uv).a * _Color.a;
	return fixed4(0,0,0,a);
}

#if defined(_ALPHABLEND_ON) || defined(_ALPHATEST_ON) || defined(_ALPHAPREMULTIPLY_ON)
DSP_V2F_SHADOW_TRANS DSPShadowVertStandard(float4 vertex : POSITION, float4 texcoord : TEXCOORD0)
{
	return DSPShadowVertTrans(vertex, texcoord);
}
fixed4 DSPShadowFragStandard(DSP_V2F_SHADOW_TRANS i) : COLOR
{
	return DSPShadowFragTrans(i);
}
#else
float4 DSPShadowVertStandard(float4 vertex : POSITION) : SV_POSITION
{
	return DSPShadowVertOpaque(vertex);
}
fixed4 DSPShadowFragStandard() : COLOR
{
	return DSPShadowFragOpaque();
}
#endif

#endif // !defined(DSP_SHADOW_CGINC_INCLUDED)
