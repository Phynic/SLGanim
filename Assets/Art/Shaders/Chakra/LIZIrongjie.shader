// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:33095,y:32801,varname:node_4795,prsc:2|emission-6376-OUT,alpha-798-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31964,y:32614,varname:_MainTex,prsc:2,ntxv:0,isnm:False|TEX-5873-TEX;n:type:ShaderForge.SFN_Multiply,id:2393,x:32493,y:32701,varname:node_2393,prsc:2|A-6074-RGB,B-2053-RGB,C-797-RGB,D-2970-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:31844,y:33109,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:31858,y:32870,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:798,x:32800,y:33145,varname:node_798,prsc:2|A-6074-A,B-2053-A,C-797-A,D-4337-OUT;n:type:ShaderForge.SFN_Tex2d,id:5939,x:32005,y:33354,varname:node_5939,prsc:2,ntxv:0,isnm:False|TEX-5873-TEX;n:type:ShaderForge.SFN_If,id:4337,x:32617,y:33263,varname:node_4337,prsc:2|A-195-OUT,B-5939-R,GT-9431-OUT,EQ-9431-OUT,LT-6153-OUT;n:type:ShaderForge.SFN_Multiply,id:195,x:32380,y:33216,varname:node_195,prsc:2|A-2053-A,B-7702-OUT;n:type:ShaderForge.SFN_Subtract,id:2586,x:32618,y:33002,varname:node_2586,prsc:2|A-9693-OUT,B-195-OUT;n:type:ShaderForge.SFN_Multiply,id:8357,x:32505,y:32872,varname:node_8357,prsc:2|A-6074-RGB,B-2053-RGB;n:type:ShaderForge.SFN_ValueProperty,id:9693,x:31896,y:33040,ptovrint:False,ptlb:edge,ptin:_edge,varname:node_9693,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:7702,x:31943,y:33280,ptovrint:False,ptlb:AlphaMuilt,ptin:_AlphaMuilt,varname:node_7702,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_If,id:3964,x:32736,y:32771,varname:node_3964,prsc:2|A-2586-OUT,B-5939-R,GT-8357-OUT,EQ-8357-OUT,LT-2393-OUT;n:type:ShaderForge.SFN_Vector1,id:9431,x:32336,y:33362,varname:node_9431,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:6153,x:32359,y:33410,varname:node_6153,prsc:2,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:2970,x:31877,y:32797,ptovrint:False,ptlb:EdgeEmission,ptin:_EdgeEmission,varname:node_2970,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Multiply,id:6376,x:32864,y:32638,varname:node_6376,prsc:2|A-6560-RGB,B-3964-OUT;n:type:ShaderForge.SFN_Color,id:6560,x:32663,y:32468,ptovrint:False,ptlb:node_6560,ptin:_node_6560,varname:node_6560,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2dAsset,id:5873,x:31345,y:32894,ptovrint:False,ptlb:node_5873,ptin:_node_5873,varname:node_5873,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;proporder:6560-797-7702-9693-2970-5873;pass:END;sub:END;*/

Shader "Shader Forge/LIZIrongjie" {
    Properties {
        [HDR]_node_6560 ("node_6560", Color) = (0.5,0.5,0.5,1)
        [HDR]_TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _AlphaMuilt ("AlphaMuilt", Float ) = 1
        _edge ("edge", Float ) = 0
        _EdgeEmission ("EdgeEmission", Float ) = 2
        _node_5873 ("node_5873", 2D) = "white" {}
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
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TintColor;
            uniform float _edge;
            uniform float _AlphaMuilt;
            uniform float _EdgeEmission;
            uniform float4 _node_6560;
            uniform sampler2D _node_5873; uniform float4 _node_5873_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float node_195 = (i.vertexColor.a*_AlphaMuilt);
                float4 node_5939 = tex2D(_node_5873,TRANSFORM_TEX(i.uv0, _node_5873));
                float node_3964_if_leA = step((_edge-node_195),node_5939.r);
                float node_3964_if_leB = step(node_5939.r,(_edge-node_195));
                float4 _MainTex = tex2D(_node_5873,TRANSFORM_TEX(i.uv0, _node_5873));
                float3 node_8357 = (_MainTex.rgb*i.vertexColor.rgb);
                float3 emissive = (_node_6560.rgb*lerp((node_3964_if_leA*(_MainTex.rgb*i.vertexColor.rgb*_TintColor.rgb*_EdgeEmission))+(node_3964_if_leB*node_8357),node_8357,node_3964_if_leA*node_3964_if_leB));
                float3 finalColor = emissive;
                float node_4337_if_leA = step(node_195,node_5939.r);
                float node_4337_if_leB = step(node_5939.r,node_195);
                float node_9431 = 1.0;
                fixed4 finalRGBA = fixed4(finalColor,(_MainTex.a*i.vertexColor.a*_TintColor.a*lerp((node_4337_if_leA*0.0)+(node_4337_if_leB*node_9431),node_9431,node_4337_if_leA*node_4337_if_leB)));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
