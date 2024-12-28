
#include "Assets/Shaders/General/UsefulCalculations.cginc"
//renders an unrotated plane on the y axis
bool SimplePlane(float3 rayDir, float3 pos, float3 scale, inout float4 Hit)
{

	//now for testing only check y downwards:
	if (rayDir.y >= 0) //needed condition
	{
		return false;
	}

	float magnitude = pos.y / rayDir.y;
	float4 hit = float4(float3(0, 0, 0) + rayDir * magnitude, 0);
	Hit = hit;
	if (abs(hit.x - pos.x) < scale.x && abs(hit.z - pos.z) < scale.z) //check if within plane bounds
	{
		return true;
	}
	else
	{
		return false;
	}
}

bool SimpleCheckboard(float3 pos, float3 spacing)
{
	//pos -= spacing / 2;
	pos = abs(pos);
	pos.xz = max(0, pos.xz + spacing.xz / 2);
	if ((pos.x / spacing.x) % 2 < 1 && (pos.z / spacing.z) % 2 < 1)
	{
		return true;
	}
	else
	{
		return false;
	}
}

bool SimpleCube(float3 pos, float4 hit, float3 dir, float3 scale, inout float3 normals)
{
	//float t1.x, t2.x, t1.y, t2.y, t1.z, t2.z, tNear, tFar;
	float3 t1;
	float3 t2;
	t1.x = (pos.x - scale.x) / dir.x;
	t2.x = (pos.x + scale.x) / dir.x;
	t1.y = (pos.y - scale.y) / dir.y;
	t2.y = (pos.y + scale.y) / dir.y;
	t1.z = (pos.z - scale.z) / dir.z;
	t2.z = (pos.z + scale.z) / dir.z;

	//float tNear = DirectedMax(float3(min(t1.x, t2.x), min(t1.y, t2.y), min(t1.z, t2.z)), normals);
	float tNear = max(min(t1.x, t2.x), max(min(t1.y, t2.y), min(t1.z, t2.z)));
	float3 suspectedNormals = float3(0, 0, 0);
	DirectedMax(float3(min(t1.x, t2.x), min(t1.y, t2.y), min(t1.z, t2.z)), suspectedNormals);
	if (t1.x < t2.x)
	{
		suspectedNormals.x = -suspectedNormals.x;
	}
	if (t1.y < t2.y)
	{
		suspectedNormals.y = -suspectedNormals.y;
	}
	if (t1.z < t2.z)
	{
		suspectedNormals.z = -suspectedNormals.z;
	}
	float tFar = min(max(t1.x, t2.x), min(max(t1.y, t2.y), max(t1.z, t2.z)));

	//far planes are closer or direction is outwards
	if (tNear > tFar || tFar < 0)
	{
		return false;
	}
	//the actual result is tNear where plane is based on max() function
	normals = suspectedNormals;
	return true;
}

float3 CubeLight(float3 hit, float3 pos, float3 scale)
{
	float3 localPos = hit - pos;
	localPos /= scale;
	float maxPos = max(localPos.x, max(localPos.y, localPos.z));
	localPos = floor(localPos / maxPos);
	return localPos;
}

float VectorAngle(float3 vect1, float3 vect2)
{
	return abs(vect1.x * vect2.x + vect1.y * vect2.y + vect1.z * vect2.z) / (length(vect1) * length(vect2));
}