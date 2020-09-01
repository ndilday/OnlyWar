using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberPicker : MonoBehaviour
{
    public Text Label;
    public Button UpButton;
    public Button DownButton;
    public Text Count;

    public int MaxValue
    {
        get
        {
            return _maxValue;
        }
        set
        {
            _maxValue = value;
            CheckLimits();
        }
    }
    public int MinValue
    {
        get
        {
            return _minValue;
        }
        set
        {
            _minValue = value;
            CheckLimits();
        }
    }
    public int CurrentValue
    {
        get
        {
            return _count;
        }
        set
        {
            _count = value;
            CheckLimits();
        }
    }

    private int _maxValue = int.MaxValue;
    private int _minValue = 0;
    private int _count = 0;

    public void Start()
    {
        Count.text = _count.ToString();
        CheckLimits();
    }

    public void UpButton_OnClick()
    {
        _count++;
        Count.text = _count.ToString();
        CheckLimits();
    }

    public void DownButton_OnClick()
    {
        _count--;
        Count.text = _count.ToString();
        CheckLimits();
    }

    public void MakeInteractable(bool isInteractable)
    {
        UpButton.gameObject.SetActive(isInteractable);
        DownButton.gameObject.SetActive(isInteractable);
    }

    private void CheckLimits()
    {
        if(_count < _minValue)
        {
            _count = _minValue;
            Count.text = _minValue.ToString();
        }
        if(_count > _maxValue)
        {
            _count = _maxValue;
            Count.text = _maxValue.ToString();
        }
        DownButton.interactable = _count > _minValue;
        UpButton.interactable = _count < _maxValue;
    }
}
