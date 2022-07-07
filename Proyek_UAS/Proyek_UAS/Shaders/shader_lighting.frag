#version 330

out vec4 outputColor;

in vec4 vertexColor;
in vec3 Normal;

uniform vec3 objColor;
uniform vec3 lightColor;
uniform vec3 lightPos;
uniform vec3 viewPos;

struct SpotLight
{
    vec3  position;
    vec3  direction;
    float cutOff;
    float outerCutOff;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};
uniform SpotLight spotLight;

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{

    vec3 lightDir = normalize(light.position - vec3(vertexColor));
    float diff = max(dot(normal, lightDir), 0.0);

    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 256);
    float distance    = length(light.position - vec3(vertexColor));
    float attenuation = 1.0 / (light.constant + light.linear * distance +
    light.quadratic * (distance * distance));

    float theta     = dot(lightDir, normalize(-light.direction));
    float epsilon   = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

    vec3 ambient = light.ambient * objColor;
    vec3 diffuse = light.diffuse * diff * objColor;
    vec3 specular = light.specular * spec * objColor;
    ambient  *= attenuation;
    diffuse  *= attenuation * intensity;
    specular *= attenuation * intensity;
    return (ambient + diffuse + specular);
}

struct DirLight 
{
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

};

struct PointLight 
{
    vec3 position;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

#define NR_POINT_LIGHTS 4
uniform PointLight pointLight[NR_POINT_LIGHTS];
uniform DirLight dirLight;
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0),256);
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance +
    light.quadratic * (distance * distance));
    
    vec3 ambient  = light.ambient  * objColor;
    vec3 diffuse  = light.diffuse  * diff * objColor;
    vec3 specular = light.specular * spec * objColor;
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 256);
    vec3 ambient  = light.ambient  * objColor;
    vec3 diffuse  = light.diffuse  * diff * objColor;
    vec3 specular = light.specular * spec * objColor;
    return (ambient + diffuse + specular);
}

void main()
{
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - vec3(vertexColor));
        //Direction Light
    //
    //    vec3 result = CalcDirLight(dirLight, norm, viewDir);
    //
    //     outputColor = vec4(result, 1.0);
         //Point Light
    //     vec3 norm = normalize(Normal);
    //     vec3 viewDir = normalize(viewPos - vec3(vertexColor));
    //     vec3 result = CalcPointLight(pointLight,norm,vec3(vertexColor),viewDir);
    //     outputColor = vec4(result, 1.0);
    
         //Spot Light
    //     vec3 norm = normalize(Normal);
    //     vec3 viewDir = normalize(viewPos - vec3(vertexColor));
    //     vec3 result = CalcSpotLight(spotLight, norm, vec3(vertexColor), viewDir);
    //     outputColor = vec4(result, 1.0);

    //Point Lights
    vec3 result = CalcDirLight(dirLight, norm, viewDir);
    for (int i = 0; i < NR_POINT_LIGHTS; i++) {
        result += CalcPointLight(pointLight[i], norm, vec3(vertexColor), viewDir);
    }
    result += CalcSpotLight(spotLight, norm, vec3(vertexColor), viewDir);
    
    outputColor = vec4(result, 1.0);
}
