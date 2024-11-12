Shader "Custom/EmissionShaderFixed"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {} // 主要纹理
        _EmissionColor ("Emission Color", Color) = (1,1,1,1) // 发光颜色
        _EmissionTex ("Emission Texture", 2D) = "white" {} // 发光纹理（默认为白色，表示无发光）
        _EmissionToggle ("Enable Emission", Float) = 0 // 控制发光开关，0 = 关闭, 1 = 开启
        _EmissionIntensity ("Emission Intensity", Range(0,10)) = 1.0 // 发光强度
        _Alpha ("Transparency", Range(0,1)) = 1.0 // 透明度
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" } // 设置渲染队列为透明队列
        LOD 200

        // 使用半透明物体的混合模式
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off // 关闭深度写入，避免透明物体覆盖其他物体
        Cull Off // 关闭背面剔除，使Shader双面渲染

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _EmissionColor; // 发光颜色
            sampler2D _EmissionTex; // 发光纹理（可选）
            float4 _EmissionTex_ST;

            float _EmissionToggle; // 发光开关
            float _EmissionIntensity; // 发光强度
            float _Alpha; // 透明度

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // 采样基础纹理并应用整体透明度
                half4 col = tex2D(_MainTex, i.uv) * _Alpha;

                // 如果发光开关开启，则采样并叠加发光效果
                if (_EmissionToggle > 0.5)
                {
                    half4 emission = tex2D(_EmissionTex, i.uv) * _EmissionColor * _EmissionIntensity;
                    col.rgb += emission.rgb;
                }

                // 如果透明度过低，则丢弃该片元以优化渲染性能
                if (col.a < 0.01)
                {
                    discard;
                }

                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}
