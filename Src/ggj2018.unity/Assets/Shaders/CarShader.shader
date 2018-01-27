Shader "Unlit/CarShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_WhiteDetect("White Factor", Range(0.01, 1)) = 0.9
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			float _WhiteDetect;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				if (col.r > _WhiteDetect && col.g > _WhiteDetect && col.b > _WhiteDetect)
				{
					col.r = 1;
					col.g = 1;
					col.b = 1;
				}
				else
				{
					col.rgb = _TintColor.rgb;
				}

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
