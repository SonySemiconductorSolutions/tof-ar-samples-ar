/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

Shader "BGChange/Mask"
{
    Properties
    {

        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MaskTexSky("Mask Sky", 2D) = "black" {}

        _Alpha("Alpha", Range(0.0, 1.0)) = 0.0
        
        [MaterialToggle]_useSky("use Sky", int) = 1
        [MaterialToggle]_invertSky("invert Sky", int) = 0
        [MaterialToggle]_invertUVXSky("invert UV X Sky", int) = 0
        [HideInInspector]_OffsetU("OffsetU", Range(-1.0,1.0)) = 0.0
        [HideInInspector]_OffsetV("OffsetV", Range(-1.0,1.0)) = 0.0
        [HideInInspector]_ScaleV("ScaleV", Range(0.0,2.0)) = 1.0
        [HideInInspector]_ScaleU("ScaleU", Range(0.0,2.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent"  "Queue" = "Transparent"}
        Cull Off ZWrite Off ZTest Always

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MaskTexSky;
            sampler2D _MainTex;


            struct v2f {
                float4 vertex : SV_POSITION;
                float4 pos : TEXCOORD0;
                float2 uv2 : TEXCOORD2;
            };

            float4 _MaskTexSky_ST;

            int _invertUVXSky;
            float _invertSky;
            float _useSky;
            float _Alpha;

            float _ScaleU;
            float _ScaleV;
            float _OffsetU;
            float _OffsetV;


            v2f vert(appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.pos =  ComputeScreenPos(o.vertex );

                float2 texT = v.texcoord;
                texT.x = texT.x * _ScaleU + _OffsetU;
                texT.y = texT.y * _ScaleV + _OffsetV;

                o.uv2 = TRANSFORM_TEX(texT, _MaskTexSky);
                return o;
            }

            half4 frag(v2f i) : COLOR
            {
                half4 baseColor = tex2Dproj(_MainTex, i.pos);

                if (i.uv2.y <= 0 && i.uv2.y >= -1 && i.uv2.x >= 0 && i.uv2.x <= +1) {
                    float2 maskSKYUV = float2(i.uv2.x, i.uv2.y);

                    if (_invertUVXSky == 1) {
                        maskSKYUV.x = i.uv2.x;
                    }
                    else {
                        maskSKYUV.x = i.uv2.x;
                    }

                    half4 maskSky = tex2D(_MaskTexSky, maskSKYUV);

                    maskSky.x = maskSky.x * _useSky + (1.0f - maskSky.x) * _invertSky;
                    baseColor.a *= maskSky.x * maskSky.x * maskSky.x;
                }
                else {
                    return half4 (0, 0, 0, 1);
                }

                baseColor.a *= _Alpha;

                return baseColor;
            }
            ENDCG
            }
    }
        FallBack Off
}
