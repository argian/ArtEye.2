Shader "Unlit/UvSensitiveWireframe"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WireThickness ("WireThickness", float) = 0
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
            float _WireThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float2 scaledUv = i.uv / fwidth(i.uv);
                
                if (scaledUv.x < _WireThickness || scaledUv.x > (1 - _WireThickness))
                {
                    col = (1, 1, 1, 0);
                }
                else
                {
                    //col = (0, 0, 0, 0);
                }
                if (scaledUv.y < _WireThickness || scaledUv.y >(1 - _WireThickness))
                {
                    col = (1, 1, 1, 0);
                }
                else
                {

                }
                col = 1 - col;
                return col;
            }
            ENDCG
        }
    }
}
