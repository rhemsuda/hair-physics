// This method gets a normal that is perpendicular to the normal we provide
float3 GetPerpNorm(float3 norm)
{
	//Get a point that is in a direction that is not colinear to the direction norm 
	//so the perpendicular vector is numerically stable regardless of the angle
	bool b = (dot(norm, float3(1, 0, 0)) > dot(norm, float3(0, 1, 0)));
	float3 temp = b ? float3(1, 0, 0) : float3(0, 1, 0);
	float3 perp = cross(norm, temp);

	//Get an arbitrary perpendicular vector to the direction norm
	return cross(norm, temp);
}

// This method implements rotation around an axis by a specific angle
float3 AngleAxisRotation(float3 norm, int angle, float3 axis)
{
	float3 p = dot(norm, axis) * axis;
	float3 f = cross(axis, norm);
	return norm * cos(radians(angle)) + p * (1 - cos(radians(angle))) + (f * sin(radians(angle)));
}