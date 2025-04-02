Shader "L3/BasicToon"
{
    Properties {

        _RampThreshold("Ramp Threshold", Range(0,1)) = 0.5
        //_SpecularPower("Specular Power", Float) = 8.0
        _Color1("Color 1" , COLOR) = (0.9,0.9,0.9,1)
        _Color2("Color 2" , COLOR) = (0.9,0.9,0.9,1)

        [Header(NOISE)]
        _NoiseTex("Noise Texture", 2D) = "white" {}  // 添加噪声纹理
        _NoiseScale("Noise Scale", Float) = 1.0
        _NoiseStrength("Noise Strength", Range(0, 0.2)) = 0.1

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
            
            #pragma vertex vert  
            #pragma fragment frag 

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            sampler2D _NoiseTex;
            float _RampThreshold;
            float4 _Color1,_Color2;
            float _NoiseScale;
            float _NoiseStrength;

            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv     : TEXCOORD0;
            };

            struct v2f {
                float4 pos      : SV_POSITION;
                float2 uv       : TEXCOORD0;
                half3 worldNormal:TEXCOORD1;
            };
            
            v2f vert(appdata_t IN){
                v2f OUT;
                OUT.pos=UnityObjectToClipPos(IN.vertex);
                OUT.uv=IN.uv * _NoiseScale; // 调整噪声采样的缩放
                OUT.worldNormal=normalize(mul((float3x3)unity_ObjectToWorld,IN.normal));
                return OUT;
            }  

            half4 frag(v2f i):SV_Target{    
                half dotProduct=saturate(dot(i.worldNormal,_WorldSpaceLightPos0.xyz));   
                
                // 采样噪声
                float noise = tex2D(_NoiseTex, i.uv).r;
                noise = (noise - 0.5) * _NoiseStrength;  // 调整噪声影响范围
                
                _RampThreshold += noise;

                half3 col = half3(1,1,1);

                if(dotProduct >= _RampThreshold){
                    col = _Color1.rgb;
                }
                else{
                    col = _Color2.rgb;
                }
                
                return half4(col , 1.0);
            }
            ENDCG
        }

        		// Shadow casting support.
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/DepthNormals"

    }
}