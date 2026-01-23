using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    public override void Awake()
    {
        base.Awake();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    private void OnSceneUnLoaded(Scene scene)
    {

    }


    [ContextMenu("Scan All Nodes in Scene")]
    public void ScanAll()
    {
        Node[] allNodes = FindObjectsOfType<Node>();
        foreach(Node node in allNodes)
        {
            node.ClearNeighbors();
            node.ScanNeighbors();
        }
    }
}
