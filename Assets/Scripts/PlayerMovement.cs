using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Node currentNode;
    private float moveSpeed;

    [Header("캐릭터 속도")]
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float illusionSpeed;
    public bool isMoving = false;

    // 초기 회전값
    private Quaternion initialRot;

    [Header("클릭 프리팹")]
    [SerializeField] private Indicator clickIndicator;

    private void Start()
    {
        initialRot = transform.rotation;
    }

    private void Update()
    {
        ClickToMove();

        if (IsEnterGoal())
        {
            GameManager.Instance.StageClear();
        }
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

                // 감지된 노드가 이동 가능한 상태일 때, 사다리 노드가 아닐 때
                if (targetNode != null && targetNode.isWalkable && targetNode.shape != NodeShape.Ladder)
                {
                    Vector3 shapeVec = targetNode.shape == NodeShape.Cube ? Vector3.up * 0.05f : Vector3.up * -0.4f;

                    Vector3 effectPos = targetNode.walkTarget + shapeVec;
                    var click = PoolManager.Instance.GetFromPool(clickIndicator);

                    click.transform.position = effectPos;
                    click.InstantiateIndicator();

                    List<Node> path = PathManager.FindPath(currentNode, targetNode);

                    if (path != null && path.Count > 0)
                    {
                        StartCoroutine(FollowPath(path));
                    }
                }
            }
        }
    }

    /// 목적지 도착 여부
    private bool IsEnterGoal()
    {
        return currentNode.type == NodeType.Goal;
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
            // 일반 이동
            yield return StartCoroutine(MoveToNode(nextNode));
            
            // 현재 위치 재설정
            currentNode = nextNode;

            // 회전판, 슬라이더의 경우 자식으로 배정
            if (currentNode.transform.parent != null)
            {
                transform.SetParent(currentNode.transform.parent);
            }
            else
            {
                transform.SetParent(null);
            }
        }
        // 움직임이 종료하면 중지 상태
        isMoving = false;

        if (IsEnterGoal())
        {
            LevelManager levelManager = LevelManager.Instance;

            levelManager.LoadStage(levelManager.currentStageIndex + 1);
        }
    }
    /// <summary>
    /// 다음 노드로 이동
    /// </summary>
    /// <param name="nextNode"> 이동 목표 노드 </param>
    /// <returns></returns>
    private IEnumerator MoveToNode(Node nextNode)
    {
        // 목표 지점 설정
        Vector3 targetPos = nextNode.walkTarget;
        // 착시 판단
        bool isNearIllusion = currentNode.IsSamePosY(nextNode);

        illusionSpeed = normalSpeed * Vector3.Distance(currentNode.transform.position, targetPos);

        // 이동 속도 결정
        moveSpeed = isNearIllusion ? normalSpeed : illusionSpeed;

        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            Vector3 direction = (targetPos - transform.position).normalized;

            // 이동 방향 주시
            if (direction != Vector3.zero)
                transform.forward = direction;

            yield return null;
        }
    }
}
