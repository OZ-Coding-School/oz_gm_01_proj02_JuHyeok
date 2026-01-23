using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IllusionManager : Singleton<IllusionManager>
{
    [Header("착시 설정")]
    [SerializeField] private float onScreenDistance = 50.0f;
    public int neighborCount = 2;
    private Camera _mainCam;

    private Node[] allNodes;

    // 설정 초기화
    public void Init(float distance = 50.0f, int count = 2)
    {
        _mainCam = Camera.main;

        onScreenDistance = distance;
        neighborCount = count;
    }

    /// <summary>
    /// 노드 리스트 갱신
    /// </summary>
    [ContextMenu("RefreshNodeList")]
    public void NodeInit()
    {
        allNodes = FindObjectsOfType<Node>();
    }

    /// <summary>
    /// 착시로 연결되는 노드의 경로 연결
    /// </summary>
    public void UpdateIllusionPaths()
    {
        StartCoroutine(StopBeforeFindCam());
        ResetIllusionPaths();

        for (int i = 0; i < allNodes.Length; i++)
        {
            for (int j = i + 1; j < allNodes.Length; j++)
            {
                // 거리가 일정 수치 이하,
                // 기존에 이웃이 아니고,
                // 길이 끊어져있으면 => 착시 이웃으로 연결
                if (allNodes[i] != allNodes[j] &&
                    IsOverapping(allNodes[i], allNodes[j]) &&
                    !allNodes[i].neighborNodes.Contains(allNodes[j]) &&
                    allNodes[i].neighborNodes.Count < neighborCount &&
                    allNodes[j].neighborNodes.Count < neighborCount &&
                    allNodes[i].isWalkable && allNodes[j].isWalkable)
                {
                    allNodes[i].illusionNeighbor = allNodes[j];
                    allNodes[j].illusionNeighbor = allNodes[i];
                }
            }

            if (allNodes[i].neighborNodes != null)
            {
                foreach (var node in allNodes[i].neighborNodes)
                {
                    if (node.neighborNodes.Contains(allNodes[i].illusionNeighbor))
                        allNodes[i].illusionNeighbor = null;
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
        Vector3 screenPosA = _mainCam.WorldToScreenPoint(a.transform.position);
        Vector3 screenPosB = _mainCam.WorldToScreenPoint(b.transform.position);

        screenPosA.z = 0;
        screenPosB.z = 0;

        return Vector3.Distance(screenPosA, screenPosB) < onScreenDistance;
    }

    private IEnumerator StopBeforeFindCam()
    {
        if (_mainCam == null) yield return null;
    }
}
