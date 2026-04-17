Shader "FT/Transparent/Bumped Specular Rim" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 0)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_MainTex ("Base (RGB) TransGloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_RimFactor ("Rim Factor", Range(-1.0,1.0)) = 1.0
	_RimPower  ("Rim Power", Range(1.0,4.0)) = 1.0
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 400
	
CGPROGRAM
#pragma surface surf BlinnPhong alpha
#pragma target 3.0

sampler2D _MainTex;
sampler2D _BumpMap;
fixed4 _Color;
half _Shininess;
float _RimFactor;
float _RimPower;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float3 viewDir;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	
	float aa = dot (normalize(IN.viewDir), o.Normal);
	if(_RimFactor>0){
		aa = pow(aa, _RimPower);
		aa = lerp(1.0, aa ,_RimFactor);
	}else{
		aa = pow(1.0-aa, _RimPower);
		aa = lerp(1.0, aa ,-_RimFactor);
	}
	
	o.Alpha = tex.a * _Color.a * aa;
}
ENDCG
}

FallBack "Transparent/VertexLit"
}