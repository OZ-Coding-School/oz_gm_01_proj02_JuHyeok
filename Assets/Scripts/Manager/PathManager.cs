using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    /// <returns></returns>
    public static List<Node> FindPath(Node startNode, Node endNode)
    {
        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(startNode);

        Dictionary<Node, Node> comeFrom = new Dictionary<Node, Node>();
        comeFrom[startNode] = null;

        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();

            if (currentNode == endNode) break;

            foreach (Node neighbor in currentNode.neighborNodes)
            {
                if (!comeFrom.ContainsKey(neighbor) && neighbor.isWalkable)
                {
                    comeFrom[neighbor] = currentNode;
                    queue.Enqueue(neighbor);
                }
            }
        }

        List<Node> path = new List<Node>();
        Node curr = endNode;
        while (curr != startNode)
        {
            if (!comeFrom.ContainsKey(curr)) return null;
            path.Add(curr);
            curr = comeFrom[curr];
        }

        path.Reverse();
        return path;
    }
}
