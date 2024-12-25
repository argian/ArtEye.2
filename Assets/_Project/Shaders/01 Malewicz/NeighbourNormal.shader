// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/NeighbourNormal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {

        CGPROGRAM
        #pragma target 3.0
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        struct v2f
 {
float2 uv : TEXCOORD0;
float4 vert : VS_POSITION;
float4 pos : SV_POSITION;
float3 color : COLOR0;
float3 normal : TEXCOORD1;
float2 normAngle : TEXCOORD2;
};

struct V_IN
{
    float4 uv : TEXCOORD0;
    float4 vertex : POSITION;
    float3 normal : NORMAL;
};

sampler2D _MainTex;
float4 _MainTex_ST;

v2f vert(V_IN v)
{
    v2f o;
    float4 nrml = float4(v.normal, 1);
    o.vert = v.vertex;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.color = (nrml + 1) / 2;
    //o.normal = UnityObjectToWorldNormal(v.normal);
    o.normal = v.normal;

    float4 objectOrigin = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
    float3 camPosDir = _WorldSpaceCameraPos.xyz - objectOrigin.xyz;
    float3 camForward = normalize(camPosDir.xyz);
    o.normAngle.x = (dot(o.normal, camForward) + 1) / 2;
    o.normAngle.y = o.normAngle.x;
    return o;
}

half4 frag(v2f i) : COLOR
{
    float3 res;
    float3 norm = (i.normal + 1) / 2;
    float depth = 1 - Linear01Depth(i.pos.z / i.pos.w) * 100;
    //res = ddx(i.normal) + ddy(i.normal); // - ddy(i.normal)
    half4 col = tex2D(_MainTex, i.uv);
    res = float3(i.normAngle.x * depth, i.normAngle.x * depth, i.normAngle.x * depth);
    //res = normalize(cross(ddx(i.vert), ddy(i.vert)));
    //res = dot(i.color, float3(0, 0, 1)) / length(i.color);
    //res = (ddx(norm) - ddy(norm));
    //res = fwidth(res) * 10;
    res %= 0.1;
    //res = abs(res - 0.05) * 2;
    res *= res * res;
    res *= 100;
    //float normalDiff = abs(res.x + res.y + res.z) / length(res.xyz);
    //normalDiff = 1 - normalDiff;

    float normalDiff = length(res);
    //return half4 (res, 1);
    /*
    if (res.x < 0.85 && res.x > 0.75)
    {
        return half4(1, 1, 1, 0);
    }
    else
    {
        return half4(0, 0, 0, 0);
    }
    */
    //return half4 (res, 1);
    //return half4 (half3(normalDiff, normalDiff, normalDiff), 1);
    return half4 (half3(res.x, res.y, res.z), 1);
}
ENDCG

}
    }
}
