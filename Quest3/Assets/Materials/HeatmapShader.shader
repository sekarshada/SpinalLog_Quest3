Shader "Unlit/HeatmapShader"
{
    Properties
    {
        _Color0("Color 0", Color) = (0,0,0,1)
        _Color1("Color 1", Color) = (0,1,0,1)
        _Color2("Color 2", Color) = (1,1,0,1)
        _Color3("Color 3", Color) = (1,0.5,0,1)
        _Color4("Color 4", Color) = (1,0,0,1)
        _Range0("Range 0", Range(0,1)) = 0
        _Range1("Range 1", Range(0,1)) = 0.2
        _Range2("Range 2", Range(0,1)) = 0.3
        _Range3("Range 3", Range(0,1)) = 0.4
        _Range4("Range 4", Range(0,1)) = 1
        _Diameter("Diameter", Range(0.1,1)) = 0.4
        _Strength("Strength", Range(0.1,4)) = 1.0
        _PulseSpeed("Pulse Speed", Range(0,5)) = 0
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
            #include "UnityCG.cginc"
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            float4 _Color0, _Color1, _Color2, _Color3, _Color4;
            float _Range0, _Range1, _Range2, _Range3, _Range4;
            float _Diameter, _Strength, _PulseSpeed;
            float _Hits[96];   // 32 * 3
            int _HitCount;
            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            float distsq(float2 a, float2 b) {
                float r = distance(a, b) / _Diameter;
                return pow(max(0.0, 1.0 - r), 2.0);
            }
            float3 heatColor(float w) {
                if (w <= _Range0) return _Color0.rgb;
                if (w >= _Range4) return _Color4.rgb;
                float ranges[5] = { _Range0, _Range1, _Range2, _Range3, _Range4 };
                float3 colors[5] = { _Color0.rgb, _Color1.rgb, _Color2.rgb, _Color3.rgb, _Color4.rgb };
                for (int i = 1; i < 5; i++) {
                    if (w < ranges[i]) {
                        float t = (w - ranges[i-1]) / (ranges[i] - ranges[i-1]);
                        return lerp(colors[i-1], colors[i], t);
                    }
                }
                return _Color0.rgb;
            }
            fixed4 frag(v2f i) : SV_Target {
               float aspect = 11.0 / 15.0; // real fabric height / width ≈ 0.733
                float2 uv = i.uv * 2.0 - 1.0;     // now in [-1, 1]
                uv.y *= aspect;    
                uv.x *= 1.0;              
                float weight = 0.0;
                for (int j = 0; j < _HitCount; j++) {
                    float2 pt = float2(_Hits[j * 3], _Hits[j * 3 + 1]);
                    float intensity = _Hits[j * 3 + 2];
                    weight += distsq(uv, pt) * intensity * _Strength;
                }
                float3 col = heatColor(weight);
                return float4(col, 1.0);
            }
            ENDCG
        }
    }
}









