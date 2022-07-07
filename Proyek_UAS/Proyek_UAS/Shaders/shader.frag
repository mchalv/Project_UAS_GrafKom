#version 330
out vec4 outputColor;

in vec4 vertexColor; 
uniform vec3 objColor;

void main()
{
	outputColor = vec4(objColor, 1.0);
}