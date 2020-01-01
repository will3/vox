Shader "Unlit/buildingTile"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Gap ("Gap", Float) = 0.4
        _GridSize ("GridSize", Float) = 3
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

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _Color;
            float _Gap;
            float _GridSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float x = i.worldPos.x;
                float z = i.worldPos.z;
                if (abs(floor(x / _GridSize) * _GridSize - x) < _Gap ||
                    abs(floor(z / _GridSize) * _GridSize - z) < _Gap) {
                    discard;
                }

                return _Color;
            }
            ENDCG
        }
    }
}
