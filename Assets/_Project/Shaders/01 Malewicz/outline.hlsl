//UNITY_SHADER_NO_UPGRADE
#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

#include "Assets/_Project/Shaders/General/UsefulCalculations.cginc"

/*
void localMelt_float(float4 WorldPos, float4 CenterPosition, float4 NoiseScale, float Time, out float4 Result)
{
    Result = WorldPos;
    float3 centeredPos = WorldPos - CenterPosition;
    centeredPos.z += Time * NoiseScale.w;
    Result.y -= (GradientNoise(centeredPos / NoiseScale.xyz) + 1) * -min(0, centeredPos.y);
    
    //return Result
}
*/

//#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hls"

//frag
void outline_float(float2 uv, UnitySamplerState sampl, float _LineWidth, float _NormalsThreshold, float _DepthThreshold, out float4 Result)
//void outline_float(out float4 Result)
{
    //Result = float4(0, 0, 0, 0);

    //not inside effect:


    //inside effect:
    //*
    //fraction take:
    //float4 col = tex2D(MainTex, uv);
    //float4 col = SAMPLE_TEXTURE2D(_CustomPostProcessInput, sampl, uv);
    //float4 col = Unity_Universal_SampleBuffer_BlitSource_float(float2(0, 0));
    //float4 col = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, uv);
    float4 col = float4(0, 0, 0, 1);
    float depth2 = SHADERGRAPH_SAMPLE_SCENE_DEPTH(uv);
    Result = float4(depth2, depth2, depth2, 1);

    float2 rounduv = float2(round(uv.x * _ScreenParams.x) / _ScreenParams.x, round(uv.y * _ScreenParams.y) / _ScreenParams.y);
    float2 move = float2((1 / _ScreenParams.x), (1 / _ScreenParams.y));

    float depth = 1 - max(abs(col.x), max(abs(col.y), abs(col.z)));
    //main loop
    for (int j = 1; j < _LineWidth; j++)
    {
        for (int i = 3; i >= 1; i--)
        {
            float2 nghuv = SampleNeighbour(i, rounduv, move * j);
            //float4 nghCol = tex2D(MainTex, nghuv);
            //float4 nghCol = SAMPLE_TEXTURE2D(_CustomPostProcessInput, sampl, nghuv);
            float4 nghCol = float4(0, 0, 0, 1);

            //reformat
            //nghCol = nghCol * 2 - 1;

            float nghDepth = 1 - max(abs(nghCol.x), max(abs(nghCol.y), abs(nghCol.z)));

            //check
            //return dot(col, nghCol) / length(col) / length(nghCol);
            //float normalDiff = abs(dot(col, nghCol)) / (length(col) * length(nghCol));
            float normalDiff = abs(col.x * nghCol.x + col.y * nghCol.y + col.z * nghCol.z) / (length(col) * length(nghCol));
            if (normalDiff < _NormalsThreshold)
                //if (normalDiff < 50)
            {
                //Result = float4(1, 1, 1, 1);
            }
            //depth:
            float depthDiff = abs(depth - nghDepth);
            if (depthDiff > _DepthThreshold)
            {
                //Result = float4(1, 1, 1, 1);
            }
        }
    }
    //*/
}

#endif //MYHLSLINCLUDE_INCLUDED