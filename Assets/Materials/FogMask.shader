Shader "Custom/FogMask"
{
    Properties
    {
        _Radius ("Radius", Range(0, 1)) = 0.5
        _Softness ("Softness", Range(0, 1)) = 0.2
        _Color ("Fog Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _Radius;
            float _Softness;
            float4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                float fog = smoothstep(_Radius, _Radius - _Softness, dist);
                return half4(_Color.rgb, fog * _Color.a);
            }
            ENDHLSL
        }
    }
}
