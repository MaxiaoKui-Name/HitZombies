Shader "Unlit/PlaneMultiply"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _OverlayTex ("Overlay (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        LOD 200

        Pass
        {
            ZWrite On // 保持深度写入，避免渲染顺序问题
            Blend SrcAlpha OneMinusSrcAlpha // 混合模式调整为处理透明度

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

            sampler2D _MainTex;  // 基本纹理
            sampler2D _OverlayTex; // 叠加纹理
            float4 _MainTex_ST;   // 用于处理纹理的缩放、平移等操作

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // 将顶点坐标转换为剪裁坐标
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); // 使用 _MainTex_ST 来处理纹理的 UV 坐标
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // 取出基本图片的颜色
                half4 baseColor = tex2D(_MainTex, i.uv);

                // 取出叠加层图片的颜色
                half4 overlayColor = tex2D(_OverlayTex, i.uv);

                // 处理透明度，避免透明区域出现黑色阴影
                overlayColor.rgb *= overlayColor.a; // 将 RGB 乘以 alpha，保持透明区域不受影响

                // 正片叠底效果（multiply）
                half4 result = baseColor * overlayColor;

                // 返回乘积后的结果
                return result;
            }
            ENDCG
        }
    }
}
