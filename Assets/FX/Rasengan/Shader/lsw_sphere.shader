// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33209,y:32712,varname:node_9361,prsc:2|emission-8073-OUT,custl-9172-OUT,alpha-5946-OUT;n:type:ShaderForge.SFN_Fresnel,id:6713,x:31975,y:32668,varname:node_6713,prsc:2;n:type:ShaderForge.SFN_Exp,id:430,x:31975,y:32794,varname:node_430,prsc:2,et:0|IN-4209-OUT;n:type:ShaderForge.SFN_Slider,id:4209,x:31578,y:32823,ptovrint:False,ptlb:exp,ptin:_exp,varname:node_4209,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1797094,max:1;n:type:ShaderForge.SFN_Power,id:8876,x:32164,y:32668,varname:node_8876,prsc:2|VAL-6713-OUT,EXP-430-OUT;n:type:ShaderForge.SFN_Tex2d,id:4728,x:32596,y:32486,varname:node_4728,prsc:2,tex:94bba27e155773f418e034dc79c3bea5,ntxv:0,isnm:False|UVIN-6577-UVOUT,TEX-4383-TEX;n:type:ShaderForge.SFN_Color,id:9552,x:32701,y:33078,ptovrint:False,ptlb:color,ptin:_color,varname:node_9552,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4167387,c2:0.5335968,c3:0.6029412,c4:1;n:type:ShaderForge.SFN_Multiply,id:5742,x:32885,y:32969,varname:node_5742,prsc:2|A-9552-RGB,B-1360-OUT;n:type:ShaderForge.SFN_TexCoord,id:4310,x:31443,y:32569,varname:node_4310,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:6577,x:32125,y:32479,varname:node_6577,prsc:2,spu:-6,spv:6|UVIN-9298-OUT;n:type:ShaderForge.SFN_Add,id:8073,x:33141,y:32509,varname:node_8073,prsc:2|A-7222-OUT,B-5742-OUT;n:type:ShaderForge.SFN_Multiply,id:5946,x:32970,y:33141,varname:node_5946,prsc:2|A-9552-A,B-3808-A,C-9041-OUT;n:type:ShaderForge.SFN_VertexColor,id:3808,x:32701,y:33225,varname:node_3808,prsc:2;n:type:ShaderForge.SFN_Slider,id:9041,x:32647,y:33387,ptovrint:False,ptlb:opacity,ptin:_opacity,varname:node_9041,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7463995,max:1;n:type:ShaderForge.SFN_Multiply,id:9172,x:32885,y:32764,varname:node_9172,prsc:2|A-9713-RGB,B-1056-OUT,C-4791-OUT;n:type:ShaderForge.SFN_Slider,id:1056,x:32427,y:32781,ptovrint:False,ptlb:streng,ptin:_streng,varname:node_1056,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Vector1,id:4999,x:32388,y:32904,varname:node_4999,prsc:2,v1:0.4;n:type:ShaderForge.SFN_Color,id:9713,x:32728,y:32614,ptovrint:False,ptlb:color_tow,ptin:_color_tow,varname:node_9713,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4189555,c2:0.7035332,c3:0.9044118,c4:1;n:type:ShaderForge.SFN_Step,id:4791,x:32564,y:32849,varname:node_4791,prsc:2|A-3244-OUT,B-4999-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4383,x:32354,y:32440,ptovrint:False,ptlb:texture,ptin:_texture,varname:node_4383,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:94bba27e155773f418e034dc79c3bea5,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Panner,id:9545,x:31800,y:32300,varname:node_9545,prsc:2,spu:-6,spv:6|UVIN-7729-OUT;n:type:ShaderForge.SFN_Tex2d,id:6116,x:32596,y:32363,varname:node_6116,prsc:2,tex:94bba27e155773f418e034dc79c3bea5,ntxv:0,isnm:False|UVIN-4305-UVOUT,TEX-4383-TEX;n:type:ShaderForge.SFN_Rotator,id:4305,x:32125,y:32349,varname:node_4305,prsc:2|UVIN-9545-UVOUT,ANG-7979-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7979,x:31936,y:32370,ptovrint:False,ptlb:ang,ptin:_ang,varname:node_7979,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:30;n:type:ShaderForge.SFN_Set,id:5541,x:31829,y:32569,varname:uv,prsc:2|IN-6220-OUT;n:type:ShaderForge.SFN_Get,id:9298,x:31829,y:32514,varname:node_9298,prsc:2|IN-5541-OUT;n:type:ShaderForge.SFN_Get,id:7729,x:31618,y:32300,varname:node_7729,prsc:2|IN-5541-OUT;n:type:ShaderForge.SFN_Set,id:4055,x:32444,y:32669,varname:power,prsc:2|IN-8876-OUT;n:type:ShaderForge.SFN_Get,id:3244,x:32388,y:32849,varname:node_3244,prsc:2|IN-4055-OUT;n:type:ShaderForge.SFN_Get,id:1360,x:32680,y:32988,varname:node_1360,prsc:2|IN-4055-OUT;n:type:ShaderForge.SFN_Multiply,id:6220,x:31647,y:32569,varname:node_6220,prsc:2|A-4310-UVOUT,B-2215-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2215,x:31443,y:32730,ptovrint:False,ptlb:uv,ptin:_uv,varname:node_2215,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:20;n:type:ShaderForge.SFN_Add,id:3249,x:32759,y:32363,varname:node_3249,prsc:2|A-6116-RGB,B-4728-RGB;n:type:ShaderForge.SFN_Multiply,id:7222,x:32922,y:32363,varname:node_7222,prsc:2|A-3249-OUT,B-3581-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3581,x:32759,y:32533,ptovrint:False,ptlb:BoWenStrength,ptin:_BoWenStrength,varname:node_3581,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;proporder:4209-9552-9041-1056-9713-4383-7979-2215-3581;pass:END;sub:END;*/

Shader "Shader Forge/lsw_sphere" {
    Properties {
        _exp ("exp", Range(0, 1)) = 0.1797094
        [HDR]_color ("color", Color) = (0.4167387,0.5335968,0.6029412,1)
        _opacity ("opacity", Range(0, 1)) = 0.7463995
        _streng ("streng", Range(0, 1)) = 1
        _color_tow ("color_tow", Color) = (0.4189555,0.7035332,0.9044118,1)
        _texture ("texture", 2D) = "white" {}
        _ang ("ang", Float ) = 30
        _uv ("uv", Float ) = 20
        _BoWenStrength ("BoWenStrength", Float ) = 10
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float _exp;
            uniform float4 _color;
            uniform float _opacity;
            uniform float _streng;
            uniform float4 _color_tow;
            uniform sampler2D _texture; uniform float4 _texture_ST;
            uniform float _ang;
            uniform float _uv;
            uniform float _BoWenStrength;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float node_4305_ang = _ang;
                float node_4305_spd = 1.0;
                float node_4305_cos = cos(node_4305_spd*node_4305_ang);
                float node_4305_sin = sin(node_4305_spd*node_4305_ang);
                float2 node_4305_piv = float2(0.5,0.5);
                float4 node_3690 = _Time + _TimeEditor;
                float2 uv = (i.uv0*_uv);
                float2 node_4305 = (mul((uv+node_3690.g*float2(-6,6))-node_4305_piv,float2x2( node_4305_cos, -node_4305_sin, node_4305_sin, node_4305_cos))+node_4305_piv);
                float4 node_6116 = tex2D(_texture,TRANSFORM_TEX(node_4305, _texture));
                float2 node_6577 = (uv+node_3690.g*float2(-6,6));
                float4 node_4728 = tex2D(_texture,TRANSFORM_TEX(node_6577, _texture));
                float node_8876 = pow((1.0-max(0,dot(normalDirection, viewDirection))),exp(_exp));
                float power = node_8876;
                float3 emissive = (((node_6116.rgb+node_4728.rgb)*_BoWenStrength)+(_color.rgb*power));
                float3 finalColor = emissive + (_color_tow.rgb*_streng*step(power,0.4));
                return fixed4(finalColor,(_color.a*i.vertexColor.a*_opacity));
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
