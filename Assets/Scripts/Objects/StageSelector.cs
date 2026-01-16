using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelector : MonoBehaviour
{
    [Header("회전 설정")]
    [SerializeField] private Transform pivot;               // 회전 중심점
    [SerializeField] private float sensitivity = 0.5f;      // 드래그 민감도
    [SerializeField] private float snapSpeed = 5f;          // 스냅 속도
    [SerializeField] private float objectAngle = 90f;       // 물체 형태에 따른 스냅 각도

    private float _currentYRotation;
    private float _targetYRotation;
    private bool _isDragging;
    private Vector2 _lastMousePos;

    void Update()
    {
        HandleInput();
        ApplyRotation();
    }

    private void HandleInput()
    {
        // 드래그 시작
        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = true;
            _lastMousePos = Input.mousePosition;
        }

        // 드래그 중
        if (Input.GetMouseButton(0) && _isDragging)
        {
            float deltaX = Input.mousePosition.x - _lastMousePos.x;
            _currentYRotation -= deltaX * sensitivity;
            _lastMousePos = Input.mousePosition;

            // 드래그 중 즉시 회전 반영
            pivot.rotation = Quaternion.Euler(0, _currentYRotation, 0);
        }

        // 드래그 종료
        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            // 가장 가까운 스테이지 각도로 보정
            float snapAngle = Mathf.Round(_currentYRotation / objectAngle) * objectAngle;

            _targetYRotation = snapAngle;
            _currentYRotation = snapAngle; // 다음 드래그 시작점 동기화

            // [선택] 클릭 판정: 드래그 거리가 매우 짧다면 해당 스테이지 선택으로 간주
            // 여기에 Raycast를 이용해 스테이지 클릭 로직을 추가할 수 있습니다.
        }
    }

    private void ApplyRotation()
    {
        if (!_isDragging)
        {
            // 드래그가 끝나면 목표 각도로 부드럽게 회전
            pivot.rotation = Quaternion.Slerp(
                pivot.rotation,
                Quaternion.Euler(0, _targetYRotation, 0),
                Time.deltaTime * snapSpeed
            );
        }
    }
}
