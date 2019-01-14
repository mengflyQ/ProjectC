// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ZDStudio/技能预警/技能提醒-矩形"
{
	 Properties
	 {
	     _Color ("色调", Color) = (1,1,1,1)
	     _MainTex ("主贴图", 2D) = "white" {}
         _A ("身头比(UV)", Range(0.001, 1)) = 0.5
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
			half4 _MainTex_ST;
			fixed _A;

			struct v2f_surf
			{
				float4 pos	    : SV_POSITION;
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
				//tc.y = 1.0f - tc.y;

				fixed fMid = 0.5f;

				fixed fMax1 = max(tc.y - _A, 0.0f);
				float v1 = (fMax1 - fMid *fMax1) / (1.0f - _A);
				fixed fMax2 = max(_A - tc.y, 0.0f);
				float v2 = fMid - fMid * fMax2 / _A;

				float v = v1 + v2;
				tc.y = v;

				fixed4 map_base = tex2D(_MainTex, tc);
				fixed4 col;
				col.rgb = map_base.rgb * IN.pack2.rgb * _Color.rgb;
				col.a = map_base.a * IN.pack2.a * _Color.a;
				return col;
			}

			ENDCG
	 	}
	 }
}
