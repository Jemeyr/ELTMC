//--------------------------- BASIC PROPERTIES ------------------------------
// The world transformation
float4x4 World;
 
// The view transformation
float4x4 View;
 
// The projection transformation
float4x4 Projection;
 
// The transpose of the inverse of the world transformation,
// used for transforming the vertex's normal
float4x4 WorldInverseTranspose;
 
//--------------------------- DIFFUSE LIGHT PROPERTIES ------------------------------
// The direction of the diffuse light
float3 DiffuseLightDirection = float3(0, 1, 0);
 
// The color of the diffuse light
float4 DiffuseColor = float4(1, 1, 1, 1);
 
// The intensity of the diffuse light
float DiffuseIntensity = 1.0;




//----------------------------Quantizer Bounds---------------------------------

float3 Bounds = float3(.95, .5, .05);
float4 LightMux = float4(1.0, .7, .35, .1);

float AmbientIntensity = .75;

float4 PlayerColor = float4(1.0,0,.5,1);


 
//--------------------------- TEXTURE PROPERTIES ------------------------------
// The texture being used for the object
texture Texture;
 
// The texture sampler, which will get the texture color
sampler2D textureSampler = sampler_state 
{
    Texture = (Texture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};
 
//--------------------------- DATA STRUCTURES ------------------------------
// The structure used to store information between the application and the
// vertex shader
struct AppToVertex
{
    float4 Position : POSITION0;            // The position of the vertex
    float3 Normal : NORMAL0;                // The vertex's normal
    float2 TextureCoordinate : TEXCOORD0;    // The texture coordinate of the vertex
};
 
// The structure used to store information between the vertex shader and the
// pixel shader
struct VertexToPixel
{
    float4 Position : POSITION0;
    float2 TextureCoordinate : TEXCOORD0;
    float3 Normal : TEXCOORD1;
};
 
//--------------------------- SHADERS ------------------------------
// The vertex shader that does cel shading.
// It really only does the basic transformation of the vertex location,
// and normal, and copies the texture coordinate over.
VertexToPixel CelVertexShader(AppToVertex input)
{
    VertexToPixel output;
 
    // Transform the position
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
 
    // Transform the normal
    output.Normal = normalize(mul(input.Normal, WorldInverseTranspose));
 
    // Copy over the texture coordinate
    output.TextureCoordinate = input.TextureCoordinate;
 
    return output;
}
 
// The pixel shader that does cel shading.  Basically, it calculates
// the color like is should, and then it discretizes the color into
// one of four colors.
float4 CelPixelShader(VertexToPixel input) : COLOR0
{
    // Calculate diffuse light amount
    float intensity = dot(normalize(DiffuseLightDirection), input.Normal) + AmbientIntensity;
    if(intensity < 0)
        intensity = 0;

 
    // Calculate what would normally be the final color, including texturing and diffuse lighting
    float4 color = tex2D(textureSampler, input.TextureCoordinate) ;
 	
	
	if(color.a < .95)
	{
		color = PlayerColor;
	}

	

	if(color.g >= .5 )//&& color.b <= 0.01 && color.r <= 0.01)
	{
		//color = PlayerColor;
	}

	color = color * DiffuseColor * DiffuseIntensity;
	
	color.a = 1;


    // Discretize the intensity, based on a few cutoff points

    if (intensity >= Bounds[0])
        color = float4(LightMux[0],LightMux[0],LightMux[0],1.0) * color;
    else if (intensity >= Bounds[1])
        color = float4(LightMux[1],LightMux[1],LightMux[1],1.0) * color;
    else if (intensity >= Bounds[2])
        color = float4(LightMux[2],LightMux[2],LightMux[2],1.0) * color;
    else
        color = float4(LightMux[3],LightMux[3],LightMux[3],1.0) * color;
	
    return color;
}
 
// The entire technique for doing toon shading
technique Toon
{
 
    // The second pass will draw the model like normal, but with the cel pixel shader, which will
    // color the model with certain colors, giving us the cel/toon effect that we are looking for.
    pass Pass2
    {
        VertexShader = compile vs_2_0 CelVertexShader();
        PixelShader = compile ps_2_0 CelPixelShader();
        CullMode = CCW;
    }
}