// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ZDStudio/技能预警/技能提醒-扇形"
{
	Properties
	{
		_Color("色调", Color) = (1,1,1,1)
		_MainTex ("主贴图", 2D) = "white" {}
		_HalfRad ("半角度", Range(0, 180)) = 30.0
	 }
	 SubShader
	 {
	 	Tags { "RenderType"="Transparent" "LightMode"="ForwardBase" "Queue"="Transparent+10" }
	 	Pass
	 	{
	 		Cull 	Back
			Blend	SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vsMesh
			#pragma fragment psMesh
			#pragma multi_compile_fwdbase
			#define UNITY_PASS_FORWARDBASE

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			sampler2D _MainTex;
			fixed4 _Color;
			half _HalfRad;
			half4 _MainTex_ST;

			struct v2f_surf
			{
				float4 pos	: SV_POSITION;
				fixed4 pack1	: TEXCOORD0;
        		fixed4 pack2    : TEXCOORD1;
			};

			v2f_surf vsMesh(appdata_full v)
			{
				v2f_surf o;
				UNITY_INITIALIZE_OUTPUT(v2f_surf, o);
				o.pos = UnityObjectToClipPos( v.vertex);
				o.pack1.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.pack2 = v.color;

				return o;
			}

			fixed4 psMesh(v2f_surf IN) : SV_Target
			{
				fixed2 tc = IN.pack1.xy;
				tc.y = 1.0f - tc.y;
				fixed2 frontDir = normalize(fixed2(0.0, -0.5f));
				fixed2 center = fixed2(0.5f, 0.5f);
				tc = tc - center;

				fixed2 tcDir = normalize(tc);
				fixed cosVal = dot(tcDir, frontDir);
				cosVal = acos(cosVal);
				_HalfRad = _HalfRad / 180.0f * 3.14159265f;
				cosVal = cosVal / _HalfRad;

				fixed al = cosVal > 0.99f ? 0.0f : 1.0f;

				fixed tcLen = length(tc);
				tcLen = tcLen / 0.5f;

				al = tcLen > 0.99f ? 0.0f : al;

				tc.x = cosVal;
				tc.y = tcLen;

				fixed4 map_base = tex2D(_MainTex, tc);
				fixed4 col;
				col.rgb = map_base.rgb * IN.pack2.rgb;
				col.a = map_base.a * al * _Color.a * IN.pack2.a;
				return col;
			}

			ENDCG
	 	}
	 }
}
