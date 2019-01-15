Shader "Unlit/voxelmodel"
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
            
            float3 _Origin;
            int _Size;

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
                
                return col;
            }
            ENDCG
        }
    }
}
