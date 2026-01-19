using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider : MonoBehaviour
{
    public enum SlideAxis { X, Y, Z }

    [Header("이동 대상")]
    [SerializeField] private Transform slider;
    [SerializeField] private Transform slidePartner;
    [SerializeField] private bool isReverse = true;

    [Header("제한 설정")]
    public SlideAxis axis = SlideAxis.X;    // 이동축
    public float maxDistance = 5f;          // 이동 가능 거리
    public float minDistance = -5f;         // 이동 가능 거리
    public float sensitivity = 4f;          // 민감도

    [Header("마테리얼")]
    [SerializeField] private Material enterMat;
    [SerializeField] private Material exitMat;

    private Vector3 _initialSliderPos;      // 슬라이더 처음 위치
    private Vector3 _initialPartnerPos;     // 슬라이드 파트너 첫 위치

    private float _currentOffset = 0f;      // 초기 위치로부터 떨어진 거리
    private bool _isCanDrag = false;        // 드래그 가능 여부
    private bool _isDragging = false;       // 드래그 중
    private Vector3 _lastMousePos;

    private Coroutine _snapCo;              

    private void Awake()
    {
        if (slider != null)
            _initialSliderPos = slider.localPosition;
        if (slidePartner != null)
            _initialPartnerPos = slidePartner.localPosition;
    }

    void OnMouseDown()
    {
        if (_isCanDrag)
        {
            _isDragging = true;
            _lastMousePos = Input.mousePosition;
        }
    }

    void OnMouseDrag()
    {
        if (!_isDragging) return;

        Vector2 currentMousePos = Input.mousePosition;

        SlideByMouse(currentMousePos);
    }

    void OnMouseUp()
    {
        _isDragging = false;

        // 이동 위치 반올림(스냅 목표 설정)
        _currentOffset = Mathf.Round(_currentOffset);

        _currentOffset = Mathf.Clamp(_currentOffset, minDistance, maxDistance);

        UpdatePositionDirectly();
    }

    /// <summary>
    /// 마우스 입력을 받아 슬라이더(and 파트너)의 위치 이동
    /// </summary>
    /// <param name="mousePos"> 현재 마우스의 좌표값 </param>
    private void SlideByMouse(Vector3 mousePos)
    {
        // 마우스 이동량 계산
        Vector3 mouseDelta = mousePos - _lastMousePos;

        // 입력값 추출
        float inputDelta = (axis == SlideAxis.Y) ? mouseDelta.y : mouseDelta.x;

        // 오프셋 업데이트
        _currentOffset += inputDelta * (sensitivity * 0.01f);
        _currentOffset = Mathf.Clamp(_currentOffset, minDistance, maxDistance);

        // 최종 위치 계산
        Vector3 targetPos = _initialSliderPos;
        Vector3 partnerTargetPos = _initialPartnerPos;

        switch (axis)
        {
            case SlideAxis.X:
                targetPos.x += _currentOffset;
                if (slidePartner != null)
                    partnerTargetPos.x += isReverse ? -_currentOffset : _currentOffset;
                break;
            case SlideAxis.Y:
                targetPos.y += _currentOffset;
                if (slidePartner != null)
                    partnerTargetPos.y += isReverse ? -_currentOffset : _currentOffset;
                break;
            case SlideAxis.Z:
                targetPos.z += _currentOffset;
                if (slidePartner != null)
                    partnerTargetPos.z += isReverse ? -_currentOffset : _currentOffset;
                break;
        }

        // 로컬 좌표로 적용
        slider.localPosition = targetPos;
        if (slidePartner != null)
            slidePartner.localPosition = partnerTargetPos;

        _lastMousePos = mousePos;
    }

    private void UpdatePositionDirectly()
    {
        // 기존에 실행 중인 스냅 코루틴 중지
        if (_snapCo != null) StopCoroutine(_snapCo);

        // 최종 목표 위치 계산
        Vector3 targetSliderPos = _initialSliderPos + (GetDirectionVector() * _currentOffset);
        Vector3 targetPartnerPos = Vector3.zero;

        if (slidePartner != null)
        {
            float partnerOffset = isReverse ? -_currentOffset : _currentOffset;
            targetPartnerPos = _initialPartnerPos + (GetDirectionVector() * partnerOffset);
        }

        // 코루틴 시작
        _snapCo = StartCoroutine(SnapRoutine(targetSliderPos, targetPartnerPos));
    }

    /// <summary>
    /// 스냅 실행 코루틴
    /// </summary>
    /// <param name="targetSlider"> 슬라이더의 목표 위치값 </param>
    /// <param name="targetPartner"> 파트너의 목표 위치값</param>
    /// <returns></returns>
    private IEnumerator SnapRoutine(Vector3 targetSlider, Vector3 targetPartner)
    {
        float t = 0;
        float snapDuration = 0.5f; // 스냅에 걸리는 시간
        Vector3 startSlider = slider.localPosition;
        Vector3 startPartner = slidePartner != null ? slidePartner.localPosition : Vector3.zero;

        while (t < 1.0f)
        {
            t += Time.deltaTime / snapDuration;

            // Lerp로 이동
            slider.localPosition = Vector3.Lerp(startSlider, targetSlider, t);
            if (slidePartner != null)
                slidePartner.localPosition = Vector3.Lerp(startPartner, targetPartner, t);

            yield return null;
        }

        // 마지막 위치 고정 및 노드 갱신
        slider.localPosition = targetSlider;
        if (slidePartner != null) slidePartner.localPosition = targetPartner;

        if (IllusionManager.Instance != null)
            IllusionManager.Instance.UpdateIllusionPaths();
    }

    private Vector3 GetDirectionVector()
    {
        switch (axis)
        {
            case SlideAxis.X: return Vector3.right;
            case SlideAxis.Y: return Vector3.up;
            case SlideAxis.Z: return Vector3.forward;
            default: return Vector3.zero;
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
