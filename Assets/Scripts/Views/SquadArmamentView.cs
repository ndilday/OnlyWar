using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WeaponSelectionSection
{
    public string Label;
    public int MinCount;
    public int MaxCount;
    public int CurrentCount;
    public List<Tuple<string, int>> Selections;
}

public class SquadArmamentView : MonoBehaviour
{
    public UnityEvent OnArmamentChanged;
    public UnityEvent<bool> OnIsFrontLineChanged;
    public GameObject NumberPickerPrefab;
    public RectTransform Pane;
    public Toggle IsFrontLine;

    private NumberPicker _default;
    private List<NumberPicker> _selections = new List<NumberPicker>();
    public void Clear()
    {
        foreach(Transform child in Pane.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        _default = null;
        _selections.Clear();
        IsFrontLine.gameObject.SetActive(false);
    }

    public void SetIsFrontLine(bool isFrontLine)
    {
        IsFrontLine.isOn = isFrontLine;
    }

    public void Initialize(bool isFrontLine, int defaultCount, string defaultLabel, List<WeaponSelectionSection> weaponSelections)
    {
        IsFrontLine.gameObject.SetActive(true);
        IsFrontLine.isOn = isFrontLine;

        int yPos = -5;
        GameObject defaultLine = Instantiate(NumberPickerPrefab,
                                new Vector3(0, yPos, 0),
                                Quaternion.identity,
                                Pane.transform);
        defaultLine.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, yPos);
        _default = defaultLine.GetComponent<NumberPicker>();
        _default.MakeInteractable(false);
        _default.OnValueChanged.AddListener(NumberPicker_OnValueChanged);
        int availableCount = defaultCount;
        //_default.CurrentValue = defaultCount;
        _default.Label.text = defaultLabel;
        _default.MaxValue = defaultCount;
        _default.DoChildrenSubtract = true;
        // it'd be cleaner to get the height from the prefab's rect transform
        yPos -= 50;
        _selections = new List<NumberPicker>();
        foreach(WeaponSelectionSection section in weaponSelections)
        {
            GameObject sectionHeader = Instantiate(NumberPickerPrefab,
                                new Vector3(0, yPos, 0),
                                Quaternion.identity,
                                Pane.transform);
            sectionHeader.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, yPos);
            var middle = sectionHeader.GetComponent<NumberPicker>();
            middle.MakeInteractable(false);
            middle.CurrentValue = section.CurrentCount;
            middle.MaxValue = section.MaxCount;
            middle.Label.text = section.Label + " (max " + section.MaxCount + ")";
            middle.DoChildrenSubtract = false;
            _default.AddChild(middle);
            yPos -= 25;
            foreach(Tuple<string, int> weapon in section.Selections)
            {
                GameObject leaf = Instantiate(NumberPickerPrefab,
                                new Vector3(0, yPos, 0),
                                Quaternion.identity,
                                Pane.transform);
                leaf.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, yPos);
                var picker = leaf.GetComponent<NumberPicker>();
                picker.MakeInteractable(true);
                picker.CurrentValue = weapon.Item2;
                availableCount -= weapon.Item2;
                picker.MaxValue = picker.CurrentValue + section.MaxCount - section.CurrentCount;
                picker.Label.text = weapon.Item1;
                middle.AddChild(picker);
                yPos -= 25;
            }
            yPos -= 25;
        }
        _default.CurrentValue = availableCount;
    }

    public List<Tuple<string, int>> GetSelections()
    {
        List<Tuple<string, int>> list = new List<Tuple<string, int>>();
        list.Add(new Tuple<string, int>(_default.Label.text, _default.CurrentValue));
        foreach(NumberPicker child in _default.Children)
        {
            foreach(NumberPicker leaf in child.Children)
            {
                list.Add(new Tuple<string, int>(leaf.Label.text, leaf.CurrentValue));
            }
        }
        return list;
    }

    public void Toggle_OnValueChanged(bool newValue)
    {
        OnIsFrontLineChanged.Invoke(newValue);
    }

    private void NumberPicker_OnValueChanged()
    {
        OnArmamentChanged.Invoke();
    }
}
