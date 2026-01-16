using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapActuator : MonoBehaviour
{
    private Vector3 activePosition;   // 버튼 눌렸을 때 위치
    private Vector3 inactivePosition; // 평상시 위치
    public float moveSpeed = 2f;

    private Vector3 _targetPosition;

    private void Start()
    {
        inactivePosition = transform.localPosition;
        activePosition = inactivePosition - new Vector3(0f, 0.07f);
        _targetPosition = inactivePosition;
    }

    private void Update()
    {
        // 부드러운 이동
        transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPosition, Time.deltaTime * moveSpeed);
    }

    // 버튼 이벤트와 연결할 함수들
    public void Activate() => _targetPosition = activePosition;
}
