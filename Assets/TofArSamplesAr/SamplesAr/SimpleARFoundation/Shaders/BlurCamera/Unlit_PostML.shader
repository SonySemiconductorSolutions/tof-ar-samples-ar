/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

Shader "Unlit/PostML"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _Mask("Mask", 2D) = "white" {}
        _MobileRGB("MobileRGB", 2D) = "white" {}
        _ShadowsRGB("ShadowsRGB", 2D) = "white" {}
        _ShadowThreshold("Shadow threshold", float) = 0.7
    }
        SubShader
        {
            // No culling or depth
            Cull Off ZWrite Off ZTest Always

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

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                sampler2D _MainTex;
                sampler2D _Mask;
                uniform sampler2D _Texture0;
                sampler2D _MobileRGB;
                uniform half4 _MobileRGB_ST;
                sampler2D _ShadowsRGB;
                uniform half4 _ShadowsRGB_ST;
                float _ShadowThreshold;

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 uv_Texture0 = i.uv.xy * _MobileRGB_ST.xy + _MobileRGB_ST.zw;
                    fixed4 colMRGB = tex2D(_MobileRGB, uv_Texture0);
                    fixed4 colShadows = tex2D(_ShadowsRGB, i.uv);
                    fixed4 colMask = tex2D(_Mask, i.uv);

                    #if UNITY_UV_STARTS_AT_TOP
                        i.uv.y = 1.0 - i.uv.y;
                    #endif
                    if (_ProjectionParams.x < 0.0)
                    {
                        i.uv.y = 1.0 - i.uv.y;
                    }

                    fixed4 colRgb = tex2D(_MainTex, i.uv);

                    // colMRGB.rgb = colShadows.a * colShadows.rgb + (1 - colShadows.a)*colMRGB.rgb;

                     colRgb.rgb = colRgb.rgb * (colMask.a) + colMRGB.rgb * (1 - colMask.a);

                        return colRgb;
                    }
                    ENDCG
                }
        }
}