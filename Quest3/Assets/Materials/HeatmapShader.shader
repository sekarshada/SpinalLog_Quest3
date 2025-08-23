Shader "Unlit/HeatmapObjectSpace_TransparentOutline"
{
    Properties
    {
        // Color ramp (keep these as your green/yellow/orange/red)
        _Color0("Color 0 (base/unused)", Color) = (0,0,0,1) // will be transparent at low weight
        _Color1("Color 1", Color) = (0,1,0,1)
        _Color2("Color 2", Color) = (1,1,0,1)
        _Color3("Color 3", Color) = (1,0.5,0,1)
        _Color4("Color 4", Color) = (1,0,0,1)

        _Range0("Range 0", Range(0,1)) = 0
        _Range1("Range 1", Range(0,1)) = 0.01
        _Range2("Range 2", Range(0,1)) = 0.2
        _Range3("Range 3", Range(0,1)) = 0.35
        _Range4("Range 4", Range(0,1)) = 0.6

        _Diameter("Blob Diameter (0..1)", Range(0.01,1)) = 0.012
        _Strength("Strength", Range(0.01,8)) = 0.7
        _PulseSpeed("Pulse Speed", Range(0,5)) = 0

        // Mapping from local position to [0..1]^2
        [Toggle(USE_XY)] _UseXY("Use XY instead of XZ", Float) = 0
        _LocalMin("Local Min (x,y or x,z)", Vector) = (-0.5, -0.5, 0, 0)
        _LocalMax("Local Max (x,y or x,z)", Vector) = ( 0.5,  0.5, 0, 0)

        // Transparency controls
        _AlphaBoost("Alpha Boost", Range(0,4)) = 1.0
        _AlphaCutoff("Alpha Cutoff (hide base)", Range(0,1)) = 0.02

        // Outline
        _OutlineThickness("Outline Thickness (in UV)", Range(0.0005, 0.05)) = 0.003
        _OutlineSoftness("Outline Softness", Range(0.0001, 0.05)) = 0.0015
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
    }

    SubShader
    {
        // Make it transparent and avoid depth writes so blending works
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Cull Off ZWrite Off ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile __ USE_XY
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float3 localPos : TEXCOORD1;
                float2 uv01     : TEXCOORD2; // mapped 0..1 uv for outline & heat
            };

            // Ramp
            float4 _Color0, _Color1, _Color2, _Color3, _Color4;
            float  _Range0, _Range1, _Range2, _Range3, _Range4;

            // Heat params
            float  _Diameter, _Strength, _PulseSpeed;

            // Hits
            float  _Hits[192];
            int    _HitCount;

            float4 _LocalMin;
            float4 _LocalMax;

            // Transparency controls
            float  _AlphaBoost;
            float  _AlphaCutoff;

            // Outline
            float  _OutlineThickness;
            float  _OutlineSoftness;
            float4 _OutlineColor;

            // soft circular falloff normalized by _Diameter
            float distsq(float2 a, float2 b)
            {
                float r = distance(a, b) / _Diameter;
                return pow(max(0.0, 1.0 - r), 2.0);
            }

            float3 heatColor(float w)
            {
                if (w <= _Range0) return _Color0.rgb;
                if (w >= _Range4) return _Color4.rgb;

                float ranges[5]  = { _Range0, _Range1, _Range2, _Range3, _Range4 };
                float3 colors[5] = { _Color0.rgb, _Color1.rgb, _Color2.rgb, _Color3.rgb, _Color4.rgb };

                [unroll]
                for (int i = 1; i < 5; i++)
                {
                    if (w < ranges[i])
                    {
                        float t = saturate((w - ranges[i-1]) / max(1e-6, (ranges[i] - ranges[i-1])));
                        return lerp(colors[i-1], colors[i], t);
                    }
                }
                return _Color0.rgb;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos      = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xyz;

                // Map to 0..1 domain here once; reuse in frag
                float2 p;
                #if defined(USE_XY)
                    p = float2(o.localPos.x, o.localPos.y);
                    float2 mn = _LocalMin.xy;
                    float2 mx = _LocalMax.xy;
                #else
                    p = float2(o.localPos.x, o.localPos.z);
                    float2 mn = _LocalMin.xy;
                    float2 mx = _LocalMax.xy;
                #endif

                float2 uv01 = (p - mn) / max(float2(1e-6,1e-6), (mx - mn));
                o.uv01 = saturate(uv01);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv01;

                // Optional pulsing
                float pulse = (_PulseSpeed > 0.0) ? (0.5 + 0.5 * sin(_Time.y * _PulseSpeed)) : 1.0;

                // Accumulate heat
                float weight = 0.0;
                [loop]
                for (int j = 0; j < _HitCount; j++)
                {
                    float2 pt = float2(_Hits[j * 3 + 0], _Hits[j * 3 + 1]);
                    float intensity = _Hits[j * 3 + 2];
                    weight += distsq(uv, pt) * intensity * _Strength * pulse;
                }
                weight = clamp(weight, 0.0, _Range4);

                // Color from ramp (green->yellow->red); base (near zero) will be invisible via alpha below
                float3 heatRgb = heatColor(weight);

                // Alpha: grow from 0 as weight rises, boosted and cutoff to hide base
                float alpha = saturate(_AlphaBoost * ((weight - _Range0) / max(1e-6, (_Range4 - _Range0))));
                // Hard cutoff to ensure “base is transparent”
                alpha *= step(_AlphaCutoff, alpha);

                // --- Outline (object-space border in 0..1 uv) ---
                // Distance to nearest edge (0 at edge, 0.5 at center)
                float dEdge = min(min(uv.x, uv.y), min(1.0 - uv.x, 1.0 - uv.y));
                // Soft edge mask: 1 near border, 0 interior
                float outlineMask = 1.0 - smoothstep(_OutlineThickness, _OutlineThickness + _OutlineSoftness, dEdge);

                // Combine: draw outline over heat
                float3 outRgb = lerp(heatRgb, _OutlineColor.rgb, outlineMask);
                float outA    = max(alpha, outlineMask * _OutlineColor.a);

                return float4(outRgb, outA);
            }
            ENDCG
        }
    }
}
