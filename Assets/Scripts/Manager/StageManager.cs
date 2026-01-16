using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [ContextMenu("Scan All Nodes in Scene")]
    public void ScanAll()
    {
        Node[] allNodes = FindObjectsOfType<Node>();
        foreach(Node node in allNodes)
        {
            //node.ClearNeighbors();
            node.ScanNeighbors();
        }
    }
}
