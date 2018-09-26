// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/Community/TFHC/Force Shield"
{
	Properties
	{
		_Opacity("Opacity", Range( 0 , 1)) = 0.5
		_ShieldPatternColor("Shield Pattern Color", Color) = (0.2470588,0.7764706,0.9098039,1)
		_ShieldPattern("Shield Pattern", 2D) = "white" {}
		[IntRange]_ShieldPatternSize("Shield Pattern Size", Range( 1 , 20)) = 5
		_ShieldPatternPower("Shield Pattern Power", Range( 0 , 100)) = 5
		_ShieldRimPower("Shield Rim Power", Range( 0 , 10)) = 7
		_IntersectIntensity("Intersect Intensity", Range( 0 , 1)) = 0.2
		_IntersectColor("Intersect Color", Color) = (1,0,0.07371521,1)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float4 _IntersectColor;
		uniform float _ShieldRimPower;
		uniform sampler2D _ShieldPattern;
		uniform float _ShieldPatternSize;
		uniform float4 _ShieldPatternColor;
		uniform sampler2D _CameraDepthTexture;
		uniform float _IntersectIntensity;
		uniform float _ShieldPatternPower;
		uniform float _Opacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float ShieldRimPower32 = _ShieldRimPower;
			float fresnelNdotV8 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode8 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV8, (10.0 + (ShieldRimPower32 - 0.0) * (0.0 - 10.0) / (10.0 - 0.0)) ) );
			float ShieldRim23 = fresnelNode8;
			float2 appendResult130 = (float2(_ShieldPatternSize , _ShieldPatternSize));
			float2 appendResult46 = (float2(1 , 0.x));
			float2 uv_TexCoord41 = i.uv_texcoord * appendResult130 + appendResult46;
			float4 ShieldPattern17 = tex2D( _ShieldPattern, uv_TexCoord41 );
			float4 ShieldPatternColor12 = _ShieldPatternColor;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth110 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth110 = abs( ( screenDepth110 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _IntersectIntensity ) );
			float clampResult113 = clamp( distanceDepth110 , 0.0 , 1.0 );
			float4 lerpResult124 = lerp( _IntersectColor , ( ( ( ShieldRim23 + ShieldPattern17 ) * 0 ) * ( 0 * ShieldPatternColor12 ) ) , clampResult113);
			float ShieldPower15 = _ShieldPatternPower;
			float4 Emission120 = ( lerpResult124 * ShieldPower15 );
			o.Albedo = Emission120.rgb;
			o.Emission = Emission120.rgb;
			o.Alpha = _Opacity;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				float3 worldNormal : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15600
1927;29;1844;1044;3090.913;1096.6;1;False;True
Node;AmplifyShaderEditor.CommentaryNode;275;-3870.329,-1061.903;Float;False;1030.896;385.0003;Comment;6;8;23;30;16;31;32;Shield RIM;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;276;-2780.04,-1808.4;Float;False;1504.24;684.7161;Comment;12;44;129;130;46;41;1;17;85;3;12;6;15;Shield Main Pattern;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-3816.578,-1010.159;Float;False;Property;_ShieldRimPower;Shield Rim Power;5;0;Create;True;0;0;False;0;7;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;44;-2642.899,-1440.7;Float;False;Constant;_Vector0;Vector 0;6;0;Create;True;0;0;False;0;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;129;-2730.04,-1558.843;Float;False;Property;_ShieldPatternSize;Shield Pattern Size;3;1;[IntRange];Create;True;0;0;False;0;5;0;1;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;-2607.967,-1238.684;Float;False;-1;;0;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;32;-3462.669,-1011.903;Float;False;ShieldRimPower;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;16;-3820.329,-878.8024;Float;False;32;ShieldRimPower;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;46;-2364.299,-1326.101;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;130;-2390.04,-1534.843;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;30;-3524.869,-878.9025;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;10;False;3;FLOAT;10;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;41;-2180.397,-1393.801;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-1921.001,-1410.9;Float;True;Property;_ShieldPattern;Shield Pattern;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;8;-3313.271,-877.2028;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;269;-2287.997,-827.7885;Float;False;1652.997;650.1895;Mix of Pattern, Wave, Rim , Impact and adding intersection highlight;18;125;22;53;210;262;5;122;113;124;127;126;120;114;110;96;95;270;20;Shield Mix for Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;20;-2219.121,-469.8993;Float;False;-1;;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;23;-3082.433,-881.2717;Float;False;ShieldRim;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;-2716.901,-1757.5;Float;False;Property;_ShieldPatternColor;Shield Pattern Color;1;0;Create;True;0;0;False;0;0.2470588,0.7764706,0.9098039,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;22;-2228.062,-639.0428;Float;False;23;ShieldRim;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;125;-2237.997,-548.186;Float;False;17;ShieldPattern;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-1531.8,-1409.1;Float;False;ShieldPattern;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;53;-2000.672,-599.0471;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;270;-2020.701,-481.7193;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;210;-1987.634,-462.2223;Float;False;-1;;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;114;-2074.785,-298.9202;Float;False;Property;_IntersectIntensity;Intersect Intensity;7;0;Create;True;0;0;False;0;0.2;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;96;-2067.383,-379.2973;Float;False;12;ShieldPatternColor;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-2411.8,-1758.4;Float;False;ShieldPatternColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;262;-1743.507,-394.9173;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-1824.537,-575.4131;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DepthFade;110;-1708.645,-296.0273;Float;False;True;1;0;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1617.291,-592.2941;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-2023.2,-1712.1;Float;False;Property;_ShieldPatternPower;Shield Pattern Power;4;0;Create;True;0;0;False;0;5;0;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;122;-1605.098,-777.7884;Float;False;Property;_IntersectColor;Intersect Color;8;0;Create;True;0;0;False;0;1,0,0.07371521,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;113;-1492.491,-333.5993;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;15;-1703.199,-1702.2;Float;False;ShieldPower;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;127;-1286.898,-429.2852;Float;False;15;ShieldPower;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;124;-1303.897,-569.6891;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;267;-3724.017,-1789.098;Float;False;830.728;358.1541;Comment;4;35;34;36;84;Animation Speed;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-1061.299,-539.6861;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;274;-3886.348,-541.7303;Float;False;1223.975;464.9008;Comment;11;78;271;66;75;65;77;70;76;102;103;273;Shield Distortion;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-316.8302,-467.6075;Float;False;Property;_Opacity;Opacity;0;0;Create;True;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;272;-269.7561,-368.8785;Float;False;-1;;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;283;-245.9568,-649.8774;Float;False;-1;;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;120;-877.9983,-564.6881;Float;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;280;-218.7568,-742.6771;Float;False;-1;;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;121;-220.9338,-555.7334;Float;False;120;Emission;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;77;-3797.22,-234.8972;Float;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;False;0;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;70;-3836.348,-333.1133;Float;False;-1;;0;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;102;-3601.571,-191.8296;Float;False;Property;_ShieldDistortion;Shield Distortion;9;0;Create;True;0;0;False;0;0.01;0;0;0.03;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;65;-3707.2,-491.7302;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;75;-3443.833,-461.863;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;273;-3186.174,-199.3568;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;271;-2912.374,-393.2568;Float;False;VertexOffset;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;78;-3108.671,-401.2883;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.01;False;4;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;76;-3612.433,-304.4543;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;-3278.071,-348.9275;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;66;-3298.04,-446.5254;Float;False;Simplex3D;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;34;-3600.425,-1739.099;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;35;-3674.017,-1545.944;Float;False;Property;_ShieldAnimSpeed;Shield Anim Speed;6;0;Create;True;0;0;False;0;3;0;-10;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-3357.889,-1615.685;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;84;-3141.289,-1604.549;Float;False;ShieldSpeed;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;25.6424,-666.4304;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;ASESampleShaders/Community/TFHC/Force Shield;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;1;False;-1;7;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;32;0;31;0
WireConnection;46;0;44;1
WireConnection;46;1;85;0
WireConnection;130;0;129;0
WireConnection;130;1;129;0
WireConnection;30;0;16;0
WireConnection;41;0;130;0
WireConnection;41;1;46;0
WireConnection;1;1;41;0
WireConnection;8;3;30;0
WireConnection;23;0;8;0
WireConnection;17;0;1;0
WireConnection;53;0;22;0
WireConnection;53;1;125;0
WireConnection;270;0;20;0
WireConnection;12;0;3;0
WireConnection;262;0;210;0
WireConnection;262;1;96;0
WireConnection;5;0;53;0
WireConnection;5;1;270;0
WireConnection;110;0;114;0
WireConnection;95;0;5;0
WireConnection;95;1;262;0
WireConnection;113;0;110;0
WireConnection;15;0;6;0
WireConnection;124;0;122;0
WireConnection;124;1;95;0
WireConnection;124;2;113;0
WireConnection;126;0;124;0
WireConnection;126;1;127;0
WireConnection;120;0;126;0
WireConnection;75;0;65;0
WireConnection;75;1;76;0
WireConnection;273;0;102;0
WireConnection;271;0;78;0
WireConnection;78;0;66;0
WireConnection;78;3;103;0
WireConnection;78;4;273;0
WireConnection;76;0;70;0
WireConnection;76;1;77;0
WireConnection;103;0;102;0
WireConnection;66;0;75;0
WireConnection;36;0;34;0
WireConnection;36;1;35;0
WireConnection;84;0;36;0
WireConnection;0;0;120;0
WireConnection;0;2;121;0
WireConnection;0;9;28;0
ASEEND*/
//CHKSM=9DC344CE8D8984BB7A8EC7AC7366EAC00ADDE779