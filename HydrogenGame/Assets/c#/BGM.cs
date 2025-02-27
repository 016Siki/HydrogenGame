using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using UnityEngine.UI;

public class BGM : MonoBehaviour
{
    AudioSource BV;
    float audioVolume;
    [SerializeField] Button TitleButton;
    float duration = 2f;
    // Start is called before the first frame update
    void Start()
    {
        BV = GetComponent<AudioSource>();
        audioVolume = BV.volume;
        //StartCoroutine(Volume(0));
        if (TitleButton != null)
        {
            TitleButton.onClick.AddListener(ToggleFade);
        }
    }

    void ToggleFade()
    {
        StartCoroutine(Volume(0));
    }

    IEnumerator Volume(float targetAlpha)
    {
        // スプライトのアルファ値が目標値に近づくまでループ
        while (!Mathf.Approximately(audioVolume, targetAlpha))
        {
            // 1フレームあたりのアルファ値の変化量を計算
            float changePerFrame = Time.deltaTime / duration;

            // 現在のアルファ値を目標値に向けて変化させる
            audioVolume = Mathf.MoveTowards(audioVolume, targetAlpha, changePerFrame);

            // 変化した色をスプライトに適用
            BV.volume = audioVolume;

            // 次のフレームまで待機
            yield return null;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

}
