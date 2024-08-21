// Made with Amplify Shader Editor v1.9.5.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TrainBlazerMaster"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		_Color_Tex("Color_Tex", 2D) = "white" {}
		_Color0("Color 0", Color) = (0.9433962,0.4240571,0.253649,0)
		_Color1("Color 1", Color) = (0.4206537,0.133633,0.8584906,0)
		_Color_Intensity("Color_Intensity", Float) = 1
		_Color_Power("Color_Power", Float) = 1
		_ColorTex_Tile_U("ColorTex_Tile_U", Float) = 1
		_ColorTex_Tile_V("ColorTex_Tile_V", Float) = 1
		_ColorTex_Offset_U("ColorTex_Offset_U", Float) = 0
		_ColorTex_Offset_V("ColorTex_Offset_V", Float) = 0
		_ColorTex_Scroll_U("ColorTex_Scroll_U", Float) = 0
		_ColorTex_Scroll_V("ColorTex_Scroll_V", Float) = 0
		_Alpha_Tex("Alpha_Tex", 2D) = "white" {}
		_Alpha_Intensity("Alpha_Intensity", Float) = 1
		_Alpha_Power("Alpha_Power", Float) = 1
		_AlphaTex_Tile_U("AlphaTex_Tile_U", Float) = 1
		_AlphaTex_Tile_V("AlphaTex_Tile_V", Float) = 1
		_AlphaTex_Offset_U("AlphaTex_Offset_U", Float) = 0
		_AlphaTex_Offset_V("AlphaTex_Offset_V", Float) = 0
		_AlphaTex_Scroll_U("AlphaTex_Scroll_U", Float) = 0
		_AlphaTex_Scroll_V("AlphaTex_Scroll_V", Float) = 0
		_Noise_Tex("Noise_Tex", 2D) = "white" {}
		_Noise_Intensity("Noise_Intensity", Float) = 0
		_NoiseTex_Tile_U("NoiseTex_Tile_U", Float) = 1
		_NoiseTex_Tile_V("NoiseTex_Tile_V", Float) = 1
		_NoiseTex_Offset_U("NoiseTex_Offset_U", Float) = 0
		_NoiseTex_Offset_V("NoiseTex_Offset_V", Float) = 0
		_NoiseTex_Scroll_U("NoiseTex_Scroll_U", Float) = 0
		_NoiseTex_Scroll_V("NoiseTex_Scroll_V", Float) = 0
		_Dissolve_Control("Dissolve_Control", Range( 0 , 1)) = 1
		_Dissolve_Sharpness("Dissolve_Sharpness", Range( 0.0001 , 1)) = 1
		_Dissolve_Tex_Range("Dissolve_Tex_Range", Float) = 0
		[Toggle(_USE_DISSOLVE_ON)] _Use_Dissolve("Use_Dissolve", Float) = 0
		[Toggle(_USE_OFFET_V_ON)] _Use_Offet_V("Use_Offet_V", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)]_Src_BlendMode("Src_BlendMode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)]_Dst_BlendMode("Dst_BlendMode", Float) = 10
		[Enum(UnityEngine.Rendering.CullMode)]_Cull_Mode("Cull_Mode", Float) = 0
		[Enum(Default,2,Always,6)]_Z_TestMode("Z_TestMode", Float) = 2
		_DepthFadeDistance("Depth Fade Distance", Float) = 0
		[Toggle(_USE_DEPTHFADE_ON)] _Use_DepthFade("Use_Depth Fade", Float) = 0
		_DepthFade_Control("DepthFade_Control", Float) = 1
		_Fresnel_Scale("Fresnel_Scale", Float) = 1
		_Fresnel_Power("Fresnel_Power", Float) = 1
		[Toggle(_USE_FRESNEL_ON)] _Use_Fresnel("Use_Fresnel", Float) = 0
		_Wpo_Scale("Wpo_Scale", Float) = 1
		_WPO_Max("WPO_Max", Float) = 1
		_WPO_Min("WPO_Min", Float) = 1
		[Toggle(_WPO_ON)] _WPO("WPO", Float) = 0
		_Gra_Mul("Gra_Mul", Float) = 1
		_Gra_Control("Gra_Control", Float) = 1
		_Gra_Power("Gra_Power", Float) = 1
		[Toggle(_USE_GRA_ON)] _Use_Gra("Use_Gra", Float) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[Toggle(_USE_MASK_ON)] _Use_Mask("Use_Mask?", Float) = 0
		_Mask_Power("Mask_Power", Float) = 1
		_Mask_Intensity("Mask_Intensity", Float) = 1
		_MaskTex_Offset_U("MaskTex_Offset_U", Float) = 0
		_MaskTex_Offset_V("MaskTex_Offset_V", Float) = 0
		_MaskTex_Tile_U("MaskTex_Tile_U", Float) = 1
		_MaskTex_Tile_V("MaskTex_Tile_V", Float) = 1
		_MaskTex_Scroll_U("MaskTex_Scroll_U", Float) = 0
		_MaskTex_Scroll_V("MaskTex_Scroll_V", Float) = 0
		_RGB_Control_A("RGB_Control_A", Float) = 0
		_RGB_Control_B("RGB_Control_B", Float) = 0


		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

		[HideInInspector][ToggleOff] _ReceiveShadows("Receive Shadows", Float) = 1.0
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" "UniversalMaterialType"="Unlit" }

		Cull [_Cull_Mode]
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 5.0
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForwardOnly" }

			Blend [_Src_BlendMode] [_Dst_BlendMode]
			ZWrite Off
			ZTest [_Z_TestMode]
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			

			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140011
			#define REQUIRE_DEPTH_TEXTURE 1


			

			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3

			

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fragment _ DEBUG_DISPLAY

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_UNLIT

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			
			#if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			
			#if ASE_SRP_VERSION >=140010
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#endif
		

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_NORMAL
			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _WPO_ON
			#pragma shader_feature_local _USE_GRA_ON
			#pragma shader_feature_local _USE_FRESNEL_ON
			#pragma shader_feature_local _USE_DEPTHFADE_ON
			#pragma shader_feature_local _USE_MASK_ON
			#pragma shader_feature_local _USE_OFFET_V_ON
			#pragma shader_feature_local _USE_DISSOLVE_ON


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef ASE_FOG
					float fogFactor : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color1;
			float4 _Color0;
			float _Src_BlendMode;
			float _DepthFadeDistance;
			float _DepthFade_Control;
			float _AlphaTex_Scroll_U;
			float _AlphaTex_Scroll_V;
			float _AlphaTex_Tile_U;
			float _AlphaTex_Tile_V;
			float _AlphaTex_Offset_U;
			float _AlphaTex_Offset_V;
			float _RGB_Control_A;
			float _RGB_Control_B;
			float _Alpha_Power;
			float _Alpha_Intensity;
			float _MaskTex_Scroll_U;
			float _MaskTex_Scroll_V;
			float _MaskTex_Tile_U;
			float _MaskTex_Tile_V;
			float _MaskTex_Offset_U;
			float _MaskTex_Offset_V;
			float _Mask_Power;
			float _Mask_Intensity;
			float _Dissolve_Tex_Range;
			float _Fresnel_Power;
			float _Fresnel_Scale;
			float _Gra_Power;
			float _Dissolve_Control;
			float _Dst_BlendMode;
			float _Cull_Mode;
			float _Z_TestMode;
			float _WPO_Min;
			float _WPO_Max;
			float _NoiseTex_Scroll_U;
			float _NoiseTex_Scroll_V;
			float _NoiseTex_Tile_U;
			float _NoiseTex_Tile_V;
			float _NoiseTex_Offset_U;
			float _Gra_Mul;
			float _NoiseTex_Offset_V;
			float _ColorTex_Scroll_U;
			float _ColorTex_Scroll_V;
			float _ColorTex_Tile_U;
			float _ColorTex_Tile_V;
			float _ColorTex_Offset_U;
			float _ColorTex_Offset_V;
			float _Noise_Intensity;
			float _Color_Power;
			float _Color_Intensity;
			float _Gra_Control;
			float _Wpo_Scale;
			float _Dissolve_Sharpness;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Noise_Tex;
			sampler2D _Color_Tex;
			sampler2D _Alpha_Tex;
			sampler2D _TextureSample0;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 temp_cast_0 = (0.0).xxx;
				float2 appendResult71 = (float2(_NoiseTex_Scroll_U , _NoiseTex_Scroll_V));
				float2 texCoord48 = v.ase_texcoord * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult62 = (float2(_NoiseTex_Tile_U , _NoiseTex_Tile_V));
				float2 appendResult61 = (float2(_NoiseTex_Offset_U , _NoiseTex_Offset_V));
				float2 panner69 = ( 1.0 * _Time.y * appendResult71 + ( ( texCoord48 * appendResult62 ) + appendResult61 ));
				float4 tex2DNode44 = tex2Dlod( _Noise_Tex, float4( panner69, 0, 0.0) );
				float lerpResult121 = lerp( _WPO_Min , ( _WPO_Max * v.ase_texcoord1.x ) , saturate( tex2DNode44.r ));
				#ifdef _WPO_ON
				float3 staticSwitch128 = ( lerpResult121 * ( v.normalOS * _Wpo_Scale ) );
				#else
				float3 staticSwitch128 = temp_cast_0;
				#endif
				
				float4 ase_clipPos = TransformObjectToHClip((v.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord4 = screenPos;
				
				o.ase_texcoord3 = v.ase_texcoord;
				o.ase_color = v.ase_color;
				o.ase_normal = v.normalOS;
				o.ase_texcoord5 = v.ase_texcoord2;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = staticSwitch128;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.positionWS = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				#ifdef ASE_FOG
					o.fogFactor = ComputeFogFactor( positionCS.z );
				#endif

				o.positionCS = positionCS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				o.ase_texcoord2 = v.ase_texcoord2;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN
				#ifdef _WRITE_RENDERING_LAYERS
				, out float4 outRenderingLayers : SV_Target1
				#endif
				 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 appendResult23 = (float2(_ColorTex_Scroll_U , _ColorTex_Scroll_V));
				float2 texCoord10 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult18 = (float2(_ColorTex_Tile_U , _ColorTex_Tile_V));
				float2 appendResult21 = (float2(_ColorTex_Offset_U , _ColorTex_Offset_V));
				float2 panner26 = ( 1.0 * _Time.y * appendResult23 + ( ( texCoord10 * appendResult18 ) + appendResult21 ));
				float2 appendResult71 = (float2(_NoiseTex_Scroll_U , _NoiseTex_Scroll_V));
				float2 texCoord48 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult62 = (float2(_NoiseTex_Tile_U , _NoiseTex_Tile_V));
				float2 appendResult61 = (float2(_NoiseTex_Offset_U , _NoiseTex_Offset_V));
				float2 panner69 = ( 1.0 * _Time.y * appendResult71 + ( ( texCoord48 * appendResult62 ) + appendResult61 ));
				float4 tex2DNode44 = tex2D( _Noise_Tex, panner69 );
				float temp_output_59_0 = ( tex2DNode44.r * _Noise_Intensity );
				float4 temp_cast_0 = (_Color_Power).xxxx;
				float4 temp_output_30_0 = ( pow( tex2D( _Color_Tex, ( panner26 + temp_output_59_0 ) ) , temp_cast_0 ) * _Color_Intensity );
				float4x4 break280 = UNITY_MATRIX_I_V;
				float4 appendResult282 = (float4(break280[ 0 ][ 2 ] , break280[ 1 ][ 2 ] , break280[ 2 ][ 2 ] , break280[ 3 ][ 2 ]));
				float3 objToWorldDir281 = mul( GetObjectToWorldMatrix(), float4( IN.ase_normal, 0 ) ).xyz;
				float fresnelNdotV145 = dot( objToWorldDir281, appendResult282.xyz );
				float fresnelNode145 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV145, 5.0 ) );
				float lerpResult144 = lerp( fresnelNode145 , tex2DNode44.r , _Gra_Control);
				float4 lerpResult130 = lerp( _Color0 , _Color1 , saturate( ( pow( lerpResult144 , _Gra_Power ) * _Gra_Mul ) ));
				#ifdef _USE_GRA_ON
				float4 staticSwitch146 = ( lerpResult130 * temp_output_30_0 );
				#else
				float4 staticSwitch146 = ( temp_output_30_0 * IN.ase_color );
				#endif
				
				float4x4 break275 = UNITY_MATRIX_I_V;
				float4 appendResult276 = (float4(break275[ 0 ][ 2 ] , break275[ 1 ][ 2 ] , break275[ 2 ][ 2 ] , break275[ 3 ][ 2 ]));
				float3 objToWorldDir267 = mul( GetObjectToWorldMatrix(), float4( IN.ase_normal, 0 ) ).xyz;
				float fresnelNdotV104 = dot( normalize( objToWorldDir267 ), appendResult276.xyz );
				float fresnelNode104 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV104, 5.0 ) );
				#ifdef _USE_FRESNEL_ON
				float staticSwitch110 = ( pow( fresnelNode104 , _Fresnel_Scale ) * _Fresnel_Power );
				#else
				float staticSwitch110 = 1.0;
				#endif
				float4 screenPos = IN.ase_texcoord4;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth96 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth96 = saturate( abs( ( screenDepth96 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( (0.0 + (_DepthFadeDistance - 0.0) * (1.0 - 0.0) / (10000.0 - 0.0)) ) ) );
				float lerpResult99 = lerp( ( 1.0 - distanceDepth96 ) , distanceDepth96 , _DepthFade_Control);
				#ifdef _USE_DEPTHFADE_ON
				float staticSwitch100 = lerpResult99;
				#else
				float staticSwitch100 = 1.0;
				#endif
				float2 appendResult79 = (float2(_AlphaTex_Scroll_U , _AlphaTex_Scroll_V));
				float2 texCoord75 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult53 = (float2(_AlphaTex_Tile_U , _AlphaTex_Tile_V));
				#ifdef _USE_OFFET_V_ON
				float staticSwitch285 = IN.ase_texcoord5.w;
				#else
				float staticSwitch285 = _AlphaTex_Offset_V;
				#endif
				float2 appendResult56 = (float2(_AlphaTex_Offset_U , staticSwitch285));
				float2 panner52 = ( 1.0 * _Time.y * appendResult79 + ( ( texCoord75 * appendResult53 ) + appendResult56 ));
				float4 tex2DNode57 = tex2D( _Alpha_Tex, ( temp_output_59_0 + panner52 ) );
				float lerpResult184 = lerp( tex2DNode57.r , tex2DNode57.g , _RGB_Control_A);
				float lerpResult187 = lerp( lerpResult184 , tex2DNode57.b , _RGB_Control_B);
				float temp_output_55_0 = ( pow( lerpResult187 , _Alpha_Power ) * _Alpha_Intensity );
				float2 appendResult173 = (float2(_MaskTex_Scroll_U , _MaskTex_Scroll_V));
				float2 texCoord177 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult176 = (float2(_MaskTex_Tile_U , _MaskTex_Tile_V));
				float2 appendResult171 = (float2(_MaskTex_Offset_U , _MaskTex_Offset_V));
				float2 panner167 = ( 1.0 * _Time.y * appendResult173 + ( ( texCoord177 * appendResult176 ) + appendResult171 ));
				#ifdef _USE_MASK_ON
				float staticSwitch164 = ( ( pow( tex2D( _TextureSample0, panner167 ).r , _Mask_Power ) * _Mask_Intensity ) * temp_output_55_0 );
				#else
				float staticSwitch164 = temp_output_55_0;
				#endif
				#ifdef _USE_DISSOLVE_ON
				float staticSwitch81 = IN.ase_texcoord3.z;
				#else
				float staticSwitch81 = _Dissolve_Control;
				#endif
				float lerpResult73 = lerp( _Dissolve_Tex_Range , 1.0 , staticSwitch81);
				float smoothstepResult86 = smoothstep( 0.0 , saturate( tex2DNode44.b ) , lerpResult73);
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = staticSwitch146.rgb;
				float Alpha = saturate( ( staticSwitch110 * ( staticSwitch100 * ( ( IN.ase_color.a * staticSwitch164 ) * ( smoothstepResult86 / _Dissolve_Sharpness ) ) ) ) );
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#if defined(_DBUFFER)
					ApplyDecalToBaseColor(IN.positionCS, Color);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( IN.positionCS );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4( EncodeMeshRenderingLayer( renderingLayers ), 0, 0, 0 );
				#endif

				return half4( Color, Alpha );
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM

			

			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140011
			#define REQUIRE_DEPTH_TEXTURE 1


			

			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_SHADOWCASTER

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature_local _WPO_ON
			#pragma shader_feature_local _USE_FRESNEL_ON
			#pragma shader_feature_local _USE_DEPTHFADE_ON
			#pragma shader_feature_local _USE_MASK_ON
			#pragma shader_feature_local _USE_OFFET_V_ON
			#pragma shader_feature_local _USE_DISSOLVE_ON


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				float3 ase_normal : NORMAL;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color1;
			float4 _Color0;
			float _Src_BlendMode;
			float _DepthFadeDistance;
			float _DepthFade_Control;
			float _AlphaTex_Scroll_U;
			float _AlphaTex_Scroll_V;
			float _AlphaTex_Tile_U;
			float _AlphaTex_Tile_V;
			float _AlphaTex_Offset_U;
			float _AlphaTex_Offset_V;
			float _RGB_Control_A;
			float _RGB_Control_B;
			float _Alpha_Power;
			float _Alpha_Intensity;
			float _MaskTex_Scroll_U;
			float _MaskTex_Scroll_V;
			float _MaskTex_Tile_U;
			float _MaskTex_Tile_V;
			float _MaskTex_Offset_U;
			float _MaskTex_Offset_V;
			float _Mask_Power;
			float _Mask_Intensity;
			float _Dissolve_Tex_Range;
			float _Fresnel_Power;
			float _Fresnel_Scale;
			float _Gra_Power;
			float _Dissolve_Control;
			float _Dst_BlendMode;
			float _Cull_Mode;
			float _Z_TestMode;
			float _WPO_Min;
			float _WPO_Max;
			float _NoiseTex_Scroll_U;
			float _NoiseTex_Scroll_V;
			float _NoiseTex_Tile_U;
			float _NoiseTex_Tile_V;
			float _NoiseTex_Offset_U;
			float _Gra_Mul;
			float _NoiseTex_Offset_V;
			float _ColorTex_Scroll_U;
			float _ColorTex_Scroll_V;
			float _ColorTex_Tile_U;
			float _ColorTex_Tile_V;
			float _ColorTex_Offset_U;
			float _ColorTex_Offset_V;
			float _Noise_Intensity;
			float _Color_Power;
			float _Color_Intensity;
			float _Gra_Control;
			float _Wpo_Scale;
			float _Dissolve_Sharpness;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Noise_Tex;
			sampler2D _Alpha_Tex;
			sampler2D _TextureSample0;


			
			float3 _LightDirection;
			float3 _LightPosition;

			VertexOutput VertexFunction( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float3 temp_cast_0 = (0.0).xxx;
				float2 appendResult71 = (float2(_NoiseTex_Scroll_U , _NoiseTex_Scroll_V));
				float2 texCoord48 = v.ase_texcoord * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult62 = (float2(_NoiseTex_Tile_U , _NoiseTex_Tile_V));
				float2 appendResult61 = (float2(_NoiseTex_Offset_U , _NoiseTex_Offset_V));
				float2 panner69 = ( 1.0 * _Time.y * appendResult71 + ( ( texCoord48 * appendResult62 ) + appendResult61 ));
				float4 tex2DNode44 = tex2Dlod( _Noise_Tex, float4( panner69, 0, 0.0) );
				float lerpResult121 = lerp( _WPO_Min , ( _WPO_Max * v.ase_texcoord1.x ) , saturate( tex2DNode44.r ));
				#ifdef _WPO_ON
				float3 staticSwitch128 = ( lerpResult121 * ( v.normalOS * _Wpo_Scale ) );
				#else
				float3 staticSwitch128 = temp_cast_0;
				#endif
				
				float4 ase_clipPos = TransformObjectToHClip((v.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_normal = v.normalOS;
				o.ase_color = v.ase_color;
				o.ase_texcoord3 = v.ase_texcoord;
				o.ase_texcoord4 = v.ase_texcoord2;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = staticSwitch128;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.positionWS = positionWS;
				#endif

				float3 normalWS = TransformObjectToWorldDir( v.normalOS );

				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif

				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

				#if UNITY_REVERSED_Z
					positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
				#else
					positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.positionCS = positionCS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				o.ase_texcoord2 = v.ase_texcoord2;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4x4 break275 = UNITY_MATRIX_I_V;
				float4 appendResult276 = (float4(break275[ 0 ][ 2 ] , break275[ 1 ][ 2 ] , break275[ 2 ][ 2 ] , break275[ 3 ][ 2 ]));
				float3 objToWorldDir267 = mul( GetObjectToWorldMatrix(), float4( IN.ase_normal, 0 ) ).xyz;
				float fresnelNdotV104 = dot( normalize( objToWorldDir267 ), appendResult276.xyz );
				float fresnelNode104 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV104, 5.0 ) );
				#ifdef _USE_FRESNEL_ON
				float staticSwitch110 = ( pow( fresnelNode104 , _Fresnel_Scale ) * _Fresnel_Power );
				#else
				float staticSwitch110 = 1.0;
				#endif
				float4 screenPos = IN.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth96 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth96 = saturate( abs( ( screenDepth96 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( (0.0 + (_DepthFadeDistance - 0.0) * (1.0 - 0.0) / (10000.0 - 0.0)) ) ) );
				float lerpResult99 = lerp( ( 1.0 - distanceDepth96 ) , distanceDepth96 , _DepthFade_Control);
				#ifdef _USE_DEPTHFADE_ON
				float staticSwitch100 = lerpResult99;
				#else
				float staticSwitch100 = 1.0;
				#endif
				float2 appendResult71 = (float2(_NoiseTex_Scroll_U , _NoiseTex_Scroll_V));
				float2 texCoord48 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult62 = (float2(_NoiseTex_Tile_U , _NoiseTex_Tile_V));
				float2 appendResult61 = (float2(_NoiseTex_Offset_U , _NoiseTex_Offset_V));
				float2 panner69 = ( 1.0 * _Time.y * appendResult71 + ( ( texCoord48 * appendResult62 ) + appendResult61 ));
				float4 tex2DNode44 = tex2D( _Noise_Tex, panner69 );
				float temp_output_59_0 = ( tex2DNode44.r * _Noise_Intensity );
				float2 appendResult79 = (float2(_AlphaTex_Scroll_U , _AlphaTex_Scroll_V));
				float2 texCoord75 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult53 = (float2(_AlphaTex_Tile_U , _AlphaTex_Tile_V));
				#ifdef _USE_OFFET_V_ON
				float staticSwitch285 = IN.ase_texcoord4.w;
				#else
				float staticSwitch285 = _AlphaTex_Offset_V;
				#endif
				float2 appendResult56 = (float2(_AlphaTex_Offset_U , staticSwitch285));
				float2 panner52 = ( 1.0 * _Time.y * appendResult79 + ( ( texCoord75 * appendResult53 ) + appendResult56 ));
				float4 tex2DNode57 = tex2D( _Alpha_Tex, ( temp_output_59_0 + panner52 ) );
				float lerpResult184 = lerp( tex2DNode57.r , tex2DNode57.g , _RGB_Control_A);
				float lerpResult187 = lerp( lerpResult184 , tex2DNode57.b , _RGB_Control_B);
				float temp_output_55_0 = ( pow( lerpResult187 , _Alpha_Power ) * _Alpha_Intensity );
				float2 appendResult173 = (float2(_MaskTex_Scroll_U , _MaskTex_Scroll_V));
				float2 texCoord177 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult176 = (float2(_MaskTex_Tile_U , _MaskTex_Tile_V));
				float2 appendResult171 = (float2(_MaskTex_Offset_U , _MaskTex_Offset_V));
				float2 panner167 = ( 1.0 * _Time.y * appendResult173 + ( ( texCoord177 * appendResult176 ) + appendResult171 ));
				#ifdef _USE_MASK_ON
				float staticSwitch164 = ( ( pow( tex2D( _TextureSample0, panner167 ).r , _Mask_Power ) * _Mask_Intensity ) * temp_output_55_0 );
				#else
				float staticSwitch164 = temp_output_55_0;
				#endif
				#ifdef _USE_DISSOLVE_ON
				float staticSwitch81 = IN.ase_texcoord3.z;
				#else
				float staticSwitch81 = _Dissolve_Control;
				#endif
				float lerpResult73 = lerp( _Dissolve_Tex_Range , 1.0 , staticSwitch81);
				float smoothstepResult86 = smoothstep( 0.0 , saturate( tex2DNode44.b ) , lerpResult73);
				

				float Alpha = saturate( ( staticSwitch110 * ( staticSwitch100 * ( ( IN.ase_color.a * staticSwitch164 ) * ( smoothstepResult86 / _Dissolve_Sharpness ) ) ) ) );
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( IN.positionCS );
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask R
			AlphaToMask Off

			HLSLPROGRAM

			

			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140011
			#define REQUIRE_DEPTH_TEXTURE 1


			

			#pragma vertex vert
			#pragma fragment frag

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature_local _WPO_ON
			#pragma shader_feature_local _USE_FRESNEL_ON
			#pragma shader_feature_local _USE_DEPTHFADE_ON
			#pragma shader_feature_local _USE_MASK_ON
			#pragma shader_feature_local _USE_OFFET_V_ON
			#pragma shader_feature_local _USE_DISSOLVE_ON


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 positionWS : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float3 ase_normal : NORMAL;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color1;
			float4 _Color0;
			float _Src_BlendMode;
			float _DepthFadeDistance;
			float _DepthFade_Control;
			float _AlphaTex_Scroll_U;
			float _AlphaTex_Scroll_V;
			float _AlphaTex_Tile_U;
			float _AlphaTex_Tile_V;
			float _AlphaTex_Offset_U;
			float _AlphaTex_Offset_V;
			float _RGB_Control_A;
			float _RGB_Control_B;
			float _Alpha_Power;
			float _Alpha_Intensity;
			float _MaskTex_Scroll_U;
			float _MaskTex_Scroll_V;
			float _MaskTex_Tile_U;
			float _MaskTex_Tile_V;
			float _MaskTex_Offset_U;
			float _MaskTex_Offset_V;
			float _Mask_Power;
			float _Mask_Intensity;
			float _Dissolve_Tex_Range;
			float _Fresnel_Power;
			float _Fresnel_Scale;
			float _Gra_Power;
			float _Dissolve_Control;
			float _Dst_BlendMode;
			float _Cull_Mode;
			float _Z_TestMode;
			float _WPO_Min;
			float _WPO_Max;
			float _NoiseTex_Scroll_U;
			float _NoiseTex_Scroll_V;
			float _NoiseTex_Tile_U;
			float _NoiseTex_Tile_V;
			float _NoiseTex_Offset_U;
			float _Gra_Mul;
			float _NoiseTex_Offset_V;
			float _ColorTex_Scroll_U;
			float _ColorTex_Scroll_V;
			float _ColorTex_Tile_U;
			float _ColorTex_Tile_V;
			float _ColorTex_Offset_U;
			float _ColorTex_Offset_V;
			float _Noise_Intensity;
			float _Color_Power;
			float _Color_Intensity;
			float _Gra_Control;
			float _Wpo_Scale;
			float _Dissolve_Sharpness;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Noise_Tex;
			sampler2D _Alpha_Tex;
			sampler2D _TextureSample0;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 temp_cast_0 = (0.0).xxx;
				float2 appendResult71 = (float2(_NoiseTex_Scroll_U , _NoiseTex_Scroll_V));
				float2 texCoord48 = v.ase_texcoord * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult62 = (float2(_NoiseTex_Tile_U , _NoiseTex_Tile_V));
				float2 appendResult61 = (float2(_NoiseTex_Offset_U , _NoiseTex_Offset_V));
				float2 panner69 = ( 1.0 * _Time.y * appendResult71 + ( ( texCoord48 * appendResult62 ) + appendResult61 ));
				float4 tex2DNode44 = tex2Dlod( _Noise_Tex, float4( panner69, 0, 0.0) );
				float lerpResult121 = lerp( _WPO_Min , ( _WPO_Max * v.ase_texcoord1.x ) , saturate( tex2DNode44.r ));
				#ifdef _WPO_ON
				float3 staticSwitch128 = ( lerpResult121 * ( v.normalOS * _Wpo_Scale ) );
				#else
				float3 staticSwitch128 = temp_cast_0;
				#endif
				
				float4 ase_clipPos = TransformObjectToHClip((v.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_normal = v.normalOS;
				o.ase_color = v.ase_color;
				o.ase_texcoord3 = v.ase_texcoord;
				o.ase_texcoord4 = v.ase_texcoord2;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = staticSwitch128;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.positionWS = positionWS;
				#endif

				o.positionCS = TransformWorldToHClip( positionWS );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				o.ase_texcoord2 = v.ase_texcoord2;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4x4 break275 = UNITY_MATRIX_I_V;
				float4 appendResult276 = (float4(break275[ 0 ][ 2 ] , break275[ 1 ][ 2 ] , break275[ 2 ][ 2 ] , break275[ 3 ][ 2 ]));
				float3 objToWorldDir267 = mul( GetObjectToWorldMatrix(), float4( IN.ase_normal, 0 ) ).xyz;
				float fresnelNdotV104 = dot( normalize( objToWorldDir267 ), appendResult276.xyz );
				float fresnelNode104 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV104, 5.0 ) );
				#ifdef _USE_FRESNEL_ON
				float staticSwitch110 = ( pow( fresnelNode104 , _Fresnel_Scale ) * _Fresnel_Power );
				#else
				float staticSwitch110 = 1.0;
				#endif
				float4 screenPos = IN.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth96 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth96 = saturate( abs( ( screenDepth96 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( (0.0 + (_DepthFadeDistance - 0.0) * (1.0 - 0.0) / (10000.0 - 0.0)) ) ) );
				float lerpResult99 = lerp( ( 1.0 - distanceDepth96 ) , distanceDepth96 , _DepthFade_Control);
				#ifdef _USE_DEPTHFADE_ON
				float staticSwitch100 = lerpResult99;
				#else
				float staticSwitch100 = 1.0;
				#endif
				float2 appendResult71 = (float2(_NoiseTex_Scroll_U , _NoiseTex_Scroll_V));
				float2 texCoord48 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult62 = (float2(_NoiseTex_Tile_U , _NoiseTex_Tile_V));
				float2 appendResult61 = (float2(_NoiseTex_Offset_U , _NoiseTex_Offset_V));
				float2 panner69 = ( 1.0 * _Time.y * appendResult71 + ( ( texCoord48 * appendResult62 ) + appendResult61 ));
				float4 tex2DNode44 = tex2D( _Noise_Tex, panner69 );
				float temp_output_59_0 = ( tex2DNode44.r * _Noise_Intensity );
				float2 appendResult79 = (float2(_AlphaTex_Scroll_U , _AlphaTex_Scroll_V));
				float2 texCoord75 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult53 = (float2(_AlphaTex_Tile_U , _AlphaTex_Tile_V));
				#ifdef _USE_OFFET_V_ON
				float staticSwitch285 = IN.ase_texcoord4.w;
				#else
				float staticSwitch285 = _AlphaTex_Offset_V;
				#endif
				float2 appendResult56 = (float2(_AlphaTex_Offset_U , staticSwitch285));
				float2 panner52 = ( 1.0 * _Time.y * appendResult79 + ( ( texCoord75 * appendResult53 ) + appendResult56 ));
				float4 tex2DNode57 = tex2D( _Alpha_Tex, ( temp_output_59_0 + panner52 ) );
				float lerpResult184 = lerp( tex2DNode57.r , tex2DNode57.g , _RGB_Control_A);
				float lerpResult187 = lerp( lerpResult184 , tex2DNode57.b , _RGB_Control_B);
				float temp_output_55_0 = ( pow( lerpResult187 , _Alpha_Power ) * _Alpha_Intensity );
				float2 appendResult173 = (float2(_MaskTex_Scroll_U , _MaskTex_Scroll_V));
				float2 texCoord177 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult176 = (float2(_MaskTex_Tile_U , _MaskTex_Tile_V));
				float2 appendResult171 = (float2(_MaskTex_Offset_U , _MaskTex_Offset_V));
				float2 panner167 = ( 1.0 * _Time.y * appendResult173 + ( ( texCoord177 * appendResult176 ) + appendResult171 ));
				#ifdef _USE_MASK_ON
				float staticSwitch164 = ( ( pow( tex2D( _TextureSample0, panner167 ).r , _Mask_Power ) * _Mask_Intensity ) * temp_output_55_0 );
				#else
				float staticSwitch164 = temp_output_55_0;
				#endif
				#ifdef _USE_DISSOLVE_ON
				float staticSwitch81 = IN.ase_texcoord3.z;
				#else
				float staticSwitch81 = _Dissolve_Control;
				#endif
				float lerpResult73 = lerp( _Dissolve_Tex_Range , 1.0 , staticSwitch81);
				float smoothstepResult86 = smoothstep( 0.0 , saturate( tex2DNode44.b ) , lerpResult73);
				

				float Alpha = saturate( ( staticSwitch110 * ( staticSwitch100 * ( ( IN.ase_color.a * staticSwitch164 ) * ( smoothstepResult86 / _Dissolve_Sharpness ) ) ) ) );
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( IN.positionCS );
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "SceneSelectionPass"
			Tags { "LightMode"="SceneSelectionPass" }

			Cull Off
			AlphaToMask Off

			HLSLPROGRAM

			

			#define ASE_FOG 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140011
			#define REQUIRE_DEPTH_TEXTURE 1


			

			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			
			#if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			
			#if ASE_SRP_VERSION >=140010
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#endif
		

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature_local _WPO_ON
			#pragma shader_feature_local _USE_FRESNEL_ON
			#pragma shader_feature_local _USE_DEPTHFADE_ON
			#pragma shader_feature_local _USE_MASK_ON
			#pragma shader_feature_local _USE_OFFET_V_ON
			#pragma shader_feature_local _USE_DISSOLVE_ON


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color1;
			float4 _Color0;
			float _Src_BlendMode;
			float _DepthFadeDistance;
			float _DepthFade_Control;
			float _AlphaTex_Scroll_U;
			float _AlphaTex_Scroll_V;
			float _AlphaTex_Tile_U;
			float _AlphaTex_Tile_V;
			float _AlphaTex_Offset_U;
			float _AlphaTex_Offset_V;
			float _RGB_Control_A;
			float _RGB_Control_B;
			float _Alpha_Power;
			float _Alpha_Intensity;
			float _MaskTex_Scroll_U;
			float _MaskTex_Scroll_V;
			float _MaskTex_Tile_U;
			float _MaskTex_Tile_V;
			float _MaskTex_Offset_U;
			float _MaskTex_Offset_V;
			float _Mask_Power;
			float _Mask_Intensity;
			float _Dissolve_Tex_Range;
			float _Fresnel_Power;
			float _Fresnel_Scale;
			float _Gra_Power;
			float _Dissolve_Control;
			float _Dst_BlendMode;
			float _Cull_Mode;
			float _Z_TestMode;
			float _WPO_Min;
			float _WPO_Max;
			float _NoiseTex_Scroll_U;
			float _NoiseTex_Scroll_V;
			float _NoiseTex_Tile_U;
			float _NoiseTex_Tile_V;
			float _NoiseTex_Offset_U;
			float _Gra_Mul;
			float _NoiseTex_Offset_V;
			float _ColorTex_Scroll_U;
			float _ColorTex_Scroll_V;
			float _ColorTex_Tile_U;
			float _ColorTex_Tile_V;
			float _ColorTex_Offset_U;
			float _ColorTex_Offset_V;
			float _Noise_Intensity;
			float _Color_Power;
			float _Color_Intensity;
			float _Gra_Control;
			float _Wpo_Scale;
			float _Dissolve_Sharpness;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Noise_Tex;
			sampler2D _Alpha_Tex;
			sampler2D _TextureSample0;


			
			int _ObjectId;
			int _PassValue;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 temp_cast_0 = (0.0).xxx;
				float2 appendResult71 = (float2(_NoiseTex_Scroll_U , _NoiseTex_Scroll_V));
				float2 texCoord48 = v.ase_texcoord * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult62 = (float2(_NoiseTex_Tile_U , _NoiseTex_Tile_V));
				float2 appendResult61 = (float2(_NoiseTex_Offset_U , _NoiseTex_Offset_V));
				float2 panner69 = ( 1.0 * _Time.y * appendResult71 + ( ( texCoord48 * appendResult62 ) + appendResult61 ));
				float4 tex2DNode44 = tex2Dlod( _Noise_Tex, float4( panner69, 0, 0.0) );
				float lerpResult121 = lerp( _WPO_Min , ( _WPO_Max * v.ase_texcoord1.x ) , saturate( tex2DNode44.r ));
				#ifdef _WPO_ON
				float3 staticSwitch128 = ( lerpResult121 * ( v.normalOS * _Wpo_Scale ) );
				#else
				float3 staticSwitch128 = temp_cast_0;
				#endif
				
				float4 ase_clipPos = TransformObjectToHClip((v.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord = screenPos;
				
				o.ase_normal = v.normalOS;
				o.ase_color = v.ase_color;
				o.ase_texcoord1 = v.ase_texcoord;
				o.ase_texcoord2 = v.ase_texcoord2;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = staticSwitch128;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );

				o.positionCS = TransformWorldToHClip(positionWS);

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				o.ase_texcoord2 = v.ase_texcoord2;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float4x4 break275 = UNITY_MATRIX_I_V;
				float4 appendResult276 = (float4(break275[ 0 ][ 2 ] , break275[ 1 ][ 2 ] , break275[ 2 ][ 2 ] , break275[ 3 ][ 2 ]));
				float3 objToWorldDir267 = mul( GetObjectToWorldMatrix(), float4( IN.ase_normal, 0 ) ).xyz;
				float fresnelNdotV104 = dot( normalize( objToWorldDir267 ), appendResult276.xyz );
				float fresnelNode104 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV104, 5.0 ) );
				#ifdef _USE_FRESNEL_ON
				float staticSwitch110 = ( pow( fresnelNode104 , _Fresnel_Scale ) * _Fresnel_Power );
				#else
				float staticSwitch110 = 1.0;
				#endif
				float4 screenPos = IN.ase_texcoord;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth96 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth96 = saturate( abs( ( screenDepth96 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( (0.0 + (_DepthFadeDistance - 0.0) * (1.0 - 0.0) / (10000.0 - 0.0)) ) ) );
				float lerpResult99 = lerp( ( 1.0 - distanceDepth96 ) , distanceDepth96 , _DepthFade_Control);
				#ifdef _USE_DEPTHFADE_ON
				float staticSwitch100 = lerpResult99;
				#else
				float staticSwitch100 = 1.0;
				#endif
				float2 appendResult71 = (float2(_NoiseTex_Scroll_U , _NoiseTex_Scroll_V));
				float2 texCoord48 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult62 = (float2(_NoiseTex_Tile_U , _NoiseTex_Tile_V));
				float2 appendResult61 = (float2(_NoiseTex_Offset_U , _NoiseTex_Offset_V));
				float2 panner69 = ( 1.0 * _Time.y * appendResult71 + ( ( texCoord48 * appendResult62 ) + appendResult61 ));
				float4 tex2DNode44 = tex2D( _Noise_Tex, panner69 );
				float temp_output_59_0 = ( tex2DNode44.r * _Noise_Intensity );
				float2 appendResult79 = (float2(_AlphaTex_Scroll_U , _AlphaTex_Scroll_V));
				float2 texCoord75 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult53 = (float2(_AlphaTex_Tile_U , _AlphaTex_Tile_V));
				#ifdef _USE_OFFET_V_ON
				float staticSwitch285 = IN.ase_texcoord2.w;
				#else
				float staticSwitch285 = _AlphaTex_Offset_V;
				#endif
				float2 appendResult56 = (float2(_AlphaTex_Offset_U , staticSwitch285));
				float2 panner52 = ( 1.0 * _Time.y * appendResult79 + ( ( texCoord75 * appendResult53 ) + appendResult56 ));
				float4 tex2DNode57 = tex2D( _Alpha_Tex, ( temp_output_59_0 + panner52 ) );
				float lerpResult184 = lerp( tex2DNode57.r , tex2DNode57.g , _RGB_Control_A);
				float lerpResult187 = lerp( lerpResult184 , tex2DNode57.b , _RGB_Control_B);
				float temp_output_55_0 = ( pow( lerpResult187 , _Alpha_Power ) * _Alpha_Intensity );
				float2 appendResult173 = (float2(_MaskTex_Scroll_U , _MaskTex_Scroll_V));
				float2 texCoord177 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult176 = (float2(_MaskTex_Tile_U , _MaskTex_Tile_V));
				float2 appendResult171 = (float2(_MaskTex_Offset_U , _MaskTex_Offset_V));
				float2 panner167 = ( 1.0 * _Time.y * appendResult173 + ( ( texCoord177 * appendResult176 ) + appendResult171 ));
				#ifdef _USE_MASK_ON
				float staticSwitch164 = ( ( pow( tex2D( _TextureSample0, panner167 ).r , _Mask_Power ) * _Mask_Intensity ) * temp_output_55_0 );
				#else
				float staticSwitch164 = temp_output_55_0;
				#endif
				#ifdef _USE_DISSOLVE_ON
				float staticSwitch81 = IN.ase_texcoord1.z;
				#else
				float staticSwitch81 = _Dissolve_Control;
				#endif
				float lerpResult73 = lerp( _Dissolve_Tex_Range , 1.0 , staticSwitch81);
				float smoothstepResult86 = smoothstep( 0.0 , saturate( tex2DNode44.b ) , lerpResult73);
				

				surfaceDescription.Alpha = saturate( ( staticSwitch110 * ( staticSwitch100 * ( ( IN.ase_color.a * staticSwitch164 ) * ( smoothstepResult86 / _Dissolve_Sharpness ) ) ) ) );
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "ScenePickingPass"
			Tags { "LightMode"="Picking" }

			AlphaToMask Off

			HLSLPROGRAM

			

			#define ASE_FOG 1
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140011
			#define REQUIRE_DEPTH_TEXTURE 1


			

			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT

			#define SHADERPASS SHADERPASS_DEPTHONLY

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			
			#if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			
			#if ASE_SRP_VERSION >=140010
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#endif
		

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature_local _WPO_ON
			#pragma shader_feature_local _USE_FRESNEL_ON
			#pragma shader_feature_local _USE_DEPTHFADE_ON
			#pragma shader_feature_local _USE_MASK_ON
			#pragma shader_feature_local _USE_OFFET_V_ON
			#pragma shader_feature_local _USE_DISSOLVE_ON


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color1;
			float4 _Color0;
			float _Src_BlendMode;
			float _DepthFadeDistance;
			float _DepthFade_Control;
			float _AlphaTex_Scroll_U;
			float _AlphaTex_Scroll_V;
			float _AlphaTex_Tile_U;
			float _AlphaTex_Tile_V;
			float _AlphaTex_Offset_U;
			float _AlphaTex_Offset_V;
			float _RGB_Control_A;
			float _RGB_Control_B;
			float _Alpha_Power;
			float _Alpha_Intensity;
			float _MaskTex_Scroll_U;
			float _MaskTex_Scroll_V;
			float _MaskTex_Tile_U;
			float _MaskTex_Tile_V;
			float _MaskTex_Offset_U;
			float _MaskTex_Offset_V;
			float _Mask_Power;
			float _Mask_Intensity;
			float _Dissolve_Tex_Range;
			float _Fresnel_Power;
			float _Fresnel_Scale;
			float _Gra_Power;
			float _Dissolve_Control;
			float _Dst_BlendMode;
			float _Cull_Mode;
			float _Z_TestMode;
			float _WPO_Min;
			float _WPO_Max;
			float _NoiseTex_Scroll_U;
			float _NoiseTex_Scroll_V;
			float _NoiseTex_Tile_U;
			float _NoiseTex_Tile_V;
			float _NoiseTex_Offset_U;
			float _Gra_Mul;
			float _NoiseTex_Offset_V;
			float _ColorTex_Scroll_U;
			float _ColorTex_Scroll_V;
			float _ColorTex_Tile_U;
			float _ColorTex_Tile_V;
			float _ColorTex_Offset_U;
			float _ColorTex_Offset_V;
			float _Noise_Intensity;
			float _Color_Power;
			float _Color_Intensity;
			float _Gra_Control;
			float _Wpo_Scale;
			float _Dissolve_Sharpness;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Noise_Tex;
			sampler2D _Alpha_Tex;
			sampler2D _TextureSample0;


			
			float4 _SelectionID;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 temp_cast_0 = (0.0).xxx;
				float2 appendResult71 = (float2(_NoiseTex_Scroll_U , _NoiseTex_Scroll_V));
				float2 texCoord48 = v.ase_texcoord * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult62 = (float2(_NoiseTex_Tile_U , _NoiseTex_Tile_V));
				float2 appendResult61 = (float2(_NoiseTex_Offset_U , _NoiseTex_Offset_V));
				float2 panner69 = ( 1.0 * _Time.y * appendResult71 + ( ( texCoord48 * appendResult62 ) + appendResult61 ));
				float4 tex2DNode44 = tex2Dlod( _Noise_Tex, float4( panner69, 0, 0.0) );
				float lerpResult121 = lerp( _WPO_Min , ( _WPO_Max * v.ase_texcoord1.x ) , saturate( tex2DNode44.r ));
				#ifdef _WPO_ON
				float3 staticSwitch128 = ( lerpResult121 * ( v.normalOS * _Wpo_Scale ) );
				#else
				float3 staticSwitch128 = temp_cast_0;
				#endif
				
				float4 ase_clipPos = TransformObjectToHClip((v.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord = screenPos;
				
				o.ase_normal = v.normalOS;
				o.ase_color = v.ase_color;
				o.ase_texcoord1 = v.ase_texcoord;
				o.ase_texcoord2 = v.ase_texcoord2;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = staticSwitch128;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );
				o.positionCS = TransformWorldToHClip(positionWS);
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				o.ase_texcoord2 = v.ase_texcoord2;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float4x4 break275 = UNITY_MATRIX_I_V;
				float4 appendResult276 = (float4(break275[ 0 ][ 2 ] , break275[ 1 ][ 2 ] , break275[ 2 ][ 2 ] , break275[ 3 ][ 2 ]));
				float3 objToWorldDir267 = mul( GetObjectToWorldMatrix(), float4( IN.ase_normal, 0 ) ).xyz;
				float fresnelNdotV104 = dot( normalize( objToWorldDir267 ), appendResult276.xyz );
				float fresnelNode104 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV104, 5.0 ) );
				#ifdef _USE_FRESNEL_ON
				float staticSwitch110 = ( pow( fresnelNode104 , _Fresnel_Scale ) * _Fresnel_Power );
				#else
				float staticSwitch110 = 1.0;
				#endif
				float4 screenPos = IN.ase_texcoord;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth96 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth96 = saturate( abs( ( screenDepth96 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( (0.0 + (_DepthFadeDistance - 0.0) * (1.0 - 0.0) / (10000.0 - 0.0)) ) ) );
				float lerpResult99 = lerp( ( 1.0 - distanceDepth96 ) , distanceDepth96 , _DepthFade_Control);
				#ifdef _USE_DEPTHFADE_ON
				float staticSwitch100 = lerpResult99;
				#else
				float staticSwitch100 = 1.0;
				#endif
				float2 appendResult71 = (float2(_NoiseTex_Scroll_U , _NoiseTex_Scroll_V));
				float2 texCoord48 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult62 = (float2(_NoiseTex_Tile_U , _NoiseTex_Tile_V));
				float2 appendResult61 = (float2(_NoiseTex_Offset_U , _NoiseTex_Offset_V));
				float2 panner69 = ( 1.0 * _Time.y * appendResult71 + ( ( texCoord48 * appendResult62 ) + appendResult61 ));
				float4 tex2DNode44 = tex2D( _Noise_Tex, panner69 );
				float temp_output_59_0 = ( tex2DNode44.r * _Noise_Intensity );
				float2 appendResult79 = (float2(_AlphaTex_Scroll_U , _AlphaTex_Scroll_V));
				float2 texCoord75 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult53 = (float2(_AlphaTex_Tile_U , _AlphaTex_Tile_V));
				#ifdef _USE_OFFET_V_ON
				float staticSwitch285 = IN.ase_texcoord2.w;
				#else
				float staticSwitch285 = _AlphaTex_Offset_V;
				#endif
				float2 appendResult56 = (float2(_AlphaTex_Offset_U , staticSwitch285));
				float2 panner52 = ( 1.0 * _Time.y * appendResult79 + ( ( texCoord75 * appendResult53 ) + appendResult56 ));
				float4 tex2DNode57 = tex2D( _Alpha_Tex, ( temp_output_59_0 + panner52 ) );
				float lerpResult184 = lerp( tex2DNode57.r , tex2DNode57.g , _RGB_Control_A);
				float lerpResult187 = lerp( lerpResult184 , tex2DNode57.b , _RGB_Control_B);
				float temp_output_55_0 = ( pow( lerpResult187 , _Alpha_Power ) * _Alpha_Intensity );
				float2 appendResult173 = (float2(_MaskTex_Scroll_U , _MaskTex_Scroll_V));
				float2 texCoord177 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult176 = (float2(_MaskTex_Tile_U , _MaskTex_Tile_V));
				float2 appendResult171 = (float2(_MaskTex_Offset_U , _MaskTex_Offset_V));
				float2 panner167 = ( 1.0 * _Time.y * appendResult173 + ( ( texCoord177 * appendResult176 ) + appendResult171 ));
				#ifdef _USE_MASK_ON
				float staticSwitch164 = ( ( pow( tex2D( _TextureSample0, panner167 ).r , _Mask_Power ) * _Mask_Intensity ) * temp_output_55_0 );
				#else
				float staticSwitch164 = temp_output_55_0;
				#endif
				#ifdef _USE_DISSOLVE_ON
				float staticSwitch81 = IN.ase_texcoord1.z;
				#else
				float staticSwitch81 = _Dissolve_Control;
				#endif
				float lerpResult73 = lerp( _Dissolve_Tex_Range , 1.0 , staticSwitch81);
				float smoothstepResult86 = smoothstep( 0.0 , saturate( tex2DNode44.b ) , lerpResult73);
				

				surfaceDescription.Alpha = saturate( ( staticSwitch110 * ( staticSwitch100 * ( ( IN.ase_color.a * staticSwitch164 ) * ( smoothstepResult86 / _Dissolve_Sharpness ) ) ) ) );
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;
				outColor = _SelectionID;

				return outColor;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormalsOnly" }

			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

			

        	#pragma multi_compile_instancing
        	#pragma multi_compile _ LOD_FADE_CROSSFADE
        	#define ASE_FOG 1
        	#define _SURFACE_TYPE_TRANSPARENT 1
        	#define ASE_SRP_VERSION 140011
        	#define REQUIRE_DEPTH_TEXTURE 1


			

        	#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

			

			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define VARYINGS_NEED_NORMAL_WS

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			
			#if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			
			#if ASE_SRP_VERSION >=140010
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#endif
		

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            #if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature_local _WPO_ON
			#pragma shader_feature_local _USE_FRESNEL_ON
			#pragma shader_feature_local _USE_DEPTHFADE_ON
			#pragma shader_feature_local _USE_MASK_ON
			#pragma shader_feature_local _USE_OFFET_V_ON
			#pragma shader_feature_local _USE_DISSOLVE_ON


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float3 normalWS : TEXCOORD0;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color1;
			float4 _Color0;
			float _Src_BlendMode;
			float _DepthFadeDistance;
			float _DepthFade_Control;
			float _AlphaTex_Scroll_U;
			float _AlphaTex_Scroll_V;
			float _AlphaTex_Tile_U;
			float _AlphaTex_Tile_V;
			float _AlphaTex_Offset_U;
			float _AlphaTex_Offset_V;
			float _RGB_Control_A;
			float _RGB_Control_B;
			float _Alpha_Power;
			float _Alpha_Intensity;
			float _MaskTex_Scroll_U;
			float _MaskTex_Scroll_V;
			float _MaskTex_Tile_U;
			float _MaskTex_Tile_V;
			float _MaskTex_Offset_U;
			float _MaskTex_Offset_V;
			float _Mask_Power;
			float _Mask_Intensity;
			float _Dissolve_Tex_Range;
			float _Fresnel_Power;
			float _Fresnel_Scale;
			float _Gra_Power;
			float _Dissolve_Control;
			float _Dst_BlendMode;
			float _Cull_Mode;
			float _Z_TestMode;
			float _WPO_Min;
			float _WPO_Max;
			float _NoiseTex_Scroll_U;
			float _NoiseTex_Scroll_V;
			float _NoiseTex_Tile_U;
			float _NoiseTex_Tile_V;
			float _NoiseTex_Offset_U;
			float _Gra_Mul;
			float _NoiseTex_Offset_V;
			float _ColorTex_Scroll_U;
			float _ColorTex_Scroll_V;
			float _ColorTex_Tile_U;
			float _ColorTex_Tile_V;
			float _ColorTex_Offset_U;
			float _ColorTex_Offset_V;
			float _Noise_Intensity;
			float _Color_Power;
			float _Color_Intensity;
			float _Gra_Control;
			float _Wpo_Scale;
			float _Dissolve_Sharpness;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Noise_Tex;
			sampler2D _Alpha_Tex;
			sampler2D _TextureSample0;


			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 temp_cast_0 = (0.0).xxx;
				float2 appendResult71 = (float2(_NoiseTex_Scroll_U , _NoiseTex_Scroll_V));
				float2 texCoord48 = v.ase_texcoord * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult62 = (float2(_NoiseTex_Tile_U , _NoiseTex_Tile_V));
				float2 appendResult61 = (float2(_NoiseTex_Offset_U , _NoiseTex_Offset_V));
				float2 panner69 = ( 1.0 * _Time.y * appendResult71 + ( ( texCoord48 * appendResult62 ) + appendResult61 ));
				float4 tex2DNode44 = tex2Dlod( _Noise_Tex, float4( panner69, 0, 0.0) );
				float lerpResult121 = lerp( _WPO_Min , ( _WPO_Max * v.ase_texcoord1.x ) , saturate( tex2DNode44.r ));
				#ifdef _WPO_ON
				float3 staticSwitch128 = ( lerpResult121 * ( v.normalOS * _Wpo_Scale ) );
				#else
				float3 staticSwitch128 = temp_cast_0;
				#endif
				
				float4 ase_clipPos = TransformObjectToHClip((v.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord1 = screenPos;
				
				o.ase_normal = v.normalOS;
				o.ase_color = v.ase_color;
				o.ase_texcoord2 = v.ase_texcoord;
				o.ase_texcoord3 = v.ase_texcoord2;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = staticSwitch128;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );
				float3 normalWS = TransformObjectToWorldNormal(v.normalOS);

				o.positionCS = TransformWorldToHClip(positionWS);
				o.normalWS.xyz =  normalWS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				o.ase_texcoord2 = v.ase_texcoord2;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			void frag( VertexOutput IN
				, out half4 outNormalWS : SV_Target0
			#ifdef _WRITE_RENDERING_LAYERS
				, out float4 outRenderingLayers : SV_Target1
			#endif
				 )
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float4x4 break275 = UNITY_MATRIX_I_V;
				float4 appendResult276 = (float4(break275[ 0 ][ 2 ] , break275[ 1 ][ 2 ] , break275[ 2 ][ 2 ] , break275[ 3 ][ 2 ]));
				float3 objToWorldDir267 = mul( GetObjectToWorldMatrix(), float4( IN.ase_normal, 0 ) ).xyz;
				float fresnelNdotV104 = dot( normalize( objToWorldDir267 ), appendResult276.xyz );
				float fresnelNode104 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV104, 5.0 ) );
				#ifdef _USE_FRESNEL_ON
				float staticSwitch110 = ( pow( fresnelNode104 , _Fresnel_Scale ) * _Fresnel_Power );
				#else
				float staticSwitch110 = 1.0;
				#endif
				float4 screenPos = IN.ase_texcoord1;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth96 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth96 = saturate( abs( ( screenDepth96 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( (0.0 + (_DepthFadeDistance - 0.0) * (1.0 - 0.0) / (10000.0 - 0.0)) ) ) );
				float lerpResult99 = lerp( ( 1.0 - distanceDepth96 ) , distanceDepth96 , _DepthFade_Control);
				#ifdef _USE_DEPTHFADE_ON
				float staticSwitch100 = lerpResult99;
				#else
				float staticSwitch100 = 1.0;
				#endif
				float2 appendResult71 = (float2(_NoiseTex_Scroll_U , _NoiseTex_Scroll_V));
				float2 texCoord48 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult62 = (float2(_NoiseTex_Tile_U , _NoiseTex_Tile_V));
				float2 appendResult61 = (float2(_NoiseTex_Offset_U , _NoiseTex_Offset_V));
				float2 panner69 = ( 1.0 * _Time.y * appendResult71 + ( ( texCoord48 * appendResult62 ) + appendResult61 ));
				float4 tex2DNode44 = tex2D( _Noise_Tex, panner69 );
				float temp_output_59_0 = ( tex2DNode44.r * _Noise_Intensity );
				float2 appendResult79 = (float2(_AlphaTex_Scroll_U , _AlphaTex_Scroll_V));
				float2 texCoord75 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult53 = (float2(_AlphaTex_Tile_U , _AlphaTex_Tile_V));
				#ifdef _USE_OFFET_V_ON
				float staticSwitch285 = IN.ase_texcoord3.w;
				#else
				float staticSwitch285 = _AlphaTex_Offset_V;
				#endif
				float2 appendResult56 = (float2(_AlphaTex_Offset_U , staticSwitch285));
				float2 panner52 = ( 1.0 * _Time.y * appendResult79 + ( ( texCoord75 * appendResult53 ) + appendResult56 ));
				float4 tex2DNode57 = tex2D( _Alpha_Tex, ( temp_output_59_0 + panner52 ) );
				float lerpResult184 = lerp( tex2DNode57.r , tex2DNode57.g , _RGB_Control_A);
				float lerpResult187 = lerp( lerpResult184 , tex2DNode57.b , _RGB_Control_B);
				float temp_output_55_0 = ( pow( lerpResult187 , _Alpha_Power ) * _Alpha_Intensity );
				float2 appendResult173 = (float2(_MaskTex_Scroll_U , _MaskTex_Scroll_V));
				float2 texCoord177 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult176 = (float2(_MaskTex_Tile_U , _MaskTex_Tile_V));
				float2 appendResult171 = (float2(_MaskTex_Offset_U , _MaskTex_Offset_V));
				float2 panner167 = ( 1.0 * _Time.y * appendResult173 + ( ( texCoord177 * appendResult176 ) + appendResult171 ));
				#ifdef _USE_MASK_ON
				float staticSwitch164 = ( ( pow( tex2D( _TextureSample0, panner167 ).r , _Mask_Power ) * _Mask_Intensity ) * temp_output_55_0 );
				#else
				float staticSwitch164 = temp_output_55_0;
				#endif
				#ifdef _USE_DISSOLVE_ON
				float staticSwitch81 = IN.ase_texcoord2.z;
				#else
				float staticSwitch81 = _Dissolve_Control;
				#endif
				float lerpResult73 = lerp( _Dissolve_Tex_Range , 1.0 , staticSwitch81);
				float smoothstepResult86 = smoothstep( 0.0 , saturate( tex2DNode44.b ) , lerpResult73);
				

				surfaceDescription.Alpha = saturate( ( staticSwitch110 * ( staticSwitch100 * ( ( IN.ase_color.a * staticSwitch164 ) * ( smoothstepResult86 / _Dissolve_Sharpness ) ) ) ) );
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( IN.positionCS );
				#endif

				#if defined(_GBUFFER_NORMALS_OCT)
					float3 normalWS = normalize(IN.normalWS);
					float2 octNormalWS = PackNormalOctQuadEncode(normalWS);           // values between [-1, +1], must use fp32 on some platforms
					float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);   // values between [ 0,  1]
					half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);      // values between [ 0,  1]
					outNormalWS = half4(packedNormalWS, 0.0);
				#else
					float3 normalWS = IN.normalWS;
					outNormalWS = half4(NormalizeNormalPerPixel(normalWS), 0.0);
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
				#endif
			}

			ENDHLSL
		}

	
	}
	
	CustomEditor "UnityEditor.ShaderGraphUnlitGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}
/*ASEBEGIN
Version=19501
Node;AmplifyShaderEditor.CommentaryNode;84;-6496,-432;Inherit;False;3152.278;523.6549;Noise;16;51;74;61;66;65;46;62;50;71;48;60;59;77;43;69;44;Noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-6448,-112;Inherit;False;Property;_NoiseTex_Tile_U;NoiseTex_Tile_U;22;0;Create;True;0;0;0;False;0;False;1;6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-6448,16;Inherit;False;Property;_NoiseTex_Tile_V;NoiseTex_Tile_V;23;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;85;-6547.878,286.3017;Inherit;False;4173.309;1861.142;Alpha;29;68;55;54;64;42;80;52;63;78;72;56;67;79;70;58;53;75;41;57;163;181;183;182;184;187;189;186;284;285;Alpha;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;62;-6192,-112;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-5936,16;Inherit;False;Property;_NoiseTex_Offset_V;NoiseTex_Offset_V;25;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-5936,-112;Inherit;False;Property;_NoiseTex_Offset_U;NoiseTex_Offset_U;24;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;48;-6448,-368;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;61;-5680,-112;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-5424,-112;Inherit;False;Property;_NoiseTex_Scroll_U;NoiseTex_Scroll_U;26;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-5424,16;Inherit;False;Property;_NoiseTex_Scroll_V;NoiseTex_Scroll_V;27;0;Create;True;0;0;0;False;0;False;0;0.85;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-6560,784;Inherit;False;Property;_AlphaTex_Tile_U;AlphaTex_Tile_U;14;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-6528,976;Inherit;False;Property;_AlphaTex_Tile_V;AlphaTex_Tile_V;15;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-5936,-368;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-5985.876,720.3015;Inherit;False;Property;_AlphaTex_Offset_V;AlphaTex_Offset_V;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;284;-6160,1072;Inherit;True;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;58;-5985.876,592.3016;Inherit;False;Property;_AlphaTex_Offset_U;AlphaTex_Offset_U;16;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;60;-5424,-368;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;71;-5168,-112;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;53;-6241.877,592.3016;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;75;-6497.878,336.3017;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;285;-5776,1040;Inherit;False;Property;_Use_Offet_V;Use_Offet_V;32;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;56;-5729.877,592.3016;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;80;-5473.877,720.3015;Inherit;False;Property;_AlphaTex_Scroll_V;AlphaTex_Scroll_V;19;0;Create;True;0;0;0;False;0;False;0;1.26;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;69;-4912,-368;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-5985.876,336.3017;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-5472,592;Inherit;False;Property;_AlphaTex_Scroll_U;AlphaTex_Scroll_U;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;79;-5217.877,592.3016;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;44;-4592,-336;Inherit;True;Property;_Noise_Tex;Noise_Tex;20;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleAddOpNode;64;-5473.877,336.3017;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;77;-3664,-16;Inherit;False;Property;_Noise_Intensity;Noise_Intensity;21;0;Create;True;0;0;0;False;0;False;0;-0.19;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;174;-5776,2992;Inherit;False;Property;_MaskTex_Tile_V;MaskTex_Tile_V;58;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;168;-5776,2864;Inherit;False;Property;_MaskTex_Tile_U;MaskTex_Tile_U;57;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-3888,-304;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;52;-4961.877,336.3017;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;41;-4560,416;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;172;-5264,2992;Inherit;False;Property;_MaskTex_Offset_V;MaskTex_Offset_V;56;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;177;-5776,2608;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;176;-5520,2864;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;175;-5264,2864;Inherit;False;Property;_MaskTex_Offset_U;MaskTex_Offset_U;55;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;186;-5104,960;Inherit;False;257;166;Comment;1;185;R=0 G=1;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;171;-5008,2864;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;170;-4752,2864;Inherit;False;Property;_MaskTex_Scroll_U;MaskTex_Scroll_U;59;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;169;-5264,2608;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;178;-4752,2992;Inherit;False;Property;_MaskTex_Scroll_V;MaskTex_Scroll_V;60;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;57;-4272,352;Inherit;True;Property;_Alpha_Tex;Alpha_Tex;11;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.CommentaryNode;189;-5072,1280;Inherit;False;257;166;RorG=0 B=1;1;188;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;173;-4496,2864;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;179;-4752,2608;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;183;-3904,576;Inherit;True;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;181;-3904,352;Inherit;True;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;185;-5088,1008;Inherit;False;Property;_RGB_Control_A;RGB_Control_A;61;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;167;-4240,2608;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;182;-3904,800;Inherit;True;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;184;-3600,352;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;188;-5024,1328;Inherit;False;Property;_RGB_Control_B;RGB_Control_B;62;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;113;-3088,-864;Inherit;False;2084.38;700.4745;Fresnel;11;111;115;114;109;104;108;276;267;275;272;265;Fresnel;0,0.3452382,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;37;-405.1981,1484.277;Inherit;False;1720.179;1026.643;Dissolve;10;88;87;86;45;76;47;83;73;81;49;Dissolve;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;159;-3664,2432;Inherit;True;Property;_TextureSample0;Texture Sample 0;51;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;166;-3328,2624;Inherit;False;Property;_Mask_Power;Mask_Power;53;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-3440,704;Inherit;False;Property;_Alpha_Power;Alpha_Power;13;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;187;-3376,352;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;103;-2123.591,101.3874;Inherit;False;1201.373;480.5209;Depth Fade;8;102;98;100;95;97;96;99;283;Depth Fade;0,0.3377483,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-3275.909,721.7024;Inherit;False;Property;_Alpha_Intensity;Alpha_Intensity;12;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-3104,2656;Inherit;False;Property;_Mask_Intensity;Mask_Intensity;54;0;Create;True;0;0;0;False;0;False;1;1.55;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;165;-3136,2464;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;42;-3104,352;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-2096,304;Inherit;False;Property;_DepthFadeDistance;Depth Fade Distance;37;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.InverseViewMatrixNode;272;-3088,-704;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-336,1840;Inherit;False;Property;_Dissolve_Control;Dissolve_Control;28;0;Create;True;0;0;0;False;0;False;1;0.96;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;49;-355.6036,1978.335;Inherit;True;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;45;-355.6036,1594.334;Inherit;False;Property;_Dissolve_Tex_Range;Dissolve_Tex_Range;30;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-355.6036,1722.334;Inherit;False;Constant;_Float0;Float 0;29;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;161;-2800,2512;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-2832,352;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;283;-1984,400;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;10000;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;265;-2688,-784;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;275;-2848,-704;Inherit;False;FLOAT4x4;1;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.StaticSwitch;81;28.39538,1850.335;Inherit;False;Property;_Use_Dissolve;Use_Dissolve;31;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;102;-1759.295,415.9078;Inherit;False;284;166;1;1;101;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;83;-88.55496,2118.625;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;96;-1834.852,286.745;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;-2592,608;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;73;176,1552;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TransformDirectionNode;267;-2464,-784;Inherit;False;Object;World;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;276;-2608,-592;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;127;-473.9324,-43.20628;Inherit;False;1137.778;711.6151;WPO;12;123;158;116;128;122;129;121;119;124;156;120;126;WPO;0,0.3448277,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;88;542.0842,1850.162;Inherit;False;Property;_Dissolve_Sharpness;Dissolve_Sharpness;29;0;Create;True;0;0;0;False;0;False;1;0.88;0.0001;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;97;-1619.904,152.7386;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;91;-1054.028,-1144.579;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;101;-1709.295,465.9079;Inherit;False;Property;_DepthFade_Control;DepthFade_Control;39;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;86;544,1600;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;164;-1003.146,1313.492;Inherit;False;Property;_Use_Mask;Use_Mask?;52;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;104;-2176,-736;Inherit;True;Standard;WorldNormal;ViewDir;True;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;108;-1936,-304;Inherit;False;Property;_Fresnel_Scale;Fresnel_Scale;40;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;126;-297.1532,197.4737;Inherit;False;Property;_WPO_Max;WPO_Max;44;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;99;-1362.573,278.6441;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-677,1297;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-1399.542,151.3871;Inherit;False;Constant;_Float1;Float 1;37;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;277;-3104,-96;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;87;816,1536;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;114;-1680,-688;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;109;-1616,-320;Inherit;False;Property;_Fresnel_Power;Fresnel_Power;41;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;158;-416,304;Inherit;True;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;123;-393.3506,-2.984496;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;120;-298.3896,552.4794;Inherit;False;Property;_Wpo_Scale;Wpo_Scale;43;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-180.5201,-3.005164;Inherit;False;Property;_WPO_Min;WPO_Min;45;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-485.0031,1298.753;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;100;-1157.217,235.4303;Inherit;False;Property;_Use_DepthFade;Use_Depth Fade;38;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-1648,-800;Inherit;False;Constant;_Float2;Float 2;41;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;156;-159.0155,255.2473;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;116;-167.898,380.2019;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;-1424,-688;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;121;-6.730499,125.6904;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;110;-960,-768;Inherit;False;Property;_Use_Fresnel;Use_Fresnel;42;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-640,-512;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;48,464;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;36;-4568.845,-1887.57;Inherit;False;3546.594;564.0267;Color;19;32;27;23;20;10;26;28;31;18;25;11;15;29;13;30;22;19;24;21;Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;148;-2847.013,-3519.135;Inherit;False;2855.342;976.9148;Color;13;137;136;145;143;131;144;139;140;130;134;33;132;149;Color Gra;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;159.9762,146.6521;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;112;-384,-512;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;129;275.2186,18.13389;Inherit;False;Constant;_Float3;Float 3;45;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-4518.845,-1453.569;Inherit;False;Property;_ColorTex_Tile_V;ColorTex_Tile_V;6;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-4518.845,-1581.569;Inherit;False;Property;_ColorTex_Tile_U;ColorTex_Tile_U;5;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;18;-4262.843,-1581.569;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-4006.847,-1453.569;Inherit;False;Property;_ColorTex_Offset_V;ColorTex_Offset_V;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-4006.847,-1581.569;Inherit;False;Property;_ColorTex_Offset_U;ColorTex_Offset_U;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;21;-3750.847,-1581.569;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-4006.847,-1837.57;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-3494.847,-1581.569;Inherit;False;Property;_ColorTex_Scroll_U;ColorTex_Scroll_U;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-3494.847,-1453.569;Inherit;False;Property;_ColorTex_Scroll_V;ColorTex_Scroll_V;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;23;-3238.847,-1581.569;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-3494.847,-1837.57;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;26;-2982.847,-1837.57;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;131;-990.1985,-3134.218;Inherit;False;Property;_Color1;Color 1;2;0;Create;True;0;0;0;False;0;False;0.4206537,0.133633,0.8584906,0;0.4206536,0.1336329,0.8584906,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;132;-2304.113,-2804.169;Inherit;False;Property;_Gra_Control;Gra_Control;48;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;139;-1792,-3328;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;144;-2176,-3328;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;134;-2797.013,-2767.22;Inherit;True;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;128;451.0571,79.75725;Inherit;False;Property;_WPO;WPO;46;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-344.6567,-1904.189;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;33;-985.8628,-3355.393;Inherit;False;Property;_Color0;Color 0;1;0;Create;True;0;0;0;False;0;False;0.9433962,0.4240571,0.253649,0;0.9433962,0.424057,0.2536489,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SaturateNode;143;-911.4664,-2841.28;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;140;-1536,-3328;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;149;-226.8254,-3053.474;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-1534.912,-2857.729;Inherit;False;Property;_Gra_Mul;Gra_Mul;47;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;136;-1790.912,-2857.729;Inherit;False;Property;_Gra_Power;Gra_Power;49;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;130;-648.6078,-3241.07;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.InverseViewMatrixNode;278;-3979.562,-3448.859;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.NormalVertexDataNode;279;-3579.562,-3528.859;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;280;-3739.562,-3448.859;Inherit;False;FLOAT4x4;1;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TransformDirectionNode;281;-3355.562,-3528.859;Inherit;False;Object;World;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;282;-3499.562,-3336.859;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1200,-1824;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-1248,-1680;Inherit;False;Property;_Color_Intensity;Color_Intensity;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;29;-1424,-1824;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1472,-1680;Inherit;False;Property;_Color_Power;Color_Power;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-2726.847,-1581.569;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;27;-2470.847,-1837.57;Inherit;True;Property;_Color_Tex;Color_Tex;0;0;Create;True;0;0;0;True;0;False;-1;None;fb1417fceb13efd49a26500e993f809b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.FresnelNode;145;-2720.771,-3345.943;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-4518.845,-1837.57;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;146;64,-3024;Inherit;True;Property;_Use_Gra;Use_Gra;50;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;90;-128,-512;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;2068.168,111.2293;Inherit;False;Property;_Src_BlendMode;Src_BlendMode;33;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;2068.168,239.2294;Inherit;False;Property;_Dst_BlendMode;Dst_BlendMode;34;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;2068.168,367.2294;Inherit;False;Property;_Cull_Mode;Cull_Mode;35;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;92;2068.168,495.2295;Inherit;False;Property;_Z_TestMode;Z_TestMode;36;1;[Enum];Create;True;0;2;Default;2;Always;6;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;201;2048,-480;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;203;2048,-480;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;204;2048,-480;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;True;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;205;2048,-480;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;206;2048,-480;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;207;2048,-480;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;SceneSelectionPass;0;6;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;208;2048,-480;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ScenePickingPass;0;7;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;209;2048,-480;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormals;0;8;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;210;2048,-480;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormalsOnly;0;9;DepthNormalsOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;True;9;d3d11;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;202;2048,-480;Float;False;True;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;TrainBlazerMaster;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;2;True;_Cull_Mode;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Unlit;True;7;True;12;all;0;True;True;1;5;True;_Src_BlendMode;10;True;_Dst_BlendMode;0;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;3;True;_Z_TestMode;True;True;0;False;;0;False;;True;1;LightMode=UniversalForwardOnly;False;False;0;;0;0;Standard;22;Surface;1;638530648090467903;  Blend;0;638530799383313825;Two Sided;0;638530647988166175;Forward Only;0;0;Cast Shadows;1;0;  Use Shadow Threshold;0;0;Receive Shadows;1;0;GPU Instancing;1;0;LOD CrossFade;1;0;Built-in Fog;1;0;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Vertex Position,InvertActionOnDeselection;1;0;0;10;False;True;True;True;False;False;True;True;True;False;False;;False;0
WireConnection;62;0;50;0
WireConnection;62;1;51;0
WireConnection;61;0;65;0
WireConnection;61;1;66;0
WireConnection;46;0;48;0
WireConnection;46;1;62;0
WireConnection;60;0;46;0
WireConnection;60;1;61;0
WireConnection;71;0;74;0
WireConnection;71;1;43;0
WireConnection;53;0;63;0
WireConnection;53;1;70;0
WireConnection;285;1;67;0
WireConnection;285;0;284;4
WireConnection;56;0;58;0
WireConnection;56;1;285;0
WireConnection;69;0;60;0
WireConnection;69;2;71;0
WireConnection;78;0;75;0
WireConnection;78;1;53;0
WireConnection;79;0;72;0
WireConnection;79;1;80;0
WireConnection;44;1;69;0
WireConnection;64;0;78;0
WireConnection;64;1;56;0
WireConnection;59;0;44;1
WireConnection;59;1;77;0
WireConnection;52;0;64;0
WireConnection;52;2;79;0
WireConnection;41;0;59;0
WireConnection;41;1;52;0
WireConnection;176;0;168;0
WireConnection;176;1;174;0
WireConnection;171;0;175;0
WireConnection;171;1;172;0
WireConnection;169;0;177;0
WireConnection;169;1;176;0
WireConnection;57;1;41;0
WireConnection;173;0;170;0
WireConnection;173;1;178;0
WireConnection;179;0;169;0
WireConnection;179;1;171;0
WireConnection;183;0;57;2
WireConnection;181;0;57;1
WireConnection;167;0;179;0
WireConnection;167;2;173;0
WireConnection;182;0;57;3
WireConnection;184;0;181;0
WireConnection;184;1;183;0
WireConnection;184;2;185;0
WireConnection;159;1;167;0
WireConnection;187;0;184;0
WireConnection;187;1;182;0
WireConnection;187;2;188;0
WireConnection;165;0;159;1
WireConnection;165;1;166;0
WireConnection;42;0;187;0
WireConnection;42;1;54;0
WireConnection;161;0;165;0
WireConnection;161;1;162;0
WireConnection;55;0;42;0
WireConnection;55;1;68;0
WireConnection;283;0;95;0
WireConnection;275;0;272;0
WireConnection;81;1;47;0
WireConnection;81;0;49;3
WireConnection;83;0;44;3
WireConnection;96;0;283;0
WireConnection;163;0;161;0
WireConnection;163;1;55;0
WireConnection;73;0;45;0
WireConnection;73;1;76;0
WireConnection;73;2;81;0
WireConnection;267;0;265;0
WireConnection;276;0;275;2
WireConnection;276;1;275;6
WireConnection;276;2;275;10
WireConnection;276;3;275;14
WireConnection;97;0;96;0
WireConnection;86;0;73;0
WireConnection;86;2;83;0
WireConnection;164;1;55;0
WireConnection;164;0;163;0
WireConnection;104;0;267;0
WireConnection;104;4;276;0
WireConnection;99;0;97;0
WireConnection;99;1;96;0
WireConnection;99;2;101;0
WireConnection;40;0;91;4
WireConnection;40;1;164;0
WireConnection;277;0;44;1
WireConnection;87;0;86;0
WireConnection;87;1;88;0
WireConnection;114;0;104;0
WireConnection;114;1;108;0
WireConnection;123;0;277;0
WireConnection;89;0;40;0
WireConnection;89;1;87;0
WireConnection;100;1;98;0
WireConnection;100;0;99;0
WireConnection;156;0;126;0
WireConnection;156;1;158;1
WireConnection;115;0;114;0
WireConnection;115;1;109;0
WireConnection;121;0;124;0
WireConnection;121;1;156;0
WireConnection;121;2;123;0
WireConnection;110;1;111;0
WireConnection;110;0;115;0
WireConnection;93;0;100;0
WireConnection;93;1;89;0
WireConnection;119;0;116;0
WireConnection;119;1;120;0
WireConnection;122;0;121;0
WireConnection;122;1;119;0
WireConnection;112;0;110;0
WireConnection;112;1;93;0
WireConnection;18;0;13;0
WireConnection;18;1;15;0
WireConnection;21;0;19;0
WireConnection;21;1;20;0
WireConnection;11;0;10;0
WireConnection;11;1;18;0
WireConnection;23;0;25;0
WireConnection;23;1;24;0
WireConnection;22;0;11;0
WireConnection;22;1;21;0
WireConnection;26;0;22;0
WireConnection;26;2;23;0
WireConnection;139;0;144;0
WireConnection;139;1;136;0
WireConnection;144;0;145;0
WireConnection;144;1;134;0
WireConnection;144;2;132;0
WireConnection;134;0;44;1
WireConnection;128;1;129;0
WireConnection;128;0;122;0
WireConnection;35;0;30;0
WireConnection;35;1;91;0
WireConnection;143;0;140;0
WireConnection;140;0;139;0
WireConnection;140;1;137;0
WireConnection;149;0;130;0
WireConnection;149;1;30;0
WireConnection;130;0;33;0
WireConnection;130;1;131;0
WireConnection;130;2;143;0
WireConnection;280;0;278;0
WireConnection;281;0;279;0
WireConnection;282;0;280;2
WireConnection;282;1;280;6
WireConnection;282;2;280;10
WireConnection;282;3;280;14
WireConnection;30;0;29;0
WireConnection;30;1;31;0
WireConnection;29;0;27;0
WireConnection;29;1;32;0
WireConnection;28;0;26;0
WireConnection;28;1;59;0
WireConnection;27;1;28;0
WireConnection;145;0;281;0
WireConnection;145;4;282;0
WireConnection;146;1;35;0
WireConnection;146;0;149;0
WireConnection;90;0;112;0
WireConnection;202;2;146;0
WireConnection;202;3;90;0
WireConnection;202;5;128;0
ASEEND*/
//CHKSM=B908DCDFA90A4CD5FE5019B562B84B6EC2D9F69C