Shader "ArtEye/AreaOutline"
{
    Properties
    {
        _LineColor("LineColor", Color) = (1, 1, 1, 1)
        _AreaShape("AreaShape", int) = 0
        _AreaSize("AreaSize", vector) = (1, 1, 1, 0)
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
        "Queue" = "Geometry+50"
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

            #pragma target 2.0

            //basic library
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            //depth texture
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            //normal texture
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            //color texture
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            #include "Assets/_Project/Shaders/General/UsefulCalculations.cginc"
            //#include "Assets/_Project/Shaders/General/ShapesCalculations.cginc"
            #include "Assets/_Project/Shaders/General/ShapeObject.cginc"

            struct Attributes
            {
            float4 positionOS   : POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            };

        struct Varyings
        {
            // The positions in this struct must have the SV_POSITION semantic.
            float4 clipPos  : SV_POSITION;
            float4 screenPos : TEXCOORD0;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        Varyings vert(Attributes IN)
        {
            // Declaring the output object (OUT) with the Varyings struct.
            Varyings OUT;
            //set output as stereo
            UNITY_SETUP_INSTANCE_ID(IN);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
            // The TransformObjectToHClip function transforms vertex positions
            // from object space to homogenous clip space.
            OUT.clipPos = TransformObjectToHClip(IN.positionOS.xyz);
            //yeah vr specialties stuff again
            OUT.screenPos = ComputeScreenPos(OUT.clipPos);
            // Returning the output.
            return OUT;
        }

        vector _LineColor;

        int _AreaShape;
        vector _AreaSize;
        int _SamplingSize;
        float _LineWidth;
        float _NormalsThreshold;
        float _DepthThreshold;


        half4 frag(Varyings IN) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
            // To calculate the UV coordinates for sampling the depth buffer,
            // divide the pixel location by the render target resolution
            // _ScaledScreenParams.
            float4 col = float4(0, 0, 0, 1);
            float2 uv = IN.screenPos.xy / IN.screenPos.w;

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


            //alternative to clip, display color buffer
            //float3 sceneCol = SampleSceneColor(uv);
            //return float4(sceneCol * depth, 1);

            //checking if in desired area
            float3 worldPos = ComputeWorldSpacePosition(uv, rawDepth, UNITY_MATRIX_I_VP);
            float4 objectOrigin = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
            float sdfDist = SceneShapeObject(objectOrigin.xyz, worldPos, _AreaSize, _AreaShape);
            if (sdfDist > 0)
            {
                clip(-1);
            }
            else if (-sdfDist < _LineWidth)//make lines at edges of area
            {
                //col = _LineColor * (1 - (abs(orgDist - _AreaSize) / _LineWidth));
                col = _LineColor * -sdfDist / _LineWidth;
            }

            //main loop
            for (int j = 1; j < _SamplingSize; j++)
            {
                for (int i = 8; i >= 1; i--)
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
                    half depthDiff = abs(depth - nghDepth);

                    float3 nghNormal = SampleSceneNormals(nghuv);
                    float normalDiff = acos(dot(normal, nghNormal) / (length(normal) * length(nghNormal)));

                    if (depthDiff > _DepthThreshold || normalDiff > _NormalsThreshold)
                    {
                        //make lines scale with world space not screen space
                        float3 nghWorldPos = ComputeWorldSpacePosition(nghuv, rawDepth, UNITY_MATRIX_I_VP); //yes, we use original depth to check distance as if points were perpendicular, otherwise we could not compare real distane when depth is much different
                        float worldDist = length(worldPos - nghWorldPos);
                        if (worldDist < _LineWidth)
                        {
                            return _LineColor * (1 - (worldDist / _LineWidth));
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