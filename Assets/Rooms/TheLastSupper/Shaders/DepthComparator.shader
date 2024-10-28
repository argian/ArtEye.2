Shader "Unlit/DepthComparator"
{
    Properties
    {
        _Comparison("Comparison", float) = 98
        _Depth("Depth", float) = 100
        _Depth2("Depth2", float) = -0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        ColorMask 0
        ZTest Always
        ZWrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D_float _CameraDepthTexture;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 screenPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }
float _Comparison;
float _Depth;
float _Depth2;
            float frag(v2f i) : SV_Depth
            {
                float2 screenUv = i.screenPos.xy / i.screenPos.w;
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUv);
                depth = Linear01Depth(depth) * _ProjectionParams.z;
                // sample the texture
                if (depth < _Comparison)
                {
                    return _Depth;
                }
                return _Depth2;
            }
            ENDCG
        }
    }
}
