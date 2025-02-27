using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Fadeout : MonoBehaviour
{
    SpriteRenderer sp;
    Color spriteColor;
    float duration = 1f;
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        spriteColor = sp.color;
        StartCoroutine(Fade(0));
    }

    IEnumerator Fade(float targetAlpha)
    {
        while (!Mathf.Approximately(spriteColor.a, targetAlpha))
        {
            float changePerFrame = Time.deltaTime / duration;
            spriteColor.a = Mathf.MoveTowards(spriteColor.a, targetAlpha, changePerFrame);
            sp.color = spriteColor;
            yield return null;
        }
    }
}

