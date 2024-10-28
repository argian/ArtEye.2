Shader "Unlit/DepthForce"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Depth ("Depth", float) = 100
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        ColorMask 0
        ZTest Always
        ZWrite on

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }


float _Depth;
            float frag (v2f i) : SV_Depth
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                //paste set value as depth
                return _Depth;
            }
            ENDCG
        }
    }
}
