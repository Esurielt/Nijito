// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "UI/UIGlitch"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Speed("Speed", Float) = 12
        _SpeedThreshold("Speed Threshold", Float) = 0.3
        _GlitchSize("Glitch Size", Float) = 10
        _Color("Tint", Color) = (1,1,1,1)

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }

            Cull Off
            Lighting Off
            ZWrite Off
            ZTest[unity_GUIZTestMode]
            Blend One OneMinusSrcAlpha
            ColorMask[_ColorMask]


            Pass
            {
                Name "Default"
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0

                #include "UnityCG.cginc"
                #include "UnityUI.cginc"

                #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
                #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

                struct SurfaceDescriptionInputs
                {
                    float4 uv0;
                    float3 TimeParameters;
                };

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord  : TEXCOORD0;
                    float4 worldPosition : TEXCOORD1;
                    half4  mask : TEXCOORD2;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                sampler2D _MainTex;
                fixed4 _Color;
                fixed4 _TextureSampleAdd;
                float4 _ClipRect;
                float4 _MainTex_ST;
                float _MaskSoftnessX;
                float _MaskSoftnessY;

                float _Speed;
                float _SpeedThreshold;
                float _GlitchSize;
                

                void Unity_Multiply_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }

                void Unity_Posterize_float4(float4 In, float4 Steps, out float4 Out)
                {
                    Out = floor(In / (1 / Steps)) * (1 / Steps);
                }


                inline float Unity_SimpleNoise_RandomValue_float(float2 uv)
                {
                    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
                }

                inline float Unity_SimpleNnoise_Interpolate_float(float a, float b, float t)
                {
                    return (1.0 - t) * a + (t * b);
                }


                inline float Unity_SimpleNoise_ValueNoise_float(float2 uv)
                {
                    float2 i = floor(uv);
                    float2 f = frac(uv);
                    f = f * f * (3.0 - 2.0 * f);

                    uv = abs(frac(uv) - 0.5);
                    float2 c0 = i + float2(0.0, 0.0);
                    float2 c1 = i + float2(1.0, 0.0);
                    float2 c2 = i + float2(0.0, 1.0);
                    float2 c3 = i + float2(1.0, 1.0);
                    float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                    float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                    float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                    float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                    float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                    float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                    float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                    return t;
                }
                void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
                {
                    float t = 0.0;

                    float freq = pow(2.0, float(0));
                    float amp = pow(0.5, float(3 - 0));
                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                    freq = pow(2.0, float(1));
                    amp = pow(0.5, float(3 - 1));
                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                    freq = pow(2.0, float(2));
                    amp = pow(0.5, float(3 - 2));
                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                    Out = t;
                }

                void Unity_Step_float(float Edge, float In, out float Out)
                {
                    Out = step(Edge, In);
                }

                void Unity_Add_float2(float2 A, float2 B, out float2 Out)
                {
                    Out = A + B;
                }


                // Graph Vertex
                // GraphVertex: <None>

                float2 glitchedUV(float2 UV)
                {
                    float4 _UV_88E6EDA3_Out_0 = float4(UV.x,UV.y,1,1);
                    float _Property_C78549F8_Out_0 = _GlitchSize;
                    float _Multiply_F651462E_Out_2;
                    Unity_Multiply_float(2, _Property_C78549F8_Out_0, _Multiply_F651462E_Out_2);
                    float4 _Vector4_847AEF66_Out_0 = float4(_Property_C78549F8_Out_0, _Multiply_F651462E_Out_2, _Property_C78549F8_Out_0, _Property_C78549F8_Out_0);
                    float4 _Posterize_978BFB26_Out_2;
                    Unity_Posterize_float4(_UV_88E6EDA3_Out_0, _Vector4_847AEF66_Out_0, _Posterize_978BFB26_Out_2);
                    float _SimpleNoise_90ED7004_Out_2;
                    Unity_SimpleNoise_float((_Posterize_978BFB26_Out_2[1].xx), 300, _SimpleNoise_90ED7004_Out_2);
                    float _Multiply_4543A1C0_Out_2;
                    Unity_Multiply_float(_SimpleNoise_90ED7004_Out_2, 1, _Multiply_4543A1C0_Out_2);
                    float _Property_9209B32A_Out_0 = _Speed;
                    float _SimpleNoise_BF1C7C7E_Out_2;
                    Unity_SimpleNoise_float(_Time.y, _Property_9209B32A_Out_0, _SimpleNoise_BF1C7C7E_Out_2);
                    float _Property_CBD8EC11_Out_0 = _SpeedThreshold;
                    float _Step_463E6C29_Out_2 = _SimpleNoise_BF1C7C7E_Out_2;
                    Unity_Step_float(_SimpleNoise_BF1C7C7E_Out_2, _Property_CBD8EC11_Out_0, _Step_463E6C29_Out_2);
                    float _Multiply_79F98465_Out_2;
                    Unity_Multiply_float(_Multiply_4543A1C0_Out_2, _Step_463E6C29_Out_2, _Multiply_79F98465_Out_2);
                    float2 _Vector2_ABE6255A_Out_0 = float2(_Multiply_79F98465_Out_2, 0);
                    float2 _Add_C0429CBE_Out_2;
                    Unity_Add_float2(_Vector2_ABE6255A_Out_0, (_UV_88E6EDA3_Out_0.xy), _Add_C0429CBE_Out_2);
                    return _Posterize_978BFB26_Out_2;
                }

                v2f vert(appdata_t v)
                {
                    v2f OUT;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                    float4 vPosition = UnityObjectToClipPos(v.vertex);
                    OUT.worldPosition = v.vertex;
                    OUT.vertex = vPosition;

                    float2 pixelSize = vPosition.w;
                    pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                    float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                    float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                    float2 modifieduv0 = glitchedUV(v.texcoord);
                    OUT.texcoord = float4(modifieduv0.x, modifieduv0.y, maskUV.x, maskUV.y);
                    OUT.mask = half4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + abs(pixelSize.xy)));

                    OUT.color = v.color * _Color;
                    return OUT;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    half4 color = float4(IN.texcoord.x,0,0,1);// (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd)* IN.color;

                    #ifdef UNITY_UI_CLIP_RECT
                    half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                    color.a *= m.x * m.y;
                    #endif

                    #ifdef UNITY_UI_ALPHACLIP
                    clip(color.a - 0.001);
                    #endif

                    color.rgb *= color.a;

                    return color;
                }
                
            ENDCG
            }
        }
}