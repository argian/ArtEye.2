Shader "Hidden/FinalOutline"
{
    Properties
    {
        _LineColor("LineColor", Color) = (1, 1, 1, 1)
        _LineWidth("LineWidth", int) = 1
        _NormalsThreshold("NormalsTreshold", Range(0, 1)) = 1
        _DepthThreshold("DepthTreshold", Range(0, 1)) = 1
    }

        // The SubShader block containing the Shader code.
        SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags { "RenderType" = "Opaque"
        "Queue" = "Overlay-1"
        "RenderPipeline" = "UniversalPipeline" }

        Ztest Always
        Zwrite Off

        Pass
        {
            HLSLPROGRAM
            // This line defines the name of the vertex shader.
            #pragma vertex vert
            // This line defines the name of the fragment shader.
            #pragma fragment frag

            //basic library
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            //depth texture
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            //normal texture
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            //color texture
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            #include "Assets/_Project/Shaders/General/UsefulCalculations.cginc"

            struct Attributes
            {
            float4 positionOS   : POSITION;
            };

        struct Varyings
        {
            // The positions in this struct must have the SV_POSITION semantic.
            float4 positionHCS  : SV_POSITION;
        };


        Varyings vert(Attributes IN)
        {
            // Declaring the output object (OUT) with the Varyings struct.
            Varyings OUT;
            // The TransformObjectToHClip function transforms vertex positions
            // from object space to homogenous clip space.
            OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
            // Returning the output.
            return OUT;
        }

        vector _LineColor;

        int _LineWidth;
        float _NormalsThreshold;
        float _DepthThreshold;


        half4 frag(Varyings IN) : SV_Target
        {
            // To calculate the UV coordinates for sampling the depth buffer,
            // divide the pixel location by the render target resolution
            // _ScaledScreenParams.
            float4 col = float4(0, 0, 0, 1);
            float2 uv = IN.positionHCS.xy / _ScaledScreenParams.xy;

            // Sample the depth from the Camera depth texture.
            #if UNITY_REVERSED_Z
                real depth = SampleSceneDepth(uv);
            #else
                // Adjust Z to match NDC for OpenGL ([-1, 1])
                real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(uv));
            #endif

            //simplify depth
            depth = LinearEyeDepth(depth, _ZBufferParams);
            //depth debugging
            //return float4(depth, depth, depth, 1);

            //sample normal
            float3 normal = SampleSceneNormals(uv);
            //normal = (normal + 1) / 2;
            //float3 normal = LoadSceneNormals(uint2(0, 0));

            //fraction take:
            float2 rounduv = float2(round(uv.x * _ScaledScreenParams.x) / _ScaledScreenParams.x, round(uv.y * _ScaledScreenParams.y) / _ScaledScreenParams.y);
            float2 move = float2((1 / _ScaledScreenParams.x), (1 / _ScaledScreenParams.y));

            //return float4(normal.x, normal.x, normal.x, 1);

            //float3 sceneCol = SampleSceneColor(uv);
            //return float4(sceneCol, 1);

            //main loop
            for (int j = 1; j < _LineWidth; j++)
            {
                for (int i = 3; i >= 1; i--)
                {
                    float2 nghuv = SampleNeighbour(i, rounduv, move * j);

                    #if UNITY_REVERSED_Z
                        real nghDepth = SampleSceneDepth(nghuv);
                    #else
                        // Adjust Z to match NDC for OpenGL ([-1, 1])
                        real nghDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(nghuv));
                    #endif
                    //simplify depth
                    nghDepth = LinearEyeDepth(nghDepth, _ZBufferParams);
                    //depth:
                    half depthDiff = abs(depth - nghDepth);
                    if (depthDiff > _DepthThreshold)
                    {
                        return _LineColor;
                    }
                }
            }

            return col;
        }
        ENDHLSL
    }
    }
}