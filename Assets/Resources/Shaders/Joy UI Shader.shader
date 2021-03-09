Shader "Joy Shaders/Joy UI Shader (DEPRECATED)"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
        _Happiness("_Happiness", Range(0, 1)) = 0.5

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Pass
        {
            Name "Pass"
            Tags
            {
                // LightMode: <None>
            }

            // Render State
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            ZTest [unity_GUIZTestMode]
            ZWrite Off
            // ColorMask: <None>


            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma shader_feature _ _SAMPLE_GI
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _AlphaClip 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define SHADERPASS_UNLIT

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float _Happiness;
            CBUFFER_END
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;
            SAMPLER(_SampleTexture2D_1473C187_Sampler_3_Linear_Repeat);

            // Graph Functions

            void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }

            void Unity_Clamp_float(float In, float Min, float Max, out float Out)
            {
                Out = clamp(In, Min, Max);
            }

            void Unity_Saturation_float(float3 In, float Saturation, out float3 Out)
            {
                float luma = dot(In, float3(0.2126729, 0.7151522, 0.0721750));
                Out = luma.xxx + Saturation.xxx * (In - luma.xxx);
            }

            void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
            {
                RGBA = float4(R, G, B, A);
                RGB = float3(R, G, B);
                RG = float2(R, G);
            }

            // Graph Vertex
            // GraphVertex: <None>

            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float4 uv0;
                float4 VertexColor;
            };

            struct SurfaceDescription
            {
                float3 Color;
                float Alpha;
                float AlphaClipThreshold;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _SampleTexture2D_1473C187_RGBA_0 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv0.xy);
                float _SampleTexture2D_1473C187_R_4 = _SampleTexture2D_1473C187_RGBA_0.r;
                float _SampleTexture2D_1473C187_G_5 = _SampleTexture2D_1473C187_RGBA_0.g;
                float _SampleTexture2D_1473C187_B_6 = _SampleTexture2D_1473C187_RGBA_0.b;
                float _SampleTexture2D_1473C187_A_7 = _SampleTexture2D_1473C187_RGBA_0.a;
                float4 _Multiply_70C2D85D_Out_2;
                Unity_Multiply_float(_SampleTexture2D_1473C187_RGBA_0, IN.VertexColor, _Multiply_70C2D85D_Out_2);
                float _Split_4C4CB1DE_R_1 = _Multiply_70C2D85D_Out_2[0];
                float _Split_4C4CB1DE_G_2 = _Multiply_70C2D85D_Out_2[1];
                float _Split_4C4CB1DE_B_3 = _Multiply_70C2D85D_Out_2[2];
                float _Split_4C4CB1DE_A_4 = _Multiply_70C2D85D_Out_2[3];
                float3 _Vector3_C7FDC040_Out_0 = float3(_Split_4C4CB1DE_R_1, _Split_4C4CB1DE_G_2, _Split_4C4CB1DE_B_3);
                float _Property_7B7B37A_Out_0 = _Happiness;
                float _Clamp_57DD97F8_Out_3;
                Unity_Clamp_float(_Property_7B7B37A_Out_0, 0, 1, _Clamp_57DD97F8_Out_3);
                float3 _Saturation_78F8BEA9_Out_2;
                Unity_Saturation_float(_Vector3_C7FDC040_Out_0, _Clamp_57DD97F8_Out_3, _Saturation_78F8BEA9_Out_2);
                float _Split_935E3B1B_R_1 = _Saturation_78F8BEA9_Out_2[0];
                float _Split_935E3B1B_G_2 = _Saturation_78F8BEA9_Out_2[1];
                float _Split_935E3B1B_B_3 = _Saturation_78F8BEA9_Out_2[2];
                float _Split_935E3B1B_A_4 = 0;
                float4 _Combine_A8D255DC_RGBA_4;
                float3 _Combine_A8D255DC_RGB_5;
                float2 _Combine_A8D255DC_RG_6;
                Unity_Combine_float(_Split_935E3B1B_R_1, _Split_935E3B1B_G_2, _Split_935E3B1B_B_3, 1,
                                    _Combine_A8D255DC_RGBA_4, _Combine_A8D255DC_RGB_5, _Combine_A8D255DC_RG_6);
                surface.Color = _Combine_A8D255DC_RGB_5;
                surface.Alpha = _Split_4C4CB1DE_A_4;
                surface.AlphaClipThreshold = 0.5;
                return surface;
            }

            // --------------------------------------------------
            // Structs and Packing

            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 color : COLOR;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0;
                float4 color;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                float4 interp00 : TEXCOORD0;
                float4 interp01 : TEXCOORD1;
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output = (PackedVaryings)0;
                output.positionCS = input.positionCS;
                output.interp00.xyzw = input.texCoord0;
                output.interp01.xyzw = input.color;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }

            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = input.positionCS;
                output.texCoord0 = input.interp00.xyzw;
                output.color = input.interp01.xyzw;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }

            // --------------------------------------------------
            // Build Graph Inputs

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);


                output.uv0 = input.texCoord0;
                output.VertexColor = input.color;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // Render State
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Back
            ZTest LEqual
            ZWrite On
            ColorMask 0


            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _AlphaClip 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define SHADERPASS_DEPTHONLY

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float _Happiness;
            CBUFFER_END
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;
            SAMPLER(_SampleTexture2D_1473C187_Sampler_3_Linear_Repeat);

            // Graph Functions

            void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }

            // Graph Vertex
            // GraphVertex: <None>

            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float4 uv0;
                float4 VertexColor;
            };

            struct SurfaceDescription
            {
                float Alpha;
                float AlphaClipThreshold;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _SampleTexture2D_1473C187_RGBA_0 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv0.xy);
                float _SampleTexture2D_1473C187_R_4 = _SampleTexture2D_1473C187_RGBA_0.r;
                float _SampleTexture2D_1473C187_G_5 = _SampleTexture2D_1473C187_RGBA_0.g;
                float _SampleTexture2D_1473C187_B_6 = _SampleTexture2D_1473C187_RGBA_0.b;
                float _SampleTexture2D_1473C187_A_7 = _SampleTexture2D_1473C187_RGBA_0.a;
                float4 _Multiply_70C2D85D_Out_2;
                Unity_Multiply_float(_SampleTexture2D_1473C187_RGBA_0, IN.VertexColor, _Multiply_70C2D85D_Out_2);
                float _Split_4C4CB1DE_R_1 = _Multiply_70C2D85D_Out_2[0];
                float _Split_4C4CB1DE_G_2 = _Multiply_70C2D85D_Out_2[1];
                float _Split_4C4CB1DE_B_3 = _Multiply_70C2D85D_Out_2[2];
                float _Split_4C4CB1DE_A_4 = _Multiply_70C2D85D_Out_2[3];
                surface.Alpha = _Split_4C4CB1DE_A_4;
                surface.AlphaClipThreshold = 0.5;
                return surface;
            }

            // --------------------------------------------------
            // Structs and Packing

            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 color : COLOR;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0;
                float4 color;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                float4 interp00 : TEXCOORD0;
                float4 interp01 : TEXCOORD1;
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output = (PackedVaryings)0;
                output.positionCS = input.positionCS;
                output.interp00.xyzw = input.texCoord0;
                output.interp01.xyzw = input.color;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }

            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = input.positionCS;
                output.texCoord0 = input.interp00.xyzw;
                output.color = input.interp01.xyzw;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }

            // --------------------------------------------------
            // Build Graph Inputs

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);


                output.uv0 = input.texCoord0;
                output.VertexColor = input.color;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
            ENDHLSL
        }

    }
    FallBack "Hidden/Shader Graph/FallbackError"
}