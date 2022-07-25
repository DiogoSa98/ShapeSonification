Shader "Custom/SimpleSoundVisualizer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM


            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "SoundVisualizerHelpers.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            int totalSoundSamples;
            StructuredBuffer<float> soundSamplesBuffer;

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv-.5; // put origin in center
                uv *= 4.; // scale to 2 (top right corner is (2, 2)

                float3 col = float3(0.000, 0.000, 0.000);
                col = lerp(col, float3(0.031, 0.031, 0.031), float(sdBox(cube(uv), float2(.49, .49)) <= 0.));
                col = lerp(col, float3(0.404,0.984,0.396), sdSound(uv, totalSoundSamples, soundSamplesBuffer));
                
                float2 puv = i.uv;
                puv *= 1. - i.uv;
                col *= pow(puv.x * puv.y * 30.,.5);

                fixed4 fragColor = float4(col * float3(0.000,0.667,1.000),1.0);

                fragColor.rgb += float3(.1, .1, .1);

                return fragColor;
            }
            ENDCG
        }
    }
}
