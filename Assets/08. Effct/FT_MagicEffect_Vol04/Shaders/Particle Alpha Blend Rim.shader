// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FT/Particles/Alpha Blended Rim" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	_RimFactor ("Rim Factor", Range(-1.0,1.0)) = 1.0
	_RimPower  ("Rim Power", Range(1.0,32.0)) = 1.0
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	AlphaTest Greater .01
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off

	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			float _RimFactor;
			float _RimPower;
			
			struct appdata_t {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD1;
				#endif
				#ifdef SHADER_API_D3D11
				float4 normal : NORMAL;
				#else
				float4 normal : TEXCOORD2;
				#endif
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = mul(UNITY_MATRIX_IT_MV, v.normal);
				#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			sampler2D_float _CameraDepthTexture;
			float _InvFade;
			
			fixed4 frag (v2f i) : SV_Target
			{
				float aa = abs(dot(normalize(i.normal.xyz), float3(0,0,1)));
				if(_RimFactor>0){
					aa = pow(aa, _RimPower);
					aa = lerp(1.0, aa ,_RimFactor);
				}else{
					aa = pow(1.0-aa, _RimPower);
					aa = lerp(1.0, aa ,-_RimFactor);
				}
				i.color.a *= aa;	
				
				#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
				float partZ = i.projPos.z;
				float fade = saturate (_InvFade * (sceneZ-partZ));
				i.color.a *= fade;
				#endif
				
				return 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
			}
			ENDCG 
		}
	}	
}
}
