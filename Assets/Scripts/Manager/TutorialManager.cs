using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : Singleton<TutorialManager>
{
    [Header("UI 설정")]
    public List<TextFade> tutorialTexts = new List<TextFade>();

    [Header("대기 설정")]
    public float displayDelay = 2.0f;

    private int _textIndex = 0;
    private bool _isTutorialActive = false;
    private bool _isTransitioning = false;

    public void ShowTutorial()
    {
        if (LevelManager.Instance.currentStageIndex == 0)
        {
            if (_textIndex == tutorialTexts.Count)
            {
                _isTutorialActive = false;
                return;
            }

            StartCoroutine(SequenceCo());
        }
    }

    private IEnumerator SequenceCo()
    {
        _isTransitioning = true;
        _isTutorialActive = true;

        foreach (var text in tutorialTexts)
        {
            text.gameObject.SetActive(true);
            yield return null;
        }
        tutorialTexts[0].TextFadeIn();

        _isTransitioning = false;
    }

    public void OnPlayerAction()
    {
        if (!_isTutorialActive || _isTransitioning) return;

        StartCoroutine(TransitionToNext(_textIndex));
    }

    private IEnumerator TransitionToNext(int index)
    {
        _isTransitioning = true;

        tutorialTexts[index].TextFadeOut();

        yield return new WaitForSeconds(displayDelay);

        _textIndex++;

        if (_textIndex < tutorialTexts.Count)
        {
            tutorialTexts[_textIndex].TextFadeIn();
            _isTransitioning = false;
        }
        else
        {
            _isTutorialActive = false;
            foreach (var text in tutorialTexts)
            {
                text.gameObject.SetActive(false);
            }
        }
    }
}
