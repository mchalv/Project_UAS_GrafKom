#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
out vec4 vertexColor;
out vec3 Normal;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
	gl_Position = vec4(aPosition, 1.0) * model * view * projection;
	vertexColor = vec4(aPosition, 1.0) * model;
	Normal = aNormal*mat3(transpose(inverse(model)));
}