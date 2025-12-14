using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerUIPopUpManager : MonoBehaviour
{
    [Header("YOU DIED Pop Up")]
    [SerializeField] GameObject youDiedPopUp;
    [SerializeField] TextMeshProUGUI youDiedPopUpBGT;
    [SerializeField] TextMeshProUGUI youDiedPopUpText;
    [SerializeField] CanvasGroup youDiedPopUpCanvasGroup;

    public void SendYouDiedPopUp()
    {
        youDiedPopUp.SetActive(true);
        youDiedPopUpBGT.characterSpacing = 0;
        StartCoroutine(StretchPopUpTextOverTime(youDiedPopUpBGT, 8, 8.32f));
        StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 5));
        StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedPopUpCanvasGroup, 2 , 5));
    }

    private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount)
    {
        if(duration > 0f)
        {
            text.characterSpacing = 0;
            float timer = 0;
            yield return null;

            while(duration < timer)
            {
                timer += Time.deltaTime;
                text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * (Time.deltaTime / 20));
                yield return null;
            }
        }
    }

    private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration)
    {
        if(duration > 0)
        {
            canvas.alpha = 0;
            float timer = 0;

            yield return null;

            while(timer < duration)
            {
                timer += Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 1,duration * Time.deltaTime);
                yield return null;
            }
        }

        canvas.alpha = 1;

        yield return null;
    }

    private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvas, float duration, float delay)
    {
        if(duration > 0)
        {
            while (delay > 0)
            {
                delay -= Time.deltaTime;
                yield return null;
            }

            canvas.alpha = 1;
            float timer = 0;

            yield return null;

            while(timer < duration)
            {
                timer += Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 0,duration * Time.deltaTime);
                yield return null;
            }
        }

        canvas.alpha = 0;

        yield return null;

    }
}
