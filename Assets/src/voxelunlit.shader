Shader "Unlit/voxelunlit"
{
	Properties
	{
		
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);            
				UNITY_TRANSFER_FOG(o,o.vertex);
                o.color = v.color;
                o.normal = v.normal;
                o.uv = v.uv;
				return o;
			}

            float getWaterfall(float waterfall, float total, float mid, float speed, float offset, float bubbleWidth, float intensity) {
                float waterfallV = ((waterfall - _Time[0] * speed + offset) % total);
                if (waterfallV < 0) {
                    waterfallV += total;
                }
                float v = mid - waterfallV;
                if (v < 0) {
                    return 1.0;
                }
                if (v < bubbleWidth) {
                    return (1 + ((bubbleWidth - v) / bubbleWidth) * intensity);
                }
                return 1.0;
            }
			fixed4 frag (v2f i) : SV_Target
			{         
                float4 color = i.color;
                float4 lightColor = float4(255 / 255.0, 244 / 255.0, 214 / 255.0, 1.0);
                float4 diffuse;

                //float3 light = normalize(float3(1.0, 1.0, 1.0));
                //float ratio = clamp( dot(i.normal, light), 0, 1);
                //ratio = 1 - ((1 - ratio) * 0.5);
                //diffuse = color * ratio * lightColor;

                diffuse = color * lightColor;

                float ambientStrength = 0.5;
                float4 ambient = float4(1.0, 1.0, 1.0, 1.0) * color * ambientStrength;

                // sample the texture
                fixed4 col = diffuse + ambient;

                float waterfall = i.uv.x;

                if (waterfall > 0) {
                    // float random = sin(waterfall * 1000);
                    // random -= 0.5;
                    // random *= 0.2;

                    float factor = 
                    getWaterfall(waterfall, 13.0, 5.0, 50, 0, 1.0, 1.5) + 
                    getWaterfall(waterfall, 37.0, 6.0, 75, 3.0, 1.5, 1.0);

                    factor /= 2.0;

                    if (factor > 2) {
                        factor = 2;
                    }

                    col *= factor;
                }

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
                
				return col;
			}
			ENDCG
		}
	}
}
