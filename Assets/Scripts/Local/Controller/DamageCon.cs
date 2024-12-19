using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public float moveDuration = 0.6f; // 移动持续时间
    private float moveDistance = 30f; // 移动距离
    public AnimationCurve arcCurve; // 抛物线曲线

    private Text damageText;
    private Vector3 startPos;
    private Vector3 endPos;
    private float elapsedTime = 0f;

    void Awake()
    {
        damageText = GetComponent<Text>();
    }

    /// <summary>
    /// 初始化DamageText
    /// </summary>
    /// <param name="damage">显示的伤害数值</param>
    public void Initialize(long damage)
    {
        damageText.text = damage.ToString();
        // 随机生成0°到180°的角度
        float angle = Random.Range(0f, 180f);
        // 根据角度计算移动方向
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        // 设置起始位置
        startPos = transform.GetComponent<RectTransform>().anchoredPosition;
        // 设置结束位置
        endPos = startPos + (Vector3)(direction * moveDistance);
        elapsedTime = 0f;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / moveDuration);

        // 使用缓出函数调整t，使得初始移动较快，后期减速
        float t_eased = 1 - Mathf.Pow(1 - t, 2); // 二次缓出

        // 使用动画曲线控制Y轴的抛物线效果
        float arc = arcCurve.Evaluate(t_eased);
        // 计算当前位置
        Vector3 currentPos = Vector3.Lerp(startPos, endPos, t_eased) + Vector3.up * arc;
        transform.GetComponent<RectTransform>().anchoredPosition = currentPos;

        if (t >= 1f)
        {
            Destroy(gameObject); // 移动完成后销毁
        }
    }
}
