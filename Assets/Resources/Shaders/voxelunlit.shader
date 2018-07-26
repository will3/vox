// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

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
                     
            StructuredBuffer<float> _VisionBuffer;
            float2 _VisionOrigin;
            float _VisionSize;
            int _VisionResolution;

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

            float Epsilon = 1e-10;

            float3 rgb2hcv(in float3 RGB)
            {
                // Based on work by Sam Hocevar and Emil Persson
                float4 P = lerp(float4(RGB.bg, -1.0, 2.0/3.0), float4(RGB.gb, 0.0, -1.0/3.0), step(RGB.b, RGB.g));
                float4 Q = lerp(float4(P.xyw, RGB.r), float4(RGB.r, P.yzx), step(P.x, RGB.r));
                float C = Q.x - min(Q.w, Q.y);
                float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
                return float3(H, C, Q.x);
            }

            float3 rgb2hsl(in float3 RGB)
            {
                float3 HCV = rgb2hcv(RGB);
                float L = HCV.z - HCV.y * 0.5;
                float S = HCV.y / (1 - abs(L * 2 - 1) + Epsilon);
                return float3(HCV.x, S, L);
            }

            float3 hsl2rgb(float3 c)
            {
                c = float3(frac(c.x), clamp(c.yz, 0.0, 1.0));
                float3 rgb = clamp(abs(fmod(c.x * 6.0 + float3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
                return c.z + c.y * (rgb - 0.5) * (1.0 - abs(2.0 * c.z - 1.0));
            }

            float getVision(float3 worldPos) {
                int visionX = floor((worldPos.x - _VisionOrigin.x) / _VisionResolution);
                int visionZ = floor((worldPos.z - _VisionOrigin.y) / _VisionResolution);

                int index = visionX * (_VisionSize / _VisionResolution) + visionZ;
                return _VisionBuffer[index];
            }

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);            
				UNITY_TRANSFER_FOG(o,o.vertex);
                o.normal = v.normal;
                o.uv = v.uv;

                if (_VisionSize > 0) {
                    float3 worldPos = mul (unity_ObjectToWorld, v.vertex).xyz;

                    float vision = getVision(worldPos);

                    //float3 hsl = rgb2hsl(v.color);
                    //hsl.y *= vision;
                    //float lightness = vision * 0.7 + 0.3;
                    //hsl.z *= lightness;
                    //float3 color = hsl2rgb(hsl);
                    //o.color = float4(color.xyz, 1.0);

                    float4 color = v.color * vision;
                    o.color = color;
                } else {
                    o.color = v.color;
                }

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
