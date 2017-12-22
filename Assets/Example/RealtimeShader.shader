Shader "Custom/RealtimeShader" {
	Properties {
		_Diffuse ("Base (RGB)", 2D) = "white" {}
		_NormalWithSpecular ("Normal With Specular (RGB)", 2D) = "blue" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300
		Pass {
		    Name "FORWARD"
		    Tags { "LightMode" = "ForwardBase" }

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
			sampler2D _NormalWithSpecular;
			
			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 lightDir: TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				fixed3 vlight  : TEXCOORD3;
				LIGHTING_COORDS(4,5)
			};
			
			v2f vert(a2v v) {
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.uv = TRANSFORM_TEX(v.texcoord, _Diffuse);
				
				float3 worldN = mul((float3x3)_Object2World, SCALED_NORMAL);
				TANGENT_SPACE_ROTATION;
				o.lightDir = mul (rotation, ObjSpaceLightDir(v.vertex));
				o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));

				// SH/ambient and vertex lights				
				float3 shlight = ShadeSH9 (float4(worldN,1.0));
				o.vlight = shlight;
				#ifdef VERTEXLIGHT_ON
				float3 worldPos = mul(_Object2World, v.vertex).xyz;
				o.vlight += Shade4PointLights (
					unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
					unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
					unity_4LightAtten0, worldPos, worldN );
				#endif // VERTEXLIGHT_ON

				// pass lighting information to pixel shader
				TRANSFER_VERTEX_TO_FRAGMENT(o);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				fixed atten = LIGHT_ATTENUATION(i);
				fixed3 albedo = tex2D(_Diffuse, i.uv).rgb;
				
				fixed4 c = 0;

				fixed3 tangentLightDir = normalize(i.lightDir);
				fixed3 tangentViewDir = normalize(i.viewDir);
				
				// Get the texel in the normal map
				fixed3 packedNormal = tex2D(_NormalWithSpecular, i.uv);
				fixed specularStrength = packedNormal.z;
				
				fixed3 tangentNormal = packedNormal * 2 - 1;
				tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
				
				// use light probe to get ambient light
				fixed3 ambient = albedo * i.vlight;
				// or use UNITY_LIGHTMODEL_AMBIENT
				//fixed3 ambient = albedo * UNITY_LIGHTMODEL_AMBIENT;
				
				// half lambert, if use light probe, consider using lambert
				fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(tangentLightDir, tangentNormal));// * 0.5 + 0.5);

				fixed3 halfDir = normalize(tangentLightDir + tangentViewDir);
				
				fixed3 specular = _LightColor0.rgb * specularStrength * pow(max(0, dot(tangentNormal, halfDir)), 128);

				c.rgb = ambient + (diffuse + specular) * atten;

				return fixed4(c.rgb, 1);
			}
			
			ENDCG
		}
	}
	FallBack "Diffuse"
}