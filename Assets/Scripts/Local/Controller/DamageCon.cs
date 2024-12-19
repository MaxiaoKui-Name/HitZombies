using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public float moveDuration = 0.6f; // �ƶ�����ʱ��
    private float moveDistance = 30f; // �ƶ�����
    public AnimationCurve arcCurve; // ����������

    private Text damageText;
    private Vector3 startPos;
    private Vector3 endPos;
    private float elapsedTime = 0f;

    void Awake()
    {
        damageText = GetComponent<Text>();
    }

    /// <summary>
    /// ��ʼ��DamageText
    /// </summary>
    /// <param name="damage">��ʾ���˺���ֵ</param>
    public void Initialize(long damage)
    {
        damageText.text = damage.ToString();
        // �������0�㵽180��ĽǶ�
        float angle = Random.Range(0f, 180f);
        // ���ݽǶȼ����ƶ�����
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        // ������ʼλ��
        startPos = transform.GetComponent<RectTransform>().anchoredPosition;
        // ���ý���λ��
        endPos = startPos + (Vector3)(direction * moveDistance);
        elapsedTime = 0f;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / moveDuration);

        // ʹ�û�����������t��ʹ�ó�ʼ�ƶ��Ͽ죬���ڼ���
        float t_eased = 1 - Mathf.Pow(1 - t, 2); // ���λ���

        // ʹ�ö������߿���Y���������Ч��
        float arc = arcCurve.Evaluate(t_eased);
        // ���㵱ǰλ��
        Vector3 currentPos = Vector3.Lerp(startPos, endPos, t_eased) + Vector3.up * arc;
        transform.GetComponent<RectTransform>().anchoredPosition = currentPos;

        if (t >= 1f)
        {
            Destroy(gameObject); // �ƶ���ɺ�����
        }
    }
}
