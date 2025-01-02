//UNITY_SHADER_NO_UPGRADE
#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

float3 hash(float3 p)
{
    p = float3(dot(p, float3(127.1, 311.7, 74.7)),
        dot(p, float3(269.5, 183.3, 246.1)),
        dot(p, float3(113.5, 271.9, 124.6)));
    return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
}

float GradientNoise(in float3 p)
{
    float3 i = floor(p);
    float3 f = frac(p);
    float3 u = f * f * (3.0 - 2.0 * f);
    return lerp(lerp(lerp(dot(hash(i + float3(0.0, 0.0, 0.0)), f - float3(0.0, 0.0, 0.0)),
        dot(hash(i + float3(1.0, 0.0, 0.0)), f - float3(1.0, 0.0, 0.0)), u.x),
        lerp(dot(hash(i + float3(0.0, 1.0, 0.0)), f - float3(0.0, 1.0, 0.0)),
            dot(hash(i + float3(1.0, 1.0, 0.0)), f - float3(1.0, 1.0, 0.0)), u.x), u.y),
        lerp(lerp(dot(hash(i + float3(0.0, 0.0, 1.0)), f - float3(0.0, 0.0, 1.0)),
            dot(hash(i + float3(1.0, 0.0, 1.0)), f - float3(1.0, 0.0, 1.0)), u.x),
            lerp(dot(hash(i + float3(0.0, 1.0, 1.0)), f - float3(0.0, 1.0, 1.0)),
                dot(hash(i + float3(1.0, 1.0, 1.0)), f - float3(1.0, 1.0, 1.0)), u.x), u.y), u.z);
}

void GradientNoise_float(float3 position, out float result)
{
    result = GradientNoise(position);
}


//returns 
/*
void frag_float(float4 CenterPosition, float4 WorldPos, float4 ObjectCenter, float4 NoiseScale, out float4 Result)
{
    Result = WorldPos;
    float3 centeredPos = WorldPos - ObjectCenter;
	centeredPos.z += _Time * NoiseScale.w;
    Result.y -= (GradientNoise(centeredPos / NoiseScale.xyz) + 1) * -min(0, centeredPos.y);
    
    //return Result;
}
*/

void localMelt_float(float4 WorldPos, float4 CenterPosition, float4 NoiseScale, float Time, out float4 Result)
{
    Result = WorldPos;
    float3 centeredPos = WorldPos - CenterPosition;
    centeredPos.z += Time * NoiseScale.w;
    Result.y -= (GradientNoise(centeredPos / NoiseScale.xyz) + 1) / 2 * -min(0, centeredPos.y);
    
    //return Result
}

#endif //MYHLSLINCLUDE_INCLUDED