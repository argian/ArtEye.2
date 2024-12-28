#ifndef SHAPES_CALCULATIONS_INCLUDED
#include "ShapesCalculations.cginc"
#endif

#ifndef SHAPE_OBJECT_INCLUDED
#define SHAPE_OBJECT_INCLUDED

//calculates shape directly in object space
float SingleShape(float3 p, int type)
{
	float result = 1;
	if (type == 0)
	{
		result = SphereShape(p);
	}
	else if (type == 1)
	{
		result = CubeShape(p);
	}
	//else
	//error shape

	return result;
}

//transforms object data and calculates shape

float SceneShapeObject(float3 position, float3 pointCheck, float4 scale, int type)
{
	//transform position to object space
	float3 p = pointCheck - position;
	//here rotation if needed to add at some point
	p /= scale.xyz;
	float result = SingleShape(p, type);
	result *= min(scale.x, min(scale.y, scale.z));
	result -= scale.w;
	return result;
}

float SceneShapeObject(float3 position, float3 scale, float extrusion, float3 pointCheck, int type)
{
	//transform position to object space
	float3 p = pointCheck - position;
	//here rotation if needed to add at some point
	p /= scale;
	float result = SingleShape(p, type);
	result *= min(scale.x, min(scale.y, scale.z));
	result -= extrusion;
	return result;
}

#endif