using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchZoom : MonoBehaviour
{
    bool notOverUI = true;
    Vector3 touchStart;
    Vector3 lockStart;
    public float speed = 0.25f;
    public float zoomOutMin = 1;
    public float zoomOutMax = 10;
    public Vector2 minPan = new Vector2(0.0f, 0.0f);
    public Vector2 maxPan = new Vector2(8.0f, 8.0f);

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            notOverUI = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            lockStart = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (lockStart.y < 0.75f && lockStart.y > 0.25f && notOverUI)
            {
                touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                notOverUI = false;
            }
        }
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            zoom(difference * 0.01f * speed);
        }
        else if (Input.GetMouseButton(0) && notOverUI)
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float newX = Camera.main.transform.position.x + direction.x;
            float newY = Camera.main.transform.position.y + direction.y;
            if (newX > maxPan.x || newX < minPan.x) { direction.x = 0; }
            if (newY > maxPan.y || newY < minPan.y) { direction.y = 0; }
            Camera.main.transform.position += direction;
            
        }
        zoom(Input.GetAxis("Mouse ScrollWheel")*speed);
    }

    void zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }
}

