Shader "Custom/DistanceBasedVisibility"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PlayerPos ("Player Position", Vector) = (0,0,0,0)
        _VisibilityRadius ("Visibility Radius", Float) = 2.0
        _FadeDistance ("Fade Distance", Float) = 1.0
        [Toggle] _AffectedBySanity ("Affected by Sanity", Float) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _PlayerPos;
            float _VisibilityRadius;
            float _FadeDistance;
            float _AffectedBySanity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Se não é afetado pela sanidade, renderiza normalmente
                if (_AffectedBySanity < 0.5)
                    return col;
                
                // Calcula distância do jogador
                float dist = distance(i.worldPos, _PlayerPos);
                
                // Calcula alpha baseado na distância
                float alpha = 1.0 - smoothstep(_VisibilityRadius, _VisibilityRadius + _FadeDistance, dist);
                
                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
}