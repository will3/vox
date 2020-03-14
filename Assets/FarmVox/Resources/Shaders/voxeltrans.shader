Shader "Unlit/voxeltrans"
{
    Properties
    {
        
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"
            #include "vision.cginc"
            #include "shadow.cginc"
            
            float3 _Origin;
            int _Size;
            float _ShadowStrength;

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

            float _VisionRange;
            float3 _PlayerPosition;
            float _VisionGridSize;
            float _VisionBlurRange;

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

            int3 getCoord(int index) {
                VoxelData data = _VoxelData[index];
                return data.coord;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);            
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.normal = v.normal;
                o.uv = v.uv;

                int index = floor(v.uv.x);
                int3 coord = getCoord(index);

                float4 c = v.color;

                int3 worldCoord = coord + floor(_Origin);
                
                o.color = c;

                float shadowHeight = getVoxelShadow(coord, _ShadowMapSize, _LightDir, 
                    _ShadowMap00,
                    _ShadowMap01,
                    _ShadowMap10,
                    _ShadowMap11);
                float shadow = shadowHeight > worldCoord.y ? 1.0 : 0;
                o.color.xyz *= 1 - shadow * _ShadowStrength;

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
