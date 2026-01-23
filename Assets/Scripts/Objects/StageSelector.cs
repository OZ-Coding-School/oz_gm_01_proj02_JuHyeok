using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelector : MonoBehaviour
{
    [Header("회전 설정")]
    [SerializeField] private Transform pivot;               // 회전 중심점
    [SerializeField] private float sensitivity = 0.5f;      // 드래그 민감도
    [SerializeField] private float snapSpeed = 5f;          // 스냅 속도
    [SerializeField] private float objectAngle = 90f;       // 물체 형태에 따른 스냅 각도

    [Header("클릭 판정")]
    [SerializeField] private float clickThreshold = 15f;    // 클릭 인식 판정 거리
    [SerializeField] private float timeThreshold = 0.2f;    // 클릭 인식 판정 시간

    public int currentAngleStage;

    private float _currentYRotation;            // 현재 Y 회전값
    private float _targetYRotation;             // 목표 Y 회전값
    private bool _isDragging;
    private Vector2 _lastMousePos;

    private float _mouseDownTime;
    private Vector2 _mouseDownPos;

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

            // 클릭 판정 초기화
            _mouseDownTime = Time.time;
            _mouseDownPos = Input.mousePosition;
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

            // 드래그 한 거리, 시간 계산
            float dragDistance = Vector2.Distance(_mouseDownPos, Input.mousePosition);
            float dragDuration = Time.time - _mouseDownTime;

            if (dragDistance < clickThreshold && dragDuration < timeThreshold)
            {
                CheckStageClick();
            }

            // 가장 가까운 스테이지 각도로 보정
            float snapAngle = Mathf.Round(_currentYRotation / objectAngle) * objectAngle;

            _targetYRotation = snapAngle;
            _currentYRotation = snapAngle; // 다음 드래그 시작점 동기화

            // 클릭 판정: 드래그 거리가 매우 짧다면 해당 스테이지 선택으로 간주
        }
    }

    private void ApplyRotation()
    {
        if (!_isDragging)
        {
            // 목표 각도로 회전
            pivot.rotation = Quaternion.Slerp(
                pivot.rotation,
                Quaternion.Euler(0, _targetYRotation, 0),
                Time.deltaTime * snapSpeed
            );
        }
    }

    private void CheckStageClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            StageFlat flat = hit.collider.GetComponent<StageFlat>();

            if (flat != null)
            {
                int maxCleared = PlayerPrefs.GetInt("MaxClearedStage", 0);

                if (flat.stageIndex <= maxCleared)
                {
                    PlayerPrefs.SetInt("SelectedStage", flat.stageIndex);
                    SceneManager.LoadScene("Stage");
                }
                else
                {

                }
            }
        }
    }
}
