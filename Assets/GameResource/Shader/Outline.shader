Shader "Unlit/Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineWidth ("OutlineWidth", Range(0., 5.)) = 5.
        _OutlineColor ("OutlineColor", Color) = (0, 1, 1, 1)
        _OutlineSize ("OutlineSize", Range(1., 100.)) = 100.
        _LayerCount ("Layer Count", Range(1, 5)) = 2
        _TimeOffset ("Time Offset", Range(0., 1.)) = 0.3
        _AnimSpeed ("Animation Speed", Range(0.1, 5.)) = 0.6
        _ClipRect ("Clip Rect", Vector) = (-32767, -32767, 32767, 32767)
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Transparent"
        }
        Blend One OneMinusSrcAlpha
        ZWrite Off
        ZTest LEqual
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT

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
                float2 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _OutlineWidth;
            float4 _OutlineColor;
            float _OutlineSize;
            float _LayerCount;
            float _TimeOffset;
            float _AnimSpeed;
            float4 _ClipRect;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xy;
                return o;
            }

            // 计算单层轮廓的函数
            // layerTime: 0-1 范围的归一化时间
            float CalculateLayerAlpha(float2 uv, float colAlpha, float layerTime)
            {
                float timeAnim = sqrt(layerTime) * _OutlineSize;

                float2 offset = _MainTex_TexelSize.xy * timeAnim;
                float2 uvUp = uv + float2(0, 1) * offset;
                float2 uvDown = uv + float2(0, -1) * offset;
                float2 uvLeft = uv + float2(-1, 0) * offset;
                float2 uvRight = uv + float2(1, 0) * offset;
                float2 uvUpLeft = uv + float2(-1, 1) * offset;
                float2 uvUpRight = uv + float2(1, 1) * offset;
                float2 uvDownLeft = uv + float2(-1, -1) * offset;
                float2 uvDownRight = uv + float2(1, -1) * offset;

                float up = tex2D(_MainTex, uvUp).a;
                float down = tex2D(_MainTex, uvDown).a;
                float left = tex2D(_MainTex, uvLeft).a;
                float right = tex2D(_MainTex, uvRight).a;
                float up_left = tex2D(_MainTex, uvUpLeft).a;
                float up_right = tex2D(_MainTex, uvUpRight).a;
                float down_left = tex2D(_MainTex, uvDownLeft).a;
                float down_right = tex2D(_MainTex, uvDownRight).a;

                if (colAlpha < 0.1 && (up > 0.1 || down > 0.1 || left > 0.1 || right > 0.1 ||
                    up_left > 0.1 || up_right > 0.1 || down_left > 0.1 || down_right > 0.1))
                {
                    return 1.0;
                }
                
                // 边缘检测：UV接近纹理边界时强制发光
                float edgeThreshold = 0.02;
                if (uv.x < edgeThreshold || uv.x > 1.0 - edgeThreshold || 
                    uv.y < edgeThreshold || uv.y > 1.0 - edgeThreshold)
                {
                    return 1.0;
                }
                
                return 0.0;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                if (_OutlineWidth <= 0.001)
                {
                    if (col.a > 0.1)
                        return fixed4(col.rgb * col.a, col.a);
                    else
                    {
                        discard;
                        return fixed4(0, 0, 0, 0);
                    }
                }

                float totalOutlineAlpha = 0;
                float colAlpha = col.a;
                float cycleTime = _Time.y * _AnimSpeed;

                float t0 = frac(cycleTime);
                float a0 = CalculateLayerAlpha(i.uv, colAlpha, t0);
                float p0 = lerp(0.8, 0, t0);
                totalOutlineAlpha += a0 * p0;

                if (_LayerCount > 1.0)
                {
                    float t1 = frac(cycleTime - _TimeOffset);
                    float a1 = CalculateLayerAlpha(i.uv, colAlpha, t1);
                    float p1 = lerp(0.8, 0, t1);
                    totalOutlineAlpha += a1 * p1;
                }

                if (_LayerCount > 2.0)
                {
                    float t2 = frac(cycleTime - _TimeOffset * 2.0);
                    float a2 = CalculateLayerAlpha(i.uv, colAlpha, t2);
                    float p2 = lerp(0.8, 0, t2);
                    totalOutlineAlpha += a2 * p2;
                }

                if (_LayerCount > 3.0)
                {
                    float t3 = frac(cycleTime - _TimeOffset * 3.0);
                    float a3 = CalculateLayerAlpha(i.uv, colAlpha, t3);
                    float p3 = lerp(0.8, 0, t3);
                    totalOutlineAlpha += a3 * p3;
                }

                if (_LayerCount > 4.0)
                {
                    float t4 = frac(cycleTime - _TimeOffset * 4.0);
                    float a4 = CalculateLayerAlpha(i.uv, colAlpha, t4);
                    float p4 = lerp(0.8, 0, t4);
                    totalOutlineAlpha += a4 * p4;
                }

                totalOutlineAlpha = saturate(totalOutlineAlpha);

                if (totalOutlineAlpha > 0.01)
                {
                    return fixed4(_OutlineColor.rgb * totalOutlineAlpha, totalOutlineAlpha);
                }
                else if (col.a > 0.1)
                {
                    #ifdef UNITY_UI_CLIP_RECT
                        float2 clipPosition = i.worldPosition - _ClipRect.xy;
                        float2 clipSize = _ClipRect.zw - _ClipRect.xy;
                        if (clipPosition.x < 0 || clipPosition.y < 0 || clipPosition.x > clipSize.x || clipPosition.y > clipSize.y)
                            discard;
                    #endif
                    return fixed4(col.rgb * col.a, col.a);
                }
                else
                {
                    discard;
                    return fixed4(0, 0, 0, 0);
                }
            }
            ENDCG
        }
    }
}
