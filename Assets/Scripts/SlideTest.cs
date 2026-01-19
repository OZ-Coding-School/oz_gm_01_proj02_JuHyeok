using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideTest : MonoBehaviour
{
    Vector3 clickPoint;
    float upDownSpeed = 5.0f;

    void OnMouseDown()
    {
        clickPoint = Input.mousePosition;
    }

    void OnMouseDrag()
    {
        Vector3 diff = Input.mousePosition - clickPoint;
        Vector3 pos = transform.position;

        pos.y += diff.y * Time.deltaTime * upDownSpeed;
        transform.position = pos;

        clickPoint = Input.mousePosition;
    }
}
