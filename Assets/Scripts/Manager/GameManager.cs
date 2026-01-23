using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // 클릭 프리팹
    public Indicator IndicatorPrefab;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Stage")
        {
            SetPool();
            SetIllusion();
        }

        if (scene.name == "Select")
        {
            StageFlat[] flats = FindObjectsOfType<StageFlat>();

            foreach (var flat in flats)
            {
                flat.RefreshStatus();
            }
        }
    }

    private void OnSceneUnLoaded(Scene scene)
    {
        if (scene.name == "Stage")
        {
            RemovePool();
        }
    }

    public void StageClear()
    {
        // 클리어 연출 코루틴

        SceneManager.LoadScene("Select");
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
