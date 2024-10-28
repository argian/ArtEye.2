// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/FlattenObject"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        //verts are object space oriented
        void vert(inout appdata_full v)
        {
            float4 objectOrigin = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
            float4 worldVertex = mul(unity_ObjectToWorld, v.vertex);

            /*
            float originCameraDist = distance(_WorldSpaceCameraPos, objectOrigin);
            float vertexCameraDist = distance(_WorldSpaceCameraPos, worldVertex);
            float3 CameraDir = mul((float3x3)unity_CameraToWorld, float3(0, 0, 1));
            v.vertex += CameraDir * (vertexCameraDist - originCameraDist);
        `   */
            //version relative to camera rot
            /*
            float4 cameraVertex = mul(unity_WorldToCamera, worldVertex);
            float4 cameraOrigin = mul(unity_WorldToCamera, objectOrigin);
            cameraVertex.z = cameraOrigin.z + (cameraVertex.z - cameraOrigin.z) * 0.001;
            //cameraVertex.z = cameraOrigin.z;
            v.vertex = mul(unity_WorldToObject, mul(unity_CameraToWorld, cameraVertex));
            */

            //version relative to camera pos
            //get direction relative to pos of camera and object
            float3 camPosDir = _WorldSpaceCameraPos.xyz - objectOrigin.xyz;
            //rotation matrix creation:
            float3 forward = normalize(camPosDir.xyz);
            float3 right = normalize(cross(forward, float3(0, 1, 0)));
            float3 up = cross(right, forward); // does not need to be normalized
            float3x3 rotationMatrix = float3x3(right, up, forward);            
            //applying the matrix
            float3 cameraVertex = mul(rotationMatrix, worldVertex.xyz);
            float3 cameraOrigin = mul(rotationMatrix, objectOrigin.xyz);
            //changing vertex
            v.vertex.xyz += forward * ((cameraOrigin.z - cameraVertex.z) * 1.00390625);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
