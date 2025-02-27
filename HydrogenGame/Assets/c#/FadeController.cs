using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    // スプライトレンダラーの参照
    SpriteRenderer sp;
    // スプライトのカラー
    Color spriteColor;
    // フェードの継続時間
    public float duration = 1f;
    // フェードアウト中かどうかのフラグ
    private bool isFadingOut;
    // フェードボタンの参照
    [SerializeField] Button fadeButton;

    // Startは最初のフレームの前に一度呼び出されます
    void Start()
    {
        // スプライトレンダラーコンポーネントを取得
        sp = GetComponent<SpriteRenderer>();
        // スプライトの現在のカラーを取得
        spriteColor = sp.color;

        // ゲーム開始時にフェードアウトを自動的に開始しないようにする
        // StartCoroutine(Fade(1)); // この行を削除またはコメントアウト

        // フェードボタンがセットされていれば、クリックリスナーを追加
        if (fadeButton != null)
        {
            fadeButton.onClick.AddListener(ToggleFade);
        }
    }

    // フェードの切り替えを行うメソッド
    void ToggleFade()
    {
        // すでにフェード中なら何もしない
        if (isFading) return;

        // フェードを開始するコルーチンを呼び出す
        StartCoroutine(Fade(isFadingOut ? 0 : 1));
        // フェードアウト状態を反転させる
        isFadingOut = !isFadingOut;
    }

    // フェード処理を行うコルーチン
    IEnumerator Fade(float targetAlpha)
    {
        // フェード中フラグを立てる
        isFading = true;

        // フェードボタンを無効にして、フェード処理中の操作を防ぐ
        fadeButton.interactable = false;

        // スプライトの透明度がターゲットアルファに近づくまで繰り返す
        while (!Mathf.Approximately(spriteColor.a, targetAlpha))
        {
            // フレームごとのアルファ値の変化量を計算（継続時間に基づく）
            float changePerFrame = Time.deltaTime / duration;
            // 現在のアルファ値をターゲットアルファに向かって変更
            spriteColor.a = Mathf.MoveTowards(spriteColor.a, targetAlpha, changePerFrame);
            // スプライトのカラーを更新
            sp.color = spriteColor;
            // 次のフレームまで待機
            yield return null;
        }

        // フェード中フラグを解除
        isFading = false;

        // フェードボタンを再度有効にする
        fadeButton.interactable = true;
    }

    // フェード中フラグ
    private bool isFading = false;
}
