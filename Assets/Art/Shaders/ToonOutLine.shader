// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:0,x:34913,y:32056,varname:node_0,prsc:2|custl-84-OUT,olwid-7598-OUT,olcol-9157-RGB;n:type:ShaderForge.SFN_Color,id:80,x:34005,y:32010,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:82,x:33954,y:31773,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:_Diffuse,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:637e817d3c6658b4ca447b4c9a8a87ef,ntxv:3,isnm:False;n:type:ShaderForge.SFN_Multiply,id:84,x:34483,y:32053,cmnt:Diffuse Light,varname:node_84,prsc:2|A-9613-OUT,B-80-RGB,C-8223-OUT;n:type:ShaderForge.SFN_NormalVector,id:2157,x:33020,y:32224,prsc:2,pt:True;n:type:ShaderForge.SFN_LightVector,id:7191,x:33020,y:32083,varname:node_7191,prsc:2;n:type:ShaderForge.SFN_Dot,id:5570,x:33280,y:32205,varname:node_5570,prsc:2,dt:0|A-7191-OUT,B-2157-OUT;n:type:ShaderForge.SFN_Lerp,id:8223,x:34498,y:32263,varname:node_8223,prsc:2|A-8188-OUT,B-9438-OUT,T-4802-OUT;n:type:ShaderForge.SFN_Color,id:9157,x:34493,y:32657,ptovrint:False,ptlb:OutLineColor,ptin:_OutLineColor,varname:node_9157,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Vector1,id:4802,x:34409,y:32474,varname:node_4802,prsc:2,v1:0.6;n:type:ShaderForge.SFN_Vector1,id:8188,x:34271,y:32208,varname:node_8188,prsc:2,v1:1.5;n:type:ShaderForge.SFN_If,id:2761,x:33573,y:32258,varname:node_2761,prsc:2|A-5570-OUT,B-5820-OUT,GT-8869-OUT,EQ-8869-OUT,LT-5820-OUT;n:type:ShaderForge.SFN_Vector1,id:5820,x:33280,y:32374,varname:node_5820,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:8869,x:33280,y:32468,varname:node_8869,prsc:2,v1:1;n:type:ShaderForge.SFN_LightAttenuation,id:8410,x:33557,y:31917,varname:node_8410,prsc:2;n:type:ShaderForge.SFN_Vector1,id:459,x:33557,y:32143,cmnt:如果为0则对遮挡阴影无反应,varname:node_459,prsc:2,v1:0.5;n:type:ShaderForge.SFN_If,id:9438,x:33889,y:32180,varname:node_9438,prsc:2|A-8410-OUT,B-459-OUT,GT-2761-OUT,EQ-3617-OUT,LT-3617-OUT;n:type:ShaderForge.SFN_Vector1,id:3617,x:33642,y:32397,varname:node_3617,prsc:2,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:7598,x:34089,y:32696,ptovrint:False,ptlb:OutLineWidth,ptin:_OutLineWidth,varname:node_7598,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.02;n:type:ShaderForge.SFN_SwitchProperty,id:9613,x:34231,y:31894,ptovrint:False,ptlb:Gray,ptin:_Gray,varname:node_9613,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-82-RGB,B-82-G;proporder:80-82-9157-7598-9613;pass:END;sub:END;*/

Shader "Shader/ToonOutLine" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Diffuse ("Diffuse", 2D) = "bump" {}
        _OutLineColor ("OutLineColor", Color) = (0,0,0,1)
        _OutLineWidth ("OutLineWidth", Float ) = 0.02
        [MaterialToggle] _Gray ("Gray", Float ) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "Outline"
            Tags {
            }
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float4 _OutLineColor;
            uniform float _OutLineWidth;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( float4(v.vertex.xyz + v.normal*_OutLineWidth,1) );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                return fixed4(_OutLineColor.rgb,0);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform fixed _Gray;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float node_9438_if_leA = step(attenuation,0.5);
                float node_9438_if_leB = step(0.5,attenuation);
                float node_3617 = 0.0;
                float node_5820 = 0.0;
                float node_2761_if_leA = step(dot(lightDirection,normalDirection),node_5820);
                float node_2761_if_leB = step(node_5820,dot(lightDirection,normalDirection));
                float node_8869 = 1.0;
                float3 finalColor = (lerp( _Diffuse_var.rgb, _Diffuse_var.g, _Gray )*_Color.rgb*lerp(1.5,lerp((node_9438_if_leA*node_3617)+(node_9438_if_leB*lerp((node_2761_if_leA*node_5820)+(node_2761_if_leB*node_8869),node_8869,node_2761_if_leA*node_2761_if_leB)),node_3617,node_9438_if_leA*node_9438_if_leB),0.6));
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform fixed _Gray;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float node_9438_if_leA = step(attenuation,0.5);
                float node_9438_if_leB = step(0.5,attenuation);
                float node_3617 = 0.0;
                float node_5820 = 0.0;
                float node_2761_if_leA = step(dot(lightDirection,normalDirection),node_5820);
                float node_2761_if_leB = step(node_5820,dot(lightDirection,normalDirection));
                float node_8869 = 1.0;
                float3 finalColor = (lerp( _Diffuse_var.rgb, _Diffuse_var.g, _Gray )*_Color.rgb*lerp(1.5,lerp((node_9438_if_leA*node_3617)+(node_9438_if_leB*lerp((node_2761_if_leA*node_5820)+(node_2761_if_leB*node_8869),node_8869,node_2761_if_leA*node_2761_if_leB)),node_3617,node_9438_if_leA*node_9438_if_leB),0.6));
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
