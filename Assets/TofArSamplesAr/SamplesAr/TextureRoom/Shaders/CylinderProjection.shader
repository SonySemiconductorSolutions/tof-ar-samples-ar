/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

Shader "TextureRoom/CylinderProjection"
{
    Properties
    {
        _MainTexture ("MainTexture", 2D) = "white" {}
        _OcclusionStencil ("_OcclusionStencil", 2D) = "white" {}
        _DisplaySize("DisplaySize", Vector) = (1.0,1.0,1.0,1.0) //x:width y:height
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="AlphaTest" "LightMode" = "ForwardBase" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ IS_HUMAN_OCCULUSION
            #pragma multi_compile_fwdbase 

            #include "UnityCG.cginc"
            #include "../Shaders/Wireframe.hlsl"

            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #include "UnityStandardUtils.cginc"
           

            sampler2D _MainTexture;
            sampler2D _StampTexture;
            float4 _MainTexture_ST;
            float4 _Center;
            float _Radius;
            float _GlowStrength;
            int _CurrentMode;

            sampler2D _OcclusionStencil;
            float4 _DisplaySize;
             
            //static values
            static const float PI = 3.14159265;
            static const float2 height = float2(-1.25, 1.25);
            static const int textRepeatCount = 6;
            static const float wireFrameWidth = 0.8;
            static const float2 shadowContrastThrethold = float2(0.1, 1);
            static const float ambientLightStrenfth = 0.3;

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 posWS : TEXCOORD1;
                float3 normal : NORMAL;
                float vdotn : TEXCOORD2;
                SHADOW_COORDS(3)
                float2 uv2 : TEXCOORD4;
            };

            float map(float value, float min1, float max1, float min2, float max2) {
                return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex.xyz);
                o.posWS = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv;

                float3 normalWS = mul(unity_ObjectToWorld,v.normal);
                o.normal = normalWS;

                float3 viewDir = normalize(mul(unity_ObjectToWorld,_WorldSpaceCameraPos));
                o.vdotn = abs(dot(viewDir, normalWS));

                //UV calculation for processing without effects on human parts
                //ref:https://edom18.hateblo.jp/entry/2019/08/11/223803
                float4 vertexInput = UnityObjectToClipPos(v.vertex);
                float4 screenPos = ComputeScreenPos(vertexInput); 
                float2 nextUV = screenPos.xy / screenPos.w;

                float screenWidth = _DisplaySize.x;
                float screenHeight = _DisplaySize.y;

                float ratio = 1.62;
                float2 uv2 = float2 ((1-nextUV.y) ,(1-nextUV.x)); 
                uv2.y /= ratio;
                uv2.y += 1.0 - (ratio * 0.5);

                o.uv2 = uv2;
                //

                TRANSFER_SHADOW(o)
                
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 xyz = normalize(i.posWS - _Center);
                float u, v, lon, lat;
                float2 uv;
                
                //Cylinder mapping calculation part
                lon = atan2(xyz.z, xyz.x);
                u = lon * (1.0 / PI);
                u = u * 0.5 + 0.5;
                float _u = u;
                u = frac(u * textRepeatCount); 
                v = map(i.posWS.y, height.x, height.y, 0, 1);
                float2 originalUV = float2(_u, v);

                u = u * _MainTexture_ST.x + _MainTexture_ST.z;
                v = v * _MainTexture_ST.y + _MainTexture_ST.w;
                uv = float2(1 - u, v);

                float4 value = float4(0, 0, 0, 0);
                if(_CurrentMode == 0 || _CurrentMode == 1){
                    value = tex2D (_MainTexture, uv);
                }
                if(_CurrentMode == 2){
                    value += tex2D(_StampTexture, originalUV);
                }
                float4 c = float4(value.r, value.g, value.b, value.a);

                float _d = abs(dot(normalize(float3(0, 1, 0)), i.normal));
                c *= 1-_d;

                //Get direction and intensity from light and calculate shading
                //Normalize the vector of the second light
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                //Normalize normals in world coordinate system
                float3 N = i.normal;
                float NdotL = saturate(dot(L, i.normal));     
                //Consider the color of the lights
                float3 ambient= ShadeSHPerVertex(i.normal, _LightColor0* LIGHT_ATTENUATION(i)) * ambientLightStrenfth;
                //Calculate shadow
                c.rgb *= clamp(NdotL*SHADOW_ATTENUATION(i) ,shadowContrastThrethold.x,shadowContrastThrethold.y) + ambient;

                //Wireframe Drawing
                float4 wf = float4(0, 0, 0, 0);
                float d = distance(i.posWS.xz, _Center.xz);
                if(d > _Radius - wireFrameWidth*0.5 && d < _Radius + wireFrameWidth*0.5){
                    float s = smoothstep(0, wireFrameWidth*0.5, abs(_Radius - d));
                    float4 w = culcWireFrame(i.uv, float4(1, 1, 1, 0.4), 1);
                    wf = lerp(w, c, s);
                }
                c = c * 3 + wf;

                //The part that calculates human occlusion
                float4 stencilCol = tex2D(_OcclusionStencil, i.uv2);
                int isHuman = step(0.1,stencilCol.r);
                c.a *=  (1 - isHuman);

                return c;
            }
            ENDCG
        }
    }
}
