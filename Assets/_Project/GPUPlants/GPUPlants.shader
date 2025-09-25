Shader "Custom/GPUPlants"
{
    Properties { _MainTex ("Atlas", 2D) = "white" {} }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            float4x4 _VP;

            struct SegmentData {
                float2 pos; float rot; float scale;
                float4 uvRect;
                uint plantId; uint segId;
                float z; uint flags;
            };

            StructuredBuffer<SegmentData> _Segments;
            sampler2D _MainTex;

            struct appdata { float3 pos:POSITION; float2 uv:TEXCOORD0; uint iid:SV_InstanceID; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };

            v2f vert (appdata v)
            {
                SegmentData s = _Segments[v.iid];
                float c = cos(s.rot), si = sin(s.rot);
                float2 p = v.pos.xy * s.scale;
                float2 r = float2(p.x*c - p.y*si, p.x*si + p.y*c);
                float3 world = float3(s.pos + r, s.z);

                v2f o;
                o.pos = mul(_VP, float4(world, 1));
                o.uv  = s.uvRect.xy + v.uv * s.uvRect.zw;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDHLSL
        }
    }
}
