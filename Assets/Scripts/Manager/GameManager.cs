using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // 클릭 프리팹
    public Indicator IndicatorPrefab;

    public override void Awake()
    {
        SetPool();
        SetIllusion();

        gameObject.AddComponent<StageManager>();
        transform.SetParent(GameObject.Find("@Managers").transform);
    }

    public void StageClear()
    {
        Debug.Log($"스테이지 클리어");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    public void SetPool() => Managers.Pool.CreatePool(IndicatorPrefab, 5);

    public void RemovePool() => Managers.Pool.RemovePool(IndicatorPrefab);
    

    public void SetIllusion(float distance = 50.0f, int count = 2)
    {
        Managers.Illusion.Init(distance, count);
    }
}
