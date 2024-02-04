using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Image image;
    public float flashSpeed;

    private Coroutine coroutine;

    public void Flash() // (1) 어딘가에서 플래시를 터트리면
    {
        if(coroutine != null) // = 이전에 코루틴을 돌린 적 있다는 뜻(?)
        {
            StopCoroutine(coroutine); // 이미 동작한 적이 있다면, 코루틴을 정지 해 놓는다.
        }
        image.enabled = true; // (2) 켜지고,
        image.color = Color.red;
        coroutine = StartCoroutine(FadeAway());
    }
    private IEnumerator FadeAway()
    {
        float startAlpha = 0.3f;
        float a = startAlpha;

        while (a > 0.0f)
        {
            a -= (startAlpha / flashSpeed) * Time.deltaTime; // (3) 일정시간동안 감소하겠다.
            image.color = new Color(1.0f, 0.0f, 0.0f, a);
            yield return null;
        }
        image.enabled = false;
    }
}
