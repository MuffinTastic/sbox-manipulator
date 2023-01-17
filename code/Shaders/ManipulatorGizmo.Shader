HEADER
{
	Description = "Manipulator Gizmo Shader";
	DevShader = true;
}

FEATURES
{
    #include "common/features.hlsl"
}

COMMON
{
	#define USE_CUSTOM_SHADING
	#include "common/shared.hlsl"
	
}

struct VertexInput
{
	#include "common/vertexinput.hlsl"
};

struct PixelInput
{
	#include "common/pixelinput.hlsl"
};

VS
{
	#include "common/vertex.hlsl"
	//
	// Main
	//
	PixelInput MainVs( INSTANCED_SHADER_PARAMS( VertexInput i ) )
	{
		PixelInput o = ProcessVertex( i );
		return FinalizeVertex( o );
	}
}

PS
{

	//Blend SrcAlpha OneMinusSrcAlpha
	//RenderState( BlendEnable, false );
    //RenderState( SrcBlend, SRC_ALPHA );
    //RenderState( DstBlend, INV_SRC_ALPHA );
	//#define BLEND_MODE_ALREADY_SET

    RenderState( DepthWriteEnable, true );
    RenderState( DepthEnable, true );
	#define DEPTH_STATE_ALREADY_SET

	RenderState( CullMode, NONE );
	#define CULL_MODE_ALREADY_SET

    #include "common/pixel.hlsl"

	float4 _ColorMain < UiType(Color); UiGroup("Color"); Attribute("Color"); > ;

	//
	// Main
	//
	float4 MainPs( PixelInput i ): SV_Target
	{
		return _ColorMain;
	}
}
