Shader "Custom/EmissionShaderFixed"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {} // ��Ҫ����
        _EmissionColor ("Emission Color", Color) = (1,1,1,1) // ������ɫ
        _EmissionTex ("Emission Texture", 2D) = "white" {} // ��������Ĭ��Ϊ��ɫ����ʾ�޷��⣩
        _EmissionToggle ("Enable Emission", Float) = 0 // ���Ʒ��⿪�أ�0 = �ر�, 1 = ����
        _EmissionIntensity ("Emission Intensity", Range(0,10)) = 1.0 // ����ǿ��
        _Alpha ("Transparency", Range(0,1)) = 1.0 // ͸����
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" } // ������Ⱦ����Ϊ͸������
        LOD 200

        // ʹ�ð�͸������Ļ��ģʽ
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off // �ر����д�룬����͸�����帲����������
        Cull Off // �رձ����޳���ʹShader˫����Ⱦ

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

            float4 _EmissionColor; // ������ɫ
            sampler2D _EmissionTex; // ����������ѡ��
            float4 _EmissionTex_ST;

            float _EmissionToggle; // ���⿪��
            float _EmissionIntensity; // ����ǿ��
            float _Alpha; // ͸����

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // ������������Ӧ������͸����
                half4 col = tex2D(_MainTex, i.uv) * _Alpha;

                // ������⿪�ؿ���������������ӷ���Ч��
                if (_EmissionToggle > 0.5)
                {
                    half4 emission = tex2D(_EmissionTex, i.uv) * _EmissionColor * _EmissionIntensity;
                    col.rgb += emission.rgb;
                }

                // ���͸���ȹ��ͣ�������ƬԪ���Ż���Ⱦ����
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
