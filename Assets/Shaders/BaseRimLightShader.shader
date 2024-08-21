Shader "Shaders/BaseRimLightShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _BaseMap ("Base Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _MetallicSmoothnessMap ("Metallic Smoothness Map", 2D) = "white" {}
        _OcclusionMap ("Occlusion Map", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (0,0,0,1)
        _EmissionMap ("Emission Map", 2D) = "black" {}
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0.1, 10.0)) = 3.0
        _RimIntensity ("Rim Intensity", Range(0.1, 5.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                float4 tangentOS : TANGENT;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float3 tangentWS : TEXCOORD3;
                float3 bitangentWS : TEXCOORD4;
            };

            float4 _BaseColor;
            float4 _RimColor;
            float _RimPower;
            float _RimIntensity;
            float4 _EmissionColor;
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_MetallicSmoothnessMap);
            SAMPLER(sampler_MetallicSmoothnessMap);
            TEXTURE2D(_OcclusionMap);
            SAMPLER(sampler_OcclusionMap);
            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                float3 positionWS = TransformObjectToWorld(input.positionOS);
                output.positionCS = TransformWorldToHClip(positionWS);
                output.normalWS = normalize(TransformObjectToWorldNormal(input.normalOS));
                output.viewDirWS = normalize(_WorldSpaceCameraPos - positionWS);
                output.uv = input.uv;

                float3 tangentWS = normalize(TransformObjectToWorldDir(input.tangentOS.xyz));
                float3 bitangentWS = cross(output.normalWS, tangentWS) * input.tangentOS.w;
                output.tangentWS = tangentWS;
                output.bitangentWS = bitangentWS;

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                // Sample the base texture
                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;

                // Sample the normal map
                half3 normalMap = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv).xyz * 2.0 - 1.0;
                half3 normalWS = normalize(input.tangentWS * normalMap.x + input.bitangentWS * normalMap.y + input.normalWS * normalMap.z);

                // Sample the metallic smoothness map
                half metallic = SAMPLE_TEXTURE2D(_MetallicSmoothnessMap, sampler_MetallicSmoothnessMap, input.uv).r;
                half smoothness = SAMPLE_TEXTURE2D(_MetallicSmoothnessMap, sampler_MetallicSmoothnessMap, input.uv).a;

                // Sample the occlusion map
                half occlusion = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, input.uv).r;

                // Sample the emission map
                half3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).rgb * _EmissionColor.rgb;

                // Calculate the rim lighting factor
                float rim = 1.0 - saturate(dot(normalWS, input.viewDirWS));
                rim = pow(rim, _RimPower) * _RimIntensity;

                // Combine base color, rim light color, occlusion, and emission
                half3 color = baseColor.rgb * occlusion + _RimColor.rgb * rim + emission;
                return half4(color, baseColor.a);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}