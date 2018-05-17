// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1558,x:32963,y:32713,varname:node_1558,prsc:2|emission-8682-OUT;n:type:ShaderForge.SFN_Tex2d,id:1449,x:32035,y:32674,ptovrint:False,ptlb:Source,ptin:_Source,varname:node_1449,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2719,x:32035,y:33074,ptovrint:False,ptlb:g_SolidColor,ptin:_g_SolidColor,varname:node_2719,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4984,x:32035,y:32867,ptovrint:False,ptlb:g_mBlur,ptin:_g_mBlur,varname:node_4984,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Vector1,id:4577,x:32035,y:33270,varname:node_4577,prsc:2,v1:0;n:type:ShaderForge.SFN_If,id:7743,x:32368,y:33100,varname:node_7743,prsc:2|A-2719-A,B-4577-OUT,GT-4577-OUT,EQ-4984-RGB,LT-4984-RGB;n:type:ShaderForge.SFN_Add,id:8682,x:32410,y:32815,varname:node_8682,prsc:2|A-1449-RGB,B-7743-OUT;proporder:1449-2719-4984;pass:END;sub:END;*/

Shader "Shader/Combine" {
    Properties {
        _Source ("Source", 2D) = "white" {}
        _g_SolidColor ("g_SolidColor", 2D) = "white" {}
        _g_mBlur ("g_mBlur", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 100
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Source; uniform float4 _Source_ST;
            uniform sampler2D _g_SolidColor; uniform float4 _g_SolidColor_ST;
            uniform sampler2D _g_mBlur; uniform float4 _g_mBlur_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _Source_var = tex2D(_Source,TRANSFORM_TEX(i.uv0, _Source));
                float4 _g_SolidColor_var = tex2D(_g_SolidColor,TRANSFORM_TEX(i.uv0, _g_SolidColor));
                float node_4577 = 0.0;
                float node_7743_if_leA = step(_g_SolidColor_var.a,node_4577);
                float node_7743_if_leB = step(node_4577,_g_SolidColor_var.a);
                float4 _g_mBlur_var = tex2D(_g_mBlur,TRANSFORM_TEX(i.uv0, _g_mBlur));
                float3 emissive = (_Source_var.rgb+lerp((node_7743_if_leA*_g_mBlur_var.rgb)+(node_7743_if_leB*node_4577),_g_mBlur_var.rgb,node_7743_if_leA*node_7743_if_leB));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
