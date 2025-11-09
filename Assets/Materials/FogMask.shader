Shader "Custom/InsanityFog_FinalCollapse_V3"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.1, 0, 0, 1)
        _PulseColor ("Pulse Color", Color) = (0.5, 0, 0, 1)
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _CrackTex ("Fracture Texture", 2D) = "white" {}
        _Distortion ("Distortion", Range(0, 1)) = 0.25
        _Density ("Fog Density", Range(0, 3)) = 1
        _Speed ("Fog Speed", Range(0, 2)) = 0.25
        _Scale ("Fog Scale", Range(0.1, 5)) = 1.6
        _CrackIntensity ("Crack Intensity", Range(0, 15)) = 0
        _RimDarkness ("Rim Darkness", Range(0, 6)) = 1.2
        _InnerDarkness ("Inner Darkness", Range(0, 2)) = 0.8
        _HeartbeatSpeed ("Heartbeat Speed", Range(0.5, 3)) = 1.2
        _HeartbeatIntensity ("Heartbeat Intensity", Range(0, 1)) = 0.4
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Overlay" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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

            sampler2D _NoiseTex;
            sampler2D _CrackTex;
            float4 _BaseColor;
            float4 _PulseColor;
            float _Density;
            float _Speed;
            float _Scale;
            float _Distortion;
            float _CrackIntensity;
            float _RimDarkness;
            float _InnerDarkness;
            float _HeartbeatSpeed;
            float _HeartbeatIntensity;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Função para calcular a visibilidade em um ponto específico
            float CalculateVisibility(float2 worldUV, float2 center, float currentDensity, float currentCrackIntensity, float currentRimDarkness, float currentInnerDarkness)
            {
                float2 uv = worldUV * _Scale;
                float t = _Time.y * _Speed;

                float n1 = tex2D(_NoiseTex, uv + t).r;
                float n2 = tex2D(_NoiseTex, uv - t * 0.4).g;
                float distort = (n1 - n2) * _Distortion;
                float2 distUV = uv + distort;

                // Calcula a máscara de névoa
                float fogMask = pow(tex2D(_NoiseTex, distUV).r, 0.8) * 1.7;
                float fogAlpha = saturate(fogMask * currentDensity);

                // Rachaduras
                float3 crackSample = tex2D(_CrackTex, worldUV).rgb;
                float crackMask = 1.0 - max(max(crackSample.r, crackSample.g), crackSample.b);
                crackMask = pow(crackMask, 1.1);

                float distFromCenter = distance(worldUV, center);

                // Sombra interna
                float innerShadow = smoothstep(0.25, 0.55, distFromCenter);
                float visibility = (1.0 - innerShadow * currentInnerDarkness);

                // Bordas e rachaduras
                float rim = smoothstep(0.15, 1.0, distFromCenter);
                float cracks = rim * currentCrackIntensity * crackMask * 3.2;

                // Escuridão nas bordas
                float rimDark = pow(smoothstep(0.05, 0.95, distFromCenter), 3.5);
                visibility *= (1.0 - rimDark * currentRimDarkness);

                // Aplica névoa e rachaduras
                visibility = lerp(visibility, 0.0, saturate(cracks + rimDark * 1.5 + fogAlpha));

                return saturate(visibility);
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv * _Scale;
                float t = _Time.y * _Speed;

                float n1 = tex2D(_NoiseTex, uv + t).r;
                float n2 = tex2D(_NoiseTex, uv - t * 0.4).g;
                float distort = (n1 - n2) * _Distortion;
                float2 distUV = uv + distort;

                // Batimento cardíaco sutil
                float heartbeat = pow(sin(_Time.y * _HeartbeatSpeed) * 0.5 + 0.5, 2.0);
                float heartbeatBoost = 1.0 + heartbeat * _HeartbeatIntensity;

                // Cor base vermelha escura (sem puxar pra roxo)
                float3 fogColor = lerp(_BaseColor.rgb, _PulseColor.rgb, 0.2) * heartbeatBoost;

                // Névoa mais contrastada
                float fogMask = pow(tex2D(_NoiseTex, distUV).r, 0.8) * 1.7;
                float fogAlpha = saturate(fogMask * _Density) * _BaseColor.a;

                // Rachaduras puramente pretas
                float3 crackSample = tex2D(_CrackTex, i.uv).rgb;
                float crackMask = 1.0 - max(max(crackSample.r, crackSample.g), crackSample.b);
                crackMask = pow(crackMask, 1.1);

                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);

                // ⚫ Reduz a área visível: foco circular pequeno e preto no meio
                float innerShadow = smoothstep(0.25, 0.55, dist);
                fogColor *= (1.0 - innerShadow * _InnerDarkness);

                // Bordas fechando
                float rim = smoothstep(0.15, 1.0, dist);
                float cracks = rim * _CrackIntensity * crackMask * 3.2;

                // Escuridão extrema nas bordas
                float rimDark = pow(smoothstep(0.05, 0.95, dist), 3.5);
                fogColor *= (1.0 - rimDark * _RimDarkness);

                // Preto absoluto + rachaduras vindo das bordas
                float3 finalColor = lerp(fogColor, float3(0, 0, 0), saturate(cracks + rimDark * 1.5));

                return half4(finalColor, fogAlpha);
            }
            ENDHLSL
        }
    }
}