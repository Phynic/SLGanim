// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33457,y:34056,varname:node_9361,prsc:2|normal-1033-OUT,custl-9339-OUT,alpha-7920-OUT,refract-916-OUT;n:type:ShaderForge.SFN_Tex2d,id:3951,x:31317,y:33473,ptovrint:False,ptlb:Diffuse_copy,ptin:_Diffuse_copy,varname:_Diffuse_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:27fdc82073ea4b54ea288df9eb4d9d78,ntxv:0,isnm:False;n:type:ShaderForge.SFN_If,id:5916,x:31853,y:33975,varname:node_5916,prsc:2|A-6971-OUT,B-3951-R,GT-9446-OUT,EQ-9446-OUT,LT-4267-OUT;n:type:ShaderForge.SFN_Vector1,id:9446,x:31270,y:34205,varname:node_9446,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:4267,x:31270,y:34267,varname:node_4267,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:3662,x:32769,y:34025,varname:node_3662,prsc:2|A-3951-A,B-5002-OUT;n:type:ShaderForge.SFN_VertexColor,id:2554,x:31157,y:33829,varname:node_2554,prsc:2;n:type:ShaderForge.SFN_If,id:864,x:31853,y:34191,varname:node_864,prsc:2|A-2554-R,B-3951-R,GT-9446-OUT,EQ-9446-OUT,LT-4267-OUT;n:type:ShaderForge.SFN_Add,id:6971,x:31461,y:33775,varname:node_6971,prsc:2|A-7124-OUT,B-2554-R;n:type:ShaderForge.SFN_ValueProperty,id:7124,x:31305,y:33702,ptovrint:False,ptlb:0_勾边大小,ptin:_0_,varname:node_2095,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Subtract,id:594,x:32128,y:34135,varname:node_594,prsc:2|A-5916-OUT,B-864-OUT;n:type:ShaderForge.SFN_Multiply,id:8261,x:32339,y:34087,varname:node_8261,prsc:2|A-594-OUT,B-2552-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2552,x:32101,y:34384,ptovrint:False,ptlb:1_勾边亮度,ptin:_1_,varname:node_7276,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:100;n:type:ShaderForge.SFN_Add,id:5002,x:32546,y:34041,varname:node_5002,prsc:2|A-5916-OUT,B-8261-OUT;n:type:ShaderForge.SFN_Multiply,id:2224,x:32074,y:33562,varname:node_2224,prsc:2|A-2014-RGB,B-1836-OUT;n:type:ShaderForge.SFN_Multiply,id:5005,x:32300,y:33501,varname:node_5005,prsc:2|A-1858-OUT,B-2224-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1858,x:32074,y:33443,ptovrint:False,ptlb:2_diffuse强度,ptin:_2_diffuse,varname:node_7493,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_Multiply,id:9339,x:32568,y:33551,varname:node_9339,prsc:2|A-5005-OUT,B-3951-R;n:type:ShaderForge.SFN_Multiply,id:7920,x:32980,y:33977,varname:node_7920,prsc:2|A-2554-A,B-3662-OUT;n:type:ShaderForge.SFN_Color,id:2014,x:31698,y:33272,ptovrint:False,ptlb:3_color,ptin:_3_color,varname:node_5389,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.8,c2:0.3,c3:0.1,c4:1;n:type:ShaderForge.SFN_Lerp,id:1033,x:33060,y:34194,varname:node_1033,prsc:2|A-6392-OUT,B-3951-A,T-5576-OUT;n:type:ShaderForge.SFN_Vector3,id:6392,x:32717,y:34216,varname:node_6392,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:5576,x:32278,y:34662,ptovrint:False,ptlb:4_扭曲强度,ptin:_4_,varname:node_2822,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2,max:1;n:type:ShaderForge.SFN_Multiply,id:574,x:32884,y:34762,varname:node_574,prsc:2|A-5576-OUT,B-3440-OUT;n:type:ShaderForge.SFN_Vector1,id:3440,x:32608,y:34852,varname:node_3440,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:916,x:33261,y:34774,varname:node_916,prsc:2|A-4199-OUT,B-574-OUT;n:type:ShaderForge.SFN_Multiply,id:4199,x:32884,y:34552,varname:node_4199,prsc:2|A-6543-OUT,B-2554-A;n:type:ShaderForge.SFN_ComponentMask,id:6543,x:32604,y:34387,varname:node_6543,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-3951-RGB;n:type:ShaderForge.SFN_Add,id:1836,x:31586,y:33420,varname:node_1836,prsc:2|A-1197-OUT,B-3951-RGB;n:type:ShaderForge.SFN_TexCoord,id:4879,x:30295,y:33451,varname:node_4879,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector2,id:8192,x:30346,y:33298,varname:node_8192,prsc:2,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Distance,id:4226,x:30516,y:33280,varname:node_4226,prsc:2|A-4879-UVOUT,B-8192-OUT;n:type:ShaderForge.SFN_OneMinus,id:1173,x:30686,y:33264,varname:node_1173,prsc:2|IN-4226-OUT;n:type:ShaderForge.SFN_Power,id:1197,x:30874,y:33331,varname:node_1197,prsc:2|VAL-1173-OUT,EXP-6514-OUT;n:type:ShaderForge.SFN_Slider,id:6514,x:30542,y:33518,ptovrint:False,ptlb:node_758,ptin:_node_758,varname:node_758,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:7.864451,max:11;proporder:3951-1858-2014-6514-5576-7124-2552;pass:END;sub:END;*/

Shader "Shader Forge/daoguang_01" {
    Properties {
        _Diffuse_copy ("Diffuse_copy", 2D) = "white" {}
        _2_diffuse ("2_diffuse强度", Float ) = 10
        [HDR]_3_color ("3_color", Color) = (0.8,0.3,0.1,1)
        _node_758 ("node_758", Range(0, 11)) = 7.864451
        _4_ ("4_扭曲强度", Range(0, 1)) = 0.2
        _0_ ("0_勾边大小", Float ) = 0.1
        _1_ ("1_勾边亮度", Float ) = 100
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
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
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _Diffuse_copy; uniform float4 _Diffuse_copy_ST;
            uniform float _0_;
            uniform float _1_;
            uniform float _2_diffuse;
            uniform float4 _3_color;
            uniform float _4_;
            uniform float _node_758;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD5;
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 _Diffuse_copy_var = tex2D(_Diffuse_copy,TRANSFORM_TEX(i.uv0, _Diffuse_copy));
                float3 normalLocal = lerp(float3(0,0,1),float3(_Diffuse_copy_var.a,_Diffuse_copy_var.a,_Diffuse_copy_var.a),_4_);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + ((_Diffuse_copy_var.rgb.rg*i.vertexColor.a)*(_4_*0.1));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
                float3 finalColor = ((_2_diffuse*(_3_color.rgb*(pow((1.0 - distance(i.uv0,float2(0.5,0.5))),_node_758)+_Diffuse_copy_var.rgb)))*_Diffuse_copy_var.r);
                float node_5916_if_leA = step((_0_+i.vertexColor.r),_Diffuse_copy_var.r);
                float node_5916_if_leB = step(_Diffuse_copy_var.r,(_0_+i.vertexColor.r));
                float node_4267 = 0.0;
                float node_9446 = 1.0;
                float node_5916 = lerp((node_5916_if_leA*node_4267)+(node_5916_if_leB*node_9446),node_9446,node_5916_if_leA*node_5916_if_leB);
                float node_864_if_leA = step(i.vertexColor.r,_Diffuse_copy_var.r);
                float node_864_if_leB = step(_Diffuse_copy_var.r,i.vertexColor.r);
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,(i.vertexColor.a*(_Diffuse_copy_var.a*(node_5916+((node_5916-lerp((node_864_if_leA*node_4267)+(node_864_if_leB*node_9446),node_9446,node_864_if_leA*node_864_if_leB))*_1_))))),1);
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
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
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
