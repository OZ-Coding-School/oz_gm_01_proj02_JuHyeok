using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("노드 정보")]
    public bool isWalkable = true;
    public float stepCost = 1f;

    [Header("연결된 노드")]
    public List<Node> neighborNodes = new List<Node>();

    [HideInInspector]
    public Node illusionNeighbor;           // 착시로 연결되는 노드

    // 캐릭터가 설 노드 중앙값
    public Vector3 walkTarget => transform.position + Vector3.up * 0.5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = isWalkable ? Color.green : Color.red;

        foreach (var neighbor in neighborNodes)
        {
            if(neighbor != null)
            {
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
            }
        }
    }
}
