using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public void InstantiateIndicator()
    {
        gameObject.SetActive(true);

        StartCoroutine(IndicatorCo());
    }

    private IEnumerator IndicatorCo()
    {
        yield return new WaitForSeconds(0.5f);

        // 풀에 인디케이터 반환
        Managers.Pool.ReturnPool(this);
    }
}
