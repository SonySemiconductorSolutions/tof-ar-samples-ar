/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ARFoundation/MeshOcclusion"
{	
	Properties
	{
		_MainColor("Color", Color) = (1, 1, 1)
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
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			fixed4 _MainColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{

				float alpha = 0.0;

				alpha *= _MainColor.a;
				fixed4 col = _MainColor;

				return fixed4(col.rgb,alpha);
			}
			ENDCG
		}
	}

FallBack "Diffuse"
}