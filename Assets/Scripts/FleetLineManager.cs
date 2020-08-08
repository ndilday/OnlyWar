 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetLineManager : MonoBehaviour
{
    public GameObject FleetLinePrefab;
    Dictionary<int, GameObject> _lines = new Dictionary<int, GameObject>();
    public void CreateLine(int index, Vector2 startPoint, Vector2 endPoint, Color color)
    {
        RemoveLine(index);
        GameObject newLine = Instantiate(FleetLinePrefab);
        LineRenderer lRend = newLine.GetComponent<LineRenderer>();
        lRend.SetPosition(0, startPoint);
        lRend.SetPosition(1, endPoint);
        lRend.startColor = color;
        lRend.endColor = color;
        _lines[index] = newLine;
    }
    public void RemoveLine(int index)
    {
        if(_lines.ContainsKey(index))
        {
            Object.Destroy(_lines[index]);
        }
    }
}
