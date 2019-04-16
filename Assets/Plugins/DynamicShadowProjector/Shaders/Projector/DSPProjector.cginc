// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#if !defined(DSP_PROJECTOR_CGINC_INCLUDED)
#define DSP_PROJECTOR_CGINC_INCLUDED
#include "UnityCG.cginc"

struct DSP_V2F_PROJECTOR {
	float4 uvShadow : TEXCOORD0;
	half2 alpha    : COLOR;  // fixed precision is ok for most GPU, but I saw a problem on Tegra 3.
	UNITY_FOG_COORDS(1)
	float4 pos : SV_POSITION;
};

struct DSP_V2F_PROJECTOR_LIGHT {
	float4 uvShadow : TEXCOORD0;
	half2 alpha    : COLOR;  // fixed precision is ok for most GPU, but I saw a problem on Tegra 3.
	UNITY_FOG_COORDS(1)
	float4 pos : SV_POSITION;
};

uniform float4x4 unity_Projector;
uniform float4x4 unity_ProjectorClip;
uniform float _ClipScale;  // compatible with Fast Shadow Receiver
uniform fixed _Alpha;      // compatible with Fast Shadow Receiver
uniform fixed _Ambient;    // compatible with Fast Shadow Receiver

sampler2D _ShadowTex;
sampler2D _LightTex;

half DSPCalculateDiffuseLightAlpha(float4 vertex, float3 normal)
{
	float diffuse = -dot(normal, normalize(float3(unity_Projector[2][0],unity_Projector[2][1], unity_Projector[2][2])));
	return _Alpha * diffuse;
}

half DSPCalculateDiffuseShadowAlpha(float4 vertex, float3 normal)
{
	float diffuse = -dot(normal, normalize(float3(unity_Projector[2][0],unity_Projector[2][1], unity_Projector[2][2])));
	// this calculation is not linear. it is better to do in fragment shader. but in most case, it won't be a problem.
	return _Alpha * diffuse / (_Ambient + saturate(diffuse));
}

DSP_V2F_PROJECTOR_LIGHT DSPProjectorVertLightNoFalloff(float4 vertex : POSITION, float3 normal : NORMAL)
{
	DSP_V2F_PROJECTOR_LIGHT o;
	o.pos = UnityObjectToClipPos (vertex);
	o.uvShadow = mul (unity_Projector, vertex);
	o.alpha.x = _ClipScale * mul(unity_ProjectorClip, vertex).x;
	o.alpha.y = DSPCalculateDiffuseLightAlpha(vertex, normal);
	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

DSP_V2F_PROJECTOR DSPProjectorVertNoFalloff(float4 vertex : POSITION, float3 normal : NORMAL)
{
	DSP_V2F_PROJECTOR o;
	o.pos = UnityObjectToClipPos (vertex);
	o.uvShadow = mul (unity_Projector, vertex);
	o.alpha.x = _ClipScale * mul(unity_ProjectorClip, vertex).x;
	o.alpha.y = DSPCalculateDiffuseShadowAlpha(vertex, normal);
	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

DSP_V2F_PROJECTOR_LIGHT DSPProjectorVertLightLinearFalloff(float4 vertex : POSITION, float3 normal : NORMAL)
{
	DSP_V2F_PROJECTOR_LIGHT o;
	o.pos = UnityObjectToClipPos (vertex);
	o.uvShadow = mul (unity_Projector, vertex);
	float z = mul(unity_ProjectorClip, vertex).x;
	o.alpha.x = _ClipScale * z;
	o.alpha.y = DSPCalculateDiffuseLightAlpha(vertex, normal) * (1.0f - z); // falloff
	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

DSP_V2F_PROJECTOR DSPProjectorVertLinearFalloff(float4 vertex : POSITION, float3 normal : NORMAL)
{
	DSP_V2F_PROJECTOR o;
	o.pos = UnityObjectToClipPos (vertex);
	o.uvShadow = mul (unity_Projector, vertex);
	float z = mul(unity_ProjectorClip, vertex).x;
	o.alpha.x = _ClipScale * z;
	o.alpha.y = DSPCalculateDiffuseShadowAlpha(vertex, normal);
	o.alpha.y *= (1.0f - z); // falloff
	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

fixed DSPGetShadowAlpha(DSP_V2F_PROJECTOR i)
{
	return saturate(saturate(i.alpha.x)*i.alpha.y);
}

fixed4 DSPCalculateFinalLightColor(fixed4 texColor, DSP_V2F_PROJECTOR_LIGHT i)
{
	fixed alpha = saturate(saturate(i.alpha.x)*i.alpha.y);
	texColor.rgb = lerp(fixed3(0,0,0), texColor.rgb, alpha);
	UNITY_APPLY_FOG_COLOR(i.fogCoord, texColor, fixed4(0,0,0,0));
	return texColor;
}

fixed4 DSPCalculateFinalShadowColor(fixed4 texColor, DSP_V2F_PROJECTOR i)
{
	fixed alpha = DSPGetShadowAlpha(i);
	texColor.rgb = lerp(fixed3(1,1,1), texColor.rgb, alpha);
	UNITY_APPLY_FOG_COLOR(i.fogCoord, texColor, fixed4(1,1,1,1));
	return texColor;
}

fixed4 DSPProjectorFrag(DSP_V2F_PROJECTOR i) : SV_Target
{
	fixed4 shadow = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
	return DSPCalculateFinalShadowColor(shadow, i);
}

fixed4 DSPProjectorFragLight(DSP_V2F_PROJECTOR_LIGHT i) : SV_Target
{
	fixed4 light = tex2Dproj(_LightTex, UNITY_PROJ_COORD(i.uvShadow));
	return DSPCalculateFinalLightColor(light, i);
}

fixed4 DSPProjectorFragLightWithShadow(DSP_V2F_PROJECTOR_LIGHT i) : SV_Target
{
	fixed4 light = tex2Dproj(_LightTex, UNITY_PROJ_COORD(i.uvShadow));
	fixed3 shadow = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
	light.rgb *= shadow.rgb;
	return DSPCalculateFinalLightColor(light, i);
}
#endif // !defined(DSP_PROJECTOR_CGINC_INCLUDED)
