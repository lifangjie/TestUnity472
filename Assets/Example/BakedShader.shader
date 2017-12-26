Shader "Custom/BakedShader" {
	Properties {
		_Diffuse ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 300
		Pass {
		    Name "FORWARD"
		    Tags { "LightMode" = "ForwardBase" "Queue" = "Geometry" }
		    ZWrite On

			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			
			#include "HLSLSupport.cginc"
			#include "UnityShaderVariables.cginc"
			#define UNITY_PASS_FORWARDBASE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			
			sampler2D _Diffuse;
			float4 _Diffuse_ST;
			uniform sampler2D _BumpMap;
			float4 unity_LightmapST;
			sampler2D unity_Lightmap;
			
			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 lightDir: TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float2 lmap : TEXCOORD3;
				LIGHTING_COORDS(4,5)
			};
			
			v2f vert(a2v v) {
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				o.uv = TRANSFORM_TEX(v.texcoord, _Diffuse);
				
				float3 worldN = mul((float3x3)_Object2World, SCALED_NORMAL);
				TANGENT_SPACE_ROTATION;
				o.lightDir = mul (rotation, ObjSpaceLightDir(v.vertex));
				o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
				

				TRANSFER_VERTEX_TO_FRAGMENT(o);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				fixed atten = LIGHT_ATTENUATION(i) * 0.7 + 0.3;
				fixed3 albedo = tex2D(_Diffuse, i.uv).rgb;
				
				fixed4 c = 0;
				
				fixed3 tangentLightDir = normalize(i.lightDir);
				fixed3 tangentViewDir = normalize(i.viewDir);
				
				fixed3 packedNormal = tex2D(_BumpMap, i.uv);
				fixed specularStrength = packedNormal.z;
				
				fixed3 tangentNormal = packedNormal * 2 - 1;
				tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
				
				fixed3 halfDir = normalize(tangentLightDir + tangentViewDir);
				
				fixed3 specular = _LightColor0.rgb * specularStrength * pow(max(0, dot(tangentNormal, halfDir)), 128);

				fixed4 lmtex = tex2D(unity_Lightmap, i.lmap.xy);
				fixed3 lm = DecodeLightmap (lmtex);
				#ifdef SHADOWS_SCREEN
				#if defined(UNITY_NO_RGBM)
				c.rgb += (albedo + specular) * min(lm, atten*2);
				#else
				c.rgb += (albedo + specular) * max(min(lm,(atten*2)*lmtex.rgb), lm*atten);
				#endif
				#else // SHADOWS_SCREEN
				c.rgb += (albedo + specular) * lm;
				#endif // SHADOWS_SCREEN
				
				return fixed4(c.rgb, 1);
			}
			
			ENDCG
		}
	}
	FallBack "Diffuse"
}