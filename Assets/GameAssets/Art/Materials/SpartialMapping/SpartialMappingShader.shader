Shader "Custom/SpartialMappingShader"
{
	Properties
	{
		_BaseColor("Base color", Color) = (0.0, 0.0, 0.0, 1.0)
		_WireColor("Wire color", Color) = (1.0, 1.0, 1.0, 1.0)
		_WireThickness("Wire thickness", Range(0, 800)) = 100
		_PulseFactor("Pulse rate", Range(0,1)) = 1
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{

		CGPROGRAM
		#pragma vertex vert
		#pragma geometry geom
		#pragma fragment frag

		#include "UnityCG.cginc"

		float4 _BaseColor;
		float4 _WireColor;
		float _WireThickness;
		float _PulseFactor;

		struct appdata
		{
			float4 vertex : POSITION;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct v2g
		{
			float4 projectionSpaceVertex : SV_POSITION;
			float4 worldSpacePosition : TEXCOORD1;
			UNITY_VERTEX_OUTPUT_STEREO_EYE_INDEX
		};

		struct g2f
		{
			float4 projectionSpaceVertex : SV_POSITION;
			float4 worldSpacePosition : TEXCOORD0;
			float4 dist : TEXCOORD1;

			UNITY_VERTEX_OUTPUT_STEREO
		};

		v2g vert(appdata v)
		{
			v2g o;
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_INITIALIZE_OUTPUT_STEREO_EYE_INDEX(o);

			o.projectionSpaceVertex = UnityObjectToClipPos(v.vertex);
			o.worldSpacePosition = mul(unity_ObjectToWorld, v.vertex);
			return o;
		}

		[maxvertexcount(3)]
		void geom(triangle v2g i[3], inout TriangleStream<g2f> triangleStream)
		{
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i[0]);

			float2 p0 = i[0].projectionSpaceVertex.xy / i[0].projectionSpaceVertex.w;
			float2 p1 = i[1].projectionSpaceVertex.xy / i[1].projectionSpaceVertex.w;
			float2 p2 = i[2].projectionSpaceVertex.xy / i[2].projectionSpaceVertex.w;

			float2 edge0 = p2 - p1;
			float2 edge1 = p2 - p0;
			float2 edge2 = p1 - p0;

			// To find the distance to the opposite edge, we take the
			// formula for finding the area of a triangle Area = Base/2 * Height,
			// and solve for the Height = (Area * 2)/Base.
			// We can get the area of a triangle by taking its cross product
			// divided by 2.  However we can avoid dividing our area/base by 2
			// since our cross product will already be double our area.
			float area = abs(edge1.x * edge2.y - edge1.y * edge2.x);
			float wireThickness = 800 - _WireThickness;

			g2f o;
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
			o.worldSpacePosition = i[0].worldSpacePosition;
			o.projectionSpaceVertex = i[0].projectionSpaceVertex;
			o.dist.xyz = float3((area / length(edge0)), 0.0, 0.0) * o.projectionSpaceVertex.w * wireThickness;
			o.dist.w = 1.0 / o.projectionSpaceVertex.w;
			triangleStream.Append(o);

			o.worldSpacePosition = i[1].worldSpacePosition;
			o.projectionSpaceVertex = i[1].projectionSpaceVertex;
			o.dist.xyz = float3(0.0, (area / length(edge1)), 0.0) * o.projectionSpaceVertex.w * wireThickness;
			o.dist.w = 1.0 / o.projectionSpaceVertex.w;
			triangleStream.Append(o);

			o.worldSpacePosition = i[2].worldSpacePosition;
			o.projectionSpaceVertex = i[2].projectionSpaceVertex;
			o.dist.xyz = float3(0.0, 0.0, (area / length(edge2))) * o.projectionSpaceVertex.w * wireThickness;
			o.dist.w = 1.0 / o.projectionSpaceVertex.w;
			triangleStream.Append(o);
		}

		float4 frag(g2f i) : COLOR
		{
			// Calculate  minimum distance to one of the triangle lines, making sure to correct
			// for perspective-correct interpolation.
			float dist = min(i.dist[0], min(i.dist[1], i.dist[2])) * (1.0 / i.projectionSpaceVertex.w);

			// Make the intensity of the line very bright along the triangle edges but fall-off very
			// quickly.
			float I = exp2(-2 * dist * dist);

			// Fade out the alpha but not the color so we don't get any weird halo effects from
			// a fade to a different color. 
			float4 color = I * (_WireColor - (abs(frac(_Time.y * _PulseFactor) - 0.5f))) + (1 - I) * _BaseColor;
			color.a = I;
			return color;
		}
			ENDCG
		}
	}
	FallBack "Diffuse"
}