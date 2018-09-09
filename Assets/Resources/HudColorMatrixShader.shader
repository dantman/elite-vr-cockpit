Shader "Hidden/HudColorMatrixShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4x4 _Matrix;

			fixed4 frag (v2f_img i) : SV_Target
			{
				fixed4 s = tex2D(_MainTex, i.uv);
				fixed4 d = mul(_Matrix, s);

				return d;
			}
			ENDCG
		}
	}
}
