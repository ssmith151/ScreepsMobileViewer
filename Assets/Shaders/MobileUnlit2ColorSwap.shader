// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33279,y:32659,varname:node_3138,prsc:2|emission-122-OUT,alpha-9599-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32500,y:32630,ptovrint:False,ptlb:Color1,ptin:_Color1,varname:_Color1,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.1,c3:0.1,c4:0.3;n:type:ShaderForge.SFN_Time,id:4993,x:32151,y:32895,varname:node_4993,prsc:2;n:type:ShaderForge.SFN_Cos,id:1233,x:32321,y:32895,varname:node_1233,prsc:2|IN-4993-TDB;n:type:ShaderForge.SFN_Tex2d,id:6899,x:32686,y:32541,varname:__MainTex,prsc:2,tex:0000000000000000f000000000000000,ntxv:0,isnm:False|TEX-7099-TEX;n:type:ShaderForge.SFN_Blend,id:122,x:32966,y:32624,varname:node_122,prsc:2,blmd:1,clmp:True|SRC-6899-RGB,DST-7241-RGB;n:type:ShaderForge.SFN_Tex2dAsset,id:7099,x:32500,y:32399,ptovrint:False,ptlb:PreselectedText,ptin:_PreselectedText,varname:_PreselectedText,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0000000000000000f000000000000000,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Blend,id:9599,x:32966,y:32796,varname:node_9599,prsc:2,blmd:19,clmp:True|SRC-9148-OUT,DST-6899-A;n:type:ShaderForge.SFN_ValueProperty,id:9762,x:32321,y:32751,ptovrint:False,ptlb:node_9762,ptin:_node_9762,varname:_node_9762,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Max,id:9148,x:32514,y:32848,varname:node_9148,prsc:2|A-9762-OUT,B-1233-OUT;proporder:7241-7099-9762;pass:END;sub:END;*/

Shader "Shader Forge/UnlitColorBlinkAlphaTextured" {
    Properties {
        _Color1 ("Color1", Color) = (1,0.1,0.1,0.3)
        _PreselectedText ("PreselectedText", 2D) = "white" {}
        _node_9762 ("node_9762", Float ) = 0.5
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal xboxone 
            #pragma target 3.0
            uniform float4 _Color1;
            uniform sampler2D _PreselectedText; uniform float4 _PreselectedText_ST;
            uniform float _node_9762;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 __MainTex = tex2D(_PreselectedText,TRANSFORM_TEX(i.uv0, _PreselectedText));
                float3 emissive = saturate((__MainTex.rgb*_Color1.rgb));
                float3 finalColor = emissive;
                float4 node_4993 = _Time;
                float node_9148 = max(_node_9762,cos(node_4993.b));
                return fixed4(finalColor,saturate((__MainTex.a-node_9148)));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}