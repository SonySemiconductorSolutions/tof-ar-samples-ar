/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */


Shader "TofAr/RotationShader"
//Shader that handles the rotates the background according to the screen position
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ScreenRotation("Screen Rotation",Int) = 1
		_ScaleOffset("Scale Offset", Vector) = (1,1,0,0)
	}
		SubShader
		{
			Cull Off
			LOD 100
			Pass
			{
				ZWrite Off
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
				int _ScreenRotation;
				float4 _ScaleOffset;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}


				fixed4 frag(v2f i) : SV_Target
				{
					fixed2 m_uv;
					i.uv.x = i.uv.x * _ScaleOffset.x + _ScaleOffset.z;
					i.uv.y = i.uv.y * _ScaleOffset.y + _ScaleOffset.w;
					if (_ScreenRotation == 1)
					{
						m_uv.x = 1 - i.uv.y;
						m_uv.y = i.uv.x;
					}
					if (_ScreenRotation == 2)
					{
						m_uv.x = i.uv.y;
						m_uv.y = 1 - i.uv.x;
					}
					if (_ScreenRotation == 3)
					{
						m_uv.x = i.uv.x;
						m_uv.y = i.uv.y;
					}
					if (_ScreenRotation == 4)
					{
						m_uv.x = 1 - i.uv.x;
						m_uv.y = 1 - i.uv.y;
					}

					fixed4 col = tex2D(_MainTex, m_uv);
					return col;
				}
				ENDCG
			}
		}
}
