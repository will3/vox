Shader "Unlit/voxelunlit"
{
    Properties
    {
        
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
            #include "vision.cginc"
            
            float3 _Origin;
            int _Size;
            float _ShadowStrength;
            float _WaterfallShadowStrength;
            float _WaterfallSpeed;
            float _WaterfallWidth;
            float _WaterfallMin;
            float _WaterfallVariance;
            
            float _VisionRange;
            float3 _PlayerPosition;
            float _VisionGridSize;
            float _VisionBlurRange;

            struct VoxelData {
                int3 coord;
            };
            
            StructuredBuffer<VoxelData> _VoxelData;
            
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
                float3 worldPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);            
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.normal = v.normal;
                o.uv = v.uv;

                int index = floor(v.uv.x);
                float waterfall = v.uv.y;
                
                VoxelData voxelData = _VoxelData[index]; 
                int3 coord = voxelData.coord;

                float4 c = v.color;

                int3 worldCoord = coord + floor(_Origin);
                
                o.color = c;

                float shadowHeight = getShadow(coord);
                float shadow = shadowHeight > worldCoord.y ? 1.0 : 0;
                
                if (waterfall > 0) {
                    o.color.xyz *= 1 - shadow * _WaterfallShadowStrength;
                } else {
                    o.color.xyz *= 1 - shadow * _ShadowStrength;
                }
                
                float time = _Time;
                
                if (waterfall > 0) {
                    float waterfallRatio = (waterfall / _WaterfallWidth - time * _WaterfallSpeed) % 1.0;
                    
                    if (waterfallRatio < 0.0) {
                        waterfallRatio += 1.0;
                    }
                    
                    waterfallRatio *= _WaterfallVariance;
                    waterfallRatio += _WaterfallMin;
                    
                    o.color *= waterfallRatio;
                }

                if (shadowHeight == 99) {
                    o.color = float4(1.0, 0, 0, 1.0);
                    return o;
                }
                
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                return o;
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

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                float vision = getVision(
                    i.worldPos, 
                    _PlayerPosition, 
                    _VisionRange, 
                    _VisionGridSize, 
                    _VisionBlurRange);
                
                if (vision == 0) {
                    discard;
                }
                
                col *= vision;
                
                return col;
            }
            ENDCG
        }
    }
}
