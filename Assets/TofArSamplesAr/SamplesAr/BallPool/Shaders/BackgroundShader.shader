/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

Shader "Unlit/BackgroundShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BlendTex("Blend", 2D) = "white" {}
		_OverrideAlpha("Override Alpha", float) = 1
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True"}
			Cull Off
			LOD 100
			ZWrite Off

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _BlendTex;
				float _OverrideAlpha;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture

					fixed4 blend = tex2D(_BlendTex, i.uv);
					#if UNITY_UV_STARTS_AT_TOP
						i.uv.y = 1.0 - i.uv.y;
					#endif
					if (_ProjectionParams.x < 0.0)
					{
						i.uv.y = 1.0 - i.uv.y;
					}
					fixed4 col = tex2D(_MainTex, i.uv);

					col = col * col.a + blend * (1 - col.a);
					col.a = _OverrideAlpha + col.a * (1 - _OverrideAlpha);
					return col;
				}
				ENDCG
			}
		}
}
