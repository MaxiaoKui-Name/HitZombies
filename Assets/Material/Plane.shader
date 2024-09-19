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
            ZWrite On // �������д�룬������Ⱦ˳������
            Blend SrcAlpha OneMinusSrcAlpha // ���ģʽ����Ϊ����͸����

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

            sampler2D _MainTex;  // ��������
            sampler2D _OverlayTex; // ��������
            float4 _MainTex_ST;   // ���ڴ�����������š�ƽ�ƵȲ���

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // ����������ת��Ϊ��������
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); // ʹ�� _MainTex_ST ����������� UV ����
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // ȡ������ͼƬ����ɫ
                half4 baseColor = tex2D(_MainTex, i.uv);

                // ȡ�����Ӳ�ͼƬ����ɫ
                half4 overlayColor = tex2D(_OverlayTex, i.uv);

                // ����͸���ȣ�����͸��������ֺ�ɫ��Ӱ
                overlayColor.rgb *= overlayColor.a; // �� RGB ���� alpha������͸��������Ӱ��

                // ��Ƭ����Ч����multiply��
                half4 result = baseColor * overlayColor;

                // ���س˻���Ľ��
                return result;
            }
            ENDCG
        }
    }
}
