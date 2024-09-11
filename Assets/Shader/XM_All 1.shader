// Upgrade NOTE: upgraded instancing buffer 'XMAll' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "XM/All"
{
	Properties
	{
		[Enum(blend,10,add,1)]_Float1("材质模式", Float) = 10
		[Toggle]_Float4("深度写入", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)]_Ztestmode("深度测试", Float) = 4
		[Enum(UnityEngine.Rendering.CullMode)]_Float2("双面模式", Float) = 0
		[HDR]_Color0("颜色", Color) = (1,1,1,1)
		_Float14("整体颜色强度", Float) = 1
		_Float15("整体透明度", Range( 0 , 1)) = 1
		[Header(depthfade)]_Float16("软粒子（羽化边缘）", Float) = 0
		[Toggle]_Float5("反向软粒子(强化边缘）", Float) = 0
		_Float28("边缘强度", Float) = 1
		_Float30("边缘收窄", Float) = 1
		[Toggle][Header(Fresnel)]_Float33("菲尼尔开关", Float) = 0
		_power3("菲尼尔强度", Float) = 1
		_Float19("菲尼尔范围", Float) = 1
		[Toggle]_Float20("反向菲尼尔（虚化边缘）", Float) = 0
		[Header(___________________________________________________________________________________________________________________________________________________________________________________________________________________________________________)][Header(Main)]_maintex("主贴图", 2D) = "white" {}
		[KeywordEnum(R,A)] _Keyword1("主贴图通道", Float) = 1
		_Float35("主贴图细节提亮", Float) = 1
		_Float34("主贴图细节对比度", Float) = 1
		_Float36("细节平滑过度", Float) = 1
		_MainTex_U("MainTex_U", Float) = 0
		_MainTex_V("MainTex_V", Float) = 0
		_Gradienttex("混合颜色贴图", 2D) = "white" {}
		_Gradienttex_U("Gradienttex_U", Float) = 0
		_Gradienttex_V("Gradienttex_V", Float) = 0
		_Float29("颜色混合", Range( 0 , 1)) = 0
		[Header(___________________________________________________________________________________________________________________________________________________________________________________________________________________________________________)][Header(Mask)]_Mask("遮罩01", 2D) = "white" {}
		[KeywordEnum(R,A)] _Keyword0("遮罩01通道", Float) = 1
		Mask01_U("Mask01_U", Float) = 0
		Mask01_V("Mask01_V", Float) = 0
		_Mask1("遮罩02", 2D) = "white" {}
		[KeywordEnum(R,A)] _Keyword2("遮罩02通道", Float) = 1
		Mask02_U("Mask02_U", Float) = 0
		_Mask02_V("Mask02_V", Float) = 0
		_Mask3("遮罩03", 2D) = "white" {}
		[KeywordEnum(R,A)] _Keyword4("遮罩03通道", Float) = 1
		Mask03_U("Mask03_U", Float) = 0
		_Mask03_V("Mask03_V", Float) = 0
		[Header(___________________________________________________________________________________________________________________________________________________________________________________________________________________________________________)][Header(dissolove)]_DissolveTex("_DissolveTex", 2D) = "white" {}
		_DissolveTex_U("DissolveTex_U", Float) = 0
		_DissolveTex_V("DissolveTex_V", Float) = 0
		[KeywordEnum(up,down,left,right,off)] _Keyword7("溶解方向", Float) = 4
		_Float18("溶解方向强度", Float) = 1
		_Float6("溶解", Range( 0 , 1)) = 0
		_Float8("软硬", Range( 0.5 , 1)) = 0.5
		[KeywordEnum(on,off)] _Keyword5("亮边溶解", Float) = 1
		_Float17("亮边宽度", Range( 0 , 0.1)) = 0
		[HDR]_Color1("亮边颜色", Color) = (1,1,1,1)
		[Header(___________________________________________________________________________________________________________________________________________________________________________________________________________________________________________)][Header(Noise)]Distortion_Tex("Distortion_Tex", 2D) = "white" {}
		_Distortion_U("Distortion_U", Float) = 0
		_Distortion_V("Distortion_V", Float) = 0
		_flowmaptex("flowmaptex", 2D) = "white" {}
		_Float9("扰动", Range( 0 , 1)) = 0
		_Float32("flowmap扰动", Range( 0 , 1)) = 0
		[Toggle]_Float0("扰动影响mask", Float) = 0
		[Toggle]_Float13("扰动影响溶解", Float) = 0
		[Header(___________________________________________________________________________________________________________________________________________________________________________________________________________________________________________)][Header(Vertex_offset)]_vertextex("顶点偏移贴图", 2D) = "white" {}
		_VertexOffsetIntensity("VertexOffset Intensity", Float) = 0
		_VertexOffset_U("VertexOffset_U", Float) = 0
		_VertexOffset_V("VertexOffset_V", Float) = 0
		_VertexOffsetMaskTex("VertexOffset MaskTex", 2D) = "white" {}
		_VertexOffsetMaskTex_U("VertexOffset MaskTex_U", Float) = 0
		_VertexOffsetMaskTex_V("VertexOffset MaskTex_V", Float) = 0
		_VertexOffsetMaskTex2("VertexOffset MaskTex2", 2D) = "white" {}
		_VertexOffsetMaskTex_U2("VertexOffset MaskTex_U2", Float) = 0
		_VertexOffsetMaskTex_V2("VertexOffset MaskTex_V2", Float) = 0
		[Header(___________________________________________________________________________________________________________________________________________________________________________________________________________________________________________)][Header(Ref)]_reftex(" 折射（热扭曲）贴图", 2D) = "white" {}
		[KeywordEnum(on,off)] _Keyword3("折射（热扭曲）开关", Float) = 1
		_Refraction_U("Refraction_U", Float) = 0
		_Refraction_V("Refraction_V", Float) = 0
		_Float23("折射（热扭曲）强度", Float) = 0
		[Toggle][Header(___________________________________________________________________________________________________________________________________________________________________________________________________________________________________________)][Header(Custom)]_Float10("custom1xy控制主贴图偏移", Float) = 0
		[Toggle]_Float12("custom1zw控制mask01偏移", Float) = 0
		[Toggle]_Float11("custom2x控制溶解", Float) = 0
		[Toggle]_Float31("custom2y控制flowmap扭曲", Float) = 0
		[Toggle]_Float24("custom2z控制折射（热扭曲）", Float) = 0
		[Toggle]_Float22("custom2w控制顶点偏移强度", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" "Queue"="Transparent" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha [_Float1]
		AlphaToMask Off
		Cull [_Float2]
		ColorMask RGBA
		ZWrite [_Float4]
		ZTest [_Ztestmode]
		
		
		//GrabPass{ }

		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
			#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
			#else
			#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
			#endif


			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#include "UnityStandardBRDF.cginc"
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _KEYWORD3_ON _KEYWORD3_OFF
			#pragma shader_feature_local _KEYWORD7_UP _KEYWORD7_DOWN _KEYWORD7_LEFT _KEYWORD7_RIGHT _KEYWORD7_OFF
			#pragma shader_feature_local _KEYWORD1_R _KEYWORD1_A
			#pragma shader_feature_local _KEYWORD5_ON _KEYWORD5_OFF
			#pragma shader_feature_local _KEYWORD0_R _KEYWORD0_A
			#pragma shader_feature_local _KEYWORD2_R _KEYWORD2_A
			#pragma shader_feature_local _KEYWORD4_R _KEYWORD4_A


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_color : COLOR;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float _Float1;
			uniform float _Ztestmode;
			uniform float _Float4;
			uniform float _Float2;
			uniform sampler2D _vertextex;
			uniform float _VertexOffset_U;
			uniform float _VertexOffset_V;
			uniform sampler2D _VertexOffsetMaskTex;
			uniform float _VertexOffsetMaskTex_U;
			uniform float _VertexOffsetMaskTex_V;
			uniform sampler2D _VertexOffsetMaskTex2;
			uniform float _VertexOffsetMaskTex_U2;
			uniform float _VertexOffsetMaskTex_V2;
			uniform float _VertexOffsetIntensity;
			uniform float _Float22;
			uniform float _Float28;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform float _Float16;
			uniform float _Float5;
			uniform float _Float30;
			uniform sampler2D _maintex;
			uniform float _MainTex_U;
			uniform float _MainTex_V;
			uniform float _Float10;
			uniform sampler2D Distortion_Tex;
			uniform float _Distortion_U;
			uniform float _Distortion_V;
			uniform float _Float9;
			uniform sampler2D _flowmaptex;
			uniform float _Float32;
			uniform float _Float31;
			uniform sampler2D _Gradienttex;
			uniform float _Gradienttex_U;
			uniform float _Gradienttex_V;
			uniform float _Float29;
			uniform float4 _Color0;
			uniform float _Float14;
			uniform float _power3;
			uniform float _Float19;
			uniform float _Float20;
			uniform float _Float33;
			uniform float4 _Color1;
			uniform float _Float6;
			uniform float _Float11;
			uniform float _Float18;
			uniform sampler2D _DissolveTex;
			uniform float _DissolveTex_U;
			uniform float _DissolveTex_V;
			uniform float _Float13;
			uniform float _Float17;
			ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
			uniform sampler2D _reftex;
			uniform float _Refraction_V;
			uniform float _Float23;
			uniform float _Float24;
			uniform float _Float34;
			uniform float _Float35;
			uniform float _Float36;
			uniform float _Float8;
			uniform float _Float15;
			uniform sampler2D _Mask;
			uniform float Mask01_U;
			uniform float Mask01_V;
			uniform float _Float12;
			uniform float _Float0;
			uniform sampler2D _Mask1;
			uniform float Mask02_U;
			uniform float _Mask02_V;
			uniform sampler2D _Mask3;
			uniform float Mask03_U;
			uniform float _Mask03_V;
			UNITY_INSTANCING_BUFFER_START(XMAll)
				UNITY_DEFINE_INSTANCED_PROP(float4, _vertextex_ST)
#define _vertextex_ST_arr XMAll
				UNITY_DEFINE_INSTANCED_PROP(float4, _VertexOffsetMaskTex_ST)
#define _VertexOffsetMaskTex_ST_arr XMAll
				UNITY_DEFINE_INSTANCED_PROP(float4, _VertexOffsetMaskTex2_ST)
#define _VertexOffsetMaskTex2_ST_arr XMAll
				UNITY_DEFINE_INSTANCED_PROP(float4, _maintex_ST)
#define _maintex_ST_arr XMAll
				UNITY_DEFINE_INSTANCED_PROP(float4, Distortion_Tex_ST)
#define Distortion_Tex_ST_arr XMAll
				UNITY_DEFINE_INSTANCED_PROP(float4, _flowmaptex_ST)
#define _flowmaptex_ST_arr XMAll
				UNITY_DEFINE_INSTANCED_PROP(float4, _Gradienttex_ST)
#define _Gradienttex_ST_arr XMAll
				UNITY_DEFINE_INSTANCED_PROP(float4, _DissolveTex_ST)
#define _DissolveTex_ST_arr XMAll
				UNITY_DEFINE_INSTANCED_PROP(float4, _reftex_ST)
#define _reftex_ST_arr XMAll
				UNITY_DEFINE_INSTANCED_PROP(float4, _Mask_ST)
#define _Mask_ST_arr XMAll
				UNITY_DEFINE_INSTANCED_PROP(float4, _Mask1_ST)
#define _Mask1_ST_arr XMAll
				UNITY_DEFINE_INSTANCED_PROP(float4, _Mask3_ST)
#define _Mask3_ST_arr XMAll
				UNITY_DEFINE_INSTANCED_PROP(float, _Refraction_U)
#define _Refraction_U_arr XMAll
			UNITY_INSTANCING_BUFFER_END(XMAll)
			inline float4 ASE_ComputeGrabScreenPos( float4 pos )
			{
				#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
				#else
				float scale = 1.0;
				#endif
				float4 o = pos;
				o.y = pos.w * 0.5f;
				o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
				return o;
			}
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float2 appendResult390 = (float2(_VertexOffset_U , _VertexOffset_V));
				float4 _vertextex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_vertextex_ST_arr, _vertextex_ST);
				float2 uv_vertextex = v.ase_texcoord.xy * _vertextex_ST_Instance.xy + _vertextex_ST_Instance.zw;
				float2 panner168 = ( 1.0 * _Time.y * appendResult390 + uv_vertextex);
				float2 appendResult356 = (float2(_VertexOffsetMaskTex_U , _VertexOffsetMaskTex_V));
				float4 _VertexOffsetMaskTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_VertexOffsetMaskTex_ST_arr, _VertexOffsetMaskTex_ST);
				float2 uv_VertexOffsetMaskTex = v.ase_texcoord.xy * _VertexOffsetMaskTex_ST_Instance.xy + _VertexOffsetMaskTex_ST_Instance.zw;
				float2 panner358 = ( _Time.y * appendResult356 + uv_VertexOffsetMaskTex);
				float2 appendResult364 = (float2(_VertexOffsetMaskTex_U2 , _VertexOffsetMaskTex_V2));
				float4 _VertexOffsetMaskTex2_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_VertexOffsetMaskTex2_ST_arr, _VertexOffsetMaskTex2_ST);
				float2 uv_VertexOffsetMaskTex2 = v.ase_texcoord.xy * _VertexOffsetMaskTex2_ST_Instance.xy + _VertexOffsetMaskTex2_ST_Instance.zw;
				float2 panner366 = ( _Time.y * appendResult364 + uv_VertexOffsetMaskTex2);
				float4 texCoord167 = v.ase_texcoord2;
				texCoord167.xy = v.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float lerpResult176 = lerp( 1.0 , texCoord167.w , _Float22);
				float4 vertexoffset181 = ( ( tex2Dlod( _vertextex, float4( panner168, 0, 0.0) ).r * tex2Dlod( _VertexOffsetMaskTex, float4( panner358, 0, 0.0) ) * tex2Dlod( _VertexOffsetMaskTex2, float4( panner366, 0, 0.0) ) ) * float4( v.ase_normal , 0.0 ) * _VertexOffsetIntensity * lerpResult176 );
				
				float3 vertexPos97 = v.vertex.xyz;
				float4 ase_clipPos97 = UnityObjectToClipPos(vertexPos97);
				float4 screenPos97 = ComputeScreenPos(ase_clipPos97);
				o.ase_texcoord1 = screenPos97;
				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord5.xyz = ase_worldNormal;
				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord6 = screenPos;
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				o.ase_texcoord3 = v.ase_texcoord1;
				o.ase_texcoord4 = v.ase_texcoord2;
				o.ase_color = v.color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				o.ase_texcoord5.w = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexoffset181.rgb;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i , half ase_vface : VFACE) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float4 screenPos97 = i.ase_texcoord1;
				float4 ase_screenPosNorm97 = screenPos97 / screenPos97.w;
				ase_screenPosNorm97.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm97.z : ase_screenPosNorm97.z * 0.5 + 0.5;
				float screenDepth97 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm97.xy ));
				float distanceDepth97 = saturate( abs( ( screenDepth97 - LinearEyeDepth( ase_screenPosNorm97.z ) ) / ( _Float16 ) ) );
				float depthfade_switch334 = _Float5;
				float lerpResult336 = lerp( distanceDepth97 , ( 1.0 - distanceDepth97 ) , depthfade_switch334);
				float depthfade126 = lerpResult336;
				float lerpResult330 = lerp( 0.0 , depthfade126 , depthfade_switch334);
				float2 appendResult377 = (float2(_MainTex_U , _MainTex_V));
				float4 _maintex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_maintex_ST_arr, _maintex_ST);
				float2 uv_maintex = i.ase_texcoord2.xy * _maintex_ST_Instance.xy + _maintex_ST_Instance.zw;
				float2 panner36 = ( 1.0 * _Time.y * appendResult377 + uv_maintex);
				float4 texCoord39 = i.ase_texcoord3;
				texCoord39.xy = i.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult42 = (float2(texCoord39.x , texCoord39.y));
				float2 lerpResult59 = lerp( panner36 , ( uv_maintex + appendResult42 ) , _Float10);
				float2 maintexUV161 = lerpResult59;
				float2 appendResult386 = (float2(_Distortion_U , _Distortion_V));
				float4 Distortion_Tex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(Distortion_Tex_ST_arr, Distortion_Tex_ST);
				float2 uvDistortion_Tex = i.ase_texcoord2.xy * Distortion_Tex_ST_Instance.xy + Distortion_Tex_ST_Instance.zw;
				float2 panner53 = ( 1.0 * _Time.y * appendResult386 + uvDistortion_Tex);
				float noise70 = tex2D( Distortion_Tex, panner53 ).r;
				float noise_intensity67 = _Float9;
				float4 _flowmaptex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_flowmaptex_ST_arr, _flowmaptex_ST);
				float2 uv_flowmaptex = i.ase_texcoord2.xy * _flowmaptex_ST_Instance.xy + _flowmaptex_ST_Instance.zw;
				float4 tex2DNode241 = tex2D( _flowmaptex, uv_flowmaptex );
				float2 appendResult242 = (float2(tex2DNode241.r , tex2DNode241.g));
				float2 flowmap285 = appendResult242;
				float flowmap_intensity311 = _Float32;
				float4 texCoord100 = i.ase_texcoord4;
				texCoord100.xy = i.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float flpwmap_custom_switch316 = _Float31;
				float lerpResult99 = lerp( flowmap_intensity311 , texCoord100.y , flpwmap_custom_switch316);
				float2 lerpResult283 = lerp( ( maintexUV161 + ( noise70 * noise_intensity67 ) ) , flowmap285 , lerpResult99);
				float4 tex2DNode1 = tex2D( _maintex, lerpResult283 );
				float2 appendResult380 = (float2(_Gradienttex_U , _Gradienttex_V));
				float4 _Gradienttex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_Gradienttex_ST_arr, _Gradienttex_ST);
				float2 uv_Gradienttex = i.ase_texcoord2.xy * _Gradienttex_ST_Instance.xy + _Gradienttex_ST_Instance.zw;
				float2 panner229 = ( 1.0 * _Time.y * appendResult380 + uv_Gradienttex);
				float2 Gradienttex231 = panner229;
				float2 temp_cast_0 = (noise70).xx;
				float2 lerpResult235 = lerp( Gradienttex231 , temp_cast_0 , noise_intensity67);
				float4 lerpResult211 = lerp( tex2DNode1 , tex2D( _Gradienttex, lerpResult235 ) , _Float29);
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(WorldPosition);
				ase_worldViewDir = Unity_SafeNormalize( ase_worldViewDir );
				float3 ase_worldNormal = i.ase_texcoord5.xyz;
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float fresnelNdotV139 = dot( normalize( ( normalizedWorldNormal * ase_vface ) ), ase_worldViewDir );
				float fresnelNode139 = ( 0.0 + _power3 * pow( max( 1.0 - fresnelNdotV139 , 0.0001 ), _Float19 ) );
				float temp_output_140_0 = saturate( fresnelNode139 );
				float lerpResult144 = lerp( temp_output_140_0 , ( 1.0 - temp_output_140_0 ) , _Float20);
				float fresnel147 = lerpResult144;
				float lerpResult347 = lerp( 1.0 , fresnel147 , _Float33);
				float4 texCoord49 = i.ase_texcoord4;
				texCoord49.xy = i.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float lerpResult62 = lerp( _Float6 , texCoord49.x , _Float11);
				float2 texCoord263 = i.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord264 = i.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord266 = i.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord262 = i.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				#if defined(_KEYWORD7_UP)
				float staticSwitch272 = ( 1.0 - saturate( texCoord263.y ) );
				#elif defined(_KEYWORD7_DOWN)
				float staticSwitch272 = saturate( texCoord264.y );
				#elif defined(_KEYWORD7_LEFT)
				float staticSwitch272 = saturate( texCoord266.x );
				#elif defined(_KEYWORD7_RIGHT)
				float staticSwitch272 = ( 1.0 - saturate( texCoord262.x ) );
				#elif defined(_KEYWORD7_OFF)
				float staticSwitch272 = 1.0;
				#else
				float staticSwitch272 = 1.0;
				#endif
				float dis_direction277 = pow( staticSwitch272 , _Float18 );
				float2 appendResult383 = (float2(_DissolveTex_U , _DissolveTex_V));
				float4 _DissolveTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_DissolveTex_ST_arr, _DissolveTex_ST);
				float2 uv_DissolveTex = i.ase_texcoord2.xy * _DissolveTex_ST_Instance.xy + _DissolveTex_ST_Instance.zw;
				float2 panner58 = ( 1.0 * _Time.y * appendResult383 + uv_DissolveTex);
				float2 temp_cast_1 = (noise70).xx;
				float2 lerpResult308 = lerp( panner58 , temp_cast_1 , noise_intensity67);
				float4 texCoord303 = i.ase_texcoord4;
				texCoord303.xy = i.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float lerpResult307 = lerp( flowmap_intensity311 , texCoord303.y , flpwmap_custom_switch316);
				float2 lerpResult309 = lerp( lerpResult308 , flowmap285 , lerpResult307);
				float2 lerpResult89 = lerp( panner58 , lerpResult309 , _Float13);
				float2 dissolveUV92 = lerpResult89;
				float temp_output_280_0 = ( dis_direction277 * tex2D( _DissolveTex, dissolveUV92 ).r );
				float temp_output_130_0 = (0.0 + (temp_output_280_0 - -0.5) * (1.0 - 0.0) / (1.5 - -0.5));
				float temp_output_105_0 = step( lerpResult62 , temp_output_130_0 );
				float dis_edge133 = ( temp_output_105_0 - step( ( lerpResult62 + _Float17 ) , temp_output_130_0 ) );
				float4 lerpResult131 = lerp( ( ( ( _Float28 * pow( lerpResult330 , _Float30 ) ) + lerpResult211 ) * i.ase_color * _Color0 * _Float14 * lerpResult347 ) , _Color1 , dis_edge133);
				float4 screenPos = i.ase_texcoord6;
				float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( screenPos );
				float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
				float _Refraction_U_Instance = UNITY_ACCESS_INSTANCED_PROP(_Refraction_U_arr, _Refraction_U);
				float2 appendResult393 = (float2(_Refraction_U_Instance , _Refraction_V));
				float4 _reftex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_reftex_ST_arr, _reftex_ST);
				float2 uv_reftex = i.ase_texcoord2.xy * _reftex_ST_Instance.xy + _reftex_ST_Instance.zw;
				float2 panner188 = ( 1.0 * _Time.y * appendResult393 + uv_reftex);
				float4 texCoord194 = i.ase_texcoord4;
				texCoord194.xy = i.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float lerpResult193 = lerp( _Float23 , texCoord194.z , _Float24);
				float4 screenColor183 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPosNorm + ( tex2D( _reftex, panner188 ).r * ( 0.1 * lerpResult193 ) ) ).xy);
				float4 ref196 = screenColor183;
				#if defined(_KEYWORD3_ON)
				float4 staticSwitch237 = ref196;
				#elif defined(_KEYWORD3_OFF)
				float4 staticSwitch237 = lerpResult131;
				#else
				float4 staticSwitch237 = lerpResult131;
				#endif
				#if defined(_KEYWORD1_R)
				float staticSwitch14 = tex2DNode1.r;
				#elif defined(_KEYWORD1_A)
				float staticSwitch14 = tex2DNode1.a;
				#else
				float staticSwitch14 = tex2DNode1.a;
				#endif
				float lerpResult414 = lerp( staticSwitch14 , ( pow( staticSwitch14 , _Float34 ) * _Float35 ) , _Float36);
				float smoothstepResult32 = smoothstep( ( 1.0 - _Float8 ) , _Float8 , saturate( ( ( temp_output_280_0 + 1.0 ) - ( lerpResult62 * 2.0 ) ) ));
				float dis_soft122 = smoothstepResult32;
				float dis_bright124 = temp_output_105_0;
				#if defined(_KEYWORD5_ON)
				float staticSwitch239 = dis_bright124;
				#elif defined(_KEYWORD5_OFF)
				float staticSwitch239 = dis_soft122;
				#else
				float staticSwitch239 = dis_soft122;
				#endif
				float lerpResult338 = lerp( depthfade126 , 1.0 , depthfade_switch334);
				float2 appendResult371 = (float2(Mask01_U , Mask01_V));
				float4 _Mask_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_Mask_ST_arr, _Mask_ST);
				float2 uv_Mask = i.ase_texcoord2.xy * _Mask_ST_Instance.xy + _Mask_ST_Instance.zw;
				float2 panner79 = ( 1.0 * _Time.y * appendResult371 + uv_Mask);
				float4 texCoord74 = i.ase_texcoord3;
				texCoord74.xy = i.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult75 = (float2(texCoord74.z , texCoord74.w));
				float2 lerpResult80 = lerp( panner79 , ( uv_Mask + appendResult75 ) , _Float12);
				float lerpResult325 = lerp( 0.0 , ( noise70 * noise_intensity67 ) , _Float0);
				float4 tex2DNode8 = tex2D( _Mask, ( lerpResult80 + lerpResult325 ) );
				#if defined(_KEYWORD0_R)
				float staticSwitch11 = tex2DNode8.r;
				#elif defined(_KEYWORD0_A)
				float staticSwitch11 = tex2DNode8.a;
				#else
				float staticSwitch11 = tex2DNode8.a;
				#endif
				float2 appendResult374 = (float2(Mask02_U , _Mask02_V));
				float4 _Mask1_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_Mask1_ST_arr, _Mask1_ST);
				float2 uv_Mask1 = i.ase_texcoord2.xy * _Mask1_ST_Instance.xy + _Mask1_ST_Instance.zw;
				float2 panner216 = ( 1.0 * _Time.y * appendResult374 + uv_Mask1);
				float4 tex2DNode218 = tex2D( _Mask1, ( lerpResult325 + panner216 ) );
				#if defined(_KEYWORD2_R)
				float staticSwitch219 = tex2DNode218.r;
				#elif defined(_KEYWORD2_A)
				float staticSwitch219 = tex2DNode218.a;
				#else
				float staticSwitch219 = tex2DNode218.a;
				#endif
				float2 appendResult397 = (float2(Mask03_U , _Mask03_V));
				float4 _Mask3_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_Mask3_ST_arr, _Mask3_ST);
				float2 uv_Mask3 = i.ase_texcoord2.xy * _Mask3_ST_Instance.xy + _Mask3_ST_Instance.zw;
				float2 panner399 = ( 1.0 * _Time.y * appendResult397 + uv_Mask3);
				float4 tex2DNode400 = tex2D( _Mask3, panner399 );
				#if defined(_KEYWORD4_R)
				float staticSwitch401 = tex2DNode400.r;
				#elif defined(_KEYWORD4_A)
				float staticSwitch401 = tex2DNode400.a;
				#else
				float staticSwitch401 = tex2DNode400.a;
				#endif
				float Mask82 = ( staticSwitch11 * staticSwitch219 * staticSwitch401 );
				float4 appendResult7 = (float4(staticSwitch237.rgb , ( lerpResult414 * i.ase_color.a * _Color0.a * staticSwitch239 * _Float15 * lerpResult338 * Mask82 * lerpResult347 )));
				
				
				finalColor = appendResult7;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Dependency "LightMode"="ForwardBase"

	Fallback "True"
}
/*ASEBEGIN
Version=18900
153;306;1961;1073;1009.56;1651.814;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;259;-5357.104,-2330.234;Inherit;False;1636.939;987.6809;扰动;9;70;50;53;51;55;67;385;384;386;扰动;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;384;-5291.412,-1969.901;Inherit;False;Property;_Distortion_U;Distortion_U;49;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;385;-5299.911,-1886.201;Inherit;False;Property;_Distortion_V;Distortion_V;50;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;386;-5127.412,-1957.901;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;51;-5308.104,-2149.289;Inherit;False;0;50;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;315;-4792.427,2946.608;Inherit;False;1321.745;1338.922;溶解uv;17;57;302;314;58;304;308;307;306;90;309;89;92;303;317;383;381;382;溶解uv;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;53;-4955.104,-2148.289;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;312;-5106.146,-2950.022;Inherit;False;1094.708;330.5801;flowmap;7;241;242;285;310;311;305;316;flowmap;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;50;-4699.2,-2168.009;Inherit;True;Property;Distortion_Tex;Distortion_Tex;48;0;Create;False;0;0;0;False;2;Header(___________________________________________________________________________________________________________________________________________________________________________________________________________________________________________);Header(Noise);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;381;-4702.203,3167.557;Inherit;False;Property;_DissolveTex_U;DissolveTex_U;39;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;305;-4508.5,-2814.648;Inherit;False;Property;_Float31;custom2y控制flowmap扭曲;74;1;[Toggle];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;310;-4514.317,-2707.73;Inherit;False;Property;_Float32;flowmap扰动;53;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;382;-4715.203,3250.557;Inherit;False;Property;_DissolveTex_V;DissolveTex_V;40;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-4840.852,-1778.521;Inherit;False;Property;_Float9;扰动;52;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;241;-5056.146,-2894.798;Inherit;True;Property;_flowmaptex;flowmaptex;51;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;242;-4741.872,-2873.442;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;383;-4502.203,3183.557;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;278;-3229.496,771.2501;Inherit;False;2403.086;1048.659;溶解方向;15;262;265;271;263;264;270;266;268;267;269;272;274;277;279;282;溶解方向;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;316;-4238.615,-2816.66;Inherit;False;flpwmap_custom_switch;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;57;-4742.427,2996.608;Inherit;False;0;23;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;67;-4328.158,-1782.353;Inherit;False;noise_intensity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;311;-4222.354,-2702.069;Inherit;False;flowmap_intensity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;70;-4099.011,-2017.946;Inherit;False;noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;285;-4455.108,-2900.022;Inherit;False;flowmap;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;314;-4398.089,3832.437;Inherit;False;311;flowmap_intensity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;303;-4720.823,3983.53;Inherit;True;2;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;304;-4705.144,3463.439;Inherit;False;70;noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;263;-3136.698,882.8436;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;262;-3153.605,1104.564;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;317;-4204.008,3979.924;Inherit;False;316;flpwmap_custom_switch;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;58;-4347.817,3008.881;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;302;-4499.168,3562.407;Inherit;False;67;noise_intensity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;265;-2650.06,1059.232;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;306;-4248.759,3644.226;Inherit;False;285;flowmap;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;308;-4133.377,3453.493;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;264;-3132.348,1316.446;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;267;-2680.087,821.2501;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;266;-3179.496,1560.16;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;307;-3990.536,3787.854;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;163;-3183.21,-2061.124;Inherit;False;1568.74;739.7102;主贴图uv;10;39;35;42;43;36;59;161;376;375;377;主贴图uv;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;279;-1810.202,1239.203;Inherit;False;Constant;_Float25;Float 25;52;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;376;-3139.06,-1751.959;Inherit;False;Property;_MainTex_V;MainTex_V;21;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;270;-2668.326,1300.02;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;39;-3116.787,-1623.414;Inherit;True;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;269;-2168.952,1465.344;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-4076.392,3269.162;Inherit;False;Property;_Float13;扰动影响溶解;55;1;[Toggle];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;271;-2294.883,833.8645;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;268;-2671.69,1565.909;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;309;-3926.322,3607.631;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;166;-4896.141,616.814;Inherit;False;1013.145;364.3818; 软粒子;9;126;97;98;96;327;333;334;336;337; 软粒子;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;375;-3139.06,-1840.959;Inherit;False;Property;_MainTex_U;MainTex_U;20;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;89;-3861.801,3014.49;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;272;-1723.76,973.114;Inherit;False;Property;_Keyword7;溶解方向;41;0;Create;False;0;0;0;False;0;False;0;4;4;True;;KeywordEnum;5;up;down;left;right;off;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;35;-3131.354,-2011.097;Inherit;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;333;-4696.545,896.8682;Inherit;False;Property;_Float5;反向软粒子(强化边缘）;8;1;[Toggle];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;42;-2750.469,-1600.561;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;274;-1705.432,1353.576;Inherit;False;Property;_Float18;溶解方向强度;42;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-4861.523,829.1957;Inherit;False;Property;_Float16;软粒子（羽化边缘）;7;0;Create;False;0;0;0;False;1;Header(depthfade);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;236;-3034.656,-2664.823;Inherit;False;1214.206;357.624;Gradient;6;229;226;231;378;379;380;Gradienttex;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;98;-4846.141,666.814;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;146;-3141.72,3013.755;Inherit;False;1475.065;723.4756;菲尼尔;11;135;137;138;139;140;141;142;144;147;136;352;菲尼尔;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;377;-2895.06,-1805.959;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;43;-2527.261,-1737.479;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DepthFade;97;-4614.688,666.2394;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;379;-2990.985,-2402.689;Inherit;False;Property;_Gradienttex_V;Gradienttex_V;24;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-2362.992,-1337.335;Inherit;False;Property;_Float10;custom1xy控制主贴图偏移;71;1;[Toggle];Create;False;0;0;0;True;2;Header(___________________________________________________________________________________________________________________________________________________________________________________________________________________________________________);Header(Custom);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;378;-2998.783,-2487.19;Inherit;False;Property;_Gradienttex_U;Gradienttex_U;23;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;136;-3118.165,3057.115;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PannerNode;36;-2452.103,-1977.331;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FaceVariableNode;352;-3012.43,3187.22;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;92;-3694.682,3013.597;Inherit;False;dissolveUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;103;-3142.576,-1296.299;Inherit;False;1704.885;838.751;软溶解;18;49;29;93;61;23;25;31;62;24;30;26;33;34;45;32;122;280;281;软溶解;1,1,1,1;0;0
Node;AmplifyShaderEditor.PowerNode;282;-1296.819,1064.88;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;222;-6350.638,-1049.015;Inherit;False;2798.323;1460.011;mask;35;82;220;11;219;8;218;319;320;325;80;216;326;322;374;217;76;79;73;372;371;323;75;77;373;321;74;368;370;394;395;396;397;399;400;401;mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;334;-4480.229,883.1592;Inherit;False;depthfade_switch;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;277;-1050.409,1159.944;Inherit;False;dis_direction;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;137;-3052.719,3270.273;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;337;-4252.897,860.6012;Inherit;False;334;depthfade_switch;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;-3135.307,-1207.707;Inherit;False;92;dissolveUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;74;-5873.529,-674.8624;Inherit;True;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;197;-3135.871,1911.744;Inherit;False;1797.446;957.2153;折射;17;188;186;190;183;185;191;193;192;194;195;196;202;201;203;391;392;393;折射;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;138;-3076.711,3573.226;Inherit;False;Property;_Float19;菲尼尔范围;13;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;368;-5858.146,-858.8537;Inherit;False;Property;Mask01_U;Mask01_U;28;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;351;-2795.43,3144.22;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;408;-1521.138,-2410.605;Inherit;False;955.872;823.611;Comment;12;100;405;162;406;283;99;284;313;318;407;72;71;扰动影响主贴图;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;327;-4393.009,748.1248;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;380;-2721.884,-2481.99;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;226;-2984.656,-2614.823;Inherit;False;0;212;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;135;-3045.824,3462.564;Inherit;False;Property;_power3;菲尼尔强度;12;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;59;-2021.282,-1749.128;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;370;-5855.979,-765.8537;Inherit;False;Property;Mask01_V;Mask01_V;29;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;392;-3054.829,2268.994;Inherit;False;Property;_Refraction_V;Refraction_V;69;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-2853.932,-588.6647;Inherit;False;Property;_Float11;custom2x控制溶解;73;1;[Toggle];Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;23;-2886.598,-1149.208;Inherit;True;Property;_DissolveTex;_DissolveTex;38;0;Create;True;0;0;0;False;2;Header(___________________________________________________________________________________________________________________________________________________________________________________________________________________________________________);Header(dissolove);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;139;-2742.485,3325.885;Inherit;False;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;371;-5672.563,-865.8537;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;281;-2784.968,-1232.823;Inherit;False;277;dis_direction;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;75;-5515.009,-732.6094;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;321;-5326.949,-566.0151;Inherit;False;70;noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;77;-5886.552,-999.0151;Inherit;False;0;8;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;391;-3069.829,2182.994;Inherit;False;InstancedProperty;_Refraction_U;Refraction_U;68;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-3159.307,-957.4673;Inherit;False;Property;_Float6;溶解;43;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;323;-5341.359,-490.2953;Inherit;False;67;noise_intensity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;373;-5727.051,-168.0192;Inherit;False;Property;_Mask02_V;Mask02_V;33;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;336;-4236.42,666.9297;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;229;-2323.019,-2611.17;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;161;-1793.835,-1915.281;Inherit;False;maintexUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;49;-3092.576,-759.5476;Inherit;True;2;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;71;-1471.138,-2199.557;Inherit;False;70;noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;72;-1447.21,-2035.103;Inherit;False;67;noise_intensity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;372;-5725.908,-238.0192;Inherit;False;Property;Mask02_U;Mask02_U;32;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;195;-2854.264,2754.401;Inherit;False;Property;_Float24;custom2z控制折射（热扭曲）;75;1;[Toggle];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;79;-5294.365,-979.492;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;114;-3266.725,-91.41841;Inherit;False;1936.036;770.2162; 亮边溶解;8;107;109;105;106;108;124;130;133;亮边溶解;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;395;-5914.818,100.2975;Inherit;False;Property;Mask03_U;Mask03_U;36;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;322;-5064.77,-564.1521;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;192;-2800.274,2423.947;Inherit;False;Property;_Float23;折射（热扭曲）强度;70;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;194;-3068.68,2489.68;Inherit;True;2;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;394;-5914.96,170.2975;Inherit;False;Property;_Mask03_V;Mask03_V;37;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;393;-2887.829,2202.994;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;407;-1207.189,-2228.182;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-2440.672,-961.4243;Inherit;False;Constant;_Float3;Float 3;11;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;186;-3085.871,1998.77;Inherit;False;0;190;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;73;-5273.948,-677.7852;Inherit;False;Property;_Float12;custom1zw控制mask01偏移;72;1;[Toggle];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;318;-1177.717,-1744.174;Inherit;False;316;flpwmap_custom_switch;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;374;-5501.836,-225.0192;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;231;-2044.45,-2553.012;Inherit;False;Gradienttex;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;162;-1431.481,-2360.605;Inherit;False;161;maintexUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;62;-2757.467,-793.6447;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;280;-2501.269,-1225.629;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;326;-5176.75,-425.1008;Inherit;False;Property;_Float0;扰动影响mask;54;1;[Toggle];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;217;-5714.127,-371.0438;Inherit;False;0;218;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;313;-1214.078,-1882.204;Inherit;False;311;flowmap_intensity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;100;-1456.391,-1813.576;Inherit;False;2;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;126;-4074.794,668.6714;Inherit;False;depthfade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;140;-2505.02,3326.082;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-2054.981,-772.7465;Inherit;False;Constant;_Float7;Float 7;11;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;76;-5116.802,-839.5262;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;99;-915.579,-1865.656;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;396;-5903.948,-32.72707;Inherit;False;0;400;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;201;-2543.121,2352.396;Inherit;False;Constant;_Float26;Float 26;43;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;397;-5689.746,113.2975;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;80;-4991.523,-866.3081;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-2083.264,-981.4915;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;141;-2409.202,3428.264;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;142;-2560.026,3560.687;Inherit;False;Property;_Float20;反向菲尼尔（虚化边缘）;14;1;[Toggle];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;188;-2685.971,2032.571;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;325;-4913.75,-588.1008;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;216;-5121.941,-351.5206;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;405;-1078.189,-2311.182;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;193;-2515.579,2480.279;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;335;-245.3969,-2204.535;Inherit;False;334;depthfade_switch;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;180;-5587.983,875.4525;Inherit;False;2351.014;1500.265;顶点偏移;29;176;172;167;178;179;359;181;171;360;169;168;358;357;356;173;355;354;353;361;362;363;364;365;366;367;387;388;389;390;顶点偏移;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;106;-3237.559,396.0135;Inherit;False;Property;_Float17;亮边宽度;46;0;Create;False;0;0;0;False;0;False;0;0;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;234;-1141.868,-1504.726;Inherit;False;231;Gradienttex;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;332;-240.3297,-2301.698;Inherit;False;126;depthfade;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;232;-1130.526,-1369.417;Inherit;False;70;noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;284;-1054.801,-1974.284;Inherit;False;285;flowmap;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;233;-1159.092,-1251.267;Inherit;False;67;noise_intensity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-2237.378,-1131.102;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;26;-2047.399,-1124.234;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;144;-2176.633,3335.982;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;399;-5309.851,-13.20388;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;235;-782.196,-1554.894;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;109;-2828.034,390.9376;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;330;23.88808,-2259.195;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;130;-2660.92,55.1325;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-0.5;False;2;FLOAT;1.5;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;190;-2394.915,2163.322;Inherit;True;Property;_reftex; 折射（热扭曲）贴图;66;0;Create;False;0;0;0;False;2;Header(___________________________________________________________________________________________________________________________________________________________________________________________________________________________________________);Header(Ref);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;33;-2055.09,-705.9337;Inherit;False;Property;_Float8;软硬;44;0;Create;False;0;0;0;False;0;False;0.5;0.5;0.5;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;191;-2263.457,2462.681;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;388;-5134.576,1081.398;Inherit;False;Property;_VertexOffset_U;VertexOffset_U;58;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;389;-5141.576,1153.398;Inherit;False;Property;_VertexOffset_V;VertexOffset_V;59;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;361;-5469.371,1934.916;Inherit;False;Property;_VertexOffsetMaskTex_V2;VertexOffset MaskTex_V2;65;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;354;-5469.883,1404.858;Inherit;False;Property;_VertexOffsetMaskTex_U;VertexOffset MaskTex_U;61;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;283;-743.366,-2001.879;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;362;-5469.757,1845.303;Inherit;False;Property;_VertexOffsetMaskTex_U2;VertexOffset MaskTex_U2;64;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;319;-4770.528,-709.2612;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;346;34.23663,-2077.117;Inherit;False;Property;_Float30;边缘收窄;10;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;320;-4740.653,-546.1569;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;353;-5483.181,1515.826;Inherit;False;Property;_VertexOffsetMaskTex_V;VertexOffset MaskTex_V;62;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;356;-5188.599,1408.751;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StepOpNode;107;-2382.176,267.4055;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;34;-1913.377,-1014.508;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;213;-432.9276,-1401.974;Inherit;False;Property;_Float29;颜色混合;25;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;218;-4658.863,-398.6953;Inherit;True;Property;_Mask1;遮罩02;30;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-460.9511,-1950.223;Inherit;True;Property;_maintex;主贴图;15;0;Create;False;0;0;0;False;2;Header(___________________________________________________________________________________________________________________________________________________________________________________________________________________________________________);Header(Main);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;400;-4850.308,-62.29018;Inherit;True;Property;_Mask3;遮罩03;34;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;212;-533.6704,-1604.148;Inherit;True;Property;_Gradienttex;混合颜色贴图;22;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;357;-5453.054,1264.617;Inherit;False;0;359;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;345;219.3302,-2216.365;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;202;-2212.468,1969.472;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;364;-5186.148,1805.448;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;363;-5390.108,2004.452;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;147;-1878.385,3301.25;Inherit;False;fresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;203;-2040.38,2283.387;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;355;-5443.388,1596.282;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;365;-5443.952,1714.956;Inherit;False;0;367;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;8;-4638.786,-905.6971;Inherit;True;Property;_Mask;遮罩01;26;0;Create;False;0;0;0;False;2;Header(___________________________________________________________________________________________________________________________________________________________________________________________________________________________________________);Header(Mask);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;390;-4976.576,1093.398;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;173;-5078.478,925.4525;Inherit;False;0;169;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;105;-2179.739,-20.72756;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;45;-1825.274,-1152.001;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;343;-31.07868,-2496.162;Inherit;False;Property;_Float28;边缘强度;9;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;366;-4902.453,1744.863;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;401;-4376.756,-24.1417;Inherit;False;Property;_Keyword4;遮罩03通道;35;0;Create;False;0;0;0;False;0;False;0;1;1;True;;KeywordEnum;2;R;A;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;409;-144.1125,-1417.219;Inherit;False;Property;_Float34;主贴图细节对比度;18;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;219;-4342.366,-382.6179;Inherit;False;Property;_Keyword2;遮罩02通道;31;0;Create;False;0;0;0;False;0;False;0;1;1;True;;KeywordEnum;2;R;A;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;11;-4254.457,-789.0286;Inherit;False;Property;_Keyword0;遮罩01通道;27;0;Create;False;0;0;0;False;0;False;0;1;1;True;;KeywordEnum;2;R;A;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;209;-306.5648,-518.2598;Inherit;False;Constant;_Float27;Float 27;43;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;168;-4788.672,1032.506;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;150;-300.7165,-441.6401;Inherit;False;147;fresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;358;-4880.648,1325.351;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;14;-40.75647,-1525.03;Inherit;False;Property;_Keyword1;主贴图通道;16;0;Create;False;0;0;0;False;0;False;0;1;1;True;;KeywordEnum;2;R;A;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;32;-1709.235,-1099.344;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;185;-1897.62,2071.844;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;211;65.72686,-1843.73;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;108;-1912.664,169.4133;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;350;-107.1196,-351.5812;Inherit;False;Property;_Float33;菲尼尔开关;11;1;[Toggle];Create;False;0;0;0;False;1;Header(Fresnel);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;341;296.7568,-2374.239;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;359;-4602.66,1317.03;Inherit;True;Property;_VertexOffsetMaskTex;VertexOffset MaskTex;60;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;410;-79.05957,-1330.406;Inherit;False;Property;_Float35;主贴图细节提亮;17;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;220;-4081.226,-586.584;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;347;242.4988,-554.7366;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-178.6825,-891.5779;Inherit;False;Property;_Float14;整体颜色强度;5;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;367;-4566.765,1664.288;Inherit;True;Property;_VertexOffsetMaskTex2;VertexOffset MaskTex2;63;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;124;-1839.585,-31.91609;Inherit;False;dis_bright;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;179;-4057.382,1984.261;Inherit;False;Property;_Float22;custom2w控制顶点偏移强度;76;1;[Toggle];Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;178;-3840.808,1647.498;Inherit;False;Constant;_Float21;Float 21;37;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;167;-4121.148,1711.151;Inherit;True;2;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;3;-861.5941,-1184.052;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;183;-1754.151,2067.014;Inherit;False;Global;_GrabScreen0;Grab Screen 0;1;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;122;-1615.256,-851.2873;Inherit;False;dis_soft;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;133;-1613.257,88.39095;Inherit;False;dis_edge;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;329;485.4018,-1926.837;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;4;-959.7765,-850.6129;Inherit;False;Property;_Color0;颜色;4;1;[HDR];Create;False;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;169;-4509.863,994.5781;Inherit;True;Property;_vertextex;顶点偏移贴图;56;0;Create;False;0;0;0;False;2;Header(___________________________________________________________________________________________________________________________________________________________________________________________________________________________________________);Header(Vertex_offset);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;412;108.8525,-1389.092;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;125;-750.3593,-203.436;Inherit;False;124;dis_bright;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;176;-3592.009,1580.022;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;172;-4104.257,1322.58;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;411;-61.05957,-1229.406;Inherit;False;Property;_Float36;细节平滑过度;19;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;387;-4027.563,1530.401;Inherit;False;Property;_VertexOffsetIntensity;VertexOffset Intensity;57;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;340;147.6157,157.3226;Inherit;False;334;depthfade_switch;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;128;157.3221,51.57904;Inherit;False;126;depthfade;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;576.0233,-1139.041;Inherit;False;5;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;123;-732.8659,-99.02647;Inherit;False;122;dis_soft;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;196;-1558.426,2073.591;Inherit;False;ref;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;132;444.6913,-966.722;Inherit;False;Property;_Color1;亮边颜色;47;1;[HDR];Create;False;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;360;-4014.033,1006.57;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;134;456.8172,-765.9245;Inherit;False;133;dis_edge;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;413;250.2708,-1307.891;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;82;-3776.313,-588.9445;Inherit;False;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;221;592.5698,217.0276;Inherit;False;82;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;414;403.7212,-1313.76;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;338;543.0873,82.17676;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;131;757.0637,-1185.447;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;199;1176.444,-1177.331;Inherit;False;196;ref;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;239;-321.2267,-210.135;Inherit;False;Property;_Keyword5;亮边溶解;45;0;Create;False;0;0;0;False;0;False;0;1;1;True;;KeywordEnum;2;on;off;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-155.7354,-57.72403;Inherit;False;Property;_Float15;整体透明度;6;0;Create;False;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;171;-3850.968,1014.011;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;181;-3678.302,1029.461;Inherit;False;vertexoffset;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;1285.657,-227.2876;Inherit;False;8;8;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;237;1492.587,-1070.738;Inherit;False;Property;_Keyword3;折射（热扭曲）开关;67;0;Create;False;0;0;0;False;0;False;0;1;1;True;;KeywordEnum;2;on;off;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-5809.21,-1168.036;Inherit;False;Property;_Float2;双面模式;3;1;[Enum];Create;False;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-5817.741,-1544.293;Inherit;False;Property;_Float1;材质模式;0;1;[Enum];Create;False;0;2;blend;10;add;1;0;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;182;1844.156,-443.5735;Inherit;False;181;vertexoffset;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;406;-1086.189,-2154.182;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-5806.055,-1285.052;Inherit;False;Property;_Ztestmode;深度测试;2;1;[Enum];Create;False;0;0;1;UnityEngine.Rendering.CompareFunction;True;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;7;1807.766,-873.3679;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-5808.068,-1410.66;Inherit;False;Property;_Float4;深度写入;1;1;[Toggle];Create;False;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;2483.242,-752.0063;Float;False;True;-1;2;ASEMaterialInspector;100;1;XM/All;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;1;5;False;12;0;True;13;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;True;0;False;-1;True;True;2;True;22;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;True;2;True;20;True;0;True;21;True;False;0;False;-1;0;False;-1;True;2;RenderType=Opaque=RenderType;Queue=Transparent=Queue=0;True;2;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;True;1;LightMode=ForwardBase;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;False;0
WireConnection;386;0;384;0
WireConnection;386;1;385;0
WireConnection;53;0;51;0
WireConnection;53;2;386;0
WireConnection;50;1;53;0
WireConnection;242;0;241;1
WireConnection;242;1;241;2
WireConnection;383;0;381;0
WireConnection;383;1;382;0
WireConnection;316;0;305;0
WireConnection;67;0;55;0
WireConnection;311;0;310;0
WireConnection;70;0;50;1
WireConnection;285;0;242;0
WireConnection;58;0;57;0
WireConnection;58;2;383;0
WireConnection;265;0;262;1
WireConnection;308;0;58;0
WireConnection;308;1;304;0
WireConnection;308;2;302;0
WireConnection;267;0;263;2
WireConnection;307;0;314;0
WireConnection;307;1;303;2
WireConnection;307;2;317;0
WireConnection;270;0;264;2
WireConnection;269;0;265;0
WireConnection;271;0;267;0
WireConnection;268;0;266;1
WireConnection;309;0;308;0
WireConnection;309;1;306;0
WireConnection;309;2;307;0
WireConnection;89;0;58;0
WireConnection;89;1;309;0
WireConnection;89;2;90;0
WireConnection;272;1;271;0
WireConnection;272;0;270;0
WireConnection;272;2;268;0
WireConnection;272;3;269;0
WireConnection;272;4;279;0
WireConnection;42;0;39;1
WireConnection;42;1;39;2
WireConnection;377;0;375;0
WireConnection;377;1;376;0
WireConnection;43;0;35;0
WireConnection;43;1;42;0
WireConnection;97;1;98;0
WireConnection;97;0;96;0
WireConnection;36;0;35;0
WireConnection;36;2;377;0
WireConnection;92;0;89;0
WireConnection;282;0;272;0
WireConnection;282;1;274;0
WireConnection;334;0;333;0
WireConnection;277;0;282;0
WireConnection;351;0;136;0
WireConnection;351;1;352;0
WireConnection;327;0;97;0
WireConnection;380;0;378;0
WireConnection;380;1;379;0
WireConnection;59;0;36;0
WireConnection;59;1;43;0
WireConnection;59;2;60;0
WireConnection;23;1;93;0
WireConnection;139;0;351;0
WireConnection;139;4;137;0
WireConnection;139;2;135;0
WireConnection;139;3;138;0
WireConnection;371;0;368;0
WireConnection;371;1;370;0
WireConnection;75;0;74;3
WireConnection;75;1;74;4
WireConnection;336;0;97;0
WireConnection;336;1;327;0
WireConnection;336;2;337;0
WireConnection;229;0;226;0
WireConnection;229;2;380;0
WireConnection;161;0;59;0
WireConnection;79;0;77;0
WireConnection;79;2;371;0
WireConnection;322;0;321;0
WireConnection;322;1;323;0
WireConnection;393;0;391;0
WireConnection;393;1;392;0
WireConnection;407;0;71;0
WireConnection;407;1;72;0
WireConnection;374;0;372;0
WireConnection;374;1;373;0
WireConnection;231;0;229;0
WireConnection;62;0;29;0
WireConnection;62;1;49;1
WireConnection;62;2;61;0
WireConnection;280;0;281;0
WireConnection;280;1;23;1
WireConnection;126;0;336;0
WireConnection;140;0;139;0
WireConnection;76;0;77;0
WireConnection;76;1;75;0
WireConnection;99;0;313;0
WireConnection;99;1;100;2
WireConnection;99;2;318;0
WireConnection;397;0;395;0
WireConnection;397;1;394;0
WireConnection;80;0;79;0
WireConnection;80;1;76;0
WireConnection;80;2;73;0
WireConnection;30;0;62;0
WireConnection;30;1;31;0
WireConnection;141;0;140;0
WireConnection;188;0;186;0
WireConnection;188;2;393;0
WireConnection;325;1;322;0
WireConnection;325;2;326;0
WireConnection;216;0;217;0
WireConnection;216;2;374;0
WireConnection;405;0;162;0
WireConnection;405;1;407;0
WireConnection;193;0;192;0
WireConnection;193;1;194;3
WireConnection;193;2;195;0
WireConnection;24;0;280;0
WireConnection;24;1;25;0
WireConnection;26;0;24;0
WireConnection;26;1;30;0
WireConnection;144;0;140;0
WireConnection;144;1;141;0
WireConnection;144;2;142;0
WireConnection;399;0;396;0
WireConnection;399;2;397;0
WireConnection;235;0;234;0
WireConnection;235;1;232;0
WireConnection;235;2;233;0
WireConnection;109;0;62;0
WireConnection;109;1;106;0
WireConnection;330;1;332;0
WireConnection;330;2;335;0
WireConnection;130;0;280;0
WireConnection;190;1;188;0
WireConnection;191;0;201;0
WireConnection;191;1;193;0
WireConnection;283;0;405;0
WireConnection;283;1;284;0
WireConnection;283;2;99;0
WireConnection;319;0;80;0
WireConnection;319;1;325;0
WireConnection;320;0;325;0
WireConnection;320;1;216;0
WireConnection;356;0;354;0
WireConnection;356;1;353;0
WireConnection;107;0;109;0
WireConnection;107;1;130;0
WireConnection;34;0;33;0
WireConnection;218;1;320;0
WireConnection;1;1;283;0
WireConnection;400;1;399;0
WireConnection;212;1;235;0
WireConnection;345;0;330;0
WireConnection;345;1;346;0
WireConnection;364;0;362;0
WireConnection;364;1;361;0
WireConnection;147;0;144;0
WireConnection;203;0;190;1
WireConnection;203;1;191;0
WireConnection;8;1;319;0
WireConnection;390;0;388;0
WireConnection;390;1;389;0
WireConnection;105;0;62;0
WireConnection;105;1;130;0
WireConnection;45;0;26;0
WireConnection;366;0;365;0
WireConnection;366;2;364;0
WireConnection;366;1;363;0
WireConnection;401;1;400;1
WireConnection;401;0;400;4
WireConnection;219;1;218;1
WireConnection;219;0;218;4
WireConnection;11;1;8;1
WireConnection;11;0;8;4
WireConnection;168;0;173;0
WireConnection;168;2;390;0
WireConnection;358;0;357;0
WireConnection;358;2;356;0
WireConnection;358;1;355;0
WireConnection;14;1;1;1
WireConnection;14;0;1;4
WireConnection;32;0;45;0
WireConnection;32;1;34;0
WireConnection;32;2;33;0
WireConnection;185;0;202;0
WireConnection;185;1;203;0
WireConnection;211;0;1;0
WireConnection;211;1;212;0
WireConnection;211;2;213;0
WireConnection;108;0;105;0
WireConnection;108;1;107;0
WireConnection;341;0;343;0
WireConnection;341;1;345;0
WireConnection;359;1;358;0
WireConnection;220;0;11;0
WireConnection;220;1;219;0
WireConnection;220;2;401;0
WireConnection;347;0;209;0
WireConnection;347;1;150;0
WireConnection;347;2;350;0
WireConnection;367;1;366;0
WireConnection;124;0;105;0
WireConnection;183;0;185;0
WireConnection;122;0;32;0
WireConnection;133;0;108;0
WireConnection;329;0;341;0
WireConnection;329;1;211;0
WireConnection;169;1;168;0
WireConnection;412;0;14;0
WireConnection;412;1;409;0
WireConnection;176;0;178;0
WireConnection;176;1;167;4
WireConnection;176;2;179;0
WireConnection;5;0;329;0
WireConnection;5;1;3;0
WireConnection;5;2;4;0
WireConnection;5;3;94;0
WireConnection;5;4;347;0
WireConnection;196;0;183;0
WireConnection;360;0;169;1
WireConnection;360;1;359;0
WireConnection;360;2;367;0
WireConnection;413;0;412;0
WireConnection;413;1;410;0
WireConnection;82;0;220;0
WireConnection;414;0;14;0
WireConnection;414;1;413;0
WireConnection;414;2;411;0
WireConnection;338;0;128;0
WireConnection;338;2;340;0
WireConnection;131;0;5;0
WireConnection;131;1;132;0
WireConnection;131;2;134;0
WireConnection;239;1;125;0
WireConnection;239;0;123;0
WireConnection;171;0;360;0
WireConnection;171;1;172;0
WireConnection;171;2;387;0
WireConnection;171;3;176;0
WireConnection;181;0;171;0
WireConnection;6;0;414;0
WireConnection;6;1;3;4
WireConnection;6;2;4;4
WireConnection;6;3;239;0
WireConnection;6;4;95;0
WireConnection;6;5;338;0
WireConnection;6;6;221;0
WireConnection;6;7;347;0
WireConnection;237;1;199;0
WireConnection;237;0;131;0
WireConnection;406;0;162;0
WireConnection;406;1;71;0
WireConnection;406;2;72;0
WireConnection;7;0;237;0
WireConnection;7;3;6;0
WireConnection;0;0;7;0
WireConnection;0;1;182;0
ASEEND*/
//CHKSM=B1EB38DCE1619EF3268FDEB8E21B49D841801AF1