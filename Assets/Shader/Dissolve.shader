Shader "Shader/Dissolve" 
{
    Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _NoiseTex ("NoiseTex (R)",2D) = "white"{}
        _DissolveSpeed ("DissolveSpeed (Second)",Float) = 1
        _EdgeWidth("EdgeWidth",Range(0,0.5)) = 0.1
        _EdgeColor("EdgeColor",Color) = (1,1,1,1)
		_DissolveStartTime("DissolveStartTime",float) = 0
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            uniform sampler2D _MainTex;
            uniform sampler2D _NoiseTex;
            uniform float _DissolveSpeed;
            uniform float _EdgeWidth;
            uniform float4 _EdgeColor;
			uniform float _DissolveStartTime;
            float4 frag(v2f_img i):COLOR
            {
				bool isNormal = true;
				float c = 1;
                float4 texColor = tex2D(_MainTex,i.uv);
				if(_DissolveStartTime > 0 )
				{
					float DissolveFactor = saturate((_Time.y-_DissolveStartTime) * _DissolveSpeed);
					float noiseValue = tex2D(_NoiseTex,i.uv).r;
					if(noiseValue <= DissolveFactor)
					{
						discard;
					}
					float EdgeFactor = saturate((noiseValue - DissolveFactor)/(_EdgeWidth*DissolveFactor));
					float4 BlendColor = texColor * _EdgeColor;
					texColor = lerp(texColor,BlendColor,1 - EdgeFactor);
				}
				return texColor;
            }       
            ENDCG
        }
    } 
    FallBack Off
}