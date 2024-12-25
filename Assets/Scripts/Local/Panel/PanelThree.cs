using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelThree : MonoBehaviour
{
    [Header("需要挖孔的UI元素")]
    public RectTransform movingObject; // 用于定义孔洞位置和大小的UI元素


    public Material maskMaterial;
    public RectTransform maskRect;
    public Canvas canvas;

    void Start()
    {
        // 获取Image组件并创建材质实例，避免修改共享材质
        //Image img = GetComponent<Image>();
        //maskMaterial = new Material(img.material);
        //img.material = maskMaterial;
        //// 获取父级Canvas
        //canvas = GetComponentInParent<Canvas>();
        //if (canvas != null)
        //{
        //    maskRect = GetComponent<RectTransform>();
        //}
    }
    /// <summary>
    /// 更新孔洞的位置和大小
    #region[扣钱高亮]
    public void UpdateHole(Vector2 localPos, Vector2 localScale)
    {
        Image img = GetComponent<Image>();
        maskMaterial = new Material(img.material);
        img.material = maskMaterial;
        // 获取父级Canvas
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            maskRect = GetComponent<RectTransform>();
        }
        // 1. 将 movingObject 的锚点位置设置为传入的 localPos
        //    注意：如果 movingObject 的锚点在中心(0.5,0.5)，那么它会相对于中心来定位。
        movingObject.sizeDelta = localScale;
        movingObject.anchoredPosition = localPos;

        // 2. 将 movingObject 的“世界坐标”转换为当前 maskRect 的本地坐标
        //    因为要计算归一化位置，需要知道在 maskRect 中的相对位置
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            maskRect,                     // 要把坐标转换到哪个 RectTransform 下
            movingObject.position,        // movingObject 的世界位置
            canvas.worldCamera,           // 如果是 Overlay 模式，可传 null；若不为null也不影响
            out localPoint
        );

        // 3. 将本地坐标转换为归一化坐标 (0 ~ 1)
        //   这里是将 maskRect 的左下角作为(-0.5, -0.5)，右上角作为(0.5, 0.5)，
        //   因此要先除以 maskRect 的宽度/高度，然后 +0.5
        Vector2 normalizedPos = new Vector2(
            (localPoint.x / maskRect.rect.width) + 0.5f,
            (localPoint.y / maskRect.rect.height) + 0.5f
        );

        // 4. 设置材质中“孔洞”的位置（_HolePosition）
        maskMaterial.SetVector("_HolePosition", new Vector4(normalizedPos.x, normalizedPos.y, 0, 0));

        // 5. 获取 movingObject 的尺寸，并转换为归一化尺寸（相对于 maskRect）
        Vector2 holeSize = new Vector2(
            movingObject.rect.width / maskRect.rect.width,
            movingObject.rect.height / maskRect.rect.height
        );

        // 6. 设置孔洞的大小（_HoleSize）
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
