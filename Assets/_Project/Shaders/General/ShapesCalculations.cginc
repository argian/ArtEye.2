#ifndef USEFUL_CALCULATIONS_INCLUDED
#include "UsefulCalculations.cginc"
#endif

#ifndef SHAPES_CALCULATIONS_INCLUDED
#define SHAPES_CALCULATIONS_INCLUDED

float defaultBase = 0.5f;

//-----------------------------------------------------------------------------
//SHAPES TRANSFORMATIONS
//based on the default 0.5 base
//also make it work with - scales

//-------------------------------------------------------------------------------------
//SimplePrimitives (only range)

float SphereShape(float3 p)
{
    float result = 0;
    result = length(p) - 0.5f;

    return result;
}

float CubeShape(float3 p)
{
    p = abs(p);

    float result = length(float3(max(0, p.x - 0.5f), max(0, p.y - 0.5f), max(0, p.z - 0.5f)));
    if (p.x < 0.5f && p.y < 0.5f && p.z < 0.5f)
    {
    result = max(p.x - 0.5f, max(p.y - 0.5f, p.z - 0.5f));
    }

    return result;
}

float OctahedronShape(float3 p)
{
    p = abs(p);

    float sum = p.x + p.y + p.z;

    float result = length(p - float3(p / sum));
    return result * sign(sum - 0.5f);
}

float CylinderShape(float3 p)
{
    p = abs(p);

    float result = length(float2(max(0, length(p.xz) - 0.5f), max(0, p.y - 0.5f)));
    if (p.y < 0.5f && length(p.xz) < 0.5f)
    {
        result = max(p.y - 0.5f, length(p.xz) - 0.5f);
    }

    return result;
}

float ConeShape(float3 p, float4 s)
{
    p = abs(p);

    float2 rootP = float2(length(p.xz), p.y);
    float sum = length(p.xz) + p.y;

    float result = length(rootP - float2(rootP / sum));

    return result * sign(sum - 0.5f);
}

//this gives you mathematically correct star
float StarShape(float3 p)
{
    p = abs(p);

    float result = 0;
    //outer part
    if (max(p.x, max(p.y, p.z)) > 0.5f)
    {
        if (p.x > p.y)
        {
            if (p.x > p.z)
            {
                p.x -= 0.5f;
            }
            else
            {
                p.z -= 0.5f;
            }
        }
        else if (p.y > p.z)
        {
            p.y -= 0.5f;
        }
        else
        {
            p.z -= 0.5f;
        }
        
        result = length(p);
    }
    else
    {
        //inner part
        //result = -(length(p - 0.5f) - 0.5f);
        result = -(length(p - float3(0.5f, 0.5f, 0.5f)) - sqrt(0.5f));
        //result = -(length(p - float3(0.5f, 0.5f, 0.0f)) - 0.5f);
        //result *= 1.41;
    }

    return result;
}

//-----------------------------------------------------------------------------------------
//Advanced shapes: (aka fractals)

float Mandelbulb(float3 p, float4 s)
{
    p /= s.xyz;
    float result = 0;
    float thres = length(p) - 1.2;
    if (thres > 0.2)
    {
        return thres;
    }

    // Zn <- Zn^8 + c
    // Zn' <- 8*Zn^7 + 1    
    const float power = 8.0;
    float3 z = p;
    float3 c = p;

    float dr = 1.0;
    float r = 0.0;
    for (int i = 0; i < 100; i++)
    {
        // to polar
        r = length(z);
        if (r > 2.0)
        {
            break;
        }
        float theta = acos(z.z / r);
        float phi = atan2(z.y, z.x);

        // derivate
        dr = pow(r, power - 1.0) * power * dr + 1.0;

        // scale and rotate
        float zr = pow(r, power);
        theta *= power;
        phi *= power;

        // to cartesian
        z = zr * float3(sin(theta) * cos(phi), sin(phi) * sin(theta), cos(theta));
        z += c;
    }

    result = 0.5 * log(r) * r / dr;

    return result;
}

#endif