Shader "Hair Geometry Shader" 
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "Deferred" }
			Cull Front
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex Vertex
			#pragma geometry Geometry
			#pragma fragment Fragment
			#pragma enable_d3d11_debug_symbols	
			#include "HairGeometry.cginc"
			ENDCG
		}
	}
	FallBack "Diffuse"
}