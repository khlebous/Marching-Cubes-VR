Shader "custom/SurfaceDraw" {
	SubShader{
		Tags {
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
		}
		LOD 400
		Cull Off

		CGPROGRAM
		#pragma target 5.0
		#pragma only_renderers d3d11
		#pragma surface surf Lambert vertex:VertShComputeBuff

		struct appdata {
			float4 vertex: POSITION;
			float3 normal: NORMAL;
			float4 color : COLOR;
			#ifdef SHADER_API_D3D11
			uint id: SV_VertexID;
			#endif
		};

		struct Vert
		{
			float4 position;
			float3 normal;
			float4 color;
		};

		#ifdef SHADER_API_D3D11
		StructuredBuffer<Vert> _Buffer;
		#endif

		// vertex shader
		void VertShComputeBuff(inout appdata v) {
			#ifdef SHADER_API_D3D11
			v.vertex = _Buffer[v.id].position;
			v.normal = _Buffer[v.id].normal;
			v.color = _Buffer[v.id].color;
			#endif
		}

		struct Input {
			float4 color : COLOR;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = float3(1,1,1);// IN.color.xyz;
		}
		ENDCG
	}
		FallBack "Diffuse"
}