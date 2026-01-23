using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFade : MonoBehaviour
{
    Text text;

    private bool _isTextActive = false;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    public void TextFadeOut()
    {
        StartCoroutine(TextFadeOutCo());
    }

    public void TextFadeIn()
    {
        StartCoroutine(TextFadeInCo());
    }

    /// <summary>
    /// 텍스트 알파값 1에서 0으로 전환
    /// </summary>
    /// <returns></returns>
    public IEnumerator TextFadeOutCo()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);

        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / 2.0f));
            yield return null;
        }

        text.gameObject.SetActive(false);
        _isTextActive = false;
    }
    /// <summary>
    /// 텍스트 알파값 0에서 1로 전환
    /// </summary>
    /// <returns></returns>
    public IEnumerator TextFadeInCo()
    {
        _isTextActive = true;
        text.gameObject.SetActive(true);

        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);

        while (text.color.a < 1.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / 1.0f));
            yield return null;
        }
    }
}
