// ----------------------------------------------------------------------------
// <copyright file="FastWithRectMaskSupport.shader" company="Supyrb">
//   Copyright (c) 2017 Supyrb. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   send@johannesdeml.com
// </author>
// ----------------------------------------------------------------------------

Shader "UI/FastWithRectMaskSupport"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Happiness("Happiness", Range(0, 1)) = 0.5
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "UnityUI.cginc"

    fixed4 _Color;
    fixed4 _TextureSampleAdd;
    float4 _ClipRect;

    struct appdata_t
    {
        float4 vertex : POSITION;
        float4 color : COLOR;
        float2 texcoord : TEXCOORD0;
    };

    struct v2f
    {
        float4 vertex : SV_POSITION;
        fixed4 color : COLOR;
        half2 texcoord : TEXCOORD0;
        float4 worldPosition : TEXCOORD1;
    };

    void Unity_Clamp_float(float In, float Min, float Max, out float Out)
    {
        Out = clamp(In, Min, Max);
    }

    void Unity_Saturation_float(float3 In, float Saturation, out float3 Out)
    {
        float luma = dot(In, float3(0.2126729, 0.7151522, 0.0721750));
        Out = luma.xxx + Saturation.xxx * (In - luma.xxx);
    }

    void Unity_Combine_float(float R, float G, float B, float A, out float3 RGB)
    {
        RGB = float3(R, G, B);
    }

    v2f vert(appdata_t IN)
    {
        v2f OUT;
        OUT.worldPosition = IN.vertex;
        OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

        OUT.texcoord = IN.texcoord;

        #ifdef UNITY_HALF_TEXEL_OFFSET
        OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
        #endif

        OUT.color = IN.color * _Color;
        return OUT;
    }

    sampler2D _MainTex;
    fixed4 frag(v2f IN) : SV_Target
    {
        half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
        color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

        #ifdef UNITY_UI_ALPHACLIP
        clip (color.a - 0.001);
        #endif

        return color;
    }
    ENDCG

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile __ UNITY_UI_ALPHACLIP
            ENDCG
        }
    }
}