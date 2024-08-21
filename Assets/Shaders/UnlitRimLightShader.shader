Shader "Shaders/UnlitRimLightShader"
{
    Properties 
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {} // Normal map texture
        _EmissionMap("Emission Map", 2D) = "black" {} // Emission map texture
        _OcclusionMap("Occlusion Map", 2D) = "white" {} // Occlusion map texture
        [HDR]_EmissionColor("Emission Color", Color) = (1, 1, 1, 1) // HDR attribute added
        _RimPower("Rim Power", Range(0.01, 0.1)) = 0.1
        _RimInten("Rim Intensity", Range(0.01, 100)) = 1
        _RimSpeed("Rim Speed", Range(0.1, 10.0)) = 1.0 // Rim animation speed control
        [HDR]_RimColor("Rim Color", color) = (1, 1, 1, 1)

        _Metallic("Metallic", Range(0, 1)) = 0 // Metallic smoothness
    } 

    SubShader
    {    
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"          
            "Queue"="Geometry"        
        }
        Pass
        {
            Name "Universal Forward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;            
                float3 WorldSpaceViewDirection : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            float _RimPower, _RimInten, _RimSpeed; // Add _RimSpeed variable
            half4 _RimColor;    
            sampler2D _MainTex;
            sampler2D _NormalMap; // Sampler for normal map texture
            sampler2D _EmissionMap;
            sampler2D _OcclusionMap;
            half4 _EmissionColor;

            VertexOutput vert(VertexInput v)
            {        
                VertexOutput o;      
                o.vertex = TransformObjectToHClip(v.vertex.xyz);                      
                o.normal = TransformObjectToWorldNormal(v.normal);
                o.WorldSpaceViewDirection = normalize(_WorldSpaceCameraPos.xyz - TransformObjectToWorld(v.vertex.xyz));
                o.uv = v.uv;
                return o;
            }                    

            half4 frag(VertexOutput i) : SV_Target
            {        
                half4 texColor = tex2D(_MainTex, i.uv);
                half4 normalMap = tex2D(_NormalMap, i.uv); // Sample normal map texture
                half4 emission = tex2D(_EmissionMap, i.uv) * _EmissionColor;
                half4 occlusion = tex2D(_OcclusionMap, i.uv); // Sample occlusion map texture

                half3 light = _MainLightPosition.xyz;
                half4 color = texColor + emission; // Add emission to texture color

                half3 ambient = SampleSH(i.normal);
                half face = saturate(dot(i.WorldSpaceViewDirection, i.normal));
                half3 rim = 1.0 - (pow(face, _RimPower));

                // Calculate rim animation based on time
                float rimTime = _Time.y * _RimSpeed;
                float sinRimTime = sin(rimTime) * 0.5 + 0.5; // Normalize to range [0, 1]

                color.rgb *= saturate(dot(i.normal, light)) * _MainLightColor.rgb + ambient;
                color.rgb += rim * _RimInten * _RimColor.rgb * sinRimTime; // Apply rim animation
                //color.rgb += (1.0 - rim) * _RimInten * _RimColor.rgb; // Invert the rim effect
                return color;
            }
            ENDHLSL  
        }
    }
}
