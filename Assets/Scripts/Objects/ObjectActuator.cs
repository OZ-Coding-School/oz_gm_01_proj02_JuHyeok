using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActuator : MonoBehaviour
{
    [Header("이동 설정")]
    public Vector3 targetLocalPosition;     // 이동할 목표 좌표
    public float moveSpeed = 4f;

    private Vector3 _startPosition;
    private Vector3 _destination;

    private Rotator rotatorHandle;

    private void Awake()
    {
        _startPosition = transform.localPosition;
        _destination = _startPosition;      // 처음엔 제자리가 목적지

        rotatorHandle = GetComponentInChildren<Rotator>();
    }

    private void Update()
    {
        if (transform.localPosition != _destination)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                _destination,
                moveSpeed * Time.deltaTime
            );

            // 이동 완료 시점 체크
            if (transform.localPosition == _destination)
            {
                OnMovementComplete();
            }
        }
    }

    // 목표 좌표로 이동
    public void MoveToTarget()
    {
        _destination = targetLocalPosition;
    }

    // 원위치로 이동
    public void MoveToStart()
    {
        _destination = _startPosition;
    }

    // 이동 완료
    private void OnMovementComplete()
    {
        Debug.Log($"{gameObject.name} 이동 완료!");

        // 노드 연결 재계산
        if (FindObjectOfType<StageManager>() != null)
            FindObjectOfType<StageManager>().ScanAll();
        // 착시 노드 연결 재계산
        if (IllusionManager.Instance != null)
            IllusionManager.Instance.UpdateIllusionPaths();
    }
}
