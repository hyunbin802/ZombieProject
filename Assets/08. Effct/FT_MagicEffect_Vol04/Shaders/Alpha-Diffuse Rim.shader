Shader "FT/Transparent/Diffuse Rim" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_RimFactor ("Rim Factor", Range(-1.0,1.0)) = 1.0
	_RimPower  ("Rim Power", Range(1.0,32.0)) = 1.0
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200

CGPROGRAM
#pragma surface surf Lambert alpha
#pragma target 3.0

sampler2D _MainTex;
fixed4 _Color;
float _RimFactor;
float _RimPower;
struct Input {
	float2 uv_MainTex;
	float3 viewDir;
};

void surf (Input IN, inout SurfaceOutput o) {
	float aa = dot (normalize(IN.viewDir), o.Normal);
	if(_RimFactor>0){
		aa = pow(aa, _RimPower);
		aa = lerp(1.0, aa ,_RimFactor);
	}else{
		aa = pow(1.0-aa, _RimPower);
		aa = lerp(1.0, aa ,-_RimFactor);
	}
				
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a * aa;
}
ENDCG
}

Fallback "Transparent/VertexLit"
}
