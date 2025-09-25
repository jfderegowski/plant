Shader "Custom/GPUPlantsID"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZWrite Off
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4x4 _VP;

            struct SegmentData {
                float2 pos; float rot; float scale;
                float4 uvRect;
                uint plantId; uint segId;
                float z; uint flags;
            };

            StructuredBuffer<SegmentData> _Segments;

            struct appdata { float3 pos:POSITION; float2 uv:TEXCOORD0; uint iid:SV_InstanceID; };
            struct v2f { float4 pos:SV_POSITION; uint plantIdLow:COLOR0; uint plantIdHigh:COLOR1; uint segId:COLOR2; };

            v2f vert (appdata v)
            {
                SegmentData s = _Segments[v.iid];
                float c = cos(s.rot), si = sin(s.rot);
                float2 p = v.pos.xy * s.scale;
                float2 r = float2(p.x*c - p.y*si, p.x*si + p.y*c);
                float3 world = float3(s.pos + r, s.z);

                v2f o;
                o.pos = mul(_VP, float4(world, 1));
                uint plantPlus = s.plantId + 1u;
                o.plantIdLow  = plantPlus & 255u;
                o.plantIdHigh = (plantPlus >> 8) & 255u;
                o.segId       = s.segId + 1u;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Zakodowane 8-bitowe kanały (0=brak trafienia)
                return fixed4(
                    (i.segId) / 255.0,
                    (i.plantIdLow) / 255.0,
                    (i.plantIdHigh) / 255.0,
                    1
                );
            }
            ENDHLSL
        }
    }
}
