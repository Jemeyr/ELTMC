//-----------------------------------------------------------------------------
// PostprocessEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


// Settings controlling the edge detection filter.
float EdgeWidth = 1;
float EdgeIntensity = 1;

// The threshold for normal and depth filters, to remove noise
float NormalThreshold = 0.15;
float DepthThreshold =	0.15;

// How dark the edges should get. Depth is darker because the differences can be valid and more subtle
float NormalSensitivity =	0.35;
float DepthSensitivity =	1.35; 

// Pass in the current screen resolution.
float2 ScreenResolution;


// This texture contains the main scene image, which the edge detection is being applied over the top of.
texture SceneTexture;

sampler SceneSampler : register(s0) = sampler_state
{
    Texture = (SceneTexture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    
    AddressU = Clamp;
    AddressV = Clamp;
};


// This texture contains normals (in the color channels) 
// for the main scene image. Differences in the normal data is used
// to detect where the edges of the model are.
texture NormalTexture;

sampler NormalSampler : register(s1) = sampler_state
{
    Texture = (NormalTexture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    
    AddressU = Clamp;
    AddressV = Clamp;
};


// This texture contains normals (in the color channels) and depth (in alpha)
// for the main scene image. Differences in the normal and depth data are used
// to detect where the edges of the model are.
texture DepthTexture;

sampler DepthSampler : register(s2) = sampler_state
{
    Texture = (DepthTexture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    
    AddressU = Clamp;
    AddressV = Clamp;
};

// This is the pixel shader. It combines the input images
float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // Look up the original color from the main scene.
    float3 scene = tex2D(SceneSampler, texCoord);
    
    //The edge offset should be scaled to screen resolution
    float2 edgeOffset =  EdgeWidth/ ScreenResolution; 

	//This samples four values from the normal image, along the diagonals, and wider depending on the edge offset
    float4 n1 = tex2D(NormalSampler, texCoord + float2(-1, -1) * edgeOffset);
    float4 n2 = tex2D(NormalSampler, texCoord + float2( 1,  1) * edgeOffset);
    float4 n3 = tex2D(NormalSampler, texCoord + float2(-1,  1) * edgeOffset);
    float4 n4 = tex2D(NormalSampler, texCoord + float2( 1, -1) * edgeOffset);

	//This does the same for the depth map
	float4 d1 = tex2D(DepthSampler, texCoord + float2(-1, -1) * edgeOffset);
    float4 d2 = tex2D(DepthSampler, texCoord + float2( 1,  1) * edgeOffset);
    float4 d3 = tex2D(DepthSampler, texCoord + float2(-1,  1) * edgeOffset);
    float4 d4 = tex2D(DepthSampler, texCoord + float2( 1, -1) * edgeOffset);

	//The diagonal differences in the normal and depth map are summed
	float4 normalDiag = abs(n1 - n2) + abs(n3 - n4);
	float4 depthDiag  = abs(d1 - d2) + abs(d3 - d4);

	//And the change overall is found (using dot product to 
	//cheat and produce a single difference of the three color values)
	float normalDelta = dot(normalDiag.xyz, 1);
	float depthDelta = dot(depthDiag.xyz, 1);
	  
	    
    // This adjusts the deltas, removing any value that's below the threshold and scaling that result by sensitivity
	// this input is fed into saturate() which clamps it
    normalDelta = saturate((normalDelta - NormalThreshold) * NormalSensitivity);
    depthDelta = saturate((depthDelta - DepthThreshold) * DepthSensitivity);

    // The sum is then multiplied by the edge intensity.
    float edgeAmount = saturate(normalDelta + depthDelta) * EdgeIntensity;
        
    // Finally the input pixel is darkened by this amount (which is usually zero, although some error arises when looking along
	// flat planes, due the the depth changing a bunch as you look to infinity. This doesn't look too bad though, at least not
	// in the models used here, as it forms a gradient along plans you stand on without adding extraneous edges)
    scene *= (1 - edgeAmount);

    return float4(scene,1);
}


// Compile the pixel shader for doing edge detection.
technique EdgeDetect
{
    pass P0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
