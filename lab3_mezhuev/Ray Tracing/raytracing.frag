#version 430

#define EPSILON 0.001
#define BIG 1000000.0
const int DIFFUSE = 1;
const int REFLECTION = 2;
const int REFRACTION = 3;

uniform vec3 uCamera;  // Объявляем uniform переменную для позиции камеры
uniform float aspect;  // Объявляем uniform переменную для соотношения сторон

out vec4 fragColor;

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


struct SIntersection
{
 float Time;
 vec3 Point;
 vec3 Normal;
 vec3 Color;
 // ambient, diffuse and specular coeffs
 vec4 LightCoeffs;
 // 0 - non-reflection, 1 - mirror
 float ReflectionCoef;
 float RefractionCoef;
 int MaterialType;
};


SCamera initializeDefaultCamera()
{
    SCamera camera;
    camera.Position = vec3(0.0, 0.0, -8.0);
    camera.View = vec3(0.0, 0.0, 1.0);
    camera.Up = vec3(0.0, 1.0, 0.0);
    camera.Side = vec3(1.0, 0.0, 0.0);
    camera.Scale = vec2(1.0);
    return camera;
}

SRay GenerateRay(SCamera camera)
{
    vec2 coords = (gl_FragCoord.xy / vec2(800, 600)) * 2.0 - 1.0; // Нормализация координат
    coords.x *= aspect;
    vec3 direction = camera.View + camera.Side * coords.x + camera.Up * coords.y;
    return SRay(camera.Position, normalize(direction));
}

STriangle triangles[12];
SSphere spheres[2];

void initializeDefaultScene()
{
    /** TRIANGLES **/
    /* left wall */
    triangles[0] = STriangle(vec3(-5.0, -5.0, -5.0), vec3(-5.0, 5.0, 5.0), vec3(-5.0, 5.0, -5.0), 0);
    triangles[1] = STriangle(vec3(-5.0, -5.0, -5.0), vec3(-5.0, -5.0, 5.0), vec3(-5.0, 5.0, 5.0), 0);
    /* back wall */
    triangles[2] = STriangle(vec3(-5.0, -5.0, 5.0), vec3(5.0, -5.0, 5.0), vec3(-5.0, 5.0, 5.0), 0);
    triangles[3] = STriangle(vec3(5.0, 5.0, 5.0), vec3(-5.0, 5.0, 5.0), vec3(5.0, -5.0, 5.0), 0);
    /* right wall */
    triangles[4] = STriangle(vec3(5.0, -5.0, -5.0), vec3(5.0, 5.0, -5.0), vec3(5.0, 5.0, 5.0), 0);
    triangles[5] = STriangle(vec3(5.0, -5.0, -5.0), vec3(5.0, 5.0, 5.0), vec3(5.0, -5.0, 5.0), 0);
    /* front wall */
    triangles[6] = STriangle(vec3(-5.0, -5.0, -5.0), vec3(-5.0, 5.0, -5.0), vec3(5.0, 5.0, -5.0), 0);
    triangles[7] = STriangle(vec3(-5.0, -5.0, -5.0), vec3(5.0, 5.0, -5.0), vec3(5.0, -5.0, -5.0), 0);
    /* floor */
    triangles[8] = STriangle(vec3(-5.0, -5.0, -5.0), vec3(5.0, -5.0, -5.0), vec3(5.0, -5.0, 5.0), 0);
    triangles[9] = STriangle(vec3(-5.0, -5.0, -5.0), vec3(5.0, -5.0, 5.0), vec3(-5.0, -5.0, 5.0), 0);
    /* ceiling */
    triangles[10] = STriangle(vec3(-5.0, 5.0, -5.0), vec3(5.0, 5.0, -5.0), vec3(5.0, 5.0, 5.0), 0);
    triangles[11] = STriangle(vec3(-5.0, 5.0, -5.0), vec3(5.0, 5.0, 5.0), vec3(-5.0, 5.0, 5.0), 0);

    /** SPHERES **/
    spheres[0] = SSphere(vec3(-1.0, -1.0, -2.0), 2.0, 0);
    spheres[1] = SSphere(vec3(2.0, 1.0, 2.0), 1.0, 0);
}

bool IntersectSphere ( SSphere sphere, SRay ray, float start, float final, out float
time )
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
// // Compute the intersection of ray with a triangle using geometric solution 
// Input: // points v0, v1, v2 are the triangle's vertices 
// rayOrig and rayDir are the ray's origin (point) and the ray's direction 
// Return: // return true is the ray intersects the triangle, false otherwise 
// bool intersectTriangle(point v0, point v1, point v2, point rayOrig, vector rayDir) { 
// compute plane's normal vector 
time = -1;
vec3 A = v2 - v1;
vec3 B = v3 - v1;
// no need to normalize vector 
vec3 N = cross(A, B);
// N 
// // Step 1: finding P
// // check if ray and plane are parallel ? 
float NdotRayDirection = dot(N, ray.Direction);
if (abs(NdotRayDirection) < 0.001) 
return false;
// they are parallel so they don't intersect ! 
// compute d parameter using equation 2 
float d = dot(N, v1);
// compute t (equation 3) 
float t = -(dot(N, ray.Origin) - d) / NdotRayDirection;
// check if the triangle is in behind the ray 
if (t < 0) 
return false;
// the triangle is behind 
// compute the intersection point using equation 1 
vec3 P = ray.Origin + t * ray.Direction;
// // Step 2: inside-outside test // 
vec3 C;
// vector perpendicular to triangle's plane 
// edge 0 
vec3 edge1 = v2 - v1;
vec3 VP1 = P - v1;
C = cross(edge1, VP1);
if (dot(N, C) < 0)
return false;
// P is on the right side 
// edge 1 
vec3 edge2 = v3 - v2;
vec3 VP2 = P - v2;
C = cross(edge2, VP2);
if (dot(N, C) < 0) 
return false;
// P is on the right side 
// edge 2 
vec3 edge3 = v1 - v3;
vec3 VP3 = P - v3;
C = cross(edge3, VP3);
if (dot(N, C) < 0) 
return false;
// P is on the right side; 
time = t;
return true;
// this ray hits the triangle
}









void main()
{
    SCamera camera = initializeDefaultCamera();
    SRay ray = GenerateRay(camera);
    initializeDefaultScene();
    fragColor = vec4(abs(ray.Direction.xy), 0.0, 1.0);
}