using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    // 회전 방향 설정
    public enum RotationAxis { X, Y, Z }

    public IllusionManager illusionManager;

    [Header("회전 대상")]
    [SerializeField] private Transform rotator;

    [Header("회전 설정")]
    [SerializeField] private RotationAxis axis = RotationAxis.Z;        // 회전 방향
    [SerializeField] private float sensitivity = 1f;                    // 민감도
    [SerializeField] private float snapAngle = 90f;                     // 보간되는 각도 단위
    [SerializeField] private float snapSpeed = 1f;                      // 보간 속도

    [Header("잠금 설정")]
    public Vector3 targetPos;                                           // 잠금 연출을 위한 목표 위치
    [SerializeField] private float lockSpeed = 1f;                      // 잠금 속도
    [SerializeField] private bool rotatorLock = true;                   // 캐릭터 위치 시 잠금 여부
    public Vector3 originPos;                                           // 원위치 저장 변수

    [Header("마테리얼")]
    [SerializeField] private Material enterMat;
    [SerializeField] private Material exitMat;

    private float _currentAngle;            // 현재 누적된 각도
    private Quaternion _targetRotation;     // 목표 각도

    // z축 회전용 변수
    private float _mouseStartAngle;
    private float _baseRotation;

    private bool _isCanDrag;            // 드래그 가능 여부
    private bool _isDragging;           // 드래그 중
    private bool _needsPathUpdate;      // 경로 업데이트 여부

    private void Start()
    {
        illusionManager = FindObjectOfType<IllusionManager>();
        // 축에 따라 회전할 각도 설정
        Vector3 currentEuler = rotator.transform.localEulerAngles;
        _currentAngle = GetAngleFromAxis(axis, currentEuler);

        // 목표 각도 초기화
        _targetRotation = rotator.transform.localRotation;

        originPos = this.transform.localPosition;
        targetPos = SetHandleTargetPos();
    }

    private void Update()
    {
        HandleInput();

        Vector3 targetposition = IsCharacterOnRotator() ? targetPos : originPos;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetposition,
            lockSpeed * Time.deltaTime);

        if (_isDragging)
        {
            RotateByMouse();
        }
        else
        {
            SnapToTarget();
        }
    }

    /// <summary>
    /// 마우스 입력 관리
    /// </summary>
    private void HandleInput()
    {
        if (_isCanDrag)
        {
            // 마우스 입력 시작
            if (Input.GetMouseButtonDown(0))
            {
                if (IsCharacterOnRotator() && rotatorLock) return;

                _isDragging = true;

                if (axis == RotationAxis.Z)
                {
                    // z축 회전은 클릭 시점의 원형 각도를 기록
                    _mouseStartAngle = GetMouseAngle();
                    _baseRotation = rotator.transform.localEulerAngles.z;
                }
            }
        }
        // 마우스 입력 종료
        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            _isCanDrag = false;

            // 가장 가까운 보간 각도 계산
            float snappedAngle = Mathf.Round(_currentAngle / snapAngle) * snapAngle;
            _targetRotation = CreateRotationFromAxis(axis, snappedAngle);

            // 값 동기화
            _currentAngle = snappedAngle;

            _needsPathUpdate = true;

            // 기존 착시 연결 제거
            if (illusionManager != null)
            {
                illusionManager.ResetIllusionPaths();
            }
        }
    }

    /// <summary>
    /// 마우스 입력을 받아 물체 회전
    /// </summary>
    private void RotateByMouse()
    {
        if (axis == RotationAxis.X)         // x축을 회전시킬 때
        {
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
            _currentAngle += mouseY;
        }
        else if (axis == RotationAxis.Y)    // y축을 회전시킬 때
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            _currentAngle -= mouseX;
        }
        else if (axis == RotationAxis.Z)    // z축을 회전시킬 때
        {
            float currentMouseAngle = GetMouseAngle();
            float angleDifference = currentMouseAngle - _mouseStartAngle;
            _currentAngle = _baseRotation + angleDifference;
        }

        rotator.transform.localRotation = CreateRotationFromAxis(axis, _currentAngle);
    }

    private void SnapToTarget()
    {
        // 각도 보간
        rotator.transform.localRotation = Quaternion.Slerp(
            rotator.transform.localRotation,
            _targetRotation,
            Time.deltaTime * snapSpeed
            );

        if (_needsPathUpdate && Quaternion.Angle(rotator.transform.localRotation, _targetRotation) < 0.1f)
        {
            // 위치 고정
            rotator.transform.localRotation = _targetRotation;
            _needsPathUpdate = false;

            // 착시 노드 연결
            if (illusionManager != null)
            {
                illusionManager.UpdateIllusionPaths();
            }
        }
    }

    public Vector3 SetHandleTargetPos()
    {
        switch(axis)
        {
            case RotationAxis.X: return originPos - new Vector3(0.5f, 0f, 0f);
            case RotationAxis.Y: return originPos - new Vector3(0f, 0.5f, 0f);
            case RotationAxis.Z: return originPos + new Vector3(0f, 0f, 0.5f);
            default: return Vector3.zero;
        }
    }

    /// <summary>
    /// 캐릭터가 회전 발판 위에 서있는지 여부
    /// </summary>
    /// <returns></returns>
    private bool IsCharacterOnRotator()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();

        if (player != null && player.currentNode != null)
        {
            return player.currentNode.transform.IsChildOf(rotator.GetChild(0));
        }
        return false;
    }

    /// <summary>
    /// 마우스 위치를 각도로 변환 (Z축 전용)
    /// </summary>
    private float GetMouseAngle()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 direction = (Vector2)Input.mousePosition - (Vector2)screenPos;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// 축에 따라 현재 각도 추출
    /// </summary>
    private float GetAngleFromAxis(RotationAxis targetAxis, Vector3 euler)
    {
        return targetAxis == RotationAxis.X ? euler.x :
               targetAxis == RotationAxis.Y ? euler.y : euler.z;
    }

    /// <summary>
    /// 축에 따른 Quaternion 생성
    /// </summary>
    /// <param name="targetAxis"> 축 설정 </param>
    /// <param name="angle"> 누적된 각도 </param>
    /// <returns> 각 축의 값에 각도 설정 </returns>
    private Quaternion CreateRotationFromAxis(RotationAxis targetAxis, float angle)
    {
        switch (targetAxis)
        {
            case RotationAxis.X: return Quaternion.Euler(angle, 0, 0);
            case RotationAxis.Y: return Quaternion.Euler(0, angle, 0);
            case RotationAxis.Z: return Quaternion.Euler(0, 0, angle);
            default: return Quaternion.identity;
        }
    }

    private void OnMouseEnter()
    {
        _isCanDrag = true;

        ChangeColor(enterMat);
    }

    private void OnMouseExit()
    {
        ChangeColor(exitMat);
    }

    private void ChangeColor(Material material)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Renderer childColor = transform.GetChild(i).GetComponent<Renderer>();

            childColor.material = material;
        }
    }
}