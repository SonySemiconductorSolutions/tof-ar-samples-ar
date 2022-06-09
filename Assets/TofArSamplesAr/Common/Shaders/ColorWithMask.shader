/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

Shader "TofAr/Segmentation/ColorWithMask" {
    Properties{
        _Color("Base", Color) = (0,0,0,0)
        _MaskTexHuman("Mask Human", 2D) = "black" {}
        _MaskTexSky("Mask Sky", 2D) = "black" {}
        [MaterialToggle]_useHuman("use Human", int) = 0
        [MaterialToggle]_invertHuman("invert Human", int) = 0
        [MaterialToggle]_useSky("use Sky", int) = 0
        [MaterialToggle]_invertSky("invert Sky", int) = 0
    }
        SubShader{

        Tags { "RenderType" = "Transparent"  "Queue" = "Transparent"}
        Cull Off ZWrite Off ZTest Always

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MaskTexHuman;
            sampler2D _MaskTexSky;
            float4 _Color;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv2 : TEXCOORD0;
                float2 uv3 : TEXCOORD1;
            };

            float4 _MaskTexHuman_ST;
            float4 _MaskTexSky_ST;
            
            float _useHuman;
            float _invertHuman;
            float _useSky;
            float _invertSky;          

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.uv2 = TRANSFORM_TEX(v.texcoord, _MaskTexHuman);
                o.uv3 = TRANSFORM_TEX(v.texcoord, _MaskTexSky);
                return o;
            }

            half4 frag(v2f i) : COLOR
            {
                half4 base = _Color;
                half4 maskHuman = tex2D(_MaskTexHuman, i.uv2);
                half4 maskSky = tex2D(_MaskTexSky, i.uv3);
                
                maskHuman.x = maskHuman.x*_useHuman + (1.0f - maskHuman.x)*_invertHuman;
                maskSky.x = maskSky.x*_useSky + (1.0f - maskSky.x)*_invertSky;
                
                maskHuman.x = maskHuman.x > maskSky.x ? maskHuman.x : maskSky.x;
                base.w = maskHuman.x * maskHuman.x * maskHuman.x;

                return base;
            }
            ENDCG
            }
    }
        FallBack Off
}