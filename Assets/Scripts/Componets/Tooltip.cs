using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    private Text _tooltipText;
    private RectTransform _backgroundRectTransform;

    private void Awake()
    {
        _backgroundRectTransform = gameObject.GetComponent<RectTransform>();
        _tooltipText = gameObject.GetComponent<Text>();
    }

    void ShowTooltip(string newText)
    {
        gameObject.SetActive(true);
        _tooltipText.text = newText;
    }

    // Update is called once per frame
    void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}
