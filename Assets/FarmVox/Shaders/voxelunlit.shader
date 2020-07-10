Shader "Unlit/voxelunlit"
{
    Properties
    {
        
    }
    SubShader
    {
        LOD 200
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            
            #include "UnityCG.cginc"
            #include "vision.cginc"
            #include "AutoLight.cginc"
            #include "shadow.cginc"
            
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
            int _NormalBanding;
            float _NormalStrength;

            struct VoxelData {
                int3 coord;
                float3 normal;
            };
            
            StructuredBuffer<VoxelData> _VoxelData;
            
            // 11 10 
            // 01 00 <- chunk
            StructuredBuffer<int> _ShadowMap00;
            StructuredBuffer<int> _ShadowMap01;
            StructuredBuffer<int> _ShadowMap10;
            StructuredBuffer<int> _ShadowMap11;
            int _ShadowMapSize;
            float3 _LightDir;

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
                float4 pos : SV_POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD1;
                LIGHTING_COORDS(2,3)
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                o.uv = v.uv;

                int index = floor(v.uv.x);
                float waterfall = v.uv.y;
                
                VoxelData voxelData = _VoxelData[index]; 
                int3 coord = voxelData.coord;
                float3 normal = voxelData.normal;

                float4 c = v.color;

                int3 worldCoord = coord + floor(_Origin);
                
                float dif = clamp(dot(normal, _LightDir * -1), 0, 1);

                if (_NormalBanding != 0.0) {
                    dif = floor(dif * _NormalBanding) / _NormalBanding;
                }

                dif = 1 - (1 - dif) * _NormalStrength;

                o.color = c * dif;

                float shadowHeight = getVoxelShadow(coord, _ShadowMapSize, _LightDir, 
                    _ShadowMap00,
                    _ShadowMap01,
                    _ShadowMap10,
                    _ShadowMap11);
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
                
                TRANSFER_VERTEX_TO_FRAGMENT(o);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 color = i.color;
                float4 lightColor = float4(255 / 255.0, 244 / 255.0, 214 / 255.0, 1.0);
                float4 diffuse;

                fixed atten = LIGHT_ATTENUATION(i);
                diffuse = color * lightColor * atten;

                float ambientStrength = 0.5;
                float4 ambient = float4(1.0, 1.0, 1.0, 1.0) * color * ambientStrength;

                // sample the texture
                fixed4 col = diffuse + ambient;

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
    FallBack "Diffuse"
}
