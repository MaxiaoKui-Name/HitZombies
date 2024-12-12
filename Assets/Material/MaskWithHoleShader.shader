Shader "Custom/MaskWithSquareHoleShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _HolePosition ("Hole Position", Vector) = (0.5, 0.5, 0, 0) // ��һ������ (0-1)
        _HoleSize ("Hole Size", Vector) = (0.1, 0.1, 0, 0) // ��һ����Ⱥ͸߶�
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _HolePosition;
            float4 _HoleSize;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mainColor = tex2D(_MainTex, i.uv);

                // ���㵱ǰ���ص������ĵľ���
                float2 diff = abs(i.uv - _HolePosition.xy);

                // �ж��Ƿ��ڿ׶���Χ��
                if (diff.x < (_HoleSize.x * 0.5) && diff.y < (_HoleSize.y * 0.5))
                {
                    discard;
                }

                return mainColor;
            }
            ENDCG
        }
    }
}
