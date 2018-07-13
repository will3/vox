Shader "Unlit/voxelunlit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
        Tags { "RenderType"="Opaque" }
		//Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 100
        // Blend SrcAlpha OneMinusSrcAlpha

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
                float4 color : COLOR;
                float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
                o.color = v.color;
                o.normal = v.normal;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{         
                float4 lightColor1 = float4(255 / 255.0, 244 / 255.0, 214 / 255.0, 1.0);
                float4 color = tex2D(_MainTex, i.uv) * i.color;
                float3 light1 = normalize(float3(1.0, 1.0, 1.0));
                float ratio1 = clamp( dot(i.normal, light1), 0, 1);
                ratio1 = 1 - ((1 - ratio1) * 0.5);
                float4 color1 = color * ratio1 * lightColor1;

                float ambientStrength = 0.5;
                float4 ambient = float4(1.0, 1.0, 1.0, 1.0) * color * ambientStrength;

                // sample the texture
                fixed4 col = color1 + ambient;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
                
				return col;
			}
			ENDCG
		}
	}
}
