Shader "Unlit/HeatmapObjectSpace"
{
    Properties
    {
        // Color ramp
        _Color0("Color 0", Color) = (0,0,0,1)
        _Color1("Color 1", Color) = (0,1,0,1)
        _Color2("Color 2", Color) = (1,1,0,1)
        _Color3("Color 3", Color) = (1,0.5,0,1)
        _Color4("Color 4", Color) = (1,0,0,1)

        _Range0("Range 0", Range(0,1)) = 0
        _Range1("Range 1", Range(0,1)) = 0.01
        _Range2("Range 2", Range(0,1)) = 0.2
        _Range3("Range 3", Range(0,1)) = 0.35
        _Range4("Range 4", Range(0,1)) = 0.6

        _Diameter("Blob Diameter (0..1)", Range(0.01,1)) = 0.02
        _Strength("Strength", Range(0.01,8)) = 0.7
        _PulseSpeed("Pulse Speed", Range(0,5)) = 0

        // === Mapping from local position to [0..1]x[0..1] ===
        // If your mesh is a Unity Quad (XY, size 1): useXY = ON, min = (-0.5,-0.5), max = (0.5,0.5)
        // If your mesh is a Unity Plane (XZ, size 10): useXY = OFF, min = (-5,-5), max = (5,5)
        [Toggle(USE_XY)] _UseXY("Use XY instead of XZ", Float) = 0
        _LocalMin("Local Min (x,y or x,z)", Vector) = (-0.5, -0.5, 0, 0)
        _LocalMax("Local Max (x,y or x,z)", Vector) = ( 0.5,  0.5, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off ZWrite On ZTest LEqual

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
                float2 uv     : TEXCOORD0; // not used but kept for compatibility
            };

            struct v2f
            {
                float4 pos       : SV_POSITION;
                float3 localPos  : TEXCOORD1; // object-space position
            };

            // Ramp
            float4 _Color0, _Color1, _Color2, _Color3, _Color4;
            float  _Range0, _Range1, _Range2, _Range3, _Range4;

            // Heat params
            float  _Diameter, _Strength, _PulseSpeed;

            // Hit buffer: up to 32 hits => 32*3 = 96 floats (we store 192 for alignment with original)
            float  _Hits[192];   // (x, y, intensity) triplets in [0..1]
            int    _HitCount;

            float4 _LocalMin;    // (minX, minY or minZ)
            float4 _LocalMax;    // (maxX, maxY or maxZ)

            v2f vert (appdata v)
            {
                v2f o;
                o.pos      = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xyz; // object space (before transform)
                return o;
            }

            // soft circular falloff normalized by _Diameter (in [0..1] uv space)
            float distsq(float2 a, float2 b)
            {
                float r = distance(a, b) / _Diameter;
                return pow(max(0.0, 1.0 - r), 2.0);
            }

            float3 heatColor(float w)
            {
                if (w <= _Range0) return _Color0.rgb;
                if (w >= _Range4) return _Color4.rgb;

                float ranges[5] = { _Range0, _Range1, _Range2, _Range3, _Range4 };
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

            fixed4 frag (v2f i) : SV_Target
            {
                // Pick axes and normalize to [0..1]^2 based on user-specified local bounds
                float2 p;
                #if defined(USE_XY)
                    p = float2(i.localPos.x, i.localPos.y);
                    float2 mn = _LocalMin.xy;
                    float2 mx = _LocalMax.xy;
                #else
                    p = float2(i.localPos.x, i.localPos.z);
                    float2 mn = _LocalMin.xy; // still use xy components
                    float2 mx = _LocalMax.xy;
                #endif

                float2 uv = (p - mn) / max(float2(1e-6,1e-6), (mx - mn));
                uv = saturate(uv); // clamp to the 0..1 domain

                // Optional pulsing (kept for parity; set _PulseSpeed > 0 to use)
                float pulse = (_PulseSpeed > 0.0) ? (0.5 + 0.5 * sin(_Time.y * _PulseSpeed)) : 1.0;

                // Accumulate contributions
                float weight = 0.0;
                [loop]
                for (int j = 0; j < _HitCount; j++)
                {
                    float2 pt = float2(_Hits[j * 3 + 0], _Hits[j * 3 + 1]); // normalized sensor coords
                    float intensity = _Hits[j * 3 + 2];
                    weight += distsq(uv, pt) * intensity * _Strength * pulse;
                }

                weight = clamp(weight, 0.0, _Range4); // keep inside ramp range
                float3 col = heatColor(weight);
                return float4(col, 1.0);
            }
            ENDCG
        }
    }
}
