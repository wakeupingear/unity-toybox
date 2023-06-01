Shader "Unlit/2D Flat" {
    SubShader {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Properties {
            CBUFFER_START(UnityPerMaterial)
                _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
            CBUFFER_END

        }
        
        Pass {
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            ENDHLSL

        }
    }
}