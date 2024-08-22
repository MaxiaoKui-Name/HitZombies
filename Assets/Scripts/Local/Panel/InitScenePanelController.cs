using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InitScenePanelController : UIBase
{
    public Slider progressBar; // 进度条
    public TextMeshPro progressText;  // 进度百分比文本

    void Start()
    {
        GetAllChild(transform);
        progressBar = childDic["ProgressBar_F"].GetComponent<Slider>();
        progressText = childDic["Percent_F"].GetComponent<TextMeshPro>();
        progressBar.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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
