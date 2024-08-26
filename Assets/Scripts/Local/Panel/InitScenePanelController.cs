using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InitScenePanelController : UIBase
{
    public Slider progressBar; // 进度条
    public TextMeshProUGUI progressText;  // 进度百分比文本
    public TextMeshProUGUI updateText; 
    void Start()
    {
        GetAllChild(transform);
        progressBar = childDic["ProgressBar_F"].GetComponent<Slider>();
        progressText = childDic["Percent_F"].GetComponent<TextMeshProUGUI>();
        progressBar.gameObject.SetActive(false);
        updateText = childDic["UpdateText_F"].GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateText(string  sentance)
    {
        updateText.text = sentance;
    }
    public void UpdateProgressBar(float progress)
    {
        if(!progressBar.gameObject.activeSelf)
           progressBar.gameObject.SetActive(true);
        if (progressBar != null)
        {
            progressBar.value = progress;
        }

        if (progressText != null)
        {
            progressText.text = $"{(progress * 100):0.00}%";
        }
    }
}
