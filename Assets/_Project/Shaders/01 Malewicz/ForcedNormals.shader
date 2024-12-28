Shader "Custom/ForcedNormals"
{

    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _NormalMap("NormalMap", 2D) = "bump" {}
    }
        SubShader{
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
          #pragma surface surf SimpleLambert noambient

        half4 LightingSimpleLambert(SurfaceOutput s, half3 lightDir, half atten)
        {
        //simply replace normals with whatever you like:
        half3 ambientLight = half3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
        //float3 aaa = UnpackNormal(half4(s.Specular, s.Gloss, s.Alpha, 0));
        float4 norm = float4(s.Gloss, s.Alpha, s.Specular, 1);
        half NdotL = max(0, dot(norm, lightDir));
        //half NdotL = max(0, dot(float3(0, 0, -1), lightDir));
        half4 c;
        c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten);
        c.a = 1;
        //c.rgb = norm;
        return c;
        }

        struct Input
        {
        float2 uv_MainTex;
        float2 uv_NormalMap;
        };

sampler2D _MainTex;
sampler2D _NormalMap;

void surf(Input IN, inout SurfaceOutput o)
{
fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
o.Albedo = c.rgb;
float3 normal = tex2D(_NormalMap, IN.uv_NormalMap);
//UnpackNormal(
//WHY WHY DO I HAVE TO DO THIS, JUST WHY
//ALL I WANTED WAS TO IGNORE WORLD NORMALS AND THIS IS WHAT I HAVE
//TO DO TO ACHIVE SUCH A SIMPLE TASK
//Damnn you all unity shader devs, your api is worth nothing
o.Gloss = normal.x;
o.Alpha = normal.y;
o.Specular = normal.z;
//o.Specular = -0.4;
}
ENDCG
    }
        Fallback "Diffuse"
}
