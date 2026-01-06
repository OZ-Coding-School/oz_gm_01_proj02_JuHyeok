using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [Header("회전 대상")]
    [SerializeField] private Transform Rotator;

    [Header("Rotation Axis")]
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;

    [Header("각도 보간")]
    [SerializeField] float degreeStep = 90f;
    [SerializeField] float snapSpeed = 5f;

    [Header("마테리얼")]
    [SerializeField] private Material enterMat;
    [SerializeField] private Material exitMat;

    private float targetAngle;
    private float sensitivity = 1f;

    private bool isCanDrag;         // 드래그 가능 여부
    private bool isDragging;        // 드래그 중
    private bool isSnapping;        // 보간 중

    private Vector2 prevDirection;

    private void Update()
    {
        // 마우스 입력
        if (isCanDrag)
        {
            if (Input.GetMouseButtonDown(0))
                StartDrag();
        }
        // 마우스 입력 끝
        if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }

        // 마우스 드래그
        if (isDragging)
            DragRotate();

        if (isSnapping)
            SnapToTarget();
    }

    private void StartDrag()
    {
        Vector2 mouseWorldPos = GetMouseWorldPos();
        prevDirection = mouseWorldPos - (Vector2)Rotator.transform.position;

        isDragging = true;
        isSnapping = false;
    }

    private void EndDrag()
    {
        isDragging = false;
        isCanDrag = false;
        SetSnapTarget();
        isSnapping = true;
    }

    /// <summary>
    /// 드래그로 물체 회전
    /// </summary>
    private void DragRotate()
    {
        Vector2 mouseWorldPos = GetMouseWorldPos();
        Vector2 currentDirection = mouseWorldPos - (Vector2)transform.position;

        if (currentDirection.sqrMagnitude < 0.001f || prevDirection.sqrMagnitude < 0.001f)
            return;

        float angle = Vector2.SignedAngle(prevDirection, currentDirection);
        Rotator.transform.Rotate(rotationAxis, angle * sensitivity, Space.Self);

        prevDirection = currentDirection;
    }

    /// <summary>
    /// 목표 각도로 보간
    /// </summary>
    private void SnapToTarget()
    {
        float currentZ = NormalizeAngle(transform.localEulerAngles.z);
        float snappingZ = Mathf.LerpAngle(currentZ, targetAngle, Time.deltaTime * snapSpeed);

        transform.localEulerAngles = new Vector3(0, 0, snappingZ);

        if (Mathf.Abs(Mathf.DeltaAngle(snappingZ, targetAngle)) < 0.1f)
        {
            transform.localEulerAngles = new Vector3(0, 0, targetAngle);
            isSnapping = false;
        }
    }

    /// <summary>
    /// 보간될 각도 반환
    /// </summary>
    private void SetSnapTarget()
    {
        float currentZ = NormalizeAngle(transform.localEulerAngles.z);
        targetAngle = Mathf.Round(currentZ / degreeStep) * degreeStep;
    }

    /// <summary>
    /// 각도 보정
    /// </summary>
    /// <param name="angle"> 트랜스폼 각도 </param>
    /// <returns> 보정된 각도 </returns>
    private float NormalizeAngle(float angle)
    {
        angle %= 360f;

        if (angle > 180f) 
            angle -= 360f;

        return angle;
    }

    /// <summary>
    /// 마우스 포지션 값 산출
    /// </summary>
    /// <returns> 월드에 적용되는 마우스 포지션 </returns>
    private Vector2 GetMouseWorldPos()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z);
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }

    private void OnMouseEnter()
    {
        isCanDrag = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            Renderer childColor = transform.GetChild(i).GetComponent<Renderer>();

            ChangeColor(childColor, enterMat);
        }
    }

    private void OnMouseExit()
    {
        isCanDrag = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            Renderer childColor = transform.GetChild(i).GetComponent<Renderer>();

            ChangeColor(childColor, exitMat);
        }
    }

    private void ChangeColor(Renderer renderer, Material material)
    {
        renderer.material = material;
    }
}
