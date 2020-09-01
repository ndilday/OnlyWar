using Iam.Scripts.Models.Equippables;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct WeaponSelectionSection
{
    public string Label;
    public int MinCount;
    public int MaxCount;
    public int CurrentCount;
    public List<Tuple<string, int>> Selections;
}

public class SquadArmamentView : MonoBehaviour
{
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
        _default.CurrentValue = defaultCount;
        _default.Label.text = defaultLabel;
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
            var picker = sectionHeader.GetComponent<NumberPicker>();
            picker.MakeInteractable(false);
            picker.CurrentValue = section.CurrentCount;
            picker.Label.text = section.Label + " (max " + section.MaxCount + ")";
            yPos -= 25;
            foreach(Tuple<string, int> weapon in section.Selections)
            {
                GameObject selection = Instantiate(NumberPickerPrefab,
                                new Vector3(0, yPos, 0),
                                Quaternion.identity,
                                Pane.transform);
                selection.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, yPos);
                picker = selection.GetComponent<NumberPicker>();
                picker.MakeInteractable(true);
                picker.CurrentValue = weapon.Item2;
                picker.MaxValue = picker.CurrentValue + section.MaxCount - section.CurrentCount;
                picker.Label.text = weapon.Item1;
                yPos -= 25;
            }
            yPos -= 25;
        }
    }
}
