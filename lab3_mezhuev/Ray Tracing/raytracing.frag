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

void main()
{
    SCamera camera = initializeDefaultCamera();
    SRay ray = GenerateRay(camera);
    initializeDefaultScene();
    fragColor = vec4(abs(ray.Direction.xy), 0.0, 1.0);
}