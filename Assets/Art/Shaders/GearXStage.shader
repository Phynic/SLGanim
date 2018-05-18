// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader/GearXStage" {
	Properties{
		//_Color("Color", Color) = (1,1,1,1)
		// http://wiki.unity3d.com/index.php?title=Outlined_Diffuse_3

		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_SSSTex("SSS (RGB)", 2D) = "white" {}
		_ILMTex("ILM (RGB)", 2D) = "white" {}

		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(.0, 2)) = .5
		_ShadowContrast("Vertex Shadow contrast", Range(0, 20)) = 1
		_DarkenInnerLineColor("Darken Inner Line Color", Range(0, 1)) = 0.2

		_LightDirection("Light Direction", Vector) = (0,0,1)
	}


CGINCLUDE
#include "UnityCG.cginc"
sampler2D _MainTex;
		sampler2D _SSSTex;
	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float4 texCoord : TEXCOORD0;
		//float4 vertexColor : COLOR;
	};

	struct v2f {
		float4 pos : POSITION;
		float4 color : COLOR;
		float4 tex : TEXCOORD0;
	};

	uniform float _Outline;
	uniform float4 _OutlineColor;
	uniform float _ShadowContrast;
	uniform float _DarkenInnerLineColor;
	uniform half3 _LightDirection;

	v2f vert(appdata v) {
		// just make a copy of incoming vertex data but scaled according to normal direction
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		float2 offset = TransformViewToProjection(norm.xy);
		o.pos.xy += offset * _Outline;
		o.tex = v.texCoord;
		
		o.color = _OutlineColor;
		return o;
	}
ENDCG

SubShader 
{
	//Tags {"Queue" = "Geometry+100" }
	CGPROGRAM
	#pragma surface surfA Lambert

	//sampler2D _MainTex;
	fixed4 _Color;

	struct Input {
		float2 uv_MainTex;
	};

	void surfA(Input IN, inout SurfaceOutput o) {
		//fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		half4 c2 = half4(1, 0, 1, 1);
		//return c2;
		// fixed4 c = (1, 0, 0, 1);
		o.Albedo = c2.rgb;
		o.Alpha = c2.a;
	}
	ENDCG

	// note that a vertex shader is specified here but its using the one above
	Pass{
		Name "OUTLINE"
		Tags{ "LightMode" = "Always" }
		Cull Front
		ZWrite On
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha
		//Offset 50,50
		//Lighting Off

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

	half4 frag(v2f i) :COLOR { 
		//fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 cLight = tex2D(_MainTex, i.tex.xy);
		fixed4 cSSS = tex2D(_SSSTex, i.tex.xy);
		fixed4 cDark = cLight * cSSS;
		//fixed4 cSSS = tex2D(_SSSTex, IN.uv_MainTex);
		//cDark = half4(1, 0, 0, 1);
		//cDark = cLight;
		cDark = cDark *0.5f;// *cDark * cDark;
		cDark.a = 1; // weapon had alpha?
		//return clamp(cDark, 0, 1); // 
		return cDark;
		//return cDark;// *cDark; //  cLight * cSSS * cSSS * cSSS; // times cSSS twice = darker outline based on skin color 
		//return i.color; 
	}

		ENDCG
	} // Pass


	// ###############################
	CGPROGRAM
  
		// noforwardadd: important to remove multiple light passes
	#pragma surface surf  CelShadingForward  vertex:vertB 
	// #pragma surface surf  CelShadingForward  vertex:vertB noforwardadd noshadow
	// Use shader model 3.0 target, to get nicer looking lighting
	#pragma target 3.0

	//sampler2D _MainTex;
	//sampler2D _SSSTex;
	sampler2D _ILMTex;
	//half _Glossiness;
	//half _Metallic;
	//fixed4 _Color;

	struct Input {
		float2 uv_MainTex;
		float3 vertexColor; // Vertex color stored here by vert() method
	};

	struct v2fB {
		float4 pos : SV_POSITION;
		fixed4 color : COLOR;
	};

	void vertB(inout appdata_full v, out Input o)
	{
		UNITY_INITIALIZE_OUTPUT(Input, o);
		o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
	}

	struct SurfaceOutputCustom
	{
		fixed3 Albedo;
		fixed3 Normal;
		fixed3 Emission;
		fixed Alpha;

		half3 BrightColor;
		half3 ShadowColor;
		half3 InnerLineColor;
		half ShadowThreshold;

		half SpecularIntensity;
		half SpecularSize;

		//fixed4 ILM;
	};

	half4 LightingCelShadingForward(SurfaceOutputCustom s, half3 lightDir, /*half3 viewDir,*/ half atten) {
		//lightDir = _LightDirection;
		//half3 h = normalize(lightDir + viewDir);
		//half diff = max(0, dot(s.Normal, lightDir));
		//float nh = max(0, dot(s.Normal, h));
		//float spec = pow(diff, 48.0f);
		//float spec = 1;// pow(diff, 1.0f);

		// Cell shading: Threshold (<=0, > 90 deg away from light), light vector, normal vector [control]
		half NdotL = dot(lightDir, s.Normal);
		//NdotL = smoothstep(0, 1.0f, NdotL);
		//NdotL = smoothstep(0, 0.025f, NdotL);

		half testDot = (NdotL + 1) / 2.0; // color 0 to 1. black = shadow, white = light
		half4 c = half4(0, 0, 0, 1);

		half4 specColor = half4(s.SpecularIntensity, s.SpecularIntensity, s.SpecularIntensity, 1);
		half blendArea = 0.04;

		//half3 lerpedColor = smoothstep(s.ShadowColor, s.BrightColor, shadowPercent);// 0.5f);
		//if (smoothstep(0, 0.025f, NdotL - s.ShadowThreshold))
		//float _DiffuseTransition = 0.1f;
		//float diffuseBlend = smoothstep(0.0, 1.0, (NdotL * 0.5) / _DiffuseTransition + 0.5);
		//NdotL = diffuseBlend;
		//if (NdotL <= (s.ShadowThreshold)) // 1 - s.ShadowThreshold. flip color e.g black = 0, white = 1 
		//NdotL = (NdotL - s.ShadowThreshold) * 0.5f;
		NdotL -= s.ShadowThreshold;
		//NdotL -= s.ShadowThreshold;
		//NdotL -= 0.2f;
		half specStrength = s.SpecularIntensity;// = 0.1f + s.SpecularIntensity;// > 1 = brighter, < 1 = darker
		if (NdotL < 0) // <= s.ShadowThreshold)
		{
			
			if ( NdotL < - s.SpecularSize -0.5f && specStrength <= 0.5f) // -0.5f)
			{
				c.rgb = s.ShadowColor *(0.5f + specStrength);// (specStrength + 0.5f);// 0.5f; //  *s.ShadowColor;
			}
			else
			{
				c.rgb = s.ShadowColor;
			}
		}
		else
		{
			if (s.SpecularSize < 1 && NdotL * 1.8f > s.SpecularSize && specStrength >= 0.5f) //  0.5f) // 1.0f)
			{
				c.rgb = s.BrightColor * (0.5f + specStrength);// 1.5f;//  *(specStrength * 2);// 2; // lighter
			}
			else
			{
				c.rgb = s.BrightColor;
			}

		}
		
		// add inner lines
		c.rgb = c.rgb * s.InnerLineColor;
			

		// spec size, 0 => black
		//c.rgb = half4(1, s.SpecularSize, 1, 1); // 1 = 

		//c.rgb = (testDot, testDot, testDot);
		//c.rgb = c.rgb * s.InnerLineColor;
		//NdotL *= 1 - s.ShadowThreshold;// half4(1, 1, 1, 1) - s.ShadowThreshold;
		/**
		//NdotL *= s.SpecularSize;
		half3 dotC = half3(NdotL, NdotL, NdotL);
		half3 shadowC = half3(1-s.ShadowThreshold, 1-s.ShadowThreshold, 1-s.ShadowThreshold);
		half3 specSizeC = half3(s.SpecularSize, s.SpecularSize, s.SpecularSize);
		half3 specSizeCInv  = half3(1-s.SpecularSize, 1-s.SpecularSize, 1-s.SpecularSize);
		half3 specIncC = half3( s.SpecularIntensity,  s.SpecularIntensity,  s.SpecularIntensity);
		half3 specIncCInv = half3(1 - s.SpecularIntensity, 1 - s.SpecularIntensity, 1 - s.SpecularIntensity);
		half3 oneC = half3(1, 1, 1);
		//half3 finalC = (dotC + specSizeC) * 0.5f ;// *shadowC;
		half3 finalC = dotC;
		//finalC = (dotC + shadowC) * 0.5f;

		// intensity: 0:black, 1:noChange, 2:brighter
		half specInt = (s.SpecularIntensity) * 2;
		//finalC = (finalC + (specSizeC * specInt)) * 0.5f;
		//finalC = (finalC + s.SpecularIntensity) * 0.5f;
		finalC = specSizeCInv;
		half specSizeInv = 1 - s.SpecularSize;
		if (specSizeInv > 0.001f)
		{
			c.rgb = dotC * (s.SpecularIntensity * 1.5F);// dotC * specSizeCInv; //  NdotL;
		}
		else
		{
			c.rgb = dotC;// half3(0, 1, 0); //  NdotL;
		}
		
		//c.rgb = dotC *  (diff + c.rgb * spec);
		//c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
		c.rgb = (s.Albedo *  diff + spec);// *atten;
		//c.rgb = _LightColor0.rgb;
		*/
		//finalC = finalC * specInt; // 0 => black, 1=>no change, 2 => brighter
		//finalC = oneC - finalC;

		// NOTE: NdotL = light direction. -1 = away, 1 = towards, 0 = 90 deg sideways
		/*if (NdotL >= 0.99f)
		{
			c.rgb = s.ShadowColor;
		}
		else
		{
			c.rgb = s.BrightColor;
		}
		*/

		//c.rgb = dotC;

		// NOTE: atten = shadows
		//c.rgb = dotC*(atten);
		//c.rgb = half4(0, s.SpecularSize, 0, 1);

		// Note: spec size is strong in highlights and dark shadows e.g hair / metal => highlight. Always black shadows
		//c.rgb = half4(0, 1-s.SpecularSize, 0, 1);

		// Note: intensity = color effect, 1 = lighter, 0 = darker, 0.5 = no effect?
		//c.rgb = half4(0, s.SpecularIntensity, 0, 1);
		
		//c.rgb = half4(0, 1-s.ShadowThreshold, 0, 1);

		/*
		if (NdotL >= 0.99f)
		{
			// dark / dark
			c.rgb = s.ShadowColor * s.ShadowColor;
		}*/
		//c.rgb = s.ShadowThreshold;

		return c;
	}

	void surf(Input IN, inout SurfaceOutputCustom  o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

		fixed4 cSSS = tex2D(_SSSTex, IN.uv_MainTex);
		fixed4 cILM = tex2D(_ILMTex, IN.uv_MainTex);

		o.BrightColor = c.rgb;
		o.ShadowColor = c.rgb * cSSS.rgb;

		float clampedLineColor = cILM.a;
		if (clampedLineColor < _DarkenInnerLineColor)
			clampedLineColor = _DarkenInnerLineColor; 

		// IN.vertexColor = (1, 0, 0, 1);

		//o.ILM = cILM;
		o.InnerLineColor = half3(clampedLineColor, clampedLineColor, clampedLineColor);
		//o.ShadowThreshold = (IN.vertexColor.r - 0.5) * _ShadowContrast + 0.5;
		//##o.ShadowThreshold *= cILM.g;
		
		//##o.ShadowThreshold = 1 - o.ShadowThreshold;
		//o.ShadowThreshold = (IN.vertexColor.r - 0.5) * _ShadowContrast + 0.5;
		
		
		
			// if vert color not white (ignored)
			//o.ShadowThreshold = 1 - cILM.g;
		float vertColor = IN.vertexColor.r;// (IN.vertexColor.r - 0.5) * _ShadowContrast + 0.5; //IN.vertexColor.r;
			// easier to combine black dark areas 
			o.ShadowThreshold = cILM.g;
			o.ShadowThreshold *= vertColor;
			o.ShadowThreshold = 1 - o.ShadowThreshold; // flip black / white


		o.SpecularIntensity = cILM.r;// 1 + (1 - cILM.r);// +cILM.r;// *2; // make whiter
		//o.SpecularSize = 1 - cILM.b;// *0.25f);
		o.SpecularSize =  1-cILM.b;// *0.25f);

		//o.ShadowThreshold = IN.vertexColor.r;

		//cILM.g;// 
		//o.ShadowThreshold += (1-cILM.g); // gen chanel, black = shadow
		//o.Albedo = c;

		//o.ShadowThreshold = IN.vertexColor.r
	}

	ENDCG

	}

	FallBack "Diffuse"
}
