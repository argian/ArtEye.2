
/*
float RoundScale(float val, float scale)
{
	float frac = val % scale / scale;
	if (frac >= 0.5)
	{
		val 
	}
}
*/

//convert vector to direction
half3 ToDirection(half3 data)
{
	return data * max(abs(data.x), max(abs(data.y), abs(data.z)));
}

//additionally returns direction in form of a vector
float DirectedMax(float3 val, inout float3 dir)
{
	if (val.x >= val.y)
	{
		if (val.x >= val.z)
		{
			dir = float3(1, 0, 0);
			return val.x;
		}
		else
		{
			dir = float3(0, 0, 1);
			return val.z;
		}
	}
	else if (val.y >= val.z)
	{
		dir = float3(0, 1, 0);
		return val.y;
	}
	else
	{
		dir = float3(0, 0, 1);
		return val.z;
	}
}

float DirectedMin(float3 val, inout float3 dir)
{
	if (val.x <= val.y)
	{
		if (val.x <= val.z)
		{
			dir = float3(1, 0, 0);
			return val.x;
		}
		else
		{
			dir = float3(0, 0, 1);
			return val.z;
		}
	}
	else if (val.y <= val.z)
	{
		dir = float3(0, 1, 0);
		return val.y;
	}
	else
	{
		dir = float3(0, 0, 1);
		return val.z;
	}
}

//v is regular vector, rot is in deegres
float3 Rotate3D(float3 pos, float3 rot) //using deegres
{
	float3 rad = 0.0174532924 * rot;
	float3 Cos = cos(rad);
	float3 Sin = sin(rad);
	//this is matrix multiplication without generating matricies
	//x axis
	pos = float3(pos.x, Cos.x * pos.y - Sin.x * pos.z, Sin.x * pos.y + Cos.x * pos.z);
	//y axis
	pos = float3(Cos.y * pos.x + Sin.y * pos.z, pos.y, -Sin.y * pos.x + Cos.y * pos.z);
	//z axis
	pos = float3(Cos.z * pos.x - Sin.z * pos.y, Sin.z * pos.x + Cos.z * pos.y, pos.z);

	return pos;
}

//input direction does not need to be actual dir, it can be any vector
float3 Rotate3DMatrix(float3 pos, float3 dir)
{
	//rotation matrix creation:
	float3 forward = normalize(dir);
	float3 right = normalize(cross(forward, float3(0, 1, 0)));
	float3 up = cross(right, forward); // does not need to be normalized
	float3x3 rotationMatrix = float3x3(right, up, forward);
	//apply matrix conversion
	return mul(rotationMatrix, pos);
}

float PseudoRandom(float Seed)
{
	float result = frac(sin(dot(Seed, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
	return result;
}

float PseudoRandom(float2 Seed)
{
	float result = frac(sin(dot(Seed, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
	return result;
}

float2 PseudoRandom2D(float2 p)
{
	return frac(sin(float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)))) * 43758.5453);
}

/*
float2 SampleCrossNeighbour(int id, int size, float2 uv, float2 move)
{
	if (id == 0)
	{

	}
	else if (id == 1)
	{

	}
	else if (id == 2)
	{

	}
	else if (id == 3)
	{

	}

	return uv;
}
*/

float2 SampleNeighbour(int id, float2 uv, float2 move)
{
	if (id == 0)
	{
		return uv;
	}
	else if (id == 1)
	{
		return float2(uv.x - move.x, uv.y + move.y);
	}
	else if (id == 2)
	{
		return float2(uv.x, uv.y + move.y);
	}
	else if (id == 3)
	{
		return float2(uv.x + move.x, uv.y + move.y);
	}
	else if (id == 4)
	{
		return float2(uv.x + move.x, uv.y);
	}
	else if (id == 5)
	{
		return float2(uv.x + move.x, uv.y - move.y);
	}
	else if (id == 6)
	{
		return float2(uv.x, uv.y - move.y);
	}
	else if (id == 7)
	{
		return float2(uv.x - move.x, uv.y - move.y);
	}
	else if (id == 8)
	{
		return float2(uv.x - move.x, uv.y);
	}

	return uv;
}

half4 ColorBlend(half4 bgCol, half4 adCol)
{
	half4 result = fixed4(1, 1, 1, 1);
	result.a = 1 - (1 - adCol.a) * (1 - bgCol.a);
	result.rgb = adCol.rgb * adCol.a / result.a + bgCol.rgb * bgCol.a * (1 - adCol.a) / result.a;

	return result;
}

half4 ColorBlend(half3 bgCol, half bgA, half3 adCol, half adA)
{
	half4 result = fixed4(1, 1, 1, 1);
	result.a = 1 - (1 - adA) * (1 - bgA);
	result.rgb = adCol.rgb * adA / result.a + bgCol.rgb * bgA * (1 - adA) / result.a;

	return result;
}

//HUE

half4 ColorToHue6(fixed4 col)
{

	half4 result = half4(0, 0, 0, col.a);
	//value //brightness
	result.y = min(col.x, min(col.y, col.z));
	//saturation //intensity
	result.z = max(col.x, max(col.y, col.z)) - result.y;

	//PRIMARY COLOR
	//blue is always giving 0, so let's just skipp it
	//matrix: b = 1 => 0 g = 1 => 1 r = 1 => 2
	result.x = floor((col.y - result.y) / result.z) * 1 + floor((col.x - result.y) / result.z) * 2;
	//excludation (weird thing, gives 1 for 2, 2 for 4, and 2 for 6 input)
	result.x = min(result.x, 2);

	//SECONDARY COLOR (fixed)
	//int Translation = 
	//Mathf.Ceil(col.y - result.y)
	result.w = ((col.x - result.y) / result.z * min(2 - result.x, 1) + ((col.y - result.y) / result.z + 1 * ceil(col.y - result.y)) * abs(result.x - 1) + ((col.z - result.y) / result.z + 2 * ceil(col.z - result.y)) * min(result.x, 1) + result.x) % 3;

	//result.w += Mathf.Floor((3 - result.w) / 3);
	//1 * Mathf.Floor((3 - result.w) / 3)

	//result.w = (result.w - 1) * Mathf.Min(Mathf.Ceil(col.y - result.y) + Mathf.Ceil(col.z - result.y), 1) + 1;

	//float heck = (result.w - 1) * Mathf.Min(Mathf.Ceil(col.y - result.y) + Mathf.Ceil(col.z - result.y), 1) + 1;

	float Exceptions = ceil(col.x - result.y) + ceil(col.y - result.y) + ceil(col.z - result.y);
	if (Exceptions == 0)
	{
		result.x = 0;
	}
	else if (Exceptions == 1)
	{
		result.x = result.x * 2 + 1;
	}
	else
	{
		result.x = result.x * 2 + abs(ceil(result.x * 2 + result.w) % 2 - result.w);
	}

	result.w = col.a;

	return result;
}

half4 HueToColor6(half4 hue)
{
	hue.x %= 6;
	if (hue.x == 0)
	{
		hue.x = 6;
	}
	//START

	//PrimaryColor:
	/*
	result.x = Mathf.Min(Mathf.Floor(hue.x / 4), 1) * (hue.y + hue.z)
	result.y = Mathf.Min(hue.x / 2, 1) * (1 - Mathf.Min(hue.x / 4, 1)) * (hue.y + hue.z)
	result.z = Mathf.Min(hue.x / 4, 1) * (hue.y + hue.z)
	*/

	half4 result = half4(0, 0, 0, hue.w);

	//OTHERIDEA
	result.w = min(floor(hue.x / 2), 2);
	result.x = max(result.w - 1, 0) * hue.z;
	result.y = (1 - abs(result.w - 1)) * hue.z;
	result.z = -min(result.w - 1, 0) * hue.z;

	//SECONDARY COLOR
	//multiplication Mathf.Ceil(hue.x + 1) % 2;

	result.w = floor(hue.x) % 3 + 2 * floor(hue.x / 6);
	float LittleValue = abs(1 * ceil(hue.x) % 2 - (hue.x - floor(hue.x)));
	//LittleValue = 1 - LittleValue
	if (LittleValue == 1 || LittleValue == 0)
	{
		LittleValue = 1 - LittleValue;
	}
	result.x += -min(result.w - 1, 0) * LittleValue * hue.z;
	result.y += (1 - abs(result.w - 1)) * LittleValue * hue.z;
	result.z += max(result.w - 1, 0) * LittleValue * hue.z;

	result.x += hue.y;
	result.y += hue.y;
	result.z += hue.y;
	result.w = hue.w;

	return result;
}

//NoiseHERE
float3 hash(float3 p)
{ // replace this by something better
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