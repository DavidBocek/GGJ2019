// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "StandardFlash"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Emission("Emission", 2D) = "white" {}
		_Rough("Rough", 2D) = "white" {}
		_FlashAmount("FlashAmount", Range( 0 , 1)) = 0
		_FlashColor("FlashColor", Color) = (1,0,0,1)
		_EmissionPower("EmissionPower", Range( 0 , 10)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _FlashColor;
		uniform float _FlashAmount;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform float _EmissionPower;
		uniform sampler2D _Rough;
		uniform float4 _Rough_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 lerpResult12 = lerp( tex2D( _Albedo, uv_Albedo ) , _FlashColor , _FlashAmount);
			o.Albedo = lerpResult12.rgb;
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			float4 lerpResult13 = lerp( tex2D( _Emission, uv_Emission ) , _FlashColor , _FlashAmount);
			o.Emission = ( lerpResult13 * _EmissionPower ).rgb;
			float2 uv_Rough = i.uv_texcoord * _Rough_ST.xy + _Rough_ST.zw;
			float4 tex2DNode6 = tex2D( _Rough, uv_Rough );
			o.Metallic = tex2DNode6.r;
			o.Smoothness = tex2DNode6.a;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
0;72;1969;940;2233.898;602.5538;1.801426;True;True
Node;AmplifyShaderEditor.TexturePropertyNode;3;-1386.985,237.9506;Float;True;Property;_Emission;Emission;1;0;Create;True;0;0;False;0;f00f0daf9ac418c4192ceb6cdcf61a24;105a1caa3a91cf5488a37a5a2cda28e5;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-1272.975,-290.9973;Float;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;False;0;3be99043bde52e54d935205855ac6638;762cf85533b2fe94da93520225d8ed8b;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;4;-1060.218,286.6276;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;8;-799.6859,77.06324;Float;False;Property;_FlashColor;FlashColor;4;0;Create;True;0;0;False;0;1,0,0,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-1183.558,91.54899;Float;False;Property;_FlashAmount;FlashAmount;3;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-913,-287;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;5;-879.2926,706.504;Float;True;Property;_Rough;Rough;2;0;Create;True;0;0;False;0;1a245f29bf8df6646bd433db6bace263;13889abe27f24454eb583e3cb01bf947;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-665.9727,524.9811;Float;False;Property;_EmissionPower;EmissionPower;5;0;Create;True;0;0;False;0;0;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;13;-400.6031,202.0898;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;12;-300.4675,-202.7222;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-190.0781,296.2822;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;6;-565.293,699.504;Float;True;Property;_TextureSample2;Texture Sample 2;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;152.1001,22.1;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;StandardFlash;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;3;0
WireConnection;2;0;1;0
WireConnection;13;0;4;0
WireConnection;13;1;8;0
WireConnection;13;2;7;0
WireConnection;12;0;2;0
WireConnection;12;1;8;0
WireConnection;12;2;7;0
WireConnection;16;0;13;0
WireConnection;16;1;15;0
WireConnection;6;0;5;0
WireConnection;0;0;12;0
WireConnection;0;2;16;0
WireConnection;0;3;6;1
WireConnection;0;4;6;4
ASEEND*/
//CHKSM=1A122B4C439A05F000C074B00F8803C89936E67C