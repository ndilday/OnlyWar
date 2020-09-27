using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Badge : MonoBehaviour
{
    [SerializeField]
    private Sprite[] _spriteArray;

    [SerializeField]
    private Color[] _colorArray;

    [SerializeField]
    private Image _targetImage;

    [HideInInspector]
    public static int NORMAL = -1;
    public static int COMBAT = 0;
    public static  int INJURED = 1;
    public static int INSUFFICIENT_MEN = 2;
    public static int TAKEOFF = 3;

    public void SetBadge(int newBadge)
    {
        _targetImage.color = _colorArray[newBadge];
        _targetImage.sprite = _spriteArray[newBadge];
    }
}
