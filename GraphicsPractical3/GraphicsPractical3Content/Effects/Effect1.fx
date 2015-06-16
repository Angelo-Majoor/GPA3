//------------------------------------------- Defines -------------------------------------------

#define MAX_LIGHTS 5

//------------------------------------- Top Level Variables -------------------------------------

// Matrices for 3D perspective projection
float4x4 View, Projection, World;
// The inverse of the World matrix
float4x4 WorldInverse;

// The diffuse color for the object
//float4 DiffuseColor;

// The ambient color for the object
float4 AmbientColor;
// The ambient intensity for the object
float AmbientIntensity;

// A source of light
//float3 PointLight;

float3 LightPositions[MAX_LIGHTS];
float4 DiffuseColor[MAX_LIGHTS];

//---------------------------------- Input / Output structures ----------------------------------

// The input of the vertex shader
struct VertexShaderInput
{
	float4 Position3D : POSITION0;
	float3 Normal : NORMAL0;
};

// The output of the vertex shader
struct VertexShaderOutput
{
	float4 Position2D : POSITION0;
	float3 Light[MAX_LIGHTS] : TEXCOORD1;
	//float4 Diffuse : TEXCOORD1;
	float3 Normal : TEXCOORD0;
};

//-------------------------------- Technique: Phong ---------------------------------------

VertexShaderOutput SimpleVertexShader(VertexShaderInput input)
{
	// Allocate an empty output struct
	VertexShaderOutput output = (VertexShaderOutput)0;

	// Do the matrix multiplications for perspective projection and the world transform
	float4 worldPosition = mul(input.Position3D, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position2D = mul(viewPosition, Projection);

	// To do correct lighting calculations using the normals, take the World matrix into
	// account. If the model gets rotated, so must the normals. One way to do this is 
	// extract the top left 3x3 matrix out of the World matrix, which holds the rotation 
	// and scaling part of the World transformation, apply that to the normals and 
	// finally normalize them so that any scaling is removed.
	// 
	// Extract the top-left 3x3 matrix out of the World matrix
	float3x3 rotationAndScale = (float3x3) World;
	// Apply this matrix to the normals
	float3 intermediateNormal = mul(rotationAndScale, input.Normal);
	// Normalize the normals
	output.Normal = normalize(intermediateNormal);
	
	for (int i = 0; i < MAX_LIGHTS; i++)
	{
		vector objectLight = mul(LightPositions[i], WorldInverse);
		output.Light[i] = normalize(objectLight - input.Position3D);
	}

	// Determine the light vector
	// First get the light vector in object space
	//vector objectLight = mul(PointLight, WorldInverse);
	//vector lightDirection = normalize(objectLight - input.Position3D);

	// Diffuse using Lambert
	//output.Diffuse = max(0, dot(input.Normal, lightDirection));

	return output;
}

float4 SimplePixelShader(VertexShaderOutput input) : COLOR0
{
	float4 ambient = AmbientColor * AmbientIntensity;

	float4 totalDiffuse = float4(0, 0, 0, 0);

	for (int j = 0; j < MAX_LIGHTS; j++)
	{
		totalDiffuse += DiffuseColor[j] * saturate(dot(input.Light[j], input.Normal));
	}

	return totalDiffuse;
}

technique MultipleLightSources
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 SimpleVertexShader();
		PixelShader = compile ps_2_0 SimplePixelShader();
	}
}