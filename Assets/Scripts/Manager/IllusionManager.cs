using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllusionManager : Singleton<IllusionManager>
{
    [Header("착시 설정")]
    [SerializeField] private float onScreenDistance = 50.0f;
    [SerializeField] private int neighborCount = 2;
    private Camera mainCam;

    private Node[] allNodes;

    public void Init(float distance = 50.0f, int count = 2)
    {
        onScreenDistance = distance;
        neighborCount = count;
    }

    public override void Awake()
    {
        mainCam = Camera.main;
    }

    private void Start()
    {
        Debug.Log($"{Vector3.Distance(new Vector3(1.5f, 0f, 0.5f), new Vector3(-3.5f, 6f, -5.5f))}");
    }

    /// <summary>
    /// 노드 리스트 갱신
    /// </summary>
    public void NodeInit()
    {
        allNodes = FindObjectsOfType<Node>();
    }

    /// <summary>
    /// 착시로 연결되는 노드의 경로 연결
    /// </summary>
    public void UpdateIllusionPaths()
    {
        ResetIllusionPaths();

        for (int i = 0; i < allNodes.Length; i++)
        {
            for (int j = i + 1; j < allNodes.Length; j++)
            {
                // 거리가 일정 수치 이하,
                // 기존에 이웃이 아니고,
                // 길이 끊어져있으면 => 착시 이웃으로 연결
                if (IsOverapping(allNodes[i], allNodes[j]) &&
                    !allNodes[i].neighborNodes.Contains(allNodes[j]) &&
                    allNodes[i].neighborNodes.Count < neighborCount &&
                    allNodes[j].neighborNodes.Count < neighborCount)
                {
                    allNodes[i].illusionNeighbor = allNodes[j];
                    allNodes[j].illusionNeighbor = allNodes[i];
                }
            }
        }
    }

    /// <summary>
    /// 착시 이웃 노드 초기화
    /// </summary>
    public void ResetIllusionPaths()
    {
        Node[] allNodes = FindObjectsOfType<Node>();

        foreach (var node in allNodes)
            node.illusionNeighbor = null;
    }

    /// <summary>
    /// 두 노드의 스크린 상 위치가 일정 수치 이하인지 판정
    /// </summary>
    /// <param name="a"> 노드 a </param>
    /// <param name="b"> 노드 b </param>
    /// <returns> true or false </returns>
    private bool IsOverapping(Node a, Node b)
    {
        Vector3 screenPosA = mainCam.WorldToScreenPoint(a.transform.position);
        Vector3 screenPosB = mainCam.WorldToScreenPoint(b.transform.position);

        screenPosA.z = 0;
        screenPosB.z = 0;

        return Vector3.Distance(screenPosA, screenPosB) < onScreenDistance;
    }
}
