using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NumberPicker : MonoBehaviour
{
    public UnityEvent OnValueChanged;
    public Text Label;
    public Button UpButton;
    public Button DownButton;
    public Text Count;

    public List<NumberPicker> Children;
    public bool DoChildrenSubtract;

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

    public NumberPicker()
    {
        Children = new List<NumberPicker>();
    }
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
        OnValueChanged.Invoke();
    }

    public void DownButton_OnClick()
    {
        _count--;
        Count.text = _count.ToString();
        CheckLimits();
        OnValueChanged.Invoke();
    }

    public void MakeInteractable(bool isInteractable)
    {
        UpButton.gameObject.SetActive(isInteractable);
        DownButton.gameObject.SetActive(isInteractable);
    }

    public void ChildNumberPicker_OnValueChanged()
    {
        // sum up child values, set our value to be = sum(child values)
        if (DoChildrenSubtract)
        {
            _count = _maxValue - Children.Sum(c => c.CurrentValue);
        }
        else
        {
            _count = Children.Sum(c => c.CurrentValue);
            foreach(NumberPicker child in Children)
            {
                child.MaxValue = child._count + _maxValue - _count;
            }
        }
        Count.text = _count.ToString();
        OnValueChanged.Invoke();
    }

    public void AddChild(NumberPicker picker)
    {
        Children.Add(picker);
        picker.OnValueChanged.AddListener(ChildNumberPicker_OnValueChanged);
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
