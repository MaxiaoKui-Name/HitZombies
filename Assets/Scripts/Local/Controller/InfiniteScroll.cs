using UnityEngine;
using UnityEngine.UIElements;

public class InfiniteScroll : Singleton<InfiniteScroll> 
{
    public float baseScrollSpeed = 1f;
    public float baseGrowthRate = 0.05f;
    public GameObject background1;
    public GameObject background2;
    public Transform[] segments;
    private float bgOriginalHeight;
    private float camHeight;
    private Vector3 bg1InitialPosition;
    private Vector3 bg2InitialPosition;
    public float bg1InitialScale = 0.54f;
    public float bg2InitialScale = 0.28f;

    void Start()
    {
        camHeight = Camera.main.orthographicSize * 2f;
        bgOriginalHeight = background1.GetComponent<SpriteRenderer>().bounds.size.y / bg1InitialScale;

        SetScale(background1, bg1InitialScale);
        SetScale(background2, bg2InitialScale);

        PositionBackgrounds();

        bg1InitialPosition = background1.transform.position;
        bg2InitialPosition = background2.transform.position;
    }

    void Update()
    {
        if (GameManage.Instance.gameState != GameState.Running)
            return;
        if (PreController.Instance.isFrozen) return;
        if (!LevelManager.Instance.isLoadBack) return;
        ScrollAndGrowBackgrounds();
        CheckAndResetPositions();

    }

    void ScrollAndGrowBackgrounds()
    {
        ScrollAndGrowBackground(background1);
        ScrollAndGrowBackground(background2);
    }
    public float scrollSpeed;
    public float growthRate;
    void ScrollAndGrowBackground(GameObject bg)
    {
        float currentScale = bg.transform.localScale.y;
        float scaleFactor = currentScale / bg1InitialScale;

        // Calculate scroll speed based on current scale
        scrollSpeed = baseScrollSpeed * scaleFactor;
        bg.transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

        // Calculate growth rate based on current scale
        growthRate = baseGrowthRate * scaleFactor;
        float newScale = currentScale + growthRate * Time.deltaTime;
        SetScale(bg, newScale);

        // Adjust position to compensate for growth
        float growthAmount = (newScale - currentScale) * bgOriginalHeight / 2;
        bg.transform.position += new Vector3(0, growthAmount, 0);
    }
    void CheckAndResetPositions()
    {
        if (IsOffScreen(background1))
        {
            ResetBackground(background1);
        }
        if (IsOffScreen(background2))
        {
            ResetBackground(background2);
        }
    }

    void ResetBackground(GameObject bg)
    {
        GameObject otherBg = (bg == background1) ? background2 : background1;
        float resetScale = bg2InitialScale;// otherBg.transform.localScale.y * bg2InitialScale / bg1InitialScale;
        SetScale(bg, resetScale);
        float otherBgTopY = otherBg.transform.position.y + (otherBg.GetComponent<SpriteRenderer>().bounds.size.y / 2);
        float resetY = otherBgTopY + (bgOriginalHeight * resetScale / 2);
        bg.transform.position = new Vector3(0, resetY, 0);
    }

    bool IsOffScreen(GameObject bg)
    {
        float bgTopY = bg.transform.position.y + (bg.GetComponent<SpriteRenderer>().bounds.size.y / 2);
        float camBottomY = Camera.main.transform.position.y - camHeight / 2;
        return bgTopY < camBottomY;
    }

    void SetScale(GameObject bg, float scale)
    {
        bg.transform.localScale = new Vector3(scale, scale, 1f);
    }

    void PositionBackgrounds()
    {
        float camTopY = Camera.main.transform.position.y + camHeight / 2;
        
        float bg1Height = bgOriginalHeight * bg1InitialScale;
        background1.transform.position = new Vector3(0, camTopY - bg1Height / 2, 0);

        float bg2Height = bgOriginalHeight * bg2InitialScale;
        background2.transform.position = new Vector3(0, background1.transform.position.y + bg1Height / 2 + bg2Height / 2, 0);
    }
    public void SetBackground(Sprite backgroundSprite, int index)
    {
        SpriteRenderer sr = segments[index].GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = backgroundSprite;
        }
    }
}