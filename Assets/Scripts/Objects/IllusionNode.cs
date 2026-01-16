using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllusionNode : MonoBehaviour
{
    private void Start()
    {
        connectNeighbor();
    }

    public void connectNeighbor()
    {
        Node neighbor = transform.parent.GetChild(0).Find("TransitionNode").GetComponent<Node>();

        if (neighbor != null && !GetComponent<Node>().neighborNodes.Contains(neighbor))
        {
            neighbor.neighborNodes.Add(this.GetComponent<Node>());
            GetComponent<Node>().neighborNodes.Add(neighbor);
        }
    }
}
