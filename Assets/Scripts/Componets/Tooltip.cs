using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    private static Tooltip _instance;
    private Text _tooltipText;
    private RectTransform _backgroundRectTransform;
    private RectTransform _parentRectTransform;

    private Tooltip()
    {
        _instance = this;
    }
    public static void ShowTooltip(string tooltipString)
    {
        _instance.ShowTooltipInternal(tooltipString);
    }

    public static void HideTooltip()
    {
        _instance.HideTooltipInternal();
    }

    private void Awake()
    {
        _backgroundRectTransform = transform.GetComponent<RectTransform>();
        _tooltipText = transform.Find("Text").GetComponent<Text>();
        _parentRectTransform = transform.parent.GetComponent<RectTransform>();
    }

    private void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRectTransform, 
                                                                Input.mousePosition, 
                                                                null,//Camera.main, 
                                                                out Vector2 localPoint);
        localPoint.x += 5;
        localPoint.y += 5;
        transform.localPosition = localPoint;
    }

    private void ShowTooltipInternal(string newText)
    {
        gameObject.SetActive(true);
        _tooltipText.text = newText;
        float paddingSize = 4f;
        Vector2 backgroundSize = new Vector2(_tooltipText.preferredWidth + 2 * paddingSize, 
                                             _tooltipText.preferredHeight + 2* paddingSize);
        _backgroundRectTransform.sizeDelta = backgroundSize;
    }

    // Update is called once per frame
    private void HideTooltipInternal()
    {
        gameObject.SetActive(false);
    }
}
