using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageFlat : MonoBehaviour
{
    [Header("패널 정보")]
    public string stageName;
    public int stageIndex;
    public Renderer clearPanel;

    [Header("마테리얼")]
    [SerializeField] private Material clearMat;
    [SerializeField] private Material readyMat;

    public void RefreshStatus()
    {
        int maxCleared = PlayerPrefs.GetInt("MaxClearedStage");

        if (stageIndex > maxCleared)
        {
            clearPanel.material = readyMat;
        }
        else
        {
            clearPanel.material = clearMat;
        }
    }
}
