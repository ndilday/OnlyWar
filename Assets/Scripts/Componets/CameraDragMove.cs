using System;
using System.Drawing;
using UnityEngine;

namespace Iam.Scripts.Componets
{
    public class CameraDragMove : MonoBehaviour
    {
        private bool _drag = false;
        private Vector3 _origin;
        public GameSettings GameSettings;

        // Start is called before the first frame update
        void Start()
        {
            //Camera.main.orthographicSize = Screen.height / 8;
        }

        void LateUpdate()
        {
            Vector3 difference = Vector3.zero;
            if (Input.GetMouseButton(1))
            {
                difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
                if (_drag == false)
                {
                    _drag = true;
                    _origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            else
            {
                _drag = false;
            }
            if (_drag == true)
            {
                MoveCamera(_origin, difference);
            }
            if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                float newSize = Mathf.Clamp(Camera.main.orthographicSize / 2f, 20, 320);
                Camera.main.orthographicSize = newSize;
            }
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                float newSize = Mathf.Clamp(Camera.main.orthographicSize * 2f, 20, 320);
                Camera.main.orthographicSize = newSize;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                difference = new Vector3(0, Camera.main.orthographicSize / 2.5f, 0);
                MoveCamera(Camera.main.transform.position, difference);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                difference = new Vector3(0, -Camera.main.orthographicSize / 2.5f, 0);
                MoveCamera(Camera.main.transform.position, difference);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                float width = Camera.main.orthographicSize * Screen.width / Screen.height;
                difference = new Vector3(width / 2.5f, 0, 0);
                MoveCamera(Camera.main.transform.position, difference);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                float width = Camera.main.orthographicSize * Screen.width / Screen.height;
                difference = new Vector3(-width / 2.5f, 0, 0);
                MoveCamera(Camera.main.transform.position, difference);
            }
        }

        private void MoveCamera(Vector3 origin, Vector3 difference)
        {
            float screenHeightInUnits = Camera.main.orthographicSize * 2.0f;
            float unitsPerPixel = screenHeightInUnits / Screen.height;
            Vector3 newPosition = origin - difference;
            float screenWidthInUnits = screenHeightInUnits * Screen.width / Screen.height;
            float mapTop = GameSettings.GalaxySize * GameSettings.MapScale.y;
            float mapRight = GameSettings.GalaxySize * GameSettings.MapScale.x;
            // assume for now that the world is 450x450
            float minX = (screenWidthInUnits / 2.0f) - (40.0f*unitsPerPixel);
            float maxX = mapRight + (40.0f * unitsPerPixel) - (screenWidthInUnits / 2.0f);
            float minY = (screenHeightInUnits / 2.0f) - (90.0f * unitsPerPixel);
            float maxY = mapTop + (100.0f * unitsPerPixel) - (screenHeightInUnits / 2.0f);
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
            Camera.main.transform.position = newPosition;
        }
    }
}
