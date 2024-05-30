#version 430

#define EPSILON  0.001
#define BIG  1000000.0

uniform int BigSphere;				
uniform vec3 ColBigSphere;			
uniform int SmallSphere;			
uniform vec3 ColSmallSphere;
uniform vec3 ColCube;
uniform vec3 ColTetr;
uniform int Cube;
uniform int Tetr;

uniform int utetr;
uniform int ucube;
uniform int ubigs;
uniform int usmalls;

uniform vec3 ColBack;
uniform int Back;
uniform vec3 ColFront;
uniform int Front;
uniform vec3 ColLeft;
uniform int Left;
uniform vec3 ColRight;
uniform int Right;
uniform vec3 ColTop;
uniform int Top;
uniform vec3 ColBot;
uniform int Bot;

uniform int RTDepth;
uniform int bigCf;
uniform int bigRCf;

uniform int wallCf;
uniform int wallRCf;

const int REFRACTION = 3;
const int DIFFUSE_REFLECTION = 1;
const int MIRROR_REFLECTION = 2;
out vec4 FragColor; 
in vec3 glPosition;

/*** DATA STRUCTURES ***/
		
struct SCamera	
{
	vec3 Position;
	vec3 View;
	vec3 Up;
	vec3 Side;
	
	vec2 Scale;
};

struct SRay	
{
	vec3 Origin;
	vec3 Direction;
};

struct SLight	
{
	vec3 Position;
};


struct SSphere	
{
	vec3 Center;
	float Radius;
	int MaterialIdx;
};


struct STriangle	
{
	vec3 v1;
	vec3 v2;
	vec3 v3;
	int MaterialIdx;
};

struct SIntersection	
{
	float Time;
	vec3 Point;
	vec3 Normal;
	vec3 Color;
	vec4 LightCoeffs;
	float ReflectionCoef;
	float RefractionCoef;
	int MaterialType;
};

struct SMaterial	
{
	vec3 Color;
	vec4 LightCoeffs;
	float ReflectionCoef;
	float RefractionCoef;
	int MaterialType;
};

struct STracingRay	
{
	SRay ray;			
	float contribution;	
	int depth;			
};

SRay GenerateRay ( SCamera uCamera )
{
	vec2 coords = glPosition.xy * uCamera.Scale;
	vec3 direction = uCamera.View + uCamera.Side * coords.x + uCamera.Up * coords.y;
	return SRay ( uCamera.Position, normalize(direction) );
}
SCamera initializeDefaultCamera()
{
	SCamera camera;
	//** CAMERA **//
	camera.Position = vec3(0.0, 0.0, -9.9);
	camera.View = vec3(0.0, 0.0, 1.0);
	camera.Up = vec3(0.0, 1.0, 0.0);
	camera.Side = vec3(1.0, 0.0, 0.0);
	camera.Scale = vec2(1.0);
	return camera;
}

SSphere spheres[2];
STriangle triangles[12];
STriangle qu[24];
STriangle tetr[4];	
SMaterial materials[10];

void initializeDefaultLightMaterials(out SLight light)
{
	//** LIGHT **//
	light.Position = vec3( -4.0, 0.0, -5.0);
	/** MATERIALS **/
	vec4 lightCoefs = vec4(0.4,0.9,0.0,512.0);

	materials[0].Color = ColBigSphere;
	materials[0].LightCoeffs = vec4(lightCoefs);
	materials[0].ReflectionCoef = 0.01 * bigCf;
	materials[0].RefractionCoef = 0.01 * bigRCf;
	if (BigSphere == 1)
		materials[0].MaterialType = REFRACTION;
	else if (BigSphere == 2)
		materials[0].MaterialType = DIFFUSE_REFLECTION;
	else 
		materials[0].MaterialType = MIRROR_REFLECTION;

	materials[1].Color = ColSmallSphere;				
	materials[1].LightCoeffs = vec4(lightCoefs);
	materials[1].ReflectionCoef = 0.5;
	materials[1].RefractionCoef = 1.5;
	if (SmallSphere == 1)
		materials[1].MaterialType = REFRACTION;
	else if (SmallSphere == 2)
		materials[1].MaterialType = DIFFUSE_REFLECTION;
	else 
		materials[1].MaterialType = MIRROR_REFLECTION;
	
	materials[8].Color = ColCube;
	materials[8].LightCoeffs = vec4(lightCoefs);
	materials[8].ReflectionCoef = 0.5;
	materials[8].RefractionCoef = 1.5;
	if (Cube == 1)
		materials[8].MaterialType = REFRACTION;
	else if (Cube == 2)
		materials[8].MaterialType = DIFFUSE_REFLECTION;
	else 
		materials[8].MaterialType = MIRROR_REFLECTION;

	materials[9].Color = ColTetr;
	materials[9].LightCoeffs = vec4(lightCoefs);
	materials[9].ReflectionCoef = 0.5;
	materials[9].RefractionCoef = 1.5;
	if (Tetr == 1)
		materials[9].MaterialType = REFRACTION;
	else if (Tetr == 2)
		materials[9].MaterialType = DIFFUSE_REFLECTION;
	else 
		materials[9].MaterialType = MIRROR_REFLECTION;

	materials[2].Color = ColLeft;	
	materials[2].LightCoeffs = vec4(lightCoefs);
	materials[2].ReflectionCoef = 0.01 * wallCf;
	materials[2].RefractionCoef = 0.01 * wallRCf;
	if (Left == 1)
		materials[2].MaterialType = REFRACTION;
	else if (Left == 2)
		materials[2].MaterialType = DIFFUSE_REFLECTION;
	else 
		materials[2].MaterialType = MIRROR_REFLECTION;
	
	materials[3].Color = ColBack;	
	materials[3].LightCoeffs = vec4(lightCoefs);
	materials[3].ReflectionCoef = 0.01 * wallCf;
	materials[3].RefractionCoef = 0.01 * wallRCf;
	if (Back == 1)
		materials[3].MaterialType = REFRACTION;
	else if (Back == 2)
		materials[3].MaterialType = DIFFUSE_REFLECTION;
	else 
		materials[3].MaterialType = MIRROR_REFLECTION;
	
	materials[4].Color = ColRight;	
	materials[4].LightCoeffs = vec4(lightCoefs);
	materials[4].ReflectionCoef = 0.01 * wallCf;
	materials[4].RefractionCoef = 0.01 * wallRCf;
	if (Right == 1)
		materials[4].MaterialType = REFRACTION;
	else if (Right == 2)
		materials[4].MaterialType = DIFFUSE_REFLECTION;
	else 
		materials[4].MaterialType = MIRROR_REFLECTION;
	
	materials[5].Color = ColBot;	
	materials[5].LightCoeffs = vec4(lightCoefs);
	materials[5].ReflectionCoef = 0.01 * wallCf;
	materials[5].RefractionCoef = 0.01 * wallRCf;
	if (Bot == 1)
		materials[5].MaterialType = REFRACTION;
	else if (Bot == 2)
		materials[5].MaterialType = DIFFUSE_REFLECTION;
	else 
		materials[5].MaterialType = MIRROR_REFLECTION;
	
	materials[6].Color = ColTop;	
	materials[6].LightCoeffs = vec4(lightCoefs);
	materials[6].ReflectionCoef = 0.01 * wallCf;
	materials[6].RefractionCoef = 0.01 * wallRCf;
	if (Top == 1)
		materials[6].MaterialType = REFRACTION;
	else if (Top == 2)
		materials[6].MaterialType = DIFFUSE_REFLECTION;
	else 
		materials[6].MaterialType = MIRROR_REFLECTION;	
	
	materials[7].Color = ColFront;	
	materials[7].LightCoeffs = vec4(lightCoefs);
	materials[7].ReflectionCoef = 0.01 * wallCf;
	materials[7].RefractionCoef = 0.01 * wallRCf;
	if (Front == 1)
		materials[7].MaterialType = REFRACTION;
	else if (Front == 2)
		materials[7].MaterialType = DIFFUSE_REFLECTION;
	else 
		materials[7].MaterialType = MIRROR_REFLECTION;	
}

void initializeDefaultScene()
{
	/** TRIANGLES **/
	/* left wall */
	float size = 10.0;
	triangles[0].v1 = vec3(-size,-size,-size);
	triangles[0].v2 = vec3(-size, size, size);		
	triangles[0].v3 = vec3(-size, size,-size);
	triangles[0].MaterialIdx = 0;
	triangles[1].v1 = vec3(-size,-size,-size);
	triangles[1].v2 = vec3(-size,-size, size);		
	triangles[1].v3 = vec3(-size, size, size);		
	triangles[1].MaterialIdx = 0;
	/* back wall */
	triangles[2].v1 = vec3(-size,-size, size);		
	triangles[2].v2 = vec3( size,-size, size);
	triangles[2].v3 = vec3(-size, size, size);		
	triangles[2].MaterialIdx = 0;
	triangles[3].v1 = vec3( size, size, size);		
	triangles[3].v2 = vec3(-size, size, size);
	triangles[3].v3 = vec3( size,-size, size);
	triangles[3].MaterialIdx = 0;
	/*right wall */ 
	triangles[4].v1 = vec3( size, size, size);
	triangles[4].v2 = vec3( size,-size, size); 
	triangles[4].v3 = vec3( size, size,-size); 
	triangles[4].MaterialIdx = 0; 
	triangles[5].v1 = vec3( size, size,-size); 
	triangles[5].v2 = vec3( size,-size, size); 
	triangles[5].v3 = vec3( size,-size,-size); 
	triangles[5].MaterialIdx = 0; 
	/*down wall */ 
	triangles[6].v1 = vec3(-size,-size, size);		
	triangles[6].v2 = vec3(-size,-size,-size); 
	triangles[6].v3 = vec3( size,-size, size); 
	triangles[6].MaterialIdx = 0; 
	triangles[7].v1 = vec3( size,-size,-size); 
	triangles[7].v2 = vec3( size,-size, size); 
	triangles[7].v3 = vec3(-size,-size,-size); 
	triangles[7].MaterialIdx = 0; 
	/*up wall */ 
	triangles[8].v1 = vec3(-size, size,-size); 
	triangles[8].v2 = vec3(-size, size, size);		
	triangles[8].v3 = vec3( size, size, size); 
	triangles[8].MaterialIdx = 0; 
	triangles[9].v1 = vec3(-size, size,-size); 
	triangles[9].v2 = vec3( size, size, size); 
	triangles[9].v3 = vec3( size, size,-size); 
	triangles[9].MaterialIdx = 0; 
	/*front wall */
	triangles[10].v1 = vec3(-size,-size,-size);		
	triangles[10].v2 = vec3( size,-size,-size);
	triangles[10].v3 = vec3(-size, size,-size);		
	triangles[10].MaterialIdx = 0;
	triangles[11].v1 = vec3( size, size,-size);		
	triangles[11].v2 = vec3(-size, size,-size);
	triangles[11].v3 = vec3( size,-size,-size);
	triangles[11].MaterialIdx = 0;
	
		spheres[0].Center = vec3(-3.0,-2.0, 0.0);
		spheres[0].Radius = 3.0;
		spheres[0].MaterialIdx = 0;
	
	
		spheres[1].Center = vec3(2.0,1.0,2.0);
		spheres[1].Radius = 1.0;
		spheres[1].MaterialIdx = 0;
	
	
	vec3 center = vec3(4.0, -4.0, 2.0);
	float hg = 2;
	/** TRIANGLES **/
	/* left wall */
	qu[0].v1 = vec3( center.x - hg, center.y + hg, center.z - hg);
	qu[0].v2 = vec3( center.x - hg, center.y + hg, center.z + hg);		
	qu[0].v3 = vec3( center.x - hg, center.y - hg, center.z - hg);
	qu[0].MaterialIdx = 0;
	qu[1].v1 = vec3( center.x - hg, center.y + hg, center.z + hg);
	qu[1].v2 = vec3( center.x - hg, center.y - hg, center.z + hg);		
	qu[1].v3 = vec3( center.x - hg, center.y - hg, center.z - hg);		
	qu[1].MaterialIdx = 0;
	/* left wall */
	/*qu[12].v1 = vec3( center.x - hg, center.y - hg, center.z - hg);
	qu[12].v2 = vec3( center.x - hg, center.y + hg, center.z + hg);		
	qu[12].v3 = vec3( center.x - hg, center.y + hg, center.z - hg);
	qu[12].MaterialIdx = 0;
	qu[13].v1 = vec3( center.x - hg, center.y - hg, center.z - hg);
	qu[13].v2 = vec3( center.x - hg, center.y - hg, center.z + hg);		
	qu[13].v3 = vec3( center.x - hg, center.y + hg, center.z + hg);		
	qu[13].MaterialIdx = 0;*/

	/* back wall */
	qu[2].v1 = vec3( center.x - hg, center.y - hg, center.z + hg);
	qu[2].v2 = vec3( center.x + hg, center.y - hg, center.z + hg);
	qu[2].v3 = vec3( center.x - hg, center.y + hg, center.z + hg);		
	qu[2].MaterialIdx = 0;
	qu[3].v1 = vec3( center.x + hg, center.y - hg, center.z + hg);
	qu[3].v2 = vec3( center.x - hg, center.y + hg, center.z + hg);
	qu[3].v3 = vec3( center.x + hg, center.y + hg, center.z + hg);
	qu[3].MaterialIdx = 0;
	/* back wall */
	qu[2].v1 = vec3( center.x - hg, center.y + hg, center.z + hg);
	qu[2].v2 = vec3( center.x + hg, center.y - hg, center.z + hg);
	qu[2].v3 = vec3( center.x - hg, center.y - hg, center.z + hg);		
	qu[2].MaterialIdx = 0;
	qu[3].v1 = vec3( center.x + hg, center.y + hg, center.z + hg);
	qu[3].v2 = vec3( center.x - hg, center.y + hg, center.z + hg);
	qu[3].v3 = vec3( center.x + hg, center.y - hg, center.z + hg);
	qu[3].MaterialIdx = 0;

	/*right wall */ 
	qu[4].v1 = vec3( center.x + hg, center.y + hg, center.z - hg);
	qu[4].v2 = vec3( center.x + hg, center.y - hg, center.z + hg); 
	qu[4].v3 = vec3( center.x + hg, center.y + hg, center.z + hg); 
	qu[4].MaterialIdx = 0; 
	qu[5].v1 = vec3( center.x + hg, center.y - hg, center.z - hg); 
	qu[5].v2 = vec3( center.x + hg, center.y - hg, center.z + hg); 
	qu[5].v3 = vec3( center.x + hg, center.y + hg, center.z - hg); 
	qu[5].MaterialIdx = 0;

	/*down wall */ 
	qu[6].v1 = vec3( center.x - hg, center.y - hg, center.z + hg);		
	qu[6].v2 = vec3( center.x - hg, center.y - hg, center.z - hg); 
	qu[6].v3 = vec3( center.x + hg, center.y - hg, center.z + hg); 
	qu[6].MaterialIdx = 0; 
	qu[7].v1 = vec3( center.x + hg, center.y - hg, center.z - hg); 
	qu[7].v2 = vec3( center.x + hg, center.y - hg, center.z + hg); 
	qu[7].v3 = vec3( center.x - hg, center.y - hg, center.z - hg); 
	qu[7].MaterialIdx = 0;
	/*down wall */ 
	qu[6].v1 = vec3( center.x + hg, center.y - hg, center.z + hg);		
	qu[6].v2 = vec3( center.x - hg, center.y - hg, center.z - hg); 
	qu[6].v3 = vec3( center.x - hg, center.y - hg, center.z + hg); 
	qu[6].MaterialIdx = 0; 
	qu[7].v1 = vec3( center.x - hg, center.y - hg, center.z - hg); 
	qu[7].v2 = vec3( center.x + hg, center.y - hg, center.z + hg); 
	qu[7].v3 = vec3( center.x + hg, center.y - hg, center.z - hg); 
	qu[7].MaterialIdx = 0;

	/*up wall */ 
	qu[8].v1 = vec3( center.x + hg, center.y + hg, center.z + hg);
	qu[8].v2 = vec3( center.x - hg, center.y + hg, center.z + hg);		
	qu[8].v3 = vec3( center.x - hg, center.y + hg, center.z - hg); 
	qu[8].MaterialIdx = 0; 
	qu[9].v1 = vec3( center.x + hg, center.y + hg, center.z - hg);
	qu[9].v2 = vec3( center.x + hg, center.y + hg, center.z + hg); 
	qu[9].v3 = vec3( center.x - hg, center.y + hg, center.z - hg); 
	qu[9].MaterialIdx = 0;

	/* front wall */
	qu[10].v1 = vec3( center.x - hg, center.y - hg, center.z - hg);		
	qu[10].v2 = vec3( center.x + hg, center.y - hg, center.z - hg);
	qu[10].v3 = vec3( center.x - hg, center.y + hg, center.z - hg);		
	qu[10].MaterialIdx = 0;
	qu[11].v1 = vec3( center.x + hg, center.y + hg, center.z - hg);		
	qu[11].v2 = vec3( center.x - hg, center.y + hg, center.z - hg);
	qu[11].v3 = vec3( center.x + hg, center.y - hg, center.z - hg);
	qu[11].MaterialIdx = 0;

	/*TETRAEDER*/
	vec3 t_center = vec3(-4.0, 4.0, 2.0);
	tetr[0].v1 = vec3(t_center.x - 2, t_center.y, t_center.z);
	tetr[0].v2 = vec3(t_center.x + 2, t_center.y, t_center.z);
	tetr[0].v3 = vec3(t_center.x, t_center.y, t_center.z - 3);
	tetr[0].MaterialIdx = 0;

	tetr[1].v1 = vec3(t_center.x - 2, t_center.y, t_center.z);
	tetr[1].v2 = vec3(t_center.x + 2, t_center.y, t_center.z);
	tetr[1].v3 = vec3(t_center.x, t_center.y + 3, t_center.z - 1);
	tetr[1].MaterialIdx = 0;

	tetr[2].v1 = vec3(t_center.x - 2, t_center.y, t_center.z);
	tetr[2].v2 = vec3(t_center.x, t_center.y, t_center.z - 3);
	tetr[2].v3 = vec3(t_center.x, t_center.y + 3, t_center.z - 1);
	tetr[2].MaterialIdx = 0;

	tetr[3].v1 = vec3(t_center.x + 2, t_center.y, t_center.z);
	tetr[3].v2 = vec3(t_center.x, t_center.y + 3, t_center.z - 1);
	tetr[3].v3 = vec3(t_center.x, t_center.y, t_center.z - 3);
	tetr[3].MaterialIdx = 0;
}

bool IntersectSphere ( SSphere sphere, SRay ray, float start, float final, out float time )
{
	ray.Origin -= sphere.Center;
	float A = dot ( ray.Direction, ray.Direction );
	float B = dot ( ray.Direction, ray.Origin );
	float C = dot ( ray.Origin, ray.Origin ) - sphere.Radius * sphere.Radius;
	float D = B * B - A * C;
	if ( D > 0.0 )
	{
		D = sqrt ( D );
		//time = min ( max ( 0.0, ( -B - D ) / A ), ( -B + D ) / A );
		float t1 = ( -B - D ) / A;
		float t2 = ( -B + D ) / A;
		if(t1 < 0 && t2 < 0)
			return false;
		if(min(t1, t2) < 0)
		{
			time = max(t1,t2);
			return true;
		}
			time = min(t1, t2);
			return true;
	}
	return false;
}

bool IntersectTriangle (SRay ray, vec3 v1, vec3 v2, vec3 v3, out float time )
{
	
	time = -1;
	vec3 A = v2 - v1;
	vec3 B = v3 - v1;
	vec3 N = cross(A, B);
	float NdotRayDirection = dot(N, ray.Direction);
	if (abs(NdotRayDirection) < EPSILON)
		return false;
	float d = dot(N, v1);
	float t = -(dot(N, ray.Origin) - d) / NdotRayDirection;
	if (t < 0)
		return false;
	vec3 P = ray.Origin + t * ray.Direction;
	vec3 C;
	
	vec3 edge1 = v2 - v1;
	vec3 VP1 = P - v1;
	C = cross(edge1, VP1);
	if (dot(N, C) < 0)
		return false;
	vec3 edge2 = v3 - v2;
	vec3 VP2 = P - v2;
	C = cross(edge2, VP2);
	if (dot(N, C) < 0)
		return false;
	vec3 edge3 = v1 - v3;
	vec3 VP3 = P - v3;
	C = cross(edge3, VP3);
	if (dot(N, C) < 0)
		return false;
	time = t;
		return true;
	
}

bool Raytrace ( SRay ray, float start, float final, inout SIntersection intersect )
{
	bool result = false;
	float test = start;
	intersect.Time = final;
	if (ubigs != 0)
	{
		SSphere sphere = spheres[0];
		if(test < intersect.Time && IntersectSphere (sphere, ray, start, final, test ))
		{
			intersect.Time = test;
			intersect.Point = ray.Origin + ray.Direction * test;
			intersect.Normal = normalize(intersect.Point - spheres[0].Center);
			intersect.Color = materials[0].Color;
			intersect.LightCoeffs = materials[0].LightCoeffs;
			intersect.ReflectionCoef = materials[0].ReflectionCoef;
			intersect.RefractionCoef = materials[0].RefractionCoef;
			intersect.MaterialType = materials[0].MaterialType;
			result = true;
		}
	}
	if (usmalls != 0)
	{
		SSphere sphere = spheres[1];
		if(test < intersect.Time && IntersectSphere (sphere, ray, start, final, test ))
		{
			intersect.Time = test;
			intersect.Point = ray.Origin + ray.Direction * test;
			intersect.Normal = normalize(intersect.Point - spheres[1].Center);
			intersect.Color = materials[1].Color;
			intersect.LightCoeffs = materials[1].LightCoeffs;
			intersect.ReflectionCoef = materials[1].ReflectionCoef;
			intersect.RefractionCoef = materials[1].RefractionCoef;
			intersect.MaterialType = materials[1].MaterialType;
			result = true;
		}
	}
	//calculate intersect with triangles
	for(int i = 0; i < 12; i++)
	{
		STriangle triangle = triangles[i];
		if(IntersectTriangle(ray, triangle.v1, triangle.v2, triangle.v3, test)
			&& test < intersect.Time)
		{
			intersect.Time = test;
			intersect.Point = ray.Origin + ray.Direction * test;
			intersect.Normal = normalize(cross(triangle.v1 - triangle.v2, triangle.v3 - triangle.v2));
			intersect.Color = materials[i /2 + 2].Color;
			intersect.LightCoeffs = materials[i /2 + 2].LightCoeffs;
			intersect.ReflectionCoef = materials[i /2 + 2].ReflectionCoef;
			intersect.RefractionCoef = materials[i /2 + 2].RefractionCoef;
			intersect.MaterialType = materials[i /2 + 2].MaterialType;
			result = true;
		}
	}
	if (ucube != 0)
	for(int i = 0; i < 12; i++)
	{
		STriangle triangle = qu[i];
		if(IntersectTriangle(ray, triangle.v1, triangle.v2, triangle.v3, test)
			&& test < intersect.Time)
		{
			intersect.Time = test;
			intersect.Point = ray.Origin + ray.Direction * test;
			intersect.Normal = normalize((cross(triangle.v1 - triangle.v2, triangle.v3 - triangle.v2)));
			intersect.Color = materials[8].Color;
			intersect.LightCoeffs = materials[8].LightCoeffs;
			intersect.ReflectionCoef = materials[8].ReflectionCoef;
			intersect.RefractionCoef = materials[8].RefractionCoef;
			intersect.MaterialType = materials[8].MaterialType;
			result = true;
		}
	}
	if (utetr != 0)
	for(int i = 0; i < 4; ++i)
	{
		STriangle triangle = tetr[i];
		if(IntersectTriangle(ray, triangle.v1, triangle.v2, triangle.v3, test)
			&& test < intersect.Time)
		{
			intersect.Time = test;
			intersect.Point = ray.Origin + ray.Direction * test;
			intersect.Normal = normalize((cross(triangle.v1 - triangle.v2, triangle.v3 - triangle.v2)));
			intersect.Color = materials[9].Color;
			intersect.LightCoeffs = materials[9].LightCoeffs;
			intersect.ReflectionCoef = materials[9].ReflectionCoef;
			intersect.RefractionCoef = materials[9].RefractionCoef;
			intersect.MaterialType = materials[9].MaterialType;
			result = true;
		}
	}
	

	return result;
}

float Shadow(SLight currLight, SIntersection intersect)
{
	float shadowing = 1.0;
	vec3 direction = normalize(currLight.Position - intersect.Point);
	float distanceLight = distance(currLight.Position, intersect.Point);
	SRay shadowRay = SRay(intersect.Point + direction * EPSILON, direction);
	SIntersection shadowIntersect;
	shadowIntersect.Time = BIG;
	if(Raytrace(shadowRay, 0, distanceLight, shadowIntersect))
	{
		shadowing = 0.0;
	}
	return shadowing;
}

vec3 Phong ( SIntersection intersect, SLight currLight, SCamera uCamera)
{
	float Unit = 1;
	float shadow = Shadow(currLight, intersect);
	vec3 light = normalize ( currLight.Position - intersect.Point );
	float diffuse = max(dot(light, intersect.Normal), 0.0);
	vec3 view = normalize(uCamera.Position - intersect.Point);
	vec3 reflected= reflect( -view, intersect.Normal );
	float specular = pow(max(dot(reflected, light), 0.0), intersect.LightCoeffs.w);
	return intersect.LightCoeffs.x * intersect.Color +
		intersect.LightCoeffs.y * diffuse * intersect.Color * shadow +
		intersect.LightCoeffs.z * specular * Unit;
}

			
struct Stack
{	
	int count;
	STracingRay ar[100];
};

Stack st;

bool isEmpty()
{
	return (st.count == 0);
}

void push(STracingRay ray)
{
	st.ar[st.count] = ray;
	st.count++;
}

STracingRay pop()
{
	st.count--;
	return st.ar[st.count];
}

void main( void )
{

	SLight ulight;
	st.count = 0 ;

	float start, final;
	SCamera uCamera = initializeDefaultCamera();
	SRay ray = GenerateRay(uCamera);
	vec3 resultColor = vec3(0,0,0);
	initializeDefaultLightMaterials(ulight);
	initializeDefaultScene();
	STracingRay trRay = STracingRay(ray, 1, 0);
	push(trRay);
	while(!isEmpty())
	{			
		STracingRay trRay = pop();
		ray = trRay.ray;
		SIntersection intersect;
		intersect.Time = BIG;
		start = 0;
		final = BIG;
		if (Raytrace(ray, start, final, intersect))
		{
			switch(intersect.MaterialType)
			{
				case DIFFUSE_REFLECTION:
				{
					float shadowing = Shadow(ulight, intersect);
					resultColor += trRay.contribution * Phong ( intersect, ulight, uCamera );
					break;
				}
				case MIRROR_REFLECTION:
				{
					if(intersect.ReflectionCoef < 1)
					{
						float contribution = trRay.contribution * (1 - intersect.ReflectionCoef);
						float shadowing = Shadow(ulight, intersect);												 
						resultColor += contribution * Phong(intersect, ulight, uCamera);
					}
					vec3 reflectDirection = reflect(ray.Direction, intersect.Normal);
					// creare reflection ray
					float contribution = trRay.contribution * intersect.ReflectionCoef;
					STracingRay reflectRay = STracingRay(
					SRay(intersect.Point + reflectDirection * EPSILON, reflectDirection),contribution, trRay.depth + 1);
					if (reflectRay.depth <= RTDepth)
					{
						push(reflectRay);
					}
					break;
				}
				case REFRACTION:
				{
					float ior;
					int sign;
					if (dot(ray.Direction, intersect.Normal) < 0)
					{
						sign = 1;
						ior = 1.0 / intersect.RefractionCoef;
					}
					else
					{
						sign = -1;
						ior = intersect.RefractionCoef;
					}
					vec3 refractionDirection = normalize(refract(ray.Direction,  intersect.Normal * sign, ior));
					vec3 refractionRayOrig = intersect.Point + EPSILON * refractionDirection;
					STracingRay refractRay = STracingRay(SRay(refractionRayOrig, refractionDirection), trRay.contribution, trRay.depth + 1);
					push(refractRay);	 
				}
			} 
		}
	} 

	FragColor = vec4 ( resultColor, 1.0 );
}