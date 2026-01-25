using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    [ContextMenu("Scan All Nodes in Scene")]
    public void ScanAll()
    {
        Node[] allNodes = FindObjectsOfType<Node>();
        foreach(Node node in allNodes)
        {
            node.ClearNeighbors();
            if (node.type == NodeType.Illusion)
            {
                node.GetComponent<IllusionNode>().connectNeighbor();
            }
            node.ScanNeighbors();
        }
    }
}
