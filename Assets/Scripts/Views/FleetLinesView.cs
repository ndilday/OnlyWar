 using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

namespace Iam.Scripts.Views
{
    public class FleetLinesView : MonoBehaviour
    {
        public Color LineColor;
        public GameObject FleetLinePrefab;
        Dictionary<int, GameObject> _lines = new Dictionary<int, GameObject>();

        public void GalaxyMapView_OnFleetLineDraw(int index, Vector2 startPoint, Vector2 endPoint)
        {
            CreateLine(index, startPoint, endPoint);
        }

        public void GalaxyMapView_OnFleetLineRemove(int index)
        {
            RemoveLine(index);
        }

        private void CreateLine(int index, Vector2 startPoint, Vector2 endPoint)
        {
            RemoveLine(index);
            GameObject newLine = Instantiate(FleetLinePrefab);
            LineRenderer lRend = newLine.GetComponent<LineRenderer>();
            lRend.SetPosition(0, startPoint);
            lRend.SetPosition(1, endPoint);
            lRend.startColor = LineColor;
            lRend.endColor = LineColor;
            _lines[index] = newLine;
        }

        private void RemoveLine(int index)
        {
            if (_lines.ContainsKey(index))
            {
                Object.Destroy(_lines[index]);
            }
        }
    }
}