// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Shadow Receiver"
{

    Properties
    {
        _Color("Shadow Color", Color) = (0, 0, 0, 0.6)
    }

    SubShader {
         Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "Lit"
            "IgnoreProjector" = "True"
        }
        
        Pass {
            Name "FORWARD"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
             // Render State Commands
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            
            #pragma vertex vert_shadow  
            #pragma fragment frag_shadow 

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"


            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv     : TEXCOORD0;
            };

            struct v2f_shadow {
            float4 pos : SV_POSITION;
            LIGHTING_COORDS(0, 1)
        };
            
        half4 _Color;

        v2f_shadow vert_shadow(appdata_full v)
        {
            v2f_shadow o;
            o.pos = UnityObjectToClipPos(v.vertex);
            TRANSFER_VERTEX_TO_FRAGMENT(o);
            return o;
        }

        half4 frag_shadow(v2f_shadow IN) : SV_Target
        {
            half atten = LIGHT_ATTENUATION(IN);
            return half4(atten, atten,atten,1);
        }
            ENDCG
        }

        		// Shadow casting support.
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/DepthNormals"

    }


    //    SubShader
    //{
    //    Tags{ "Queue" = "AlphaTest+49" }


    //        // Depth fill pass
    //        Pass
    //    {


    //        ColorMask 0

    //        CGPROGRAM

    //        #pragma vertex vert
    //        #pragma fragment frag

    //        struct v2f {
    //            float4 pos : SV_POSITION;
    //        };

    //        v2f vert(appdata_full v)
    //        {
    //            v2f o;
    //            o.pos = UnityObjectToClipPos(v.vertex);
    //            return o;
    //        }

    //        half4 frag(v2f IN) : SV_Target
    //        {
    //            return (half4)0;
    //        }

    //        ENDCG
    //    }

    //        // Forward base pass
    //            Pass
    //        {
    //            Tags { "LightMode" = "ForwardBase" }
    //            Blend SrcAlpha OneMinusSrcAlpha
    //            CGPROGRAM
    //            #pragma vertex vert_shadow
    //            #pragma fragment frag_shadow
    //            #pragma multi_compile_fwdbase
    //            ENDCG
    //        }

    //            // Forward add pass
    //            Pass
    //        {
    //            Tags { "LightMode" = "ForwardAdd" }
    //            Blend SrcAlpha OneMinusSrcAlpha
    //            CGPROGRAM
    //            #pragma vertex vert_shadow
    //            #pragma fragment frag_shadow
    //            #pragma multi_compile_fwdadd_fullshadows
    //            ENDCG
    //        }
    //}
    //FallBack "Mobile/VertexLit"
}