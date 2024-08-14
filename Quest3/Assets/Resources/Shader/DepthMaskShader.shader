Shader "depthMask"
{

    SubShader
    {
        Tags
        { 
            "RenderType" = "Opaque"
            "Queue" = "Geometry-1"
        }
        Pass
        {
            ZWrite On
            ZTest LEqual
            ColorMask 0
        }
    }
    FallBack "Diffuse"
}
