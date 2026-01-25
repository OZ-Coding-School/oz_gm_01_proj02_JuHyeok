using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadManager : Singleton<LoadManager>
{
    [Header("UI 요소")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    public override void Awake()
    {
        base.Awake();

        fadeGroup.alpha = 0f;
        fadeGroup.gameObject.SetActive(false);
    }

    /// <summary>
    /// 로딩씬 외부 호출 함수
    /// </summary>
    /// <param name="sceneName"> 이동할 씬 이름 </param>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        fadeGroup.gameObject.SetActive(true);

        // 1. 로딩 화면 페이드 인
        yield return StartCoroutine(Fade(0, 1));

        // 2. 비동기 씬 로드
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;    // 바로 씬을 활성화하지 않음

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;

            // 로딩씬 최소 1초 유지
            if (timer > 1.0f)
            {
                op.allowSceneActivation = true;
            }
        }

        yield return StartCoroutine(Fade(1, 0));

        fadeGroup.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float start, float end)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; // 씬 로딩 중에도 작동하도록 unscaled 사용
            fadeGroup.alpha = Mathf.Lerp(start, end, timer / fadeDuration);
            yield return null;
        }
        fadeGroup.alpha = end;
    }
}
