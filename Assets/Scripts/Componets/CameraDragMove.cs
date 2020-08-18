using UnityEngine;

namespace Iam.Scripts.Componets
{
    public class CameraDragMove : MonoBehaviour
    {
        private bool _drag = false;
        private Vector3 _origin;

        // Start is called before the first frame update
        void Start()
        {
            // at the start, adjust the map zoom level to be 1:1 pixel
            Camera.main.orthographicSize = Screen.height / 16;
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
                Camera.main.orthographicSize /= 1.5f;
            }
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                Camera.main.orthographicSize *= 1.5f;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                float height = Camera.main.orthographicSize * 2.0f;
                difference = new Vector3(0, height / 5.0f, 0);
                MoveCamera(Camera.main.transform.position, difference);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                float height = Camera.main.orthographicSize * 2.0f;
                difference = new Vector3(0, -height / 5.0f, 0);
                MoveCamera(Camera.main.transform.position, difference);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                float height = Camera.main.orthographicSize * 2;
                float width = height * Screen.width / Screen.height;
                difference = new Vector3(width / 5.0f, 0, 0);
                MoveCamera(Camera.main.transform.position, difference);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                float height = Camera.main.orthographicSize * 2;
                float width = height * Screen.width / Screen.height;
                difference = new Vector3(-width / 5.0f, 0, 0);
                MoveCamera(Camera.main.transform.position, difference);
            }
        }

        private void MoveCamera(Vector3 origin, Vector3 difference)
        {
            float ratio = Camera.main.orthographicSize * 2.0f / Screen.height;
            Vector3 newPosition = origin - difference;
            float height = Camera.main.orthographicSize;
            float width = height * Screen.width / Screen.height;
            // assume for now that the world is 450x450
            float minX = width - (40.0f*ratio);
            float maxX = 300.0f + (10.0f*ratio) - width;
            float minY = height - (100.0f*ratio);
            float maxY = 300.0f + (50.0f*ratio) - height;
            if (newPosition.x > maxX) newPosition.x = maxX;
            if (newPosition.x < minX) newPosition.x = minX;
            if (newPosition.y > maxY) newPosition.y = maxY;
            if (newPosition.y < minY) newPosition.y = minY;
            Camera.main.transform.position = newPosition;
        }
    }
}
