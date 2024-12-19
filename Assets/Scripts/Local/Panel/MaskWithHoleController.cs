using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MaskWithSquareHoleController : MonoBehaviour
{
    [Header("��Ҫ�ڿ׵�UIԪ��")]
    public RectTransform movingObject; // ���ڶ���׶�λ�úʹ�С��UIԪ��

    [Header("�������Ҷ���")]
    public Transform player; // ��Ҷ����Transform

    private Material maskMaterial;
    private RectTransform maskRect;
    private Canvas canvas;

    void Start()
    {
        // ��ȡImage�������������ʵ���������޸Ĺ������
        Image img = GetComponent<Image>();
        maskMaterial = new Material(img.material);
        img.material = maskMaterial;
        player = GameObject.Find("Player").transform;
        // ��ȡ����Canvas
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            maskRect = GetComponent<RectTransform>();
        }

        // ���ó�ʼ�׶���С��λ��
        UpdateHole();
    }

    void Update()
    {
        if (player != null && movingObject != null && maskRect != null)
        {
            // ��movingObject��λ������ΪPlayer����Ļλ��
            Vector3 screenPos = Camera.main.WorldToScreenPoint(player.position);

            // ����Ļ����ת��ΪCanvas�ı�������
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                maskRect,
                screenPos,
                null, // Screen Space - Overlayģʽ�������Ϊnull
                out localPos
            );

            // ����movingObject�ı���λ��
            localPos.y += 105f;
            movingObject.anchoredPosition = localPos;
            // ���¿׶���λ�úʹ�С
            UpdateHole();
        }
    }

    /// <summary>
    /// ���¿׶���λ�úʹ�С
    #region[��Ǯ����]
    void UpdateHole()
    {
        // ��ȡ movingObject �� Canvas �еı�������
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            maskRect,
            movingObject.position,
            canvas.worldCamera,
            out localPos
        );

        // ����������ת��Ϊ��һ������ (0-1)
        Vector2 normalizedPos = new Vector2(
            (localPos.x / maskRect.rect.width) + 0.5f,
            (localPos.y / maskRect.rect.height) + 0.5f
        );

        // ���ÿ׶���λ��
        maskMaterial.SetVector("_HolePosition", new Vector4(normalizedPos.x, normalizedPos.y, 0, 0));

        // ��ȡ movingObject �ĳߴ磬��ת��Ϊ��һ���ߴ�
        Vector2 holeSize = new Vector2(
            movingObject.rect.width / maskRect.rect.width,
            movingObject.rect.height / maskRect.rect.height
        );

        // ���ÿ׶��Ĵ�С
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
