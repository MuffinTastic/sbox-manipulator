HEADER
{
	Description = "Line shader for the Manipulator tool";
}

MODES
{
    Default();
    VrForward();
}

COMMON
{
    #include "system.fxc"
	#include "common.fxc"
}

//-------------------------------------------------------------------------------------------------------------------------------------------------------------
struct VS_INPUT
{
	float4 vPositionOs			: POSITION < Semantic( PosXyz ); >;
	float4 vColor				: COLOR0 < Semantic( Color ); >;

	uint nInstanceTransformID			: TEXCOORD13	< Semantic( InstanceTransformUv ); >;
	uint nInstanceID					: SV_InstanceID < Semantic( None ); >;
};

//-------------------------------------------------------------------------------------------------------------------------------------------------------------
struct PS_INPUT
{
	float4 vPositionPs			: SV_Position;
	float4 vColor				: COLOR0;
};
  

VS
{
	//
	// Main
	//
	PS_INPUT MainVs( INSTANCED_SHADER_PARAMS( VS_INPUT i ) )
	{
		PS_INPUT o;
		o.vPositionPs = Position3WsToPs(i.vPositionOs.xyz);
		o.vColor = i.vColor;
		return o;
	}
}


PS
{
	//Blend SrcAlpha OneMinusSrcAlpha
	RenderState( BlendEnable, true );
    RenderState( SrcBlend, SRC_ALPHA );
    RenderState( DstBlend, INV_SRC_ALPHA );

	//Cull Off ZWrite Off ZTest Always
    RenderState( DepthWriteEnable, false );
    RenderState( DepthEnable, false );
	//
	// Main
	//
	float4 MainPs( PS_INPUT i ) : SV_Target0
	{
		return i.vColor;
	}
}