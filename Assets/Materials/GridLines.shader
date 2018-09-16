Shader "Unlit/GridLines"
{
	Properties
	{
		_MainTex ("Mask", 2D) = "white" {}
		_Color ("Grid Line Color", Color) = (1, 1, 1, 1)
		_Interval ("Interval", float) = 1
		_Thickness ("Thickness", float) = 0.1
		_ZeroColor ("Zero Line Color", Color) = (1, 1, 1, 1)
		_ZeroThickness ("Zero Line Thickness", float) = 0.1
	}
	SubShader
	{
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		LOD 100
		
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			fixed4 _ZeroColor;
			fixed4 transparent = fixed4(0, 0, 0, 0);
			float _Interval;
			float _Thickness;
			float _ZeroThickness;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 mask = tex2D(_MainTex, i.uv);
				fixed4 col = transparent;

				float2 p = i.uv % _Interval;
				if (p.y > _Interval / 2) p.y -= _Interval;
				if (p.x > _Interval / 2) p.x -= _Interval;

				float2 z = i.uv - 0.5;

				if (abs(p.x) <= _Thickness / 2 || abs(p.y) <= _Thickness / 2) {
					col = _Color;
				}
				if (abs(z.x) <= _ZeroThickness / 2 || abs(z.y) <= _ZeroThickness / 2) {
					col = _ZeroColor;
				}

				col *= mask;

				return col;
			}
			ENDCG
		}
	}
}
