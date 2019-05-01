using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TipsGenerator : MonoBehaviour
{

    [SerializeField] private TMP_Text tipText;
    int runCount = 0;
    [TextArea()] [SerializeField] private string[] tipsArray;
    private int lastIndex = 0;

    public bool hasEscaped = false;

    private void Awake()
    {
        runCount = PlayerPrefs.GetInt("Runs");
    }

    private void Start()
    {
        if (runCount > 0)
        {
            lastIndex = PlayerPrefs.GetInt("lastIndex");
            if (++lastIndex > tipsArray.Length - 1) lastIndex = 0; else lastIndex++;
            tipText.text = tipsArray[lastIndex];
            PlayerPrefs.SetInt("lastIndex", lastIndex);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }
    private void Update()
    {
        if (hasEscaped)
        {
            Destroy(gameObject);
        }
    }
    public void IncreaseRunCount()
    {
        runCount++;
        PlayerPrefs.SetInt("Runs", runCount);
    }
}
