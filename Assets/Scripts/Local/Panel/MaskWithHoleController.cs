using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MaskWithSquareHoleController : MonoBehaviour
{
    [Header("需要挖孔的UI元素")]
    public RectTransform movingObject; // 用于定义孔洞位置和大小的UI元素

    [Header("跟随的玩家对象")]
    public Transform player; // 玩家对象的Transform

    private Material maskMaterial;
    private RectTransform maskRect;
    private Canvas canvas;

    void Start()
    {
        // 获取Image组件并创建材质实例，避免修改共享材质
        Image img = GetComponent<Image>();
        maskMaterial = new Material(img.material);
        img.material = maskMaterial;
        player = GameObject.Find("Player").transform;
        // 获取父级Canvas
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            maskRect = GetComponent<RectTransform>();
        }

        // 设置初始孔洞大小和位置
        UpdateHole();
    }

    void Update()
    {
        if (player != null && movingObject != null && maskRect != null)
        {
            // 将movingObject的位置设置为Player的屏幕位置
            Vector3 screenPos = Camera.main.WorldToScreenPoint(player.position);

            // 将屏幕坐标转换为Canvas的本地坐标
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                maskRect,
                screenPos,
                null, // Screen Space - Overlay模式下摄像机为null
                out localPos
            );

            // 设置movingObject的本地位置
            localPos.y += 105f;
            movingObject.anchoredPosition = localPos;
            // 更新孔洞的位置和大小
            UpdateHole();
        }
    }

    /// <summary>
    /// 更新孔洞的位置和大小
    #region[扣钱高亮]
    void UpdateHole()
    {
        // 获取 movingObject 在 Canvas 中的本地坐标
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            maskRect,
            movingObject.position,
            canvas.worldCamera,
            out localPos
        );

        // 将本地坐标转换为归一化坐标 (0-1)
        Vector2 normalizedPos = new Vector2(
            (localPos.x / maskRect.rect.width) + 0.5f,
            (localPos.y / maskRect.rect.height) + 0.5f
        );

        // 设置孔洞的位置
        maskMaterial.SetVector("_HolePosition", new Vector4(normalizedPos.x, normalizedPos.y, 0, 0));

        // 获取 movingObject 的尺寸，并转换为归一化尺寸
        Vector2 holeSize = new Vector2(
            movingObject.rect.width / maskRect.rect.width,
            movingObject.rect.height / maskRect.rect.height
        );

        // 设置孔洞的大小
        maskMaterial.SetVector("_HoleSize", new Vector4(holeSize.x, holeSize.y, 0, 0));
    }

    /// 动态设置孔洞大小的方法（如果需要）
    /// <param name="newSize">新的孔洞大小，范围0-1</param>
    public void SetHoleSize(Vector2 newSize)
    {
        Vector2 clampedSize = new Vector2(
            Mathf.Clamp01(newSize.x),
            Mathf.Clamp01(newSize.y)
        );
        maskMaterial.SetVector("_HoleSize", new Vector4(clampedSize.x, clampedSize.y, 0, 0));
    }
    #endregion[扣钱高亮]
}
