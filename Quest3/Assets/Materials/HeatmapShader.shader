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

        _Diameter("Blob Diameter (0..1)  (NOTE: acts as radius unless toggle below)", Range(0.001,1)) = 0.06
        [Toggle(USE_TRUE_DIAMETER)] _UseTrueDiameter("Treat value above as true diameter", Float) = 0
        _FalloffPower("Falloff Power (sharper > 2)", Range(1,16)) = 3.87
        _Strength("Strength", Range(0.01,8)) = 4
        _PulseSpeed("Pulse Speed", Range(0,5)) = 0

        // Mapping from local position to [0..1]^2
        [Toggle(USE_XY)] _UseXY("Use XY instead of XZ", Float) = 0
        _LocalMin("Local Min (x,y or x,z)", Vector) = (-0.5, -0.5, 0, 0)
        _LocalMax("Local Max (x,y or x,z)", Vector) = ( 0.5,  0.5, 0, 0)

        // Transparency controls
        _AlphaBoost("Alpha Boost", Range(0,4)) = 0.92
        _AlphaCutoff("Alpha Cutoff (hide base)", Range(0,1)) = 0

        // Outline
        _OutlineThickness("Outline Thickness (in UV)", Range(0.0005, 0.05)) = 0.003
        _OutlineSoftness("Outline Softness", Range(0.0001, 0.05)) = 0.0015
        _OutlineColor("Outline Color", Color) = (0,0,0,1)

 // Center lines (vertical in 0..1 UV)
        [Toggle] _LineEnabled("Enable Center Lines", Float) = 1
        _LineX1("Line 1 X (0..1)", Range(0,1)) = 0.342
        _LineX2("Line 2 X (0..1)", Range(0,1)) = 0.624
         _LineX3("Line 3 X (0..1)", Range(0,1)) = 0.5
        _LineThickness("Line Thickness (UV)", Range(0.0001, 0.02)) = 0.002
        _LineSoftness("Line Softness (UV)", Range(0.0000, 0.02)) = 0.0008
        _LineColor("Line Color", Color) = (1,1,1,1)


        ///////////////////////////////////////////

        _HaloScale("Halo Radius Multiplier", Range(1,10)) = 7.6
        _HaloFactor("Halo Strength (relative)", Range(0,2)) = 1.24
        _HaloPower("Halo Falloff Power", Range(1,16)) = 4.5

        //===========================//
         // Smooth blend options
        [Toggle(USE_GAUSS)] _UseGaussian("Use Gaussian Falloff", Float) = 1
        _GaussSharpness("Gaussian Sharpness", Range(0.5,10)) = 4.98
        _WeightScale("Weight Scale (lower = less red)", Range(0.05,2)) = 0.346

        //===========================//
        //////////////////////////////////////////

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

            // XR / instancing support
            #pragma multi_compile_instancing
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float3 localPos : TEXCOORD1;
                float2 uv01     : TEXCOORD2; // mapped 0..1 uv for outline & heat
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // Ramp
            float4 _Color0, _Color1, _Color2, _Color3, _Color4;
            float  _Range0, _Range1, _Range2, _Range3, _Range4;

            // Heat params
            float  _Diameter, _Strength, _PulseSpeed;
            float  _UseTrueDiameter;
            float  _FalloffPower;
            // Hits
            float  _Hits[300];
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

            // Center lines
            float  _LineEnabled;
            float  _LineX1, _LineX2, _LineX3;
            float  _LineThickness;
            float  _LineSoftness;
            float4 _LineColor;
            // soft circular falloff normalized by _Diameter

            /////////////////////////////////
            float  _HaloScale;
            float  _HaloFactor;
            float  _HaloPower;
            //===========================//
            float  _UseGaussian;
            float  _GaussSharpness;
            float _WeightScale;

            //===========================//
            /////////////////////////////////

            // float distsq(float2 a, float2 b)
            // {
            //     // distance based normalized radius
            //     float radius = (_UseTrueDiameter > 0.5) ? max(1e-6, _Diameter * 0.5) : max(1e-6, _Diameter);
            //     float r = distance(a, b) / radius;          // r = 1 at edge of influence
            //     float v = max(0.0, 1.0 - r);                // linear falloff to 0
            //     return pow(v, _FalloffPower);               // adjustable sharpness
            // }

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
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


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

                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                float2 uv = i.uv01;
                float pulse = (_PulseSpeed > 0.0) ? (0.5 + 0.5 * sin(_Time.y * _PulseSpeed)) : 1.0;

                float baseRadius = (_UseTrueDiameter > 0.5) ? max(1e-6, _Diameter * 0.5) : max(1e-6, _Diameter);

                float weight = 0.0;
                int count = min(_HitCount, 100);   // 300 floats / 3 per hit
                float radius = (_UseTrueDiameter > 0.5) ? max(1e-6, _Diameter * 0.5) : max(1e-6, _Diameter);
                float invR2 = 1.0 / (radius * radius);
                [loop]
                for (int j = 0; j < count; j++)
                {
                    float2 pt = float2(_Hits[j * 3 + 0], _Hits[j * 3 + 1]);
                    float intensity = _Hits[j * 3 + 2];

                    //////////////////////////////////
                    // float perRadiusScale = saturate(intensity);
                    // perRadiusScale = max(0.05, perRadiusScale); // prevent zero
                    //////////////////////////////////
                    // float radiusScale = 1.0; // ini berhasil terakhir
                     float2 d = uv - pt;
                    // float r = distance(uv, pt) / baseRadius;
                    // float r = length(d) / (baseRadius * radiusScale); // ini berhasil terakhir
                   
                    float r2 = dot(d,d) * invR2;
                    float r = sqrt(r2);
                    // float core = pow(saturate(1.0 - r), _FalloffPower);
                    // float core; // ini berhasil terakhir
                    float core = exp(-_GaussSharpness * r2);
                    
                    //===========================//
                    // if (_UseGaussian > 0.5)
                    // {
                    //     // Gaussian falloff
                    //     float gaussR = r * _GaussSharpness;
                    //     core = exp(-gaussR * gaussR);
                    // }
                    // else
                    // {
                    //     // power falloff
                    //     core = pow(saturate(1.0 - r), _FalloffPower);
                    // }
                    //===========================//


                    float halo = 0.0;
                    if (_HaloScale > 1.01 && _HaloFactor > 0.001)
                    {
                        float rHalo = r / _HaloScale;
                        halo = pow(saturate(1.0 - rHalo), _HaloPower) * _HaloFactor;
                    }

                    // weight += (core + halo) * intensity * _Strength * pulse;
                    // weight += (core + halo) * _Strength * pulse;
                    // Optional early out (slightly divergent, skip if you prefer)

                    weight += (core + halo) * intensity * _Strength * pulse;
                    // if (weight > _Range4 * 1.2) break;
                }

                // weight = min(weight, _Range4 * 1.2);
                weight *= _WeightScale;
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

                // Center vertical lines (in uv01 space)
                float lineMask = 0.0;
                if (_LineEnabled > 0.5)
                {
                    float dx1 = abs(uv.x - _LineX1);
                    float dx2 = abs(uv.x - _LineX2);
                    float dx3 = abs(uv.x - _LineX3);
                    float l1 = 1.0 - smoothstep(_LineThickness, _LineThickness + _LineSoftness, dx1);
                    float l2 = 1.0 - smoothstep(_LineThickness, _LineThickness + _LineSoftness, dx2);
                    float l3 = 1.0 - smoothstep(_LineThickness, _LineThickness + _LineSoftness, dx3);
                    lineMask = saturate(max(max(l1, l2), l3));
                }

                // Overlay lines above heat/outline
                outRgb = lerp(outRgb, _LineColor.rgb, lineMask * _LineColor.a);
                outA   = max(outA, lineMask * _LineColor.a);


                return float4(outRgb, outA);
            }
            ENDCG
        }
    }
}
