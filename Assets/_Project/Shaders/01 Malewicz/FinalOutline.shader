Shader "Hidden/FinalOutline"
{
    Properties
    {
        _LineColor("LineColor", Color) = (1, 1, 1, 1)
        _AreaSize("AreaSize", float) = 1
        _SamplingSize("SamplingSize", int) = 1
        _LineWidth("LineWidth", float) = 1 //in world units
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
        Cull Front

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

        float _AreaSize;
        int _SamplingSize;
        float _LineWidth;
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
                real rawDepth = SampleSceneDepth(uv);
            #else
                // Adjust Z to match NDC for OpenGL ([-1, 1])
                real rawDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(uv));
            #endif

            //simplify depth
            float depth = LinearEyeDepth(rawDepth, _ZBufferParams);
            //depth debugging
            //return float4(depth, depth, depth, 1);

            //sample normal
            float3 normal = SampleSceneNormals(uv);
            //normal = (normal + 1) / 2;
            //float3 normal = LoadSceneNormals(uint2(0, 0));

            //fraction take:
            float2 rounduv = float2(round(uv.x * _ScaledScreenParams.x) / _ScaledScreenParams.x, round(uv.y * _ScaledScreenParams.y) / _ScaledScreenParams.y);
            float2 move = float2((1 / _ScaledScreenParams.x), (1 / _ScaledScreenParams.y));

            //return float4(normal.x, normal.y, normal.z, 1);

            //float3 sceneCol = SampleSceneColor(uv);
            //return float4(sceneCol, 1);

            //checking if in desired area
            float3 worldPos = ComputeWorldSpacePosition(uv, rawDepth, UNITY_MATRIX_I_VP);
            float4 objectOrigin = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
            float orgDist = length(worldPos - objectOrigin.xyz);

            if (orgDist > _AreaSize)
            {
                clip(-1);
            }
            else if (abs(orgDist - _AreaSize) < _LineWidth) //make lines at edges of area
            {
                //return _LineColor * (abs(orgDist - _AreaSize) / _LineWidth);
                col = _LineColor * (abs(orgDist - _AreaSize) / _LineWidth);
            }


            //main loop
            for (int j = 1; j < _SamplingSize; j++)
            {
                for (int i = 3; i >= 1; i--)
                {
                    float2 nghuv = SampleNeighbour(i, rounduv, move * j);

                    #if UNITY_REVERSED_Z
                        real nghRawDepth = SampleSceneDepth(nghuv);
                    #else
                        // Adjust Z to match NDC for OpenGL ([-1, 1])
                        real nghRawDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(nghuv));
                    #endif
                    //simplify depth
                    float nghDepth = LinearEyeDepth(nghRawDepth, _ZBufferParams);
                    //depth:
                    half depthDiff = abs(depth - nghDepth);
                    if (depthDiff > _DepthThreshold)
                    {
                        //make lines scale with world space not screen space
                        float3 nghWorldPos = ComputeWorldSpacePosition(nghuv, rawDepth, UNITY_MATRIX_I_VP); //yes, we use original depth to check distance as if points were perpendicular, otherwise we could not compare real distane when depth is much different
                        float worldDist = length(worldPos - nghWorldPos);
                        if (worldDist < _LineWidth)
                        {
                            return _LineColor * (worldDist / _LineWidth);
                        }

                    }
                }
            }

            return col;
        }
        ENDHLSL
    }
    }
}