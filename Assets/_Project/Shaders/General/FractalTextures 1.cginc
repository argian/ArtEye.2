
//width (of shape)
//height
//float rStart = -2;
//float iStart = -1.25;
//increment
//zoom
uint MandelbrotNormal(uint maxIter, float2 size, float rStart, float iStart)
{
	float2 val1 = float2(0, 0);
	float2 val2 = float2(0, 0);


	uint i = 0;
	for (; i < maxIter; i++)
	{
		val2.x = val1.x * val1.x;
		val2.y = val1.y * val1.y;

		if (val2.x + val2.y <= 4.0)
		{
			val1.y = (2.0 * val1.x * val2.y + iStart + _ScreenParams.y);
			val1.x = (val2.x - val2.y + rStart + _ScreenParams.x);
		}
		else
		{
			return i;
		}
	}
	return i;//(?)
}

vector SierpinskiNormal(half2 uv, float seed)
{
	half2 fc = uv * (pow(2.0, frac(seed)));
	uint2 p = half2(fc.x - fc.y * 2 / 3, fc.y * 4 / 3);
	half res = (p.x & p.y) == 0u && p.x > 0u;
	return vector(res, res, res, 0);
	//return vector(p.x & p.y, p.x & p.y, p.x & p.y, 0);
}

float MandelbulbDist(half3 p, half4 s)
{
    float result = 0;
    float thres = length(p) - 1.2;
    if (thres > 0.2)
    {
        return thres;
    }

    // Zn <- Zn^8 + c
    // Zn' <- 8*Zn^7 + 1    
    const float power = 8.0;
    half3 z = p;
    half3 c = p;

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
        z = zr * half3(sin(theta) * cos(phi), sin(phi) * sin(theta), cos(theta));
        z += c;
    }

    result = 0.5 * log(r) * r / dr;

    return result + s.w;
}