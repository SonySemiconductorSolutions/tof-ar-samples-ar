/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ARFoundation/WireframeOcclusion"
{	
	Properties
	{
		_MainColor("Color", Color) = (1, 1, 1)
		_LineWidth("Line Width", float) = 0.01
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderQueue" = "Geometry-1"}
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			ZWrite On
			ColorMask 0
		}

		LOD 100
		Pass
		{
			ZWrite On

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma only_renderers metal
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			float _LineWidth;
			fixed4 _MainColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float mindist(float2 p0, float2 p1, float2 p) {
			  float2 vw = p1 - p0;
			  float l2 = dot(vw, vw);
			  float t = max(0, min(1, dot(p - p0, vw) / l2));
			  float2 projection = p0 + t * (vw);
			  return distance(p, projection);
			}

			fixed4 frag(v2f i) : SV_Target
			{

				float depth = (i.vertex.z);
				float lineWidth = _LineWidth * 15000 * depth;
				float2 tangent = float2(ddx(i.uv.x),ddy(i.uv.x));
				float2 binormal = float2(ddx(i.uv.y),ddy(i.uv.y));
				float uvDistMax = lineWidth * length(tangent + binormal);

				float tangentDistMax = lineWidth * length(tangent);
				float binormalDistMax = lineWidth * length(binormal);
				float minUDist = min(i.uv.x,1.0 - i.uv.x);
				float minVDist = min(i.uv.y,1.0 - i.uv.y);
				float distNormU = minUDist / tangentDistMax;
				float distNormV = minVDist / binormalDistMax;

				float2 top = float2(0.0,1.0);
				float2 right = float2(1.0,0.0);
				float uvDist = mindist(top,right,i.uv);
				float uvDistNorm = uvDist / uvDistMax;

				float distNorm = min(min(distNormU,distNormV),uvDistNorm);

				float alpha = 1.0 - smoothstep(1.0,1.0 + (1.f / lineWidth),distNorm);

				//float alpha = 1.0 - 1.0 / (1.0 + exp(-2000 * (distNorm - lineWidth)));

				alpha *= _MainColor.a;
				fixed4 col = _MainColor;

				return fixed4(col.rgb,alpha);
			}
			ENDCG
		}
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderQueue" = "Geometry-1"}
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			ZWrite On
			ColorMask 0
		}

		LOD 100

		Pass
		{
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#pragma exclude_renderers metal

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2g
			{
				float4 vertex : SV_POSITION;
			};


			struct g2f
			{
				float3 dist : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float4 _MainColor;
			float _LineWidth;

			v2g vert(appdata v)
			{
				v2g o;
				o.vertex = v.vertex;
				return o;
			}

			[maxvertexcount(3)]
			void geom(triangle v2g input[3], inout TriangleStream<g2f> stream)
			{
				float3 v0 = input[0].vertex;
				float3 v1 = input[1].vertex;
				float3 v2 = input[2].vertex;

				float3 e0 = v2 - v1;
				float3 e1 = v0 - v2;
				float3 e2 = v1 - v0;

				float dist0 = length(cross(e0, e2)) / length(e0);
				float dist1 = length(cross(e1, e0)) / length(e1);
				float dist2 = length(cross(e2, e1)) / length(e2);

				g2f o;
				o.vertex = UnityObjectToClipPos(input[0].vertex);
				o.dist = float3(0, 0, dist0);
				stream.Append(o);

				o.vertex = UnityObjectToClipPos(input[1].vertex);
				o.dist = float3(0, dist1, 0);
				stream.Append(o);

				o.vertex = UnityObjectToClipPos(input[2].vertex);
				o.dist = float3(dist2, 0, 0);
				stream.Append(o);
			}

			fixed4 frag(g2f i) : SV_Target
			{
				float3 dist = i.dist;
				float minDist = min(dist[0], min(dist[1], dist[2]));

				float a = 1 - 1 / (1 + exp(-2000 * (minDist - _LineWidth)));

				fixed4 col = _MainColor;
				col.a = col.a * a;
				return col;
			}
			ENDCG
		}
	}

FallBack "Diffuse"
}