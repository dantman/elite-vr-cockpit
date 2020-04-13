Shader "Unlit/HUDPanelBackground"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
		_Color ("Highlight Color", Color) = (1, 1, 1, 1)
		_Alpha ("Opacity", Range(0, 1)) = 1
		_Interval ("Interval", Range(0, 1)) = 1
		_FadeInsetX ("Horizontal Fade Inset", Range(0, 1)) = 0
		_FadeInsetY ("Vertical Fade Inset", Range(0, 1)) = 0
    }
    SubShader
    {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			// #pragma enable_d3d11_debug_symbols

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

            //sampler2D _MainTex;
            //float4 _MainTex_ST;
			fixed4 _Color;
			float _Alpha;
			float _Interval;
			float _FadeInsetX;
			float _FadeInsetY;

			static const float PI = 3.14159265f;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                // o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				// fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				//return col;
				//fixed4 col = fixed4(1, 1, 1, 1);
				fixed4 bg = fixed4(0, 0, 0, 1);
				fixed4 col = bg;
				// col[0] = i.uv.x > _FadeInsetX ? 1 : 0;

				float intensity = sin((i.uv.y) * (1 / _Interval) * (PI*2) - (PI/2)) / 2 + .5;
				col = _Color * intensity;

				// Fade out the edges
				fixed2 inset = abs((i.uv * 2) - 1);
				fixed2 fadeInset =  fixed2(_FadeInsetX, _FadeInsetY);
				fixed2 fadeOffset = 1 - fadeInset;

				fixed2 fadeBorder = (inset - fadeOffset) * (1 / fadeInset);
				fadeBorder = 1 - fadeBorder;

				// Debug
				//col[0] = max(fadeBorder.x, fadeBorder.y) > 0 ? 1 : 0;
				//col[1] = max(fadeBorder.x, fadeBorder.y) > .5 ? 1 : 0;
				//col[2] = max(fadeBorder.x, fadeBorder.y) > .9 ? 1 : 0;

				// @fixme The min(1, ...) is to cap it, why does it go over 1?
				// col[3] = _Alpha * max(0, min(1, min(fadeBorder.x, fadeBorder.y)));
				col[3] = _Alpha * min(max(0, min(1, fadeBorder.x)), max(0, min(1, fadeBorder.y)));
				// col[3] = _Alpha * max(0, min(1, (((fadeBorder.x) * (fadeBorder.y)))));

				return col;
            }
            ENDCG
        }
    }
}
