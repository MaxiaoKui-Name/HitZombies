using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelThree : MonoBehaviour
{
    [Header("��Ҫ�ڿ׵�UIԪ��")]
    public RectTransform movingObject; // ���ڶ���׶�λ�úʹ�С��UIԪ��


    public Material maskMaterial;
    public RectTransform maskRect;
    public Canvas canvas;

    void Start()
    {
        // ��ȡImage�������������ʵ���������޸Ĺ������
        //Image img = GetComponent<Image>();
        //maskMaterial = new Material(img.material);
        //img.material = maskMaterial;
        //// ��ȡ����Canvas
        //canvas = GetComponentInParent<Canvas>();
        //if (canvas != null)
        //{
        //    maskRect = GetComponent<RectTransform>();
        //}
    }
    /// <summary>
    /// ���¿׶���λ�úʹ�С
    #region[��Ǯ����]
    public void UpdateHole(Vector2 localPos, Vector2 localScale)
    {
        Image img = GetComponent<Image>();
        maskMaterial = new Material(img.material);
        img.material = maskMaterial;
        // ��ȡ����Canvas
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            maskRect = GetComponent<RectTransform>();
        }
        // 1. �� movingObject ��ê��λ������Ϊ����� localPos
        //    ע�⣺��� movingObject ��ê��������(0.5,0.5)����ô�����������������λ��
        movingObject.sizeDelta = localScale;
        movingObject.anchoredPosition = localPos;

        // 2. �� movingObject �ġ��������ꡱת��Ϊ��ǰ maskRect �ı�������
        //    ��ΪҪ�����һ��λ�ã���Ҫ֪���� maskRect �е����λ��
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            maskRect,                     // Ҫ������ת�����ĸ� RectTransform ��
            movingObject.position,        // movingObject ������λ��
            canvas.worldCamera,           // ����� Overlay ģʽ���ɴ� null������ΪnullҲ��Ӱ��
            out localPoint
        );

        // 3. ����������ת��Ϊ��һ������ (0 ~ 1)
        //   �����ǽ� maskRect �����½���Ϊ(-0.5, -0.5)�����Ͻ���Ϊ(0.5, 0.5)��
        //   ���Ҫ�ȳ��� maskRect �Ŀ��/�߶ȣ�Ȼ�� +0.5
        Vector2 normalizedPos = new Vector2(
            (localPoint.x / maskRect.rect.width) + 0.5f,
            (localPoint.y / maskRect.rect.height) + 0.5f
        );

        // 4. ���ò����С��׶�����λ�ã�_HolePosition��
        maskMaterial.SetVector("_HolePosition", new Vector4(normalizedPos.x, normalizedPos.y, 0, 0));

        // 5. ��ȡ movingObject �ĳߴ磬��ת��Ϊ��һ���ߴ磨����� maskRect��
        Vector2 holeSize = new Vector2(
            movingObject.rect.width / maskRect.rect.width,
            movingObject.rect.height / maskRect.rect.height
        );

        // 6. ���ÿ׶��Ĵ�С��_HoleSize��
        maskMaterial.SetVector("_HoleSize", new Vector4(holeSize.x, holeSize.y, 0, 0));
    }

    /// ��̬���ÿ׶���С�ķ����������Ҫ��
    /// <param name="newSize">�µĿ׶���С����Χ0-1</param>
    public void SetHoleSize(Vector2 newSize)
    {
        Vector2 clampedSize = new Vector2(
            Mathf.Clamp01(newSize.x),
            Mathf.Clamp01(newSize.y)
        );
        maskMaterial.SetVector("_HoleSize", new Vector4(clampedSize.x, clampedSize.y, 0, 0));
    }
    #endregion[��Ǯ����]
}
