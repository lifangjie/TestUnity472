Shader "Custom/Diffuse" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Base (RGB)", 2D) = "bump" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Pass {
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
            // compile directives
            #pragma vertex vert_surf
            #pragma fragment frag_surf
            #pragma multi_compile_fwdbase
            //#pragma multi_compile_fog
            #include "HLSLSupport.cginc"
            #include "UnityShaderVariables.cginc"
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            //#define INTERNAL_DATA
            //#define WorldReflectionVector(data,normal) data.worldRefl
            //#define WorldNormalVector(data,normal) normal


			sampler2D _MainTex;
			sampler2D _BumpMap;

			struct v2f_surf {
				float4 pos : SV_POSITION;
				float2 uv_MainTex: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
				float2 lmap : TEXCOORD1;
				#endif
				LIGHTING_COORDS(2,3)
				float2 uv_BumpMap: TEXCOORD4;
				float4 tSpace0 : TEXCOORD5;
				float4 tSpace1 : TEXCOORD6;
				float4 tSpace2 : TEXCOORD7;
			};
			
            #ifndef LIGHTMAP_OFF
			float4 unity_LightmapST;
            #endif
			float4 _MainTex_ST;
			float4 _BumpMap_ST;

            // vertex shader
			v2f_surf vert_surf (appdata_full v) {
				v2f_surf o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv_BumpMap = TRANSFORM_TEX(v.texcoord, _BumpMap);
				
				float3 worldPos = mul(_Object2World, v.vertex).xyz;
				fixed3 worldNormal = normalize(mul(SCALED_NORMAL, (float3x3)_World2Object));
				fixed3 worldTangent = normalize(mul((float3x3)_Object2World, v.tangent.xyz));
				//fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross(worldNormal, worldTangent);// * tangentSign;
				o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
				
				#ifndef LIGHTMAP_OFF
				o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				float3 worldN = mul((float3x3)_Object2World, SCALED_NORMAL);

				// SH/ambient and vertex lights
				#ifdef LIGHTMAP_OFF
				float3 shlight = ShadeSH9 (float4(worldN,1.0));
				//o.vlight = shlight;
				//#ifdef VERTEXLIGHT_ON
				//float3 worldPos = mul(_Object2World, v.vertex).xyz;
				//o.vlight += Shade4PointLights (
				//	unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
				//	unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
				//	unity_4LightAtten0, worldPos, worldN );
				//#endif // VERTEXLIGHT_ON
				#endif // LIGHTMAP_OFF

				// pass lighting information to pixel shader
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
			#ifndef LIGHTMAP_OFF
			sampler2D unity_Lightmap;
			#ifndef DIRLIGHTMAP_OFF
			sampler2D unity_LightmapInd;
			#endif
			#endif

			// fragment shader
			fixed4 frag_surf (v2f_surf IN) : SV_Target {
				fixed4 albedo = tex2D (_MainTex, IN.uv_MainTex);
				//fixed3 tangentNormal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
				fixed3 tangentNormal = tex2D (_BumpMap, IN.uv_BumpMap);
				//tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
				fixed3 normal;
				
				normal.x = dot(IN.tSpace0.xyz, tangentNormal);
				normal.y = dot(IN.tSpace1.xyz, tangentNormal);
				normal.z = dot(IN.tSpace2.xyz, tangentNormal);
				normal = fixed3(IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z);
				//normal = tangentNormal;
				normal = normalize(normal);

				// compute lighting & shadowing factor
				fixed atten = LIGHT_ATTENUATION(IN);
				fixed4 c = 0;

				float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
				// realtime lighting: call lighting function
				#ifdef LIGHTMAP_OFF
				fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz - worldPos);
				fixed diff = max (0, dot (normal, lightDir));
				c.rgb = albedo * _LightColor0.rgb * (diff * atten * 2);
				c.a = 1;
				#endif // LIGHTMAP_OFF || DIRLIGHTMAP_OFF
				//#ifdef LIGHTMAP_OFF
				//c.rgb += o.Albedo * IN.vlight;
				//#endif // LIGHTMAP_OFF

				// lightmaps:
				#ifndef LIGHTMAP_OFF
				#ifndef DIRLIGHTMAP_OFF
				// directional lightmaps
				fixed4 lmtex = tex2D(unity_Lightmap, IN.lmap.xy);
				fixed4 lmIndTex = tex2D(unity_LightmapInd, IN.lmap.xy);
				//half3 lm = LightingLambert_DirLightmap(o, lmtex, lmIndTex, 1).rgb;
				UNITY_DIRBASIS
				half3 scalePerBasisVector;
				
				half3 lm = DirLightmapDiffuse (unity_DirBasis, lmtex, lmIndTex, normal, 1, scalePerBasisVector).rgb;
				
				#else // !DIRLIGHTMAP_OFF
				// single lightmap
				fixed4 lmtex = tex2D(unity_Lightmap, IN.lmap.xy);
				fixed3 lm = DecodeLightmap (lmtex);
				#endif // !DIRLIGHTMAP_OFF

				// combine lightmaps with realtime shadows
				#ifdef SHADOWS_SCREEN
				c.rgb += albedo * min(lm, atten*2);
				#else // SHADOWS_SCREEN
				c.rgb += albedo * lm;
				#endif // SHADOWS_SCREEN
				c.a = 1;
				#endif // LIGHTMAP_OFF

				return c;
			}

			ENDCG

		}
	}
	FallBack "Diffuse"
}