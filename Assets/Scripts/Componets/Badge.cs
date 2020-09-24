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
    public const int COMBAT = 0;
    public const int INJURED = 1;
    public const int INSUFFICIENT_MEN = 2;

    public void SetBadge(int newBadge)
    {
        _targetImage.color = _colorArray[newBadge];
        _targetImage.sprite = _spriteArray[newBadge];
    }
}
