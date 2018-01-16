// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.35 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.35;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:7142,x:33187,y:32629,varname:node_7142,prsc:2|diff-7292-OUT,emission-9198-OUT,clip-9987-OUT;n:type:ShaderForge.SFN_Fresnel,id:6131,x:31981,y:33263,varname:node_6131,prsc:2|EXP-8420-OUT;n:type:ShaderForge.SFN_Slider,id:8420,x:31632,y:33252,ptovrint:False,ptlb:Fnl Exp,ptin:_FnlExp,varname:node_8420,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4.368932,max:10;n:type:ShaderForge.SFN_Tex2d,id:8312,x:32216,y:32495,ptovrint:False,ptlb:Diffuse Texture,ptin:_DiffuseTexture,varname:node_8312,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:4052,x:31860,y:33058,ptovrint:False,ptlb:Fnl Color,ptin:_FnlColor,varname:node_4052,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:5513,x:32146,y:33202,varname:node_5513,prsc:2|A-4052-RGB,B-6131-OUT;n:type:ShaderForge.SFN_Add,id:9198,x:32887,y:32861,varname:node_9198,prsc:2|A-6500-OUT,B-600-OUT;n:type:ShaderForge.SFN_Multiply,id:7400,x:32421,y:32427,varname:node_7400,prsc:2|A-4901-RGB,B-8312-RGB;n:type:ShaderForge.SFN_Color,id:4901,x:32216,y:32330,ptovrint:False,ptlb:Diffuse Color,ptin:_DiffuseColor,varname:node_4901,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:7292,x:32599,y:32427,varname:node_7292,prsc:2|A-5012-OUT,B-7400-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5012,x:32421,y:32363,ptovrint:False,ptlb:Diffuse QD,ptin:_DiffuseQD,varname:node_5012,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:8994,x:32528,y:33170,varname:node_8994,prsc:2|A-4901-A,B-8312-A;n:type:ShaderForge.SFN_Multiply,id:600,x:32346,y:33223,varname:node_600,prsc:2|A-5513-OUT,B-9888-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9888,x:32146,y:33389,ptovrint:False,ptlb:Fnl QD,ptin:_FnlQD,varname:node_9888,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_If,id:315,x:32035,y:32813,varname:node_315,prsc:2|A-5763-R,B-1002-OUT,GT-3746-OUT,EQ-3746-OUT,LT-6639-OUT;n:type:ShaderForge.SFN_If,id:8628,x:32035,y:32939,varname:node_8628,prsc:2|A-5763-R,B-303-OUT,GT-3746-OUT,EQ-3746-OUT,LT-6639-OUT;n:type:ShaderForge.SFN_Vector1,id:3746,x:31767,y:32891,varname:node_3746,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:6639,x:31767,y:32961,varname:node_6639,prsc:2,v1:0;n:type:ShaderForge.SFN_Tex2d,id:5763,x:31767,y:32644,ptovrint:False,ptlb:Mask Texture,ptin:_MaskTexture,varname:node_5763,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:303,x:31638,y:32847,varname:node_303,prsc:2|A-1002-OUT,B-1502-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1002,x:31456,y:32819,ptovrint:False,ptlb:Mask Exp,ptin:_MaskExp,varname:node_1002,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:1502,x:31456,y:32891,ptovrint:False,ptlb:Mask GB,ptin:_MaskGB,varname:_node_1002_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.05;n:type:ShaderForge.SFN_Subtract,id:3052,x:32237,y:32967,varname:node_3052,prsc:2|A-315-OUT,B-8628-OUT;n:type:ShaderForge.SFN_Multiply,id:9987,x:32738,y:33047,varname:node_9987,prsc:2|A-315-OUT,B-8994-OUT;n:type:ShaderForge.SFN_Multiply,id:1553,x:32442,y:32924,varname:node_1553,prsc:2|A-9107-RGB,B-3052-OUT;n:type:ShaderForge.SFN_Color,id:9107,x:32237,y:32826,ptovrint:False,ptlb:Mask GB Color,ptin:_MaskGBColor,varname:_DiffuseColor_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:6500,x:32612,y:32887,varname:node_6500,prsc:2|A-8419-OUT,B-1553-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8419,x:32442,y:32870,ptovrint:False,ptlb:Mask GB Exp,ptin:_MaskGBExp,varname:node_8419,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:20;proporder:4901-5012-8312-8420-4052-9888-5763-1002-1502-9107-8419;pass:END;sub:END;*/

Shader "cgwell/Dissolve_Fresnel_Opaque" {
    Properties {
        _DiffuseColor ("Diffuse Color", Color) = (1,1,1,1)
        _DiffuseQD ("Diffuse QD", Float ) = 1
        _DiffuseTexture ("Diffuse Texture", 2D) = "white" {}
        _FnlExp ("Fnl Exp", Range(0, 10)) = 4.368932
        _FnlColor ("Fnl Color", Color) = (1,0,0,1)
        _FnlQD ("Fnl QD", Float ) = 1
        _MaskTexture ("Mask Texture", 2D) = "white" {}
        _MaskExp ("Mask Exp", Float ) = 0.5
        _MaskGB ("Mask GB", Float ) = 0.05
        _MaskGBColor ("Mask GB Color", Color) = (1,0,0,1)
        _MaskGBExp ("Mask GB Exp", Float ) = 20
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float _FnlExp;
            uniform sampler2D _DiffuseTexture; uniform float4 _DiffuseTexture_ST;
            uniform float4 _FnlColor;
            uniform float4 _DiffuseColor;
            uniform float _DiffuseQD;
            uniform float _FnlQD;
            uniform sampler2D _MaskTexture; uniform float4 _MaskTexture_ST;
            uniform float _MaskExp;
            uniform float _MaskGB;
            uniform float4 _MaskGBColor;
            uniform float _MaskGBExp;
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
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 _MaskTexture_var = tex2D(_MaskTexture,TRANSFORM_TEX(i.uv0, _MaskTexture));
                float node_315_if_leA = step(_MaskTexture_var.r,_MaskExp);
                float node_315_if_leB = step(_MaskExp,_MaskTexture_var.r);
                float node_6639 = 0.0;
                float node_3746 = 1.0;
                float node_315 = lerp((node_315_if_leA*node_6639)+(node_315_if_leB*node_3746),node_3746,node_315_if_leA*node_315_if_leB);
                float4 _DiffuseTexture_var = tex2D(_DiffuseTexture,TRANSFORM_TEX(i.uv0, _DiffuseTexture));
                clip((node_315*(_DiffuseColor.a*_DiffuseTexture_var.a)) - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuseColor = (_DiffuseQD*(_DiffuseColor.rgb*_DiffuseTexture_var.rgb));
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float node_8628_if_leA = step(_MaskTexture_var.r,(_MaskExp+_MaskGB));
                float node_8628_if_leB = step((_MaskExp+_MaskGB),_MaskTexture_var.r);
                float3 emissive = ((_MaskGBExp*(_MaskGBColor.rgb*(node_315-lerp((node_8628_if_leA*node_6639)+(node_8628_if_leB*node_3746),node_3746,node_8628_if_leA*node_8628_if_leB))))+((_FnlColor.rgb*pow(1.0-max(0,dot(normalDirection, viewDirection)),_FnlExp))*_FnlQD));
/// Final Color:
                float3 finalColor = diffuse + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
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
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float _FnlExp;
            uniform sampler2D _DiffuseTexture; uniform float4 _DiffuseTexture_ST;
            uniform float4 _FnlColor;
            uniform float4 _DiffuseColor;
            uniform float _DiffuseQD;
            uniform float _FnlQD;
            uniform sampler2D _MaskTexture; uniform float4 _MaskTexture_ST;
            uniform float _MaskExp;
            uniform float _MaskGB;
            uniform float4 _MaskGBColor;
            uniform float _MaskGBExp;
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
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 _MaskTexture_var = tex2D(_MaskTexture,TRANSFORM_TEX(i.uv0, _MaskTexture));
                float node_315_if_leA = step(_MaskTexture_var.r,_MaskExp);
                float node_315_if_leB = step(_MaskExp,_MaskTexture_var.r);
                float node_6639 = 0.0;
                float node_3746 = 1.0;
                float node_315 = lerp((node_315_if_leA*node_6639)+(node_315_if_leB*node_3746),node_3746,node_315_if_leA*node_315_if_leB);
                float4 _DiffuseTexture_var = tex2D(_DiffuseTexture,TRANSFORM_TEX(i.uv0, _DiffuseTexture));
                clip((node_315*(_DiffuseColor.a*_DiffuseTexture_var.a)) - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 diffuseColor = (_DiffuseQD*(_DiffuseColor.rgb*_DiffuseTexture_var.rgb));
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _DiffuseTexture; uniform float4 _DiffuseTexture_ST;
            uniform float4 _DiffuseColor;
            uniform sampler2D _MaskTexture; uniform float4 _MaskTexture_ST;
            uniform float _MaskExp;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _MaskTexture_var = tex2D(_MaskTexture,TRANSFORM_TEX(i.uv0, _MaskTexture));
                float node_315_if_leA = step(_MaskTexture_var.r,_MaskExp);
                float node_315_if_leB = step(_MaskExp,_MaskTexture_var.r);
                float node_6639 = 0.0;
                float node_3746 = 1.0;
                float node_315 = lerp((node_315_if_leA*node_6639)+(node_315_if_leB*node_3746),node_3746,node_315_if_leA*node_315_if_leB);
                float4 _DiffuseTexture_var = tex2D(_DiffuseTexture,TRANSFORM_TEX(i.uv0, _DiffuseTexture));
                clip((node_315*(_DiffuseColor.a*_DiffuseTexture_var.a)) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
