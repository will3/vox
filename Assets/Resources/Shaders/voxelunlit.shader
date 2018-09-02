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


            struct Vision {
                float x;
                float z;
                float radius;
            };

            StructuredBuffer<Vision> _VisionBuffer;
            int _MaxVisionNumber;
            int _UseVision;
            float3 _Origin;
            int _Size;
            float _ShadowStrength;
            int _ShowGrid;

            // 11 10 
            // 01 00 <- chunk
            StructuredBuffer<int> _ShadowMap00;
            StructuredBuffer<int> _ShadowMap01;
            StructuredBuffer<int> _ShadowMap10;
            StructuredBuffer<int> _ShadowMap11;
            int _ShadowMapSize;

            int getShadow(int3 coord) {
                int x = coord.x - coord.y;
                int z = coord.z - coord.y;

                int i = 0;
                int j = 0;
                int u = x;
                int v = z;
                int size = _ShadowMapSize;

                if (x < 0) {
                    i = 1;
                    u += size;
                } 

                if (z < 0) {
                    j = 1;
                    v += size;
                }

                if (u < 0 || u >= size || v < 0 || v >= size) {
                    //return 99;
                }

                int index = u * size + v;

                if (i == 0) {
                    if (j == 0) {
                        return _ShadowMap00[index];
                    } else if (j == 1) {
                        return _ShadowMap01[index];
                    }
                } else if (i == 1) {
                    if (j == 0) {
                        return _ShadowMap10[index];
                    } else if (j == 1) {
                        return _ShadowMap11[index];
                    }
                }

                return 99;
            }

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

            float getVision(float3 worldCoord) {
                float amount = 0;
                float visionBlur = 33;

                // pos banding
                worldCoord /= 2.0;
                worldCoord = floor(worldCoord);
                worldCoord *= 2.0;

                for (int i = 0; i < _MaxVisionNumber; i++) {
                    Vision vision = _VisionBuffer[i];
                    // Break signal
                    if (vision.radius == 0) {
                        break;
                    }

                    float dx = vision.x - worldCoord.x;
                    float dz = vision.z - worldCoord.z;

                    float dis = sqrt(dx * dx + dz * dz);

                    if (dis < (vision.radius - visionBlur)) {
                        amount += 1.0;
                    } else if (dis < vision.radius) {
                        amount += (vision.radius - dis) / visionBlur;
                    }
                }
                if (amount > 1.0) {
                    amount = 1.0;
                }

                // amount banding
                // amount *= 4;
                // amount = floor(amount);
                // amount /= 4;

                return amount;
            }

            int3 getCoord(int index) {
                int x = floor(index / 35.0 / 35.0);
                index -= x * 35 * 35;
                int y = floor(index / 35);
                int z = index % 35;

                return int3(x, y, z);
            }
            
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);            
				UNITY_TRANSFER_FOG(o,o.vertex);
                o.normal = v.normal;
                o.uv = v.uv;

                int index = floor(v.uv.y);
                int3 coord = getCoord(index);

                float4 c = v.color;

                if (coord.y < 16) {
                    // o.color = float4(0.4, 0, 0, 1.0);
                    // return o;
                }

                int3 worldCoord = coord + floor(_Origin);
                
                if (_UseVision > 0) {
                    float vision = getVision(worldCoord);
                    c *= vision;
                }

                float shadowHeight = getShadow(coord);
                float shadow = shadowHeight > worldCoord.y ? 1.0 : 0;
                c *= 1 - shadow * _ShadowStrength;

                if (shadowHeight == 99) {
                    o.color = float4(1.0, 0, 0, 1.0);
                    return o;
                }
                
                if (_ShowGrid > 0) {
                    int gridSize = 5;
                    if (worldCoord.x % gridSize == 0 || worldCoord.z % gridSize == 0) {
                        
                    } else {
                        c *= 1.4;
                    }
                }
                
                o.color = c;

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{         
                int index = floor(i.uv.y);
                int3 coord = getCoord(index);
                int3 worldCoord = coord + floor(_Origin);

                float4 color = i.color;
                float4 lightColor = float4(255 / 255.0, 244 / 255.0, 214 / 255.0, 1.0);
                float4 diffuse;

                diffuse = color * lightColor;

                float ambientStrength = 0.5;
                float4 ambient = float4(1.0, 1.0, 1.0, 1.0) * color * ambientStrength;

                // sample the texture
                fixed4 col = diffuse + ambient;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

                if (worldCoord.x % 3 == 0 || worldCoord.z % 3 == 0) {
                    // col *= float4(1.0,0.5,0.5, 1.0);
                    // col *= float4(1.2, 1.2, 1.2, 1.0);
                    // col *= float4(0.5, 0.5, 0.5, 1.0);
                } else {
                    // col = float4(103 / 255.0, 234 / 255.0, 19 / 255.0, 1.0) * 0.5 + col * 0.5;
                    // col *= float4(1.0, 1.0, 1.0, 1.0);
                }
                
				return col;
			}
			ENDCG
		}
	}
}
