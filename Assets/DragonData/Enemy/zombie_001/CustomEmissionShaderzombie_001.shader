Shader "CustomEmissionShaderzombie_001"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {} // 主要纹理
        _EmissionColor ("Emission Color", Color) = (1,1,1,1) // 发光颜色
        _EmissionTex ("Emission Texture", 2D) = "white" {} // 发光纹理（可选）
        _EmissionToggle ("Enable Emission", Float) = 0 // 控制发光开关，0 = 关闭, 1 = 开启
        _EmissionIntensity ("Emission Intensity", Range(0,5)) = 1.0 // 发光强度
        _Alpha ("Transparency", Range(0,1)) = 1.0 // 透明度
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" } // 设置渲染队列为透明队列
        LOD 100

        // 使用半透明物体的混合模式
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off // 关闭深度写入，避免透明物体覆盖其他物体

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

            fixed4 frag (v2f i) : SV_Target
            {
                // 采样基础纹理
                fixed4 col = tex2D(_MainTex, i.uv);

                // 根据 Alpha 透明度控制透明部分
                col.a *= _Alpha;

                // 采样发光纹理并应用发光效果
                fixed4 emission = tex2D(_EmissionTex, i.uv) * _EmissionColor * _EmissionIntensity * _EmissionToggle;

                // 将基础颜色与发光颜色叠加
                col.rgb += emission.rgb;

                // 使用纹理的 Alpha 通道作为透明度，并确保透明度正确显示
                col.a *= tex2D(_MainTex, i.uv).a; 

                // 如果透明度过低，则丢弃该片元
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
