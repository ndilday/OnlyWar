using System.Collections.Generic;
using UnityEngine;

namespace Iam.Scripts.Views
{
    public class FleetLinesView : MonoBehaviour
    {
        [SerializeField]
        private Color LineColor;
        [SerializeField]
        private GameObject FleetLinePrefab;
        
        private readonly Dictionary<int, LineRenderer> _lines = new Dictionary<int, LineRenderer>();

        public void GalaxyMapView_OnFleetLineDraw(int index, Vector2 startPoint, Vector2 endPoint)
        {
            CreateLine(index, startPoint, endPoint);
        }

        public void GalaxyMapView_OnFleetLineRemove(int index)
        {
            RemoveLine(index);
        }

        public void GalaxyMapView_OnFleetStartPointAdjusted(int index, Vector2 newStartPoint)
        {
            AdjustLine(index, newStartPoint);
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
            _lines[index] = lRend;
        }

        private void RemoveLine(int index)
        {
            if (_lines.ContainsKey(index))
            {
                Object.Destroy(_lines[index].gameObject);
            }
            _lines.Remove(index);
        }

        private void AdjustLine(int index, Vector2 newStartPoint)
        {
            if(_lines.ContainsKey(index))
            {
                _lines[index].SetPosition(0, newStartPoint);
            }
        }
    }
}