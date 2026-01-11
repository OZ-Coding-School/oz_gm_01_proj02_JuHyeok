using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType { Normal, Illusion }

public class Node : MonoBehaviour
{

    [Header("노드 정보")]
    public bool isWalkable = true;
    public float stepCost = 1f;
    public NodeType type = NodeType.Normal;

    [Header("노드 연결")]
    public List<Node> neighborNodes = new List<Node>();
    public float scanDistance = 1.05f;

    [HideInInspector]
    public Node illusionNeighbor;           // 착시로 연결되는 노드

    // 캐릭터가 설 노드 중앙값
    public Vector3 walkTarget => transform.position + Vector3.up * 1f;

    /// <summary>
    /// 주변 노드 탐색, 이웃 노드 리스트에 할당
    /// </summary>
    [ContextMenu("Scan Neighbors")]
    public void ScanNeighbors()
    {
        neighborNodes.Clear();

        Vector3[] directions = 
            { Vector3.up, 
            Vector3.down, 
            Vector3.left, 
            Vector3.right, 
            Vector3.forward, 
            Vector3.back 
        };

        foreach (Vector3 dir in directions)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, out RaycastHit hit, scanDistance))
            {
                Node neighborNode = hit.collider.GetComponent<Node>();

                if (neighborNode != null && neighborNode != this)
                {
                    neighborNodes.Add(neighborNode);
                }
            }
        }
    }

    /// <summary>
    /// 이웃 노드 또는 현재 노드가 퍼즐인지 확인
    /// </summary>
    /// <param name="target"> 상대 노드 </param>
    /// <returns></returns>
    public bool IsNearPuzzle(Node target)
    {
        return (this.transform.parent.name == "RotatorNodes")
            || (target.transform.parent.name == "RotatorNodes")
            || (this.transform.parent.name == "SliderNodes")
            || (target.transform.parent.name == "SlicerNodes");
    }

    // 노드 연결 확인 기즈모
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;

        foreach (var neighbor in neighborNodes)
        {
            if (neighbor != null)
                Gizmos.DrawLine(transform.position + Vector3.up * 1f, neighbor.transform.position + Vector3.up * 1f);
        }
    }
    // 이동 가능 여부 확인 기즈모
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
