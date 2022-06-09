/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

Shader "Stamp"
{
    Properties
    {
        _MainTexture ("MainTexture", 2D) = "white" {}
        _fadeValue("fadeValue", Range(0, 1)) = 0
        _effectIdx("effectIdx", Int) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTexture;
            float _fadeValue;
            int _effectIdx;

            struct Attributes
            {
                float4 positionOS   : POSITION;
                 float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position  : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float rand( float2 co ){
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert(Attributes IN)
            {
                v2f o;
                o.position = UnityObjectToClipPos(IN.positionOS.xyz);
                o.uv = IN.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 c = float4(0, 0, 0, 0);
                float2 uv = float2(1-i.uv.x, i.uv.y);

                //slide effect
                if(_effectIdx == 1){ 
                    if(i.uv.y < _fadeValue){
                        c = tex2D(_MainTexture, uv);
                    }
                }
                //block effect
                else if(_effectIdx == 2){
                    float2 idx = float2( floor(i.uv.x * 15), floor(i.uv.y * 15));
                    float v = rand(idx);
                    if(v < _fadeValue){
                        c = tex2D(_MainTexture, uv);
                    }
                }
                //fade effect
                else{
                    c = tex2D(_MainTexture, uv);
                    c.a *= _fadeValue;
                }

                return c;
            }
            ENDCG
        }
    }
}
