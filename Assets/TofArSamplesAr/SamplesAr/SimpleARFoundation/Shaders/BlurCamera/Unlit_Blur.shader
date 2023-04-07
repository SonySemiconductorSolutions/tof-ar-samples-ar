/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */


Shader "Unlit/Blur"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
        // 不必要な機能はすべて切る
        ZTest Always
        Cull Off
        ZWrite Off
        Fog { Mode Off }

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
                float4 vertex : SV_POSITION;
                half2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;

            half4 _Offsets;

            static const int samplingCount = 5;
            half _Weights[samplingCount];

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 col = 0;

                [unroll]
                for (int j = samplingCount - 1; j > 0; j--)
                {
                    col += tex2D(_MainTex, i.uv - (_Offsets.xy * j)) * _Weights[j];
                }

                [unroll]
                for (int k = 0; k < samplingCount; k++)
                {
                    col += tex2D(_MainTex, i.uv + (_Offsets.xy * k)) * _Weights[k];
                }

                return col;
            }
            ENDCG
        }
    }
        FallBack Off
}