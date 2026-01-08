using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Node currentNode;
    [SerializeField] private float moveSpeed = 3f;
    private bool isMoving = false;

    // 초기 회전값
    private Quaternion initialRot;

    private void Start()
    {
        initialRot = transform.rotation;
    }

    private void Update()
    {
        ClickToMove();
    }

    private void LateUpdate()
    {
        float currentY = transform.localEulerAngles.y;

        // 회전값 동결
        transform.rotation = Quaternion.Euler(initialRot.eulerAngles.x, currentY, initialRot.eulerAngles.z);
    }

    /// <summary>
    /// 마우스 클릭으로 캐릭터를 특정 노드로 이동
    /// </summary>
    private void ClickToMove()
    {
        // 캐릭터가 중지 상태일 때 마우스 입력
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 레이캐스트에 감지된 노드
                Node targetNode = hit.collider.GetComponentInParent<Node>();

                // 감지된 노드가 이동 가능한 상태일 때
                if (targetNode != null && targetNode.isWalkable)
                {
                    List<Node> path = PathManager.FindPath(currentNode, targetNode);

                    if (path != null && path.Count > 0)
                    {
                        StartCoroutine(FollowPath(path));
                    }
                }
            }
        }
    }

    /// <summary>
    /// 경로 따라 캐릭터 이동
    /// </summary>
    /// <param name="path"> 이동해야 하는 노드 리스트 </param>
    /// <returns></returns>
    private IEnumerator FollowPath(List<Node> path)
    {
        // 움직이는 상태
        isMoving = true;

        foreach (Node nextNode in path)
        {
            // 목표 지점 설정
            Vector3 targetPos = nextNode.walkTarget;

            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                Vector3 direction = (targetPos - transform.position).normalized;

                // 이동 방향 주시
                if (direction != Vector3.zero)
                    transform.forward = direction;

                yield return null;
            }
            // 현재 위치 재설정
            currentNode = nextNode;
        }
        // 움직임이 종료하면 중지 상태
        isMoving = false;
    }
}
