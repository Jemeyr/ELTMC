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
float3 DiffuseLightDirection = float3(0.5, 1, 0.5);
 
// The color of the diffuse light
float4 DiffuseColor = float4(1, 1, 1, 1);
 
// The intensity of the diffuse light
float DiffuseIntensity = 1.0;


//----------------------------Quantizer Bounds---------------------------------

float3 Bounds = float3(.95, .5, .05);
float4 LightMux = float4(1.2, 1.0, 0.6, 0.2);

float AmbientIntensity = .1;

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
 

//--------------------------- UNIT VECTORS ------------------------------------
//These determine the normal amounts for normaldepth map
float3 xUnit = (1.0, 0.0, 0.0);
float3 yUnit = (0.0, 1.0, 0.0);
float3 zUnit = (0.0, 0.0, 1.0);






//--------------------------- STRUCTS------------------------------------

struct ToonAppToVertex
{
    float4 Position : POSITION0;            // The position of the vertex
    float3 Normal : NORMAL0;                // The vertex's normal
    float2 TextureCoordinate : TEXCOORD0;   // The texture coordinate of the vertex
};
 
// The structure used to store information between the vertex shader and the
// pixel shader
struct ToonVertexToPixel
{
    float4 Position : POSITION0;
    float2 TextureCoordinate : TEXCOORD0;
    float3 Normal : TEXCOORD1;
};


// The structure used to store information between the application and the
// vertex shader
struct NormalAppToVertex
{
    float4 Position : POSITION0;            // The position of the vertex
    float3 Normal : NORMAL0;                // The vertex's normal
};
 
// The structure used to store information between the vertex shader and the
// pixel shader
struct NormalVertexToPixel
{
    float4 Position : POSITION0;
    float3 Normal	: TEXCOORD1;
};


struct DepthAppToVertex
{
    float4 Position : POSITION0;            // The position of the vertex
    float3 Normal : NORMAL0;                // The vertex's normal
};
 
// The structure used to store information between the vertex shader and the
// pixel shader
struct DepthVertexToPixel
{
    float4 Position : POSITION0;
	float Depth		: TEXCOORD0;
};



//--------------------------- SHADERS------------------------------------

float4 DefaultTransform(float4 inputPosition)
{
	float4x4 worldViewProjection = mul(mul(World,View),Projection);

	return mul(inputPosition, worldViewProjection);
}



// The vertex shader that does cel shading.
// It really only does the basic transformation of the vertex location,
// and normal, and copies the texture coordinate over.
ToonVertexToPixel ToonVertexShader(ToonAppToVertex input)
{
    ToonVertexToPixel output;
 
	//transform the position to viewspace
    output.Position = DefaultTransform(input.Position);
 
    // Transform the normal
    output.Normal = mul(input.Normal, WorldInverseTranspose); //was normalized, but it looked bad
 
    // Copy over the texture coordinate
    output.TextureCoordinate = input.TextureCoordinate;
 
    return output;
}
 
// The pixel shader that does cel shading.  Basically, it calculates
// the color like is should, and then it discretizes the color into
// one of four colors.
float4 ToonPixelShader(ToonVertexToPixel input) : COLOR0
{
    // Calculate diffuse light amount
    float intensity = dot(normalize(DiffuseLightDirection), input.Normal) + AmbientIntensity;
    if(intensity < 0)
        intensity = 0;

 
    // Calculate what would normally be the final color, including texturing and diffuse lighting
    float4 color = tex2D(textureSampler, input.TextureCoordinate) ;
 	
	//Filter out low alpha textures and replace with playercolor
	if(color.a < .95)
	{
		color = PlayerColor;
	}

	//scale the color intensity and tint it by the light
	color = color * DiffuseColor * DiffuseIntensity;
	
	//make sure not transparent
	color.a = 1;


    // Discretize the intensity, based on a few cutoff points
    
	if (intensity >= Bounds[0])
        color = color * LightMux[0];
    else if (intensity >= Bounds[1])
        color = color * LightMux[1];
    else if (intensity >= Bounds[2])
        color = color * LightMux[2];
    else
        color = color * LightMux[3];
	
	return color;
}
 



NormalVertexToPixel NormalVertexShader(NormalAppToVertex input)
{
    NormalVertexToPixel output;
 
    // Transform the position
	output.Position = DefaultTransform(input.Position);
	
    // Transform the normal
    //output.Normal = normalize(input.Normal);
    output.Normal = normalize(mul(input.Normal, World));//do worldinversetranspose here to get a cool effect (render the normal target)
	return output;
}
 

float4 NormalPixelShader(NormalVertexToPixel input) : COLOR0
{
    float4 color;

	color.a = 1;

	//The color is given a base .33 to allow two planes with normals at 0 in different axis to be differentiated
	color.r = .33 + .66 * input.Normal.x;
	color.g = .33 + .66 * input.Normal.y;
	color.b = .33 + .66 * input.Normal.z;
	
    return color;
}
 


DepthVertexToPixel DepthVertexShader(DepthAppToVertex input)
{
    DepthVertexToPixel output;
 
	
    // Transform the position
	output.Position = DefaultTransform(input.Position);
	
	//Get depth value
	output.Depth =  output.Position.z/1600;		//sqrt causes twisty artefact
    return output;
}
 

float4 DepthPixelShader(DepthVertexToPixel input) : COLOR0
{
    float4 color = {0,0,0,1};

	//The input depth is scaled logarithmically (this isn't ideal, eventually I should move to a 32bit grayscale depth map)
	input.Depth =  -input.Depth *log(input.Depth) * 2.718;

	if(input.Depth > 1)
	{
		float4 c = {0,1,0,1};
		return c;
	}	


	//Here the different input depths are cycled through to map different depths to different colors. This would be simpler with a grayscale.
	if(input.Depth < 0.166)
	{
		color.r = input.Depth * 6;
	}
	else if(input.Depth < .333)
	{
		color.r = 1;
		color.g = (input.Depth - 0.166) * 6;
	}
	else if(input.Depth < 0.5)
	{	
		color.r = 1 - (input.Depth - 0.333) * 6;
		color.g = 1;
	}
	else if(input.Depth < 0.666)
	{	
		color.g = 1;
		color.b = (input.Depth - 0.5) * 6;
	}
	else if(input.Depth < 0.833)
	{
		color.b = 1;
		color.g = 1 - (input.Depth - 0.666) * 6;
	}
	else
	{
		color.b = 1;
		color.r = (input.Depth - 0.833) * 6;
	}


    return color;
}


//--------------------------- TECHNIQUES---------------------------------
technique Toon
{
 
    // The pass will draw the model like normal, but with the cel pixel shader, which will
    // color the model with certain colors, giving us the cel/toon effect that we are looking for.
    pass Pass1
    {
        VertexShader = compile vs_2_0 ToonVertexShader();
        PixelShader = compile ps_2_0 ToonPixelShader();
        CullMode = CCW;
    }
}

technique Normal
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 NormalVertexShader();
		PixelShader = compile ps_2_0 NormalPixelShader();
		CullMode = CCW;
	}
}

technique Depth
{
	pass Pass1
	{
		ZEnable = TRUE;
        ZWriteEnable = TRUE;
        AlphaBlendEnable = FALSE;

	
		VertexShader = compile vs_2_0 DepthVertexShader();
		PixelShader = compile ps_2_0 DepthPixelShader();
		CullMode = CCW;
	}
}
