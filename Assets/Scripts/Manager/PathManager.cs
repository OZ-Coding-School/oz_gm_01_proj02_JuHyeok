using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathManager
{
    /// <summary>
    /// 경로 설정
    /// </summary>
    /// <param name="startNode"> 경로 시작 노드 </param>
    /// <param name="endNode"> 경로 마지막 노드 </param>
    /// <returns></returns>
    public static List<Node> FindPath(Node startNode, Node endNode)
    {
        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(startNode);
        
        // 지나온 노드 저장용 딕셔너리 (첫 노드일 땐 null)
        Dictionary<Node, Node> comeFrom = new Dictionary<Node, Node>();
        comeFrom[startNode] = null;

        // BFS 알고리즘 적용
        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();
            // 마지막 노드 도착 시 break
            if (currentNode == endNode) break;

            foreach (Node neighbor in currentNode.neighborNodes)
            {
                if (!comeFrom.ContainsKey(neighbor) && neighbor.isWalkable)
                {
                    comeFrom[neighbor] = currentNode;
                    queue.Enqueue(neighbor);
                }
            }

            // 착시 연결 노드가 있는 경우 이웃으로 간주
            if (currentNode.illusionNeighbor != null && currentNode.illusionNeighbor.isWalkable)
            {
                Node illusion = currentNode.illusionNeighbor;

                if (!comeFrom.ContainsKey(illusion))
                {
                    comeFrom[illusion] = currentNode;
                    queue.Enqueue(illusion);
                }
            }
        }

        // 경로 재구성
        List<Node> path = new List<Node>();

        Node curr = endNode;
        while (curr != startNode)
        {
            // 경로에 마지막 노드가 포함되어 있지 않으면 null 반환
            if (!comeFrom.ContainsKey(curr)) return null;

            // 마지막 노드부터 역순으로 리스트에 저장
            path.Add(curr);
            curr = comeFrom[curr];
        }

        path.Reverse();
        return path;
    }
}
